﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using DesktopDuplication;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Point = SharpDX.Point;
using Rectangle = SharpDX.Rectangle;
using Resource = SharpDX.DXGI.Resource;
using ResultCode = SharpDX.DXGI.ResultCode;

namespace OWCV
{
    /// <summary>
    /// Provides access to frame-by-frame updates of a particular desktop (i.e. one monitor), with image and cursor information.
    /// </summary>
    public class DesktopDuplicator
    {
        private Device mDevice;
        private Texture2DDescription mTextureDesc;
        private OutputDescription mOutputDesc;
        private OutputDuplication mDeskDupl;

        private Texture2D desktopImageTexture;
        private OutputDuplicateFrameInformation frameInfo;
        private int mWhichOutputDevice = -1;

        private Bitmap finalImage1, finalImage2;
        private bool isFinalImage1;
        private Bitmap FinalImage
        {
            get
            {
                return isFinalImage1 ? finalImage1 : finalImage2;
            }
            set
            {
                if (isFinalImage1)
                {
                    finalImage2 = value;
                    if (finalImage1 != null) finalImage1.Dispose();
                }
                else
                {
                    finalImage1 = value;
                    if (finalImage2 != null) finalImage2.Dispose();
                }
                isFinalImage1 = !isFinalImage1;
            }
        }

        /// <summary>
        /// Duplicates the output of the specified monitor.
        /// </summary>
        /// <param name="whichMonitor">The output device to duplicate (i.e. monitor). Begins with zero, which seems to correspond to the primary monitor.</param>
        public DesktopDuplicator(int whichMonitor)
            : this(0, whichMonitor) { }

        /// <summary>
        /// Duplicates the output of the specified monitor on the specified graphics adapter.
        /// </summary>
        /// <param name="whichGraphicsCardAdapter">The adapter which contains the desired outputs.</param>
        /// <param name="whichOutputDevice">The output device to duplicate (i.e. monitor). Begins with zero, which seems to correspond to the primary monitor.</param>
        public DesktopDuplicator(int whichGraphicsCardAdapter, int whichOutputDevice)
        {
            mWhichOutputDevice = whichOutputDevice;
            Adapter1 adapter = null;
            try
            {
                adapter = new Factory1().GetAdapter1(whichGraphicsCardAdapter);
            }
            catch (SharpDXException)
            {
                throw new DesktopDuplicationException("Could not find the specified graphics card adapter.");
            }
            mDevice = new Device(adapter);
            Output output = null;
            try
            {
                output = adapter.GetOutput(whichOutputDevice);
            }
            catch (SharpDXException)
            {
                throw new DesktopDuplicationException("Could not find the specified output device.");
            }
            var output1 = output.QueryInterface<Output1>();
            mOutputDesc = output.Description;
            mTextureDesc = new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.Read,
                BindFlags = BindFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Width = mOutputDesc.DesktopBounds.Right - mOutputDesc.DesktopBounds.Left,
                Height = mOutputDesc.DesktopBounds.Bottom - mOutputDesc.DesktopBounds.Top,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Staging
            };

            try
            {
                mDeskDupl = output1.DuplicateOutput(mDevice);
            }
            catch (SharpDXException ex)
            {
                if (ex.ResultCode.Code == ResultCode.NotCurrentlyAvailable.Result.Code)
                {
                    throw new DesktopDuplicationException("There is already the maximum number of applications using the Desktop Duplication API running, please close one of the applications and try again.");
                }
                throw;
            }
        }

        /// <summary>
        /// Retrieves the latest desktop image and associated metadata.
        /// </summary>
        public Bitmap GetLatestFrame()
        {
            var texture = RetrieveFrame();
            
            mDevice.ImmediateContext.MapSubresource(texture, 0, 0, MapMode.Read, MapFlags.None, out var stream);
            MemoryStream s = new MemoryStream();
            stream.CopyTo(s);
            var imageData = s.ToArray();

            //for (int y = 0; y < texture.Description.Width; y++)
            //{
            //    for (int x = 0; x < texture.Description.Height; x++)
            //    {
            //        // read DXGI_FORMAT_B8G8R8A8_UNORM pixel:
            //        byte b = stream.Read<byte>();
            //        byte g = stream.Read<byte>();
            //        byte r = stream.Read<byte>();
            //        byte a = stream.Read<byte>();
            //        // color (r, g, b, a) and pixel position (x, y) are available
 
            //    }
            //}

            byte[] newData = new byte[imageData.Length];

            for (int x = 0; x < imageData.Length; x += 4)
            {
                byte[] pixel = new byte[4];
                Array.Copy(imageData, x, pixel, 0, 4);

                byte r = pixel[0];
                byte g = pixel[1];
                byte b = pixel[2];
                byte a = pixel[3];

                byte[] newPixel = new byte[] { b, g, r, a };

                Array.Copy(newPixel, 0, newData, x, 4);
            }

            imageData = newData;

            using (var ms = new MemoryStream(imageData))
            using (var bmp = new Bitmap(texture.Description.Width, texture.Description.Height, PixelFormat.Format32bppArgb))
            {
                BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0,
                        bmp.Width,
                        bmp.Height),
                    ImageLockMode.WriteOnly,
                    bmp.PixelFormat);

                IntPtr pNative = bmpData.Scan0;
                Marshal.Copy(imageData, 0, pNative, imageData.Length);

                bmp.UnlockBits(bmpData);

                return bmp;
            }
        }

        private Texture2D RetrieveFrame()
        {
            if (desktopImageTexture == null)
            {
                desktopImageTexture = new Texture2D(mDevice, mTextureDesc);
            }
            Resource desktopResource = null;
            frameInfo = new OutputDuplicateFrameInformation();
            try
            {
                mDeskDupl.AcquireNextFrame(500, out frameInfo, out desktopResource);
            }
            catch (SharpDXException ex)
            {
                if (ex.ResultCode.Code == ResultCode.WaitTimeout.Result.Code)
                {
                    return null;
                }
                if (ex.ResultCode.Failure)
                {
                    throw new DesktopDuplicationException("Failed to acquire next frame.");
                }
            }
            if (desktopResource != null)
            {
                using (var tempTexture = desktopResource.QueryInterface<Texture2D>())
                    mDevice.ImmediateContext.CopyResource(tempTexture, desktopImageTexture);
            
                desktopResource.Dispose();
                return desktopImageTexture;
            }
            return null;
        }


        private void RetrieveFrameMetadata(DesktopFrame frame)
        {

            if (frameInfo.TotalMetadataBufferSize > 0)
            {
                // Get moved regions
                int movedRegionsLength = 0;
                OutputDuplicateMoveRectangle[] movedRectangles = new OutputDuplicateMoveRectangle[frameInfo.TotalMetadataBufferSize];
                mDeskDupl.GetFrameMoveRects(movedRectangles.Length, movedRectangles, out movedRegionsLength);
                frame.MovedRegions = new MovedRegion[movedRegionsLength / Marshal.SizeOf(typeof(OutputDuplicateMoveRectangle))];
                for (int i = 0; i < frame.MovedRegions.Length; i++)
                {
                    frame.MovedRegions[i] = new MovedRegion
                    {
                        Source = new Point(movedRectangles[i].SourcePoint.X, movedRectangles[i].SourcePoint.Y),
                        Destination = new RawRectangle(movedRectangles[i].DestinationRect.Right, movedRectangles[i].DestinationRect.Bottom, movedRectangles[i].DestinationRect.Right - movedRectangles[i].DestinationRect.Left, movedRectangles[i].DestinationRect.Bottom - movedRectangles[i].DestinationRect.Top)
                    };
                }

                // Get dirty regions
                int dirtyRegionsLength = 0;
                var dirtyRectangles = new RawRectangle[frameInfo.TotalMetadataBufferSize];
                mDeskDupl.GetFrameDirtyRects(dirtyRectangles.Length, dirtyRectangles, out dirtyRegionsLength);
                frame.UpdatedRegions = new Rectangle[dirtyRegionsLength / Marshal.SizeOf(typeof(Rectangle))];
                for (int i = 0; i < frame.UpdatedRegions.Length; i++)
                {
                    frame.UpdatedRegions[i] = new RawRectangle(dirtyRectangles[i].Left, dirtyRectangles[i].Top, dirtyRectangles[i].Right, dirtyRectangles[i].Bottom);
                }
            }
            else
            {
                frame.MovedRegions = new MovedRegion[0];
                frame.UpdatedRegions = new Rectangle[0];
            }
        }

        private void RetrieveCursorMetadata(DesktopFrame frame)
        {
            var pointerInfo = new PointerInfo();

            // A non-zero mouse update timestamp indicates that there is a mouse position update and optionally a shape change
            if (frameInfo.LastMouseUpdateTime == 0)
                return;

            bool updatePosition = true;

            // Make sure we don't update pointer position wrongly
            // If pointer is invisible, make sure we did not get an update from another output that the last time that said pointer
            // was visible, if so, don't set it to invisible or update.

            if (!frameInfo.PointerPosition.Visible && (pointerInfo.WhoUpdatedPositionLast != mWhichOutputDevice))
                updatePosition = false;

            // If two outputs both say they have a visible, only update if new update has newer timestamp
            if (frameInfo.PointerPosition.Visible && pointerInfo.Visible && (pointerInfo.WhoUpdatedPositionLast != mWhichOutputDevice) && (pointerInfo.LastTimeStamp > frameInfo.LastMouseUpdateTime))
                updatePosition = false;

            // Update position
            if (updatePosition)
            {
                pointerInfo.Position = new Point(frameInfo.PointerPosition.Position.X, frameInfo.PointerPosition.Position.Y);
                pointerInfo.WhoUpdatedPositionLast = mWhichOutputDevice;
                pointerInfo.LastTimeStamp = frameInfo.LastMouseUpdateTime;
                pointerInfo.Visible = frameInfo.PointerPosition.Visible;
            }

            // No new shape
            if (frameInfo.PointerShapeBufferSize == 0)
                return;

            if (frameInfo.PointerShapeBufferSize > pointerInfo.BufferSize)
            {
                pointerInfo.PtrShapeBuffer = new byte[frameInfo.PointerShapeBufferSize];
                pointerInfo.BufferSize = frameInfo.PointerShapeBufferSize;
            }

            try
            {
                unsafe
                {
                    fixed (byte* ptrShapeBufferPtr = pointerInfo.PtrShapeBuffer)
                    {
                        mDeskDupl.GetFramePointerShape(frameInfo.PointerShapeBufferSize, (IntPtr)ptrShapeBufferPtr, out pointerInfo.BufferSize, out pointerInfo.ShapeInfo);
                    }
                }
            }
            catch (SharpDXException ex)
            {
                if (ex.ResultCode.Failure)
                {
                    throw new DesktopDuplicationException("Failed to get frame pointer shape.");
                }
            }

            //frame.CursorVisible = pointerInfo.Visible;
            frame.CursorLocation = new Point(pointerInfo.Position.X, pointerInfo.Position.Y);
        }

        private void ProcessFrame(DesktopFrame frame)
        {
            // Get the desktop capture texture
            var mapSource = mDevice.ImmediateContext.MapSubresource(desktopImageTexture, 0, MapMode.Read, MapFlags.None);

            var height = mOutputDesc.DesktopBounds.Right - mOutputDesc.DesktopBounds.Left;
            var width = mOutputDesc.DesktopBounds.Bottom - mOutputDesc.DesktopBounds.Top;
            FinalImage = new Bitmap(height, width, PixelFormat.Format32bppRgb);
            var boundsRect = new System.Drawing.Rectangle(0, 0, width, height);
            // Copy pixels from screen capture Texture to GDI bitmap
            var mapDest = FinalImage.LockBits(boundsRect, ImageLockMode.WriteOnly, FinalImage.PixelFormat);
            var sourcePtr = mapSource.DataPointer;
            var destPtr = mapDest.Scan0;
            for (int y = 0; y < height; y++)
            {
                // Copy a single line 
                Utilities.CopyMemory(destPtr, sourcePtr, width * 4);

                // Advance pointers
                sourcePtr = IntPtr.Add(sourcePtr, mapSource.RowPitch);
                destPtr = IntPtr.Add(destPtr, mapDest.Stride);
            }

            // Release source and dest locks
            FinalImage.UnlockBits(mapDest);
            mDevice.ImmediateContext.UnmapSubresource(desktopImageTexture, 0);
            frame.DesktopImage = FinalImage;
        }

        private void ReleaseFrame()
        {
            try
            {
                mDeskDupl.ReleaseFrame();
            }
            catch (SharpDXException ex)
            {
                if (ex.ResultCode.Failure)
                {
                    throw new DesktopDuplicationException("Failed to release frame.");
                }
            }
        }
    }
    public class DesktopDuplicationException : Exception
    {
        public DesktopDuplicationException(string message)
            : base(message) { }
    }

    /// <summary>
    /// Provides image data, cursor data, and image metadata about the retrieved desktop frame.
    /// </summary>
    public class DesktopFrame
    {
        /// <summary>
        /// Gets the bitmap representing the last retrieved desktop frame. This image spans the entire bounds of the specified monitor.
        /// </summary>
        public Bitmap DesktopImage { get; internal set; }

        /// <summary>
        /// Gets a list of the rectangles of pixels in the desktop image that the operating system moved to another location within the same image.
        /// </summary>
        /// <remarks>
        /// To produce a visually accurate copy of the desktop, an application must first process all moved regions before it processes updated regions.
        /// </remarks>
        public MovedRegion[] MovedRegions { get; internal set; }

        /// <summary>
        /// Returns the list of non-overlapping rectangles that indicate the areas of the desktop image that the operating system updated since the last retrieved frame.
        /// </summary>
        /// <remarks>
        /// To produce a visually accurate copy of the desktop, an application must first process all moved regions before it processes updated regions.
        /// </remarks>
        public Rectangle[] UpdatedRegions { get; internal set; }

        /// <summary>
        /// The number of frames that the operating system accumulated in the desktop image surface since the last retrieved frame.
        /// </summary>
        public int AccumulatedFrames { get; internal set; }

        /// <summary>
        /// Gets the location of the top-left-hand corner of the cursor. This is not necessarily the same position as the cursor's hot spot, which is the location in the cursor that interacts with other elements on the screen.
        /// </summary>
        public Point CursorLocation { get; internal set; }

        /// <summary>
        /// Gets whether the cursor on the last retrieved desktop image was visible.
        /// </summary>
        public bool CursorVisible { get; internal set; }

        /// <summary>
        /// Gets whether the desktop image contains protected content that was already blacked out in the desktop image.
        /// </summary>
        public bool ProtectedContentMaskedOut { get; internal set; }

        /// <summary>
        /// Gets whether the operating system accumulated updates by coalescing updated regions. If so, the updated regions might contain unmodified pixels.
        /// </summary>
        public bool RectanglesCoalesced { get; internal set; }
    }

    /// <summary>
    /// Describes the movement of an image rectangle within a desktop frame.
    /// </summary>
    /// <remarks>
    /// Move regions are always non-stretched regions so the source is always the same size as the destination.
    /// </remarks>
    public struct MovedRegion
    {
        /// <summary>
        /// Gets the location from where the operating system copied the image region.
        /// </summary>
        public Point Source { get; internal set; }

        /// <summary>
        /// Gets the target region to where the operating system moved the image region.
        /// </summary>
        public Rectangle Destination { get; internal set; }
    }
}
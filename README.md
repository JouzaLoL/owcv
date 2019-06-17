# owcv

# CV
- verify that FOV is center of screen
- if above is true, find result for missing shots
- find another bottleneck
  
## Use adb shell
  - android shell command to send K keystroke:

```printf '\x02\x00\x0e\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00' > /dev/hidg0```

- Open adb shell, elevate permission and send commands
- https://stackoverflow.com/questions/3308500/run-interactive-command-line-exe-using-c-sharp


## Ways to emulate a HID Keyboard
- Arduino-like (Micro, Teensy etc.)
  - https://www.unknowncheats.me/forum/c-/304403-arduino-keyboard-mouse-control.html
  - https://www.gme.cz/digispark-attiny85-usb
  - https://www.gme.cz/arduino-beetle-atmega32u4-usb#product-detail


- Android Pie has native Bluetooth HID support.
  - How to talk to the app from the PC?
    - https://stackoverflow.com/questions/46703159/how-to-use-bluetooth-hid-profile-in-android

### Android over USB
- https://github.com/Netdex/android-hid-script
- https://github.com/pelya/android-keyboard-gadget
- http://zx.rs/6/DroidDucky---Can-an-Android-quack-like-a-duck/
- **https://github.com/tejado/Authorizer/**
 
### How to send commands to app from PC
- https://github.com/TooTallNate/Java-WebSocket
- https://github.com/sta/websocket-sharp

## Fastest way to capture desktop ( preferably event-based )
- Basically 3 ways:
  - Bitmap.CopyFromScreen
    - Slowest
  - GDI+
    - Supposedly sometimes faster than DirectX
  - DirectX
  - DirectX Hooking
    - Anticheat problems
- https://stackoverflow.com/questions/6812068/c-sharp-which-is-the-fastest-way-to-take-a-screen-shot

# General loop
- proccess images sequentially, one after another, not thru timers. This way its as fast as possible
- use one thread for capturing and another for analyzing
- reuse bitmap contexts, minimize creating new ones

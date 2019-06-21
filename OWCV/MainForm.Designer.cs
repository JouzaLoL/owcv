namespace OWCV
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelDebug = new MaterialSkin.Controls.MaterialLabel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.tickMsLabel = new System.Windows.Forms.Label();
            this.tickSpeedMsValue = new System.Windows.Forms.Label();
            this.materialRaisedButton1 = new MaterialSkin.Controls.MaterialRaisedButton();
            this.materialFlatButton1 = new MaterialSkin.Controls.MaterialFlatButton();
            this.tickMs = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.tickMs)).BeginInit();
            this.SuspendLayout();
            // 
            // labelDebug
            // 
            this.labelDebug.AutoSize = true;
            this.labelDebug.BackColor = System.Drawing.Color.White;
            this.labelDebug.Depth = 0;
            this.labelDebug.Font = new System.Drawing.Font("Roboto", 11F);
            this.labelDebug.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.labelDebug.Location = new System.Drawing.Point(83, 32);
            this.labelDebug.MouseState = MaterialSkin.MouseState.HOVER;
            this.labelDebug.Name = "labelDebug";
            this.labelDebug.Size = new System.Drawing.Size(88, 19);
            this.labelDebug.TabIndex = 1;
            this.labelDebug.Text = "Debug Build";
            this.labelDebug.Visible = false;
            this.labelDebug.Click += new System.EventHandler(this.labelDebug_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Location = new System.Drawing.Point(16, 73);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(10, 3, 3, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(432, 96);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // tickMsLabel
            // 
            this.tickMsLabel.AutoSize = true;
            this.tickMsLabel.BackColor = System.Drawing.SystemColors.Window;
            this.tickMsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tickMsLabel.Location = new System.Drawing.Point(272, 175);
            this.tickMsLabel.Name = "tickMsLabel";
            this.tickMsLabel.Size = new System.Drawing.Size(140, 20);
            this.tickMsLabel.TabIndex = 4;
            this.tickMsLabel.Text = "Tick Speed in MS: ";
            // 
            // tickSpeedMsValue
            // 
            this.tickSpeedMsValue.AutoSize = true;
            this.tickSpeedMsValue.BackColor = System.Drawing.SystemColors.Window;
            this.tickSpeedMsValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tickSpeedMsValue.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tickSpeedMsValue.Location = new System.Drawing.Point(419, 175);
            this.tickSpeedMsValue.Name = "tickSpeedMsValue";
            this.tickSpeedMsValue.Size = new System.Drawing.Size(29, 20);
            this.tickSpeedMsValue.TabIndex = 5;
            this.tickSpeedMsValue.Text = "50";
            // 
            // materialRaisedButton1
            // 
            this.materialRaisedButton1.Depth = 0;
            this.materialRaisedButton1.Location = new System.Drawing.Point(16, 175);
            this.materialRaisedButton1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialRaisedButton1.Name = "materialRaisedButton1";
            this.materialRaisedButton1.Primary = true;
            this.materialRaisedButton1.Size = new System.Drawing.Size(139, 65);
            this.materialRaisedButton1.TabIndex = 6;
            this.materialRaisedButton1.Text = "Reload";
            this.materialRaisedButton1.UseVisualStyleBackColor = true;
            this.materialRaisedButton1.Click += new System.EventHandler(this.materialRaisedButton1_Click);
            // 
            // materialFlatButton1
            // 
            this.materialFlatButton1.AutoSize = true;
            this.materialFlatButton1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialFlatButton1.Depth = 0;
            this.materialFlatButton1.Location = new System.Drawing.Point(320, 238);
            this.materialFlatButton1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.materialFlatButton1.MouseState = MaterialSkin.MouseState.HOVER;
            this.materialFlatButton1.Name = "materialFlatButton1";
            this.materialFlatButton1.Primary = false;
            this.materialFlatButton1.Size = new System.Drawing.Size(128, 36);
            this.materialFlatButton1.TabIndex = 8;
            this.materialFlatButton1.Text = "Send Fire Key (K)";
            this.materialFlatButton1.UseVisualStyleBackColor = true;
            this.materialFlatButton1.Click += new System.EventHandler(this.materialFlatButton1_Click);
            // 
            // tickMs
            // 
            this.tickMs.BackColor = System.Drawing.Color.White;
            this.tickMs.LargeChange = 10;
            this.tickMs.Location = new System.Drawing.Point(276, 195);
            this.tickMs.Maximum = 100;
            this.tickMs.Minimum = 1;
            this.tickMs.Name = "tickMs";
            this.tickMs.Size = new System.Drawing.Size(172, 45);
            this.tickMs.SmallChange = 5;
            this.tickMs.TabIndex = 3;
            this.tickMs.TickFrequency = 10;
            this.tickMs.Value = 50;
            this.tickMs.Scroll += new System.EventHandler(this.tickMs_Scroll);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 485);
            this.Controls.Add(this.materialFlatButton1);
            this.Controls.Add(this.materialRaisedButton1);
            this.Controls.Add(this.tickSpeedMsValue);
            this.Controls.Add(this.tickMsLabel);
            this.Controls.Add(this.tickMs);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.labelDebug);
            this.Name = "MainForm";
            this.Text = "OWCV";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tickMs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private MaterialSkin.Controls.MaterialLabel labelDebug;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label tickMsLabel;
        private System.Windows.Forms.Label tickSpeedMsValue;
        private MaterialSkin.Controls.MaterialRaisedButton materialRaisedButton1;
        private MaterialSkin.Controls.MaterialFlatButton materialFlatButton1;
        private System.Windows.Forms.TrackBar tickMs;
    }
}


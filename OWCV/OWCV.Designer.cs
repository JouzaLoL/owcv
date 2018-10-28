namespace OWCV
{
    partial class OWCV
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
            this.btnLoad = new MaterialSkin.Controls.MaterialRaisedButton();
            this.labelDebug = new MaterialSkin.Controls.MaterialLabel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btnLoad
            // 
            this.btnLoad.Depth = 0;
            this.btnLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoad.Location = new System.Drawing.Point(264, 199);
            this.btnLoad.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Primary = true;
            this.btnLoad.Size = new System.Drawing.Size(184, 41);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "Load OWCV";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.materialRaisedButton1_Click);
            // 
            // labelDebug
            // 
            this.labelDebug.AutoSize = true;
            this.labelDebug.BackColor = System.Drawing.Color.White;
            this.labelDebug.Depth = 0;
            this.labelDebug.Font = new System.Drawing.Font("Roboto", 11F);
            this.labelDebug.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.labelDebug.Location = new System.Drawing.Point(12, 211);
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
            this.richTextBox1.Location = new System.Drawing.Point(16, 97);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(432, 96);
            this.richTextBox1.TabIndex = 2;
            this.richTextBox1.Text = "";
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // OWCV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 252);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.labelDebug);
            this.Controls.Add(this.btnLoad);
            this.Name = "OWCV";
            this.Text = "OWCV";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialSkin.Controls.MaterialRaisedButton btnLoad;
        private MaterialSkin.Controls.MaterialLabel labelDebug;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}


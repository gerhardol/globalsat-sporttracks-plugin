namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    partial class ScreenCaptureControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBoxScreenCapture = new System.Windows.Forms.GroupBox();
            this.buttonSave = new ZoneFiveSoftware.Common.Visuals.Button();
            this.buttonCaptureScreen = new ZoneFiveSoftware.Common.Visuals.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBoxScreenCapture.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(54, 19);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(120, 80);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // groupBoxScreenCapture
            // 
            this.groupBoxScreenCapture.Controls.Add(this.buttonSave);
            this.groupBoxScreenCapture.Controls.Add(this.buttonCaptureScreen);
            this.groupBoxScreenCapture.Controls.Add(this.pictureBox1);
            this.groupBoxScreenCapture.Location = new System.Drawing.Point(21, 19);
            this.groupBoxScreenCapture.Name = "groupBoxScreenCapture";
            this.groupBoxScreenCapture.Size = new System.Drawing.Size(205, 133);
            this.groupBoxScreenCapture.TabIndex = 1;
            this.groupBoxScreenCapture.TabStop = false;
            this.groupBoxScreenCapture.Text = "Screen Capture";
            // 
            // buttonSave
            // 
            this.buttonSave.BackColor = System.Drawing.Color.Transparent;
            this.buttonSave.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.buttonSave.CenterImage = null;
            this.buttonSave.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonSave.HyperlinkStyle = false;
            this.buttonSave.ImageMargin = 2;
            this.buttonSave.LeftImage = null;
            this.buttonSave.Location = new System.Drawing.Point(14, 62);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.PushStyle = true;
            this.buttonSave.RightImage = null;
            this.buttonSave.Size = new System.Drawing.Size(27, 23);
            this.buttonSave.TabIndex = 20;
            this.buttonSave.TextAlign = System.Drawing.StringAlignment.Center;
            this.buttonSave.TextLeftMargin = 2;
            this.buttonSave.TextRightMargin = 2;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonCaptureScreen
            // 
            this.buttonCaptureScreen.BackColor = System.Drawing.Color.Transparent;
            this.buttonCaptureScreen.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.buttonCaptureScreen.CenterImage = null;
            this.buttonCaptureScreen.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonCaptureScreen.HyperlinkStyle = false;
            this.buttonCaptureScreen.ImageMargin = 2;
            this.buttonCaptureScreen.LeftImage = null;
            this.buttonCaptureScreen.Location = new System.Drawing.Point(14, 28);
            this.buttonCaptureScreen.Name = "buttonCaptureScreen";
            this.buttonCaptureScreen.PushStyle = true;
            this.buttonCaptureScreen.RightImage = null;
            this.buttonCaptureScreen.Size = new System.Drawing.Size(27, 23);
            this.buttonCaptureScreen.TabIndex = 19;
            this.buttonCaptureScreen.TextAlign = System.Drawing.StringAlignment.Center;
            this.buttonCaptureScreen.TextLeftMargin = 2;
            this.buttonCaptureScreen.TextRightMargin = 2;
            this.buttonCaptureScreen.Click += new System.EventHandler(this.buttonCaptureScreen_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Bitmap Files (*.bmp)|*.bmp";
            // 
            // ScreenCaptureControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxScreenCapture);
            this.Name = "ScreenCaptureControl";
            this.Size = new System.Drawing.Size(308, 241);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBoxScreenCapture.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBoxScreenCapture;
        private ZoneFiveSoftware.Common.Visuals.Button buttonCaptureScreen;
        private ZoneFiveSoftware.Common.Visuals.Button buttonSave;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

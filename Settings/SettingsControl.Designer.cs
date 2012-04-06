namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    partial class SettingsControl
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
            this.labelName = new System.Windows.Forms.Label();
            this.labelLicense = new System.Windows.Forms.Label();
            this.labelDetect = new System.Windows.Forms.Label();
            this.buttonDetect = new ZoneFiveSoftware.Common.Visuals.Button();
            this.SuspendLayout();
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelName.Location = new System.Drawing.Point(13, 16);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(143, 13);
            this.labelName.TabIndex = 15;
            this.labelName.Text = "Globalsat Device Plugin";
            // 
            // labelLicense
            // 
            this.labelLicense.AutoSize = true;
            this.labelLicense.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLicense.Location = new System.Drawing.Point(13, 88);
            this.labelLicense.Name = "labelLicense";
            this.labelLicense.Size = new System.Drawing.Size(40, 13);
            this.labelLicense.TabIndex = 17;
            this.labelLicense.Text = "license";
            // 
            // labelDetect
            // 
            this.labelDetect.Location = new System.Drawing.Point(41, 43);
            this.labelDetect.Name = "labelDetect";
            this.labelDetect.Size = new System.Drawing.Size(193, 19);
            this.labelDetect.TabIndex = 19;
            this.labelDetect.Text = "Globalsat device not found";
            // 
            // buttonDetect
            // 
            this.buttonDetect.BackColor = System.Drawing.Color.Transparent;
            this.buttonDetect.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.buttonDetect.CenterImage = null;
            this.buttonDetect.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonDetect.HyperlinkStyle = false;
            this.buttonDetect.ImageMargin = 2;
            this.buttonDetect.LeftImage = null;
            this.buttonDetect.Location = new System.Drawing.Point(16, 43);
            this.buttonDetect.Name = "buttonDetect";
            this.buttonDetect.PushStyle = true;
            this.buttonDetect.RightImage = null;
            this.buttonDetect.Size = new System.Drawing.Size(19, 19);
            this.buttonDetect.TabIndex = 18;
            this.buttonDetect.Text = "Refresh";
            this.buttonDetect.TextAlign = System.Drawing.StringAlignment.Center;
            this.buttonDetect.TextLeftMargin = 2;
            this.buttonDetect.TextRightMargin = 2;
            this.buttonDetect.Click += new System.EventHandler(this.buttonDetect_Click);
            // 
            // SettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelDetect);
            this.Controls.Add(this.buttonDetect);
            this.Controls.Add(this.labelLicense);
            this.Controls.Add(this.labelName);
            this.Name = "SettingsControl";
            this.Size = new System.Drawing.Size(300, 115);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelName;
        //private System.Windows.Forms.Label labelGlobalsatDevice;
        private System.Windows.Forms.Label labelLicense;
        private System.Windows.Forms.Label labelDetect;
        private Common.Visuals.Button buttonDetect;
    }
}

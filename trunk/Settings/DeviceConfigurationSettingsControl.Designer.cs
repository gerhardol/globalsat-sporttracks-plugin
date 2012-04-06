namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    partial class DeviceConfigurationSettingsControl
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
            this.groupBoxDeviceConfig = new System.Windows.Forms.GroupBox();
            this.buttonExportDeviceConfig = new ZoneFiveSoftware.Common.Visuals.Button();
            this.buttonImportDeviceConfig = new ZoneFiveSoftware.Common.Visuals.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.labelStatus = new System.Windows.Forms.Label();
            this.groupBoxDeviceConfig.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxDeviceConfig
            // 
            this.groupBoxDeviceConfig.Controls.Add(this.labelStatus);
            this.groupBoxDeviceConfig.Controls.Add(this.buttonExportDeviceConfig);
            this.groupBoxDeviceConfig.Controls.Add(this.buttonImportDeviceConfig);
            this.groupBoxDeviceConfig.Location = new System.Drawing.Point(21, 19);
            this.groupBoxDeviceConfig.Name = "groupBoxDeviceConfig";
            this.groupBoxDeviceConfig.Size = new System.Drawing.Size(245, 112);
            this.groupBoxDeviceConfig.TabIndex = 12;
            this.groupBoxDeviceConfig.TabStop = false;
            this.groupBoxDeviceConfig.Text = "Device Configuration";
            // 
            // buttonExportDeviceConfig
            // 
            this.buttonExportDeviceConfig.AutoSize = true;
            this.buttonExportDeviceConfig.BackColor = System.Drawing.Color.Transparent;
            this.buttonExportDeviceConfig.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.buttonExportDeviceConfig.CenterImage = null;
            this.buttonExportDeviceConfig.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonExportDeviceConfig.HyperlinkStyle = false;
            this.buttonExportDeviceConfig.ImageMargin = 2;
            this.buttonExportDeviceConfig.LeftImage = null;
            this.buttonExportDeviceConfig.Location = new System.Drawing.Point(10, 64);
            this.buttonExportDeviceConfig.Name = "buttonExportDeviceConfig";
            this.buttonExportDeviceConfig.PushStyle = true;
            this.buttonExportDeviceConfig.RightImage = null;
            this.buttonExportDeviceConfig.Size = new System.Drawing.Size(80, 22);
            this.buttonExportDeviceConfig.TabIndex = 10;
            this.buttonExportDeviceConfig.Text = "Export to file";
            this.buttonExportDeviceConfig.TextAlign = System.Drawing.StringAlignment.Near;
            this.buttonExportDeviceConfig.TextLeftMargin = 2;
            this.buttonExportDeviceConfig.TextRightMargin = 2;
            this.buttonExportDeviceConfig.Click += new System.EventHandler(this.buttonExportDeviceConfig_Click);
            // 
            // buttonImportDeviceConfig
            // 
            this.buttonImportDeviceConfig.AutoSize = true;
            this.buttonImportDeviceConfig.BackColor = System.Drawing.Color.Transparent;
            this.buttonImportDeviceConfig.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.buttonImportDeviceConfig.CenterImage = null;
            this.buttonImportDeviceConfig.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonImportDeviceConfig.HyperlinkStyle = false;
            this.buttonImportDeviceConfig.ImageMargin = 2;
            this.buttonImportDeviceConfig.LeftImage = null;
            this.buttonImportDeviceConfig.Location = new System.Drawing.Point(10, 35);
            this.buttonImportDeviceConfig.Name = "buttonImportDeviceConfig";
            this.buttonImportDeviceConfig.PushStyle = true;
            this.buttonImportDeviceConfig.RightImage = null;
            this.buttonImportDeviceConfig.Size = new System.Drawing.Size(93, 22);
            this.buttonImportDeviceConfig.TabIndex = 9;
            this.buttonImportDeviceConfig.Text = "Import from file";
            this.buttonImportDeviceConfig.TextAlign = System.Drawing.StringAlignment.Near;
            this.buttonImportDeviceConfig.TextLeftMargin = 2;
            this.buttonImportDeviceConfig.TextRightMargin = 2;
            this.buttonImportDeviceConfig.Click += new System.EventHandler(this.buttonImportDeviceConfig_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Configuration Files (*.cfg)|*.cfg";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Configuration Files (*.cfg)|*.cfg";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(23, 93);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(37, 13);
            this.labelStatus.TabIndex = 11;
            this.labelStatus.Text = "Status";
            // 
            // DeviceConfigurationSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxDeviceConfig);
            this.Name = "DeviceConfigurationSettingsControl";
            this.Size = new System.Drawing.Size(296, 195);
            this.groupBoxDeviceConfig.ResumeLayout(false);
            this.groupBoxDeviceConfig.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxDeviceConfig;
        private ZoneFiveSoftware.Common.Visuals.Button buttonExportDeviceConfig;
        private ZoneFiveSoftware.Common.Visuals.Button buttonImportDeviceConfig;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label labelStatus;
    }
}

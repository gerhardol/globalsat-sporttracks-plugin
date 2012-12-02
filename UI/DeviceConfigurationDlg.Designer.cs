/*
Copyright (C) 2010 Zone Five Software

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library. If not, see <http://www.gnu.org/licenses/>.
 */
// Author: Aaron Averill


namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    partial class DeviceConfigurationDlg
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
            this.btnOk = new ZoneFiveSoftware.Common.Visuals.Button();
            this.btnCancel = new ZoneFiveSoftware.Common.Visuals.Button();
            this.chkImportOnlyNew = new System.Windows.Forms.CheckBox();
            this.txtHoursOffset = new ZoneFiveSoftware.Common.Visuals.TextBox();
            this.labelHoursOffset = new System.Windows.Forms.Label();
            this.buttonDetect = new ZoneFiveSoftware.Common.Visuals.Button();
            this.labelDetect = new System.Windows.Forms.Label();
            this.groupBoxDeviceConfig = new System.Windows.Forms.GroupBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.buttonExportDeviceConfig = new ZoneFiveSoftware.Common.Visuals.Button();
            this.buttonImportDeviceConfig = new ZoneFiveSoftware.Common.Visuals.Button();
            this.groupBoxScreenCapture = new System.Windows.Forms.GroupBox();
            this.buttonSave = new ZoneFiveSoftware.Common.Visuals.Button();
            this.buttonCaptureScreen = new ZoneFiveSoftware.Common.Visuals.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelRemainingTime = new System.Windows.Forms.Label();
            this.buttonDelete = new ZoneFiveSoftware.Common.Visuals.Button();
            this.dateTimePickerOldest = new System.Windows.Forms.DateTimePicker();
            this.labelDelete = new System.Windows.Forms.Label();
            this.groupBoxDeviceConfig.SuspendLayout();
            this.groupBoxScreenCapture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.BackColor = System.Drawing.Color.Transparent;
            this.btnOk.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.btnOk.CenterImage = null;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.HyperlinkStyle = false;
            this.btnOk.ImageMargin = 2;
            this.btnOk.LeftImage = null;
            this.btnOk.Location = new System.Drawing.Point(193, 362);
            this.btnOk.Name = "btnOk";
            this.btnOk.PushStyle = true;
            this.btnOk.RightImage = null;
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "Ok";
            this.btnOk.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnOk.TextLeftMargin = 2;
            this.btnOk.TextRightMargin = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.btnCancel.CenterImage = null;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.HyperlinkStyle = false;
            this.btnCancel.ImageMargin = 2;
            this.btnCancel.LeftImage = null;
            this.btnCancel.Location = new System.Drawing.Point(274, 362);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.PushStyle = true;
            this.btnCancel.RightImage = null;
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextAlign = System.Drawing.StringAlignment.Center;
            this.btnCancel.TextLeftMargin = 2;
            this.btnCancel.TextRightMargin = 2;
            // 
            // chkImportOnlyNew
            // 
            this.chkImportOnlyNew.AutoSize = true;
            this.chkImportOnlyNew.Checked = true;
            this.chkImportOnlyNew.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkImportOnlyNew.Location = new System.Drawing.Point(15, 12);
            this.chkImportOnlyNew.Name = "chkImportOnlyNew";
            this.chkImportOnlyNew.Size = new System.Drawing.Size(124, 17);
            this.chkImportOnlyNew.TabIndex = 0;
            this.chkImportOnlyNew.Text = "Import new data only";
            this.chkImportOnlyNew.UseVisualStyleBackColor = true;
            // 
            // txtHoursOffset
            // 
            this.txtHoursOffset.AcceptsReturn = false;
            this.txtHoursOffset.AcceptsTab = false;
            this.txtHoursOffset.BackColor = System.Drawing.Color.White;
            this.txtHoursOffset.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(114)))), ((int)(((byte)(108)))));
            this.txtHoursOffset.ButtonImage = null;
            this.txtHoursOffset.Location = new System.Drawing.Point(117, 37);
            this.txtHoursOffset.MaxLength = 32767;
            this.txtHoursOffset.Multiline = false;
            this.txtHoursOffset.Name = "txtHoursOffset";
            this.txtHoursOffset.ReadOnly = false;
            this.txtHoursOffset.ReadOnlyColor = System.Drawing.SystemColors.Control;
            this.txtHoursOffset.ReadOnlyTextColor = System.Drawing.SystemColors.ControlLight;
            this.txtHoursOffset.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtHoursOffset.Size = new System.Drawing.Size(45, 19);
            this.txtHoursOffset.TabIndex = 2;
            this.txtHoursOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // labelHoursOffset
            // 
            this.labelHoursOffset.Location = new System.Drawing.Point(12, 40);
            this.labelHoursOffset.Name = "labelHoursOffset";
            this.labelHoursOffset.Size = new System.Drawing.Size(100, 19);
            this.labelHoursOffset.TabIndex = 1;
            this.labelHoursOffset.Text = "Hours offset:";
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
            this.buttonDetect.Location = new System.Drawing.Point(15, 65);
            this.buttonDetect.Name = "buttonDetect";
            this.buttonDetect.PushStyle = true;
            this.buttonDetect.RightImage = null;
            this.buttonDetect.Size = new System.Drawing.Size(19, 15);
            this.buttonDetect.TabIndex = 7;
            this.buttonDetect.Text = "Refresh";
            this.buttonDetect.TextAlign = System.Drawing.StringAlignment.Center;
            this.buttonDetect.TextLeftMargin = 2;
            this.buttonDetect.TextRightMargin = 2;
            this.buttonDetect.Click += new System.EventHandler(this.buttonDetect_Click);
            // 
            // labelDetect
            // 
            this.labelDetect.AutoSize = true;
            this.labelDetect.Location = new System.Drawing.Point(114, 65);
            this.labelDetect.Name = "labelDetect";
            this.labelDetect.Size = new System.Drawing.Size(134, 13);
            this.labelDetect.TabIndex = 8;
            this.labelDetect.Text = "Globalsat device not found";
            // 
            // groupBoxDeviceConfig
            // 
            this.groupBoxDeviceConfig.Controls.Add(this.labelStatus);
            this.groupBoxDeviceConfig.Controls.Add(this.buttonExportDeviceConfig);
            this.groupBoxDeviceConfig.Controls.Add(this.buttonImportDeviceConfig);
            this.groupBoxDeviceConfig.Location = new System.Drawing.Point(15, 136);
            this.groupBoxDeviceConfig.Name = "groupBoxDeviceConfig";
            this.groupBoxDeviceConfig.Size = new System.Drawing.Size(245, 92);
            this.groupBoxDeviceConfig.TabIndex = 13;
            this.groupBoxDeviceConfig.TabStop = false;
            this.groupBoxDeviceConfig.Text = "Device Configuration";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(7, 72);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(37, 13);
            this.labelStatus.TabIndex = 11;
            this.labelStatus.Text = "Status";
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
            this.buttonExportDeviceConfig.Location = new System.Drawing.Point(10, 47);
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
            this.buttonImportDeviceConfig.Location = new System.Drawing.Point(10, 19);
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
            // groupBoxScreenCapture
            // 
            this.groupBoxScreenCapture.Controls.Add(this.buttonSave);
            this.groupBoxScreenCapture.Controls.Add(this.buttonCaptureScreen);
            this.groupBoxScreenCapture.Controls.Add(this.pictureBox1);
            this.groupBoxScreenCapture.Location = new System.Drawing.Point(15, 243);
            this.groupBoxScreenCapture.Name = "groupBoxScreenCapture";
            this.groupBoxScreenCapture.Size = new System.Drawing.Size(205, 112);
            this.groupBoxScreenCapture.TabIndex = 14;
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
            // labelRemainingTime
            // 
            this.labelRemainingTime.AutoSize = true;
            this.labelRemainingTime.Location = new System.Drawing.Point(12, 83);
            this.labelRemainingTime.Name = "labelRemainingTime";
            this.labelRemainingTime.Size = new System.Drawing.Size(118, 13);
            this.labelRemainingTime.TabIndex = 15;
            this.labelRemainingTime.Text = "<Click for remainingtime";
            this.labelRemainingTime.Click += new System.EventHandler(this.labelRemainingTime_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.BackColor = System.Drawing.Color.Transparent;
            this.buttonDelete.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.buttonDelete.CenterImage = null;
            this.buttonDelete.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonDelete.HyperlinkStyle = false;
            this.buttonDelete.ImageMargin = 2;
            this.buttonDelete.LeftImage = null;
            this.buttonDelete.Location = new System.Drawing.Point(15, 109);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.PushStyle = true;
            this.buttonDelete.RightImage = null;
            this.buttonDelete.Size = new System.Drawing.Size(19, 21);
            this.buttonDelete.TabIndex = 16;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.TextAlign = System.Drawing.StringAlignment.Center;
            this.buttonDelete.TextLeftMargin = 2;
            this.buttonDelete.TextRightMargin = 2;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // dateTimePickerOldest
            // 
            this.dateTimePickerOldest.Location = new System.Drawing.Point(40, 109);
            this.dateTimePickerOldest.Name = "dateTimePickerOldest";
            this.dateTimePickerOldest.Size = new System.Drawing.Size(200, 20);
            this.dateTimePickerOldest.TabIndex = 18;
            this.dateTimePickerOldest.Visible = false;
            // 
            // labelDelete
            // 
            this.labelDelete.AutoSize = true;
            this.labelDelete.Location = new System.Drawing.Point(40, 115);
            this.labelDelete.Name = "labelDelete";
            this.labelDelete.Size = new System.Drawing.Size(130, 13);
            this.labelDelete.TabIndex = 19;
            this.labelDelete.Text = "Delete all device activities";
            // 
            // DeviceConfigurationDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(361, 390);
            this.Controls.Add(this.labelDelete);
            this.Controls.Add(this.dateTimePickerOldest);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.labelRemainingTime);
            this.Controls.Add(this.groupBoxScreenCapture);
            this.Controls.Add(this.groupBoxDeviceConfig);
            this.Controls.Add(this.labelDetect);
            this.Controls.Add(this.buttonDetect);
            this.Controls.Add(this.txtHoursOffset);
            this.Controls.Add(this.labelHoursOffset);
            this.Controls.Add(this.chkImportOnlyNew);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeviceConfigurationDlg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "GPS Options";
            this.groupBoxDeviceConfig.ResumeLayout(false);
            this.groupBoxDeviceConfig.PerformLayout();
            this.groupBoxScreenCapture.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZoneFiveSoftware.Common.Visuals.Button btnOk;
        private ZoneFiveSoftware.Common.Visuals.Button btnCancel;
        private System.Windows.Forms.CheckBox chkImportOnlyNew;
        private ZoneFiveSoftware.Common.Visuals.TextBox txtHoursOffset;
        private System.Windows.Forms.Label labelHoursOffset;
        private Common.Visuals.Button buttonDetect;
        private System.Windows.Forms.Label labelDetect;
        private System.Windows.Forms.GroupBox groupBoxDeviceConfig;
        private System.Windows.Forms.Label labelStatus;
        private Common.Visuals.Button buttonExportDeviceConfig;
        private Common.Visuals.Button buttonImportDeviceConfig;
        private System.Windows.Forms.GroupBox groupBoxScreenCapture;
        private Common.Visuals.Button buttonSave;
        private Common.Visuals.Button buttonCaptureScreen;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelRemainingTime;
        private Common.Visuals.Button buttonDelete;
        private System.Windows.Forms.DateTimePicker dateTimePickerOldest;
        private System.Windows.Forms.Label labelDelete;
    }
}
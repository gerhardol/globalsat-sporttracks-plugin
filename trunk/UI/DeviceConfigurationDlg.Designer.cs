/*
Copyright (C) 2010 Zone Five Software

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 3 of the License, or (at your option) any later version.

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
            this.textBoxComPort = new ZoneFiveSoftware.Common.Visuals.TextBox();
            this.labelComPort = new System.Windows.Forms.Label();
            this.buttonDetect = new ZoneFiveSoftware.Common.Visuals.Button();
            this.labelDetect = new System.Windows.Forms.Label();
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
            this.btnOk.Location = new System.Drawing.Point(175, 94);
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
            this.btnCancel.Location = new System.Drawing.Point(256, 94);
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
            // textBoxComPort
            // 
            this.textBoxComPort.AcceptsReturn = false;
            this.textBoxComPort.AcceptsTab = false;
            this.textBoxComPort.BackColor = System.Drawing.Color.White;
            this.textBoxComPort.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(114)))), ((int)(((byte)(108)))));
            this.textBoxComPort.ButtonImage = null;
            this.textBoxComPort.Location = new System.Drawing.Point(117, 62);
            this.textBoxComPort.MaxLength = 32767;
            this.textBoxComPort.Multiline = false;
            this.textBoxComPort.Name = "textBoxComPort";
            this.textBoxComPort.ReadOnly = false;
            this.textBoxComPort.ReadOnlyColor = System.Drawing.SystemColors.Control;
            this.textBoxComPort.ReadOnlyTextColor = System.Drawing.SystemColors.ControlLight;
            this.textBoxComPort.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.textBoxComPort.Size = new System.Drawing.Size(45, 19);
            this.textBoxComPort.TabIndex = 6;
            this.textBoxComPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // labelComPort
            // 
            this.labelComPort.Location = new System.Drawing.Point(12, 65);
            this.labelComPort.Name = "labelComPort";
            this.labelComPort.Size = new System.Drawing.Size(100, 19);
            this.labelComPort.TabIndex = 5;
            this.labelComPort.Text = "COM Port:";
            // 
            // buttonDetect
            // 
            this.buttonDetect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDetect.BackColor = System.Drawing.Color.Transparent;
            this.buttonDetect.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.buttonDetect.CenterImage = null;
            this.buttonDetect.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonDetect.HyperlinkStyle = false;
            this.buttonDetect.ImageMargin = 2;
            this.buttonDetect.LeftImage = null;
            this.buttonDetect.Location = new System.Drawing.Point(12, 62);
            this.buttonDetect.Name = "buttonDetect";
            this.buttonDetect.PushStyle = true;
            this.buttonDetect.RightImage = null;
            this.buttonDetect.Size = new System.Drawing.Size(75, 23);
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
            // DeviceConfigurationDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 122);
            this.Controls.Add(this.labelDetect);
            this.Controls.Add(this.buttonDetect);
            this.Controls.Add(this.textBoxComPort);
            this.Controls.Add(this.labelComPort);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZoneFiveSoftware.Common.Visuals.Button btnOk;
        private ZoneFiveSoftware.Common.Visuals.Button btnCancel;
        private System.Windows.Forms.CheckBox chkImportOnlyNew;
        private ZoneFiveSoftware.Common.Visuals.TextBox txtHoursOffset;
        private System.Windows.Forms.Label labelHoursOffset;
        private Common.Visuals.TextBox textBoxComPort;
        private System.Windows.Forms.Label labelComPort;
        private Common.Visuals.Button buttonDetect;
        private System.Windows.Forms.Label labelDetect;
    }
}
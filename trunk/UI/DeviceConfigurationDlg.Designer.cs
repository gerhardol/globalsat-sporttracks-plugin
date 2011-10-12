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
            this.btnOk.Location = new System.Drawing.Point(175, 88);
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
            this.btnCancel.Location = new System.Drawing.Point(256, 88);
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
            this.txtHoursOffset.Multiline = false;
            this.txtHoursOffset.Name = "txtHoursOffset";
            this.txtHoursOffset.ReadOnly = false;
            this.txtHoursOffset.ReadOnlyColor = System.Drawing.SystemColors.Control;
            this.txtHoursOffset.ReadOnlyTextColor = System.Drawing.SystemColors.ControlLight;
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
            // DeviceConfigurationDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 116);
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
    }
}
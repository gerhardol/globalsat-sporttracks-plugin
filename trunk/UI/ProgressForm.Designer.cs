namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    partial class ProgressForm
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
            this.actionBanner1 = new ZoneFiveSoftware.Common.Visuals.ActionBanner();
            this.progressBar1 = new ZoneFiveSoftware.Common.Visuals.ProgressBar();
            this.buttonCancel = new ZoneFiveSoftware.Common.Visuals.Button();
            this.SuspendLayout();
            // 
            // actionBanner1
            // 
            this.actionBanner1.BackColor = System.Drawing.Color.Transparent;
            this.actionBanner1.HasMenuButton = false;
            this.actionBanner1.Location = new System.Drawing.Point(3, 3);
            this.actionBanner1.Name = "actionBanner1";
            this.actionBanner1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.actionBanner1.Size = new System.Drawing.Size(457, 40);
            this.actionBanner1.Style = ZoneFiveSoftware.Common.Visuals.ActionBanner.BannerStyle.Header1;
            this.actionBanner1.TabIndex = 0;
            this.actionBanner1.UseStyleFont = true;
            // 
            // progressBar1
            // 
            this.progressBar1.AnimateSpeedMs = 200;
            this.progressBar1.BackColor = System.Drawing.SystemColors.Window;
            this.progressBar1.Location = new System.Drawing.Point(45, 68);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Percent = 1F;
            this.progressBar1.Size = new System.Drawing.Size(383, 20);
            this.progressBar1.TabIndex = 1;
            // 
            // buttonCancel
            // 
            this.buttonCancel.BackColor = System.Drawing.Color.Transparent;
            this.buttonCancel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(120)))));
            this.buttonCancel.CenterImage = null;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.None;
            this.buttonCancel.HyperlinkStyle = false;
            this.buttonCancel.ImageMargin = 2;
            this.buttonCancel.LeftImage = null;
            this.buttonCancel.Location = new System.Drawing.Point(352, 114);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.PushStyle = true;
            this.buttonCancel.RightImage = null;
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.TextAlign = System.Drawing.StringAlignment.Center;
            this.buttonCancel.TextLeftMargin = 2;
            this.buttonCancel.TextRightMargin = 2;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // ProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(443, 118);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.actionBanner1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " ";
            this.ResumeLayout(false);

        }

        #endregion

        private ZoneFiveSoftware.Common.Visuals.ActionBanner actionBanner1;
        private ZoneFiveSoftware.Common.Visuals.ProgressBar progressBar1;
        private ZoneFiveSoftware.Common.Visuals.Button buttonCancel;
    }
}
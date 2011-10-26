/*
 *  Globalsat/Keymaze SportTracks Plugin
 *  Copyright 2009 John Philip 
 * 
 *  This software may be used and distributed according to the terms of the
 *  GNU Lesser General Public License version 2 or any later version.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.GPS;



namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    public partial class ProgressForm : Form
    {
        
        public event EventHandler Cancelled;


        public ProgressForm()
        {
            InitializeComponent();


            if (Plugin.Instance.Application != null)
            {
                ThemeChanged(Plugin.Instance.Application.VisualTheme);
            }

            buttonCancel.Text = CommonResources.Text.ActionCancel;


            actionBanner1.Width = this.ClientSize.Width;
            actionBanner1.Location = new Point(1, 1);
            progressBar1.Top = actionBanner1.Bottom + 40;
            progressBar1.Left = (this.ClientSize.Width - progressBar1.Width) / 2;

            buttonCancel.Top = progressBar1.Bottom + 40;
            this.ClientSize = new Size(this.ClientSize.Width, buttonCancel.Bottom + 5);


            this.Progress = 0;
            
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            MessageDialog.DrawButtonRowBackground(e.Graphics, this, theme);
        }



        public string Title
        {
            get { return actionBanner1.Text; }
            set { actionBanner1.Text = value; }
        }

        public double Progress
        {
            get { return progressBar1.Percent * 100.0; }
            set { progressBar1.Percent = (float)(value / 100.0); }
        }








        #region Public methods
        public void ThemeChanged(ITheme visualTheme)
        {
            theme = visualTheme;

            buttonCancel.ForeColor = visualTheme.ControlText;
            actionBanner1.ThemeChanged(visualTheme);
            progressBar1.ThemeChanged(visualTheme);
            this.BackColor = visualTheme.Control;
           
        }


        #endregion





        #region Private members
        private ITheme theme;
        #endregion

        private void buttonCancel_Click(object sender, EventArgs e)
        {

            //this.DialogResult = DialogResult.Cancel;
            if (Cancelled != null)
            {
                Cancelled(this, null);
            }
            this.Close();
        }






    }
}
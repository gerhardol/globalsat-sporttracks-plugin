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


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

using ZoneFiveSoftware.Common.Visuals;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    public partial class DeviceConfigurationDlg : Form
    {
        public DeviceConfigurationDlg(FitnessDevice_Globalsat fitnessDevice)
        {
            InitializeComponent();

            this.fitnessDevice = fitnessDevice;
            Text = CommonResources.Text.Devices.ConfigurationDialog_Title;
            chkImportOnlyNew.Text = Properties.Resources.DeviceConfigurationDlg_chkImportOnlyNew_Text;
            labelHoursOffset.Text = CommonResources.Text.Devices.ConfigurationDialog_HoursOffsetLabel_Text;
            btnOk.Text = CommonResources.Text.ActionOk;
            btnCancel.Text = CommonResources.Text.ActionCancel;

            if (Plugin.Instance.Application != null)
            {
                ThemeChanged(Plugin.Instance.Application.VisualTheme);
            }

            txtHoursOffset.Validated += new EventHandler(txtHoursOffset_Validated);
            btnOk.Click += new EventHandler(btnOk_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
            chkImportOnlyNew.Checked = this.fitnessDevice.configInfo.ImportOnlyNew;
            this.txtHoursOffset.Text = this.fitnessDevice.configInfo.HoursAdjustment.ToString();
            //COM Ports only configurable in xml
            this.buttonDetect.CenterImage = ZoneFiveSoftware.Common.Visuals.CommonResources.Images.Refresh16;
            this.buttonDetect.Text = "";// ZoneFiveSoftware.Common.Visuals.CommonResources.Text.ActionRefresh;
            this.labelDetect.Text = "";
        }

        #region Public properties

        internal DeviceConfigurationInfo ConfigurationInfo
        {
            get
            {
                this.fitnessDevice.configInfo.ImportOnlyNew = chkImportOnlyNew.Checked;
                return this.fitnessDevice.configInfo;
            }
        }
        #endregion

        #region Public methods
        public void ThemeChanged(ITheme visualTheme)
        {
            theme = visualTheme;
            labelHoursOffset.ForeColor = visualTheme.ControlText;
            txtHoursOffset.ThemeChanged(visualTheme);
            chkImportOnlyNew.ForeColor = visualTheme.ControlText;
            BackColor = visualTheme.Control;
        }
        #endregion

        #region Event handlers
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
#if ST_2_1
            MessageDialog.DrawButtonRowBackground(e.Graphics, this.ClientRectangle, theme);
#else
            MessageDialog.DrawButtonRowBackground(e.Graphics, this, theme);
#endif
        }

        void txtHoursOffset_Validated(object sender, EventArgs e)
        {
            try
            {
                if (txtHoursOffset.Text.Trim().Length == 0)
                {
                    this.fitnessDevice.configInfo.HoursAdjustment = 0;
                }
                else
                {
                    this.fitnessDevice.configInfo.HoursAdjustment = (int)double.Parse(txtHoursOffset.Text);
                }
            }
            catch { }
            txtHoursOffset.Text = this.fitnessDevice.configInfo.HoursAdjustment.ToString();
        }

        void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = btnOk.DialogResult;
            Close();
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = btnCancel.DialogResult;
            Close();
        }

        #endregion

        #region Private methods

        #endregion

        #region Private members
        private ITheme theme;
        private FitnessDevice_Globalsat fitnessDevice;
        #endregion

        private void buttonDetect_Click(object sender, EventArgs e)
        {
            this.labelDetect.Text = this.fitnessDevice.Detect();
        }
    }
}
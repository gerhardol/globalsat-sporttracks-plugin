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
        public DeviceConfigurationDlg(DeviceConfigurationInfo configInfo)
        {
            InitializeComponent();

            this.configInfo = configInfo;
            Text = CommonResources.Text.Devices.ConfigurationDialog_Title;
            chkImportOnlyNew.Text = ResourceLookup.DeviceConfigurationDlg_chkImportOnlyNew_Text;
            labelHoursOffset.Text = CommonResources.Text.Devices.ConfigurationDialog_HoursOffsetLabel_Text;
            labelComPort.Text = "COM Port:"; //TODO: Translate
            btnOk.Text = CommonResources.Text.ActionOk;
            btnCancel.Text = CommonResources.Text.ActionCancel;

            if (Plugin.Instance.Application != null)
            {
                ThemeChanged(Plugin.Instance.Application.VisualTheme);
            }

            txtHoursOffset.Validated += new EventHandler(txtHoursOffset_Validated);
            btnOk.Click += new EventHandler(btnOk_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
        }

        #region Public properties

        internal DeviceConfigurationInfo ConfigurationInfo
        {
            get
            {
                configInfo.ImportOnlyNew = chkImportOnlyNew.Checked;
                configInfo.ComPortsText = textBoxComPort.Text;
                return configInfo;
            }
        }
        #endregion

        #region Public methods
        public void ThemeChanged(ITheme visualTheme)
        {
            theme = visualTheme;
            labelHoursOffset.ForeColor = visualTheme.ControlText;
            txtHoursOffset.ThemeChanged(visualTheme);
            textBoxComPort.ThemeChanged(visualTheme);
            chkImportOnlyNew.ForeColor = visualTheme.ControlText;
            BackColor = visualTheme.Control;
        }
        #endregion

        #region Event handlers
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            MessageDialog.DrawButtonRowBackground(e.Graphics, this, theme);
        }

        void txtHoursOffset_Validated(object sender, EventArgs e)
        {
            try
            {
                if (txtHoursOffset.Text.Trim().Length == 0)
                {
                    configInfo.HoursAdjustment = 0;
                }
                else
                {
                    configInfo.HoursAdjustment = (int)double.Parse(txtHoursOffset.Text);
                }
            }
            catch { }
            txtHoursOffset.Text = configInfo.HoursAdjustment.ToString();
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
        private DeviceConfigurationInfo configInfo;
        #endregion

    }
}
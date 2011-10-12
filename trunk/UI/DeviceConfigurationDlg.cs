// Copyright (C) 2010 Zone Five Software
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
        public DeviceConfigurationDlg()
        {
            InitializeComponent();

            Text = CommonResources.Text.Devices.ConfigurationDialog_Title;
            chkImportOnlyNew.Text = ResourceLookup.DeviceConfigurationDlg_chkImportOnlyNew_Text;
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
        }

        #region Public properties

        internal DeviceConfigurationInfo ConfigurationInfo
        {
            get
            {
                DeviceConfigurationInfo configInfo = DeviceConfigurationInfo.Parse(null);
                configInfo.ImportOnlyNew = chkImportOnlyNew.Checked;
                configInfo.HoursAdjustment = hoursAdjustment;
                return configInfo;
            }
            set
            {
                chkImportOnlyNew.Checked = value.ImportOnlyNew;
                hoursAdjustment = value.HoursAdjustment;
                txtHoursOffset.Text = hoursAdjustment.ToString();
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
            MessageDialog.DrawButtonRowBackground(e.Graphics, this, theme);
        }

        void txtHoursOffset_Validated(object sender, EventArgs e)
        {
            int value = hoursAdjustment;
            try
            {
                if (txtHoursOffset.Text.Trim().Length == 0)
                {
                    value = 0;
                }
                else
                {
                    value = (int)double.Parse(txtHoursOffset.Text);
                }
            }
            catch { }
            if (value != hoursAdjustment)
            {
                hoursAdjustment = value;
            }
            txtHoursOffset.Text = hoursAdjustment.ToString();
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
        private int hoursAdjustment = 0;
        #endregion

    }
}
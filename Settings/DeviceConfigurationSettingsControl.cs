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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Diagnostics;

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Data.Measurement;
using ZoneFiveSoftware.Common.Visuals;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    public partial class DeviceConfigurationSettingsControl : UserControl
    {
        public DeviceConfigurationSettingsControl()
        {
            InitializeComponent();

            buttonImportDeviceConfig.LeftImage = CommonResources.Images.Import16;
            buttonExportDeviceConfig.LeftImage = CommonResources.Images.Export16;

            buttonImportDeviceConfig.Text = Properties.Resources.UI_Settings_ImportConfigButton_Text;
            buttonExportDeviceConfig.Text = Properties.Resources.UI_Settings_ExportConfigButton_Text;

            groupBoxDeviceConfig.Text = Properties.Resources.UI_Settings_DeviceConfiguration_Title;
        }

        #region Public methods

        public void ShowPage()
        {
        }



        public void ThemeChanged(ITheme visualTheme)
        {
            groupBoxDeviceConfig.ForeColor = visualTheme.ControlText;
            this.BackColor = visualTheme.Control;
        }

        public void UICultureChanged(CultureInfo culture)
        {
            buttonImportDeviceConfig.Text = Properties.Resources.UI_Settings_ImportConfigButton_Text;
            buttonExportDeviceConfig.Text = Properties.Resources.UI_Settings_ExportConfigButton_Text;

            groupBoxDeviceConfig.Text = Properties.Resources.UI_Settings_DeviceConfiguration_Title;

        }
        #endregion

        private void buttonImportDeviceConfig_Click(object sender, EventArgs e)
        {

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                GlobalsatDeviceConfiguration importedDeviceConfig = null;

                try
                {
                    importedDeviceConfig = GlobalsatDeviceConfiguration.Load(openFileDialog1.FileName);
                }
                catch (Exception)
                {
                    MessageDialog.Show(Properties.Resources.UI_Settings_ImportConfig_InvalidConfiguration, Properties.Resources.UI_Settings_ImportConfig_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    IJobMonitor jobMonitor = new JobMonitor();
                    GenericDevice device = new GenericDevice();
                    GlobalsatProtocol device2 = device.Device(jobMonitor);
                    if (device2 != null)
                    {
                        GlobalsatDeviceConfiguration currentDeviceConfig = device2.GetDeviceConfigurationData(jobMonitor);

                        if (importedDeviceConfig != null && importedDeviceConfig.DeviceName == currentDeviceConfig.DeviceName && importedDeviceConfig.SystemConfigData.Length == currentDeviceConfig.SystemConfigData.Length)
                        {
                            device2.SetDeviceConfigurationData(importedDeviceConfig, jobMonitor);
                        }
                        else
                        {
                            MessageDialog.Show(Properties.Resources.UI_Settings_ImportConfig_InvalidConfiguration, Properties.Resources.UI_Settings_ImportConfig_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageDialog.Show(ex.Message, Properties.Resources.UI_Settings_ImportConfig_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void buttonExportDeviceConfig_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                   IJobMonitor jobMonitor = new JobMonitor();
                    GenericDevice device = new GenericDevice();
                    GlobalsatProtocol device2 = device.Device(jobMonitor);
                    if (device2 != null)
                    {
                        GlobalsatDeviceConfiguration currentDeviceConfig = device2.GetDeviceConfigurationData(jobMonitor);

                        currentDeviceConfig.Save(saveFileDialog1.FileName);

                        //MessageDialog.Show(CommonResources.Text.MessageExportComplete, Properties.Resources.UI_Settings_ExportConfigButton_Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageDialog.Show(ex.Message, Properties.Resources.UI_Settings_ExportConfig_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }
    }
}

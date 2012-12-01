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
    public partial class ScreenCaptureControl : UserControl
    {
        public ScreenCaptureControl()
        {
            InitializeComponent();

            buttonCaptureScreen.CenterImage = CommonResources.Images.Refresh16;
            buttonSave.CenterImage = CommonResources.Images.Save16;

            buttonSave.Enabled = false;
            pictureBox1.Left = buttonCaptureScreen.Right + 8;

            if (Plugin.Instance.Application != null)
            {
                ThemeChanged(Plugin.Instance.Application.VisualTheme);
            }
        }

        #region Public methods

        public void ShowPage()
        {
        }

        public void ThemeChanged(ITheme visualTheme)
        {
            groupBoxScreenCapture.ForeColor = visualTheme.ControlText;
            this.BackColor = visualTheme.Control;
        }

        public void UICultureChanged(CultureInfo culture)
        {
            toolTip1.SetToolTip(buttonCaptureScreen, Properties.Resources.UI_Settings_ScreenCapture_CaptureScreen);
            toolTip1.SetToolTip(buttonSave, Properties.Resources.UI_Settings_ScreenCapture_SaveImage);

            groupBoxScreenCapture.Text = Properties.Resources.UI_Settings_ScreenCapture_Title;
        }
        #endregion

        private void buttonCaptureScreen_Click(object sender, EventArgs e)
        {
            try
            {
                IJobMonitor jobMonitor = new JobMonitor();
                FitnessDevice_GsSport device = new FitnessDevice_GsSport();
                GlobalsatProtocol device2 = device.Device();
                if (device2 != null)
                {
                    Bitmap screenshot = device2.GetScreenshot(jobMonitor);

                    if (screenshot != null)
                    {
                        this.groupBoxScreenCapture.Height += screenshot.Height - pictureBox1.Height;
                        pictureBox1.ClientSize = new Size(screenshot.Width, screenshot.Height);
                        pictureBox1.Image = screenshot;

                        buttonSave.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageDialog.Show(ex.Message, Properties.Resources.UI_Settings_ScreenCapture_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }


        private void buttonSave_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.Filter = "Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (pictureBox1.Image != null)
                    {
                        System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Bmp;
                        if (saveFileDialog1.FilterIndex == 1)
                        {
                            format = System.Drawing.Imaging.ImageFormat.Jpeg;
                        }
                        else if (saveFileDialog1.FilterIndex == 2)
                        {
                            format = System.Drawing.Imaging.ImageFormat.Png;
                        }
                        pictureBox1.Image.Save(saveFileDialog1.FileName, format);
                    }
                }
                catch (Exception ex)
                {
                    MessageDialog.Show(ex.Message, Properties.Resources.UI_Settings_ScreenCaptureSave_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }
    }
}

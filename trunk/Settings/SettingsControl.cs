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

//using KeymazePlugin.Device;



namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    public partial class SettingsControl : UserControl
    {
        public SettingsControl()
        {
            InitializeComponent();



            if (Plugin.Instance.Application != null)
            {
                labelName.Text = Plugin.Instance.Name;
                ThemeChanged(Plugin.Instance.Application.VisualTheme);
            }

        }





        #region Public methods

        public void ShowPage()
        {
            
        }



        public void ThemeChanged(ITheme visualTheme)
        {

            this.BackColor = visualTheme.Control;

        }

        public void UICultureChanged(CultureInfo culture)
        {
            //this.labelGlobalsatDevice.Text = Integration.GlobalsatDevice.CompabilityText;
            //Some untranslated strings....
            this.labelLicense.Text = "Waypoints Plugin is distributed under the GNU Lesser General Public Licence.\r\nThe Li" +
                "cense is included in the plugin installation directory and at:\r\nhttp://www.gnu.o" +
                "rg/licenses/lgpl.html.";

        }
        #endregion






        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("mailto:john.philip@sapo.pt");
            }
            catch { }
        }




    }
}

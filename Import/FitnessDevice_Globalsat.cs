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
using System.Drawing;
using System.Windows.Forms;

using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    public class FitnessDevice_Globalsat : IFitnessDevice
    {
        public FitnessDevice_Globalsat()
        {
            this.id = new Guid("16a23ea0-f5cc-11e0-be50-0800200c9a66");
            this.image = Properties.Resources.Image_48_GSSPORT;
            this.name = "Globalsat";
        }

        public Guid Id
        {
            get { return id; }
        }

        public string Name
        {
            get { return name; }
        }

        public Image Image
        {
            get { return image; }
        }

        public string ConfiguredDescription(string configurationInfo)
        {
            return Name;
        }

        public string Configure(string configurationInfo)
        {
            DeviceConfigurationInfo configInfo = DeviceConfigurationInfo.Parse(DefaultConfig, configurationInfo);
            DeviceConfigurationDlg dialog = new DeviceConfigurationDlg(configInfo);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.ConfigurationInfo.ToString();
            }
            else
            {
                return null;
            }
        }

        public virtual bool Import(string configurationInfo, IJobMonitor monitor, IImportResults importResults)
        {
            GenericDevice device = new GenericDevice(DeviceConfigurationInfo.Parse(DefaultConfig, configurationInfo));
            GlobalsatProtocol device2 = device.Device(monitor);
            if (device2 != null)
            {
                ImportJob job = device2.ImportJob(ConfiguredDescription(configurationInfo), monitor, importResults);
                if (job == null)
                {
                    return false;
                }
                return job.Import();
            }
            return false;
        }

        public virtual DeviceConfigurationInfo DefaultConfig
        {
            get
            {
                DeviceConfigurationInfo info = new DeviceConfigurationInfo();
                info.BaudRates = new List<int> { 115200, 57600 };
                return info;
            }
        }

        #region Private members
        protected Guid id;
        protected Image image;
        protected string name;
        #endregion
    }
}

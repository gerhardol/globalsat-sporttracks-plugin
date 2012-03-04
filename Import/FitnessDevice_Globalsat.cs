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

        protected virtual GlobalsatProtocol Device(string configurationInfo) { return new GenericDevice(configurationInfo); }

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
            DeviceConfigurationInfo configInfo = DeviceConfigurationInfo.Parse(Device(configurationInfo).DefaultConfig, configurationInfo);
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

        public bool Import(string configurationInfo, IJobMonitor monitor, IImportResults importResults)
        {
            bool result = false;
            GlobalsatProtocol device0 = Device(configurationInfo);
            if (device0 is GenericDevice)
            {
                //Determine the device, then dispatch
                GenericDevice device = (GenericDevice)device0;
                GlobalsatProtocol device2 = device.Device(monitor);
                if (device2 != null)
                {
                    try
                    {
                        ImportJob job = device2.ImportJob(ConfiguredDescription(configurationInfo) + " - " + device.devId,
                            monitor, importResults);

                        if (job == null)
                        {
                            monitor.ErrorText = "Import not supported for " + device.devId;
                            result = false;
                        }
                        else
                        {
                            result = job.Import();
                        }
                    }
                    catch (NotImplementedException)
                    {
                        result = false;
                    }
                }
            }
            else
            {
                //import for specific device
                monitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_OpeningDevice;
                GlobalsatProtocol device = device0;
                ImportJob job = device.ImportJob(ConfiguredDescription(configurationInfo), monitor, importResults);
                result = job.Import();
            }
            return result;
        }

        #region Private members
        protected Guid id;
        protected Image image;
        protected string name;
        #endregion
    }
}

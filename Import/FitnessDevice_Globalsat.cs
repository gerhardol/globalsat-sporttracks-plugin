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
    public abstract class FitnessDevice_Globalsat : GhConfig, IFitnessDevice
    {
        public virtual GlobalsatProtocol Device() { return this.device; }

        public abstract GlobalsatPacket PacketFactory { get; }

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
            this.configInfo = DeviceConfigurationInfo.Parse(this.configInfo, configurationInfo);
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
            this.configInfo = DeviceConfigurationInfo.Parse(this.configInfo, configurationInfo);
            //GlobalsatProtocol device0 = this.Device();
            bool generic = this is FitnessDevice_Globalsat;
            //if (device0 is GenericDevice)
            //{
            //    //Determine the device, then dispatch
            //    GlobalsatProtocol device2 = device.Device(monitor).Device();
            //    if (device2 != null)
            //    {
            //        try
            //        {
            //            ImportJob job = device2.ImportJob(ConfiguredDescription(configurationInfo) + " - " + this.Device().devId,
            //                monitor, importResults);

            //            if (job == null)
            //            {
            //                monitor.ErrorText = "Import not supported for " + this.Device().devId;
            //                result = false;
            //            }
            //            else
            //            {
            //                result = job.Import();
            //            }
            //        }
            //        catch (NotImplementedException)
            //        {
            //            result = false;
            //        }
            //    }
            //}
            //else
            {
                //import for specific device - Importjob must be implemented
                monitor.PercentComplete = 0;
                string str = ConfiguredDescription(configurationInfo);
                if (!generic)
                {
                    monitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_OpeningDevice;
                }
                ///GlobalsatProtocol device = device0;
                try
                {
                    string cfgDesc = ConfiguredDescription(configurationInfo);
                    if (generic)
                    {
                        cfgDesc += " - " + this.Device().devId;
                    }
                    ImportJob job = this.Device().ImportJob(cfgDesc, monitor, importResults);
                    if (job == null)
                    {
                        monitor.ErrorText = "Import not supported for " + this.Device().devId;
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
            if (!result)
            {
                monitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_ImportError;
            }
            return result;
        }

        public string Detect()
        {
            string identification = "Error";
            try
            {
                this.Device().Open();
                //FitnessDevice_Globalsat device2 = this.FitnessDevice;//xxx .Device(new JobMonitor());
                if (this.Device().Open())
                {
                    if (this.configInfo.AllowedIds == null || this.configInfo.AllowedIds.Count == 0)
                    {
                        identification = this.Device().devId + " (Globalsat Generic)";
                    }
                    else
                    {
                        bool found = false;
                        foreach (string s in this.configInfo.AllowedIds)
                        {
                            if (this.Device().devId.Equals(s))
                            {
                                found = true;
                                identification = this.Device().devId;
                            }
                        }
                        if (!found)
                        {
                            identification = this.Device().devId + " (" + this.configInfo.AllowedIds[0] + " Compatible)";
                        }
                    }
                    identification += " on " + this.LastValidComPort;
                }
                else
                {
                    identification = this.Device().lastDevId + " (" + ZoneFiveSoftware.Common.Visuals.CommonResources.Text.Devices.ImportJob_Status_CouldNotOpenDeviceError + ")";
                }
            }
            catch (Exception)
            {
                identification = Properties.Resources.Device_OpenDevice_Error;
            }
            finally
            {
                this.Device().Close();
            }
            return identification;
        }

        #region Private members
        protected GlobalsatProtocol device;
        protected Guid id;
        protected Image image;
        protected string name;
        #endregion
    }
}

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

using ZoneFiveSoftware.Common.Data.Fitness;
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
            get { return this.id; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public Image Image
        {
            get { return this.image; }
        }

        public string ConfiguredDescription(string configurationInfo)
        {
            return this.Name;
        }

        public string Configure(string configurationInfo)
        {
            this.configInfo.Parse(configurationInfo);
            DeviceConfigurationDlg dialog = new DeviceConfigurationDlg(this, true);
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.ConfigurationInfo.ToString();
            }
            else
            {
                return null;
            }
        }

        protected IConfiguredDevice ConfigurationDevice
        {
            get
            {
                if (configuredDevice == null)
                {
                    foreach (IConfiguredDevice d in Plugin.Instance.Application.SystemPreferences.FitnessDevices)
                    {
                        if (this.id.Equals(d.Id))
                        {
                            configuredDevice = d;
                        }
                    }
                }
                return configuredDevice;
            }
        }

        protected void GetConfigurationString()
        {
            IConfiguredDevice d = this.ConfigurationDevice;
            if (d != null)
            {
                this.configInfo.Parse(d.Configuration);
            }
            else { }
        }

        //Save dynamic information (currently lastPort)
        internal void SetDynamicConfigurationString()
        {
            if (configuredDevice == null)
            {
                configuredDevice = new GSConfiguredDevice(this.configInfo.ToString(), this.id);
                Plugin.Instance.Application.SystemPreferences.FitnessDevices.Add(configuredDevice);
            }

            if (this.configInfo.DynamicInfoChanged(configuredDevice.Configuration))
            {
                //Only save if dynamic information changed
                configuredDevice.Configuration = this.configInfo.ToString();
            }
        }

        public bool Import(string configurationInfo, IJobMonitor monitor, IImportResults importResults)
        {
            bool result = false;
            this.configInfo.Parse(configurationInfo);
            bool generic = this is FitnessDevice_GsSport;
            //import for specific device - Importjob must be implemented
            monitor.PercentComplete = 0;

            try
            {
                if (generic)
                {
                    //Always retry to reimport (other use create new device at each use, but device kept at import)
                    FitnessDevice_GsSport dev = (this as FitnessDevice_GsSport);
                    dev.DetectionAttempted = false;
                }
                else
                {
                    //Only change status message for non generic
                    monitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_OpeningDevice;
                }

                if (this.Device() != null)
                {
                    string cfgDesc = ConfiguredDescription(configurationInfo);
                    if (generic)
                    {
                        cfgDesc += " - " + this.Device().devId;
                    }
                    ImportJob job = this.Device().ImportJob(cfgDesc, monitor, importResults);
                    if (job == null)
                    {
                        string devId = this.Device().devId;
                        if (string.IsNullOrEmpty(devId))
                        {
                            devId = this.Name;
                        }
                        monitor.ErrorText = "Import not supported for " + devId;
                        result = false;
                    }
                    else
                    {
                        result = job.Import();
                    }
                }
            }
            catch (NotImplementedException)
            {
                monitor.ErrorText = ZoneFiveSoftware.SportTracks.Device.Globalsat.Properties.Resources.Device_Unsupported;
                result = false;
            }
            return result;
        }

        public string Detect()
        {
            return Detect(true);
        }

        public string Detect(bool query)
        {
            string identification = "Detection Error";
            try
            {
                if (!query || this.Device().Open())
                {
                    FitnessDevice_Globalsat cmpFitness = this;
                    if (this is FitnessDevice_GsSport)
                    {
                        cmpFitness = (this as FitnessDevice_GsSport).FitnessDevice;
                    }

                    //devId and lastDevId should not be null
                    if (cmpFitness.configInfo.AllowedIds == null || cmpFitness.configInfo.AllowedIds.Count == 0)
                    {
                        identification = cmpFitness.Device().devId + " (Globalsat Generic)";
                    }
                    else
                    {
                        bool found = false;
                        foreach (string s in cmpFitness.configInfo.AllowedIds)
                        {
                            if (cmpFitness.Device().devId.Equals(s))
                            {
                                found = true;
                                identification = cmpFitness.Device().devId;
                                break;
                            }
                        }
                        if (!found)
                        {
                            identification = cmpFitness.Device().devId + " (" + cmpFitness.Name + " Compatible)";
                        }
                    }
                    IList<string> s2 = this.configInfo.GetLastValidComPorts();
                    if (s2 != null && s2.Count > 0)
                    {
                        identification += " on " + s2[0];
                    }
                }
                else
                {
                    identification = this.Device().lastDevId + " (" + ZoneFiveSoftware.Common.Visuals.CommonResources.Text.Devices.ImportJob_Status_CouldNotOpenDeviceError + ")";
                }
            }
            catch (Exception e)
            {
                identification = Properties.Resources.Device_OpenDevice_Error + " (Detect)" + e;
            }
            finally
            {
                if (this.Device() != null)
                {
                    //this.Device().DataRecieved Should not be handled here
                    this.Device().Close();
                }
            }
            return identification;
        }

        #region Private members
        protected GlobalsatProtocol device;
        protected IConfiguredDevice configuredDevice;
        protected Guid id;
        protected Image image;
        protected string name;
        #endregion
    }

    public class GSConfiguredDevice : IConfiguredDevice
    {
        public GSConfiguredDevice(string cfg, Guid id)
        {
            this.cfg = cfg;
            this.id = id;
        }

        public string Configuration
        {
            get { return cfg; }
            set { cfg = value; }
        }
        public Guid Id { get { return id; } }

        private string cfg;
        private Guid id;
    }

}

/*
Copyright (C) 2011 Gerhard Olsson

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


using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

using ZoneFiveSoftware.Common.Visuals;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    //A generic device that should resolve to the actual device
    //There should be no methods here, all should use Device()
    class GenericDevice : GlobalsatProtocol
    {
        public GenericDevice() : base() { }
        public GenericDevice(string configInfo) : base(configInfo) { }

        public override GlobalsatPacket PacketFactory { get { return new GlobalsatPacket(); } }

        public string devId = null;
        /* Autodetect device, it is up to the caller to cache the device */
        public GlobalsatProtocol Device(IJobMonitor monitor)
        {
            monitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_OpeningDevice;
            devId = base.Open();
            //No close here
            if (!string.IsNullOrEmpty(devId))
            {
                IList<GlobalsatProtocol> Devices = new List<GlobalsatProtocol> { new Gh625XTDevice(), new Gh625Device(), new Gb580Device(), new Gh505Device(), new Gh615Device(), new Gh561Device() };
                foreach (GlobalsatProtocol g in Devices)
                {
                    if (g.DefaultConfig.AllowedIds != null)
                    {
                        foreach (string s in g.DefaultConfig.AllowedIds)
                        {
                            if (devId.StartsWith(s))
                            {
                                g.CopyPort(this);
                                //Copy settings from generic
                                g.configInfo.HoursAdjustment = this.configInfo.HoursAdjustment;
                                g.configInfo.ImportOnlyNew = this.configInfo.ImportOnlyNew;
                                return g;
                            }
                        }
                    }
                }
            }
            monitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_ImportError;
            return null;
        }

        public string Detect()
        {
            string result = "Error";
            try
            {
                string devId = this.Open();
                GlobalsatProtocol device2 = this.Device(new JobMonitor());
                if (device2 != null)
                {
                    if (device2.configInfo.AllowedIds == null || device2.configInfo.AllowedIds.Count == 0)
                    {
                        devId += " (Globalsat Generic)";
                    }
                    else
                    {
                        bool found = false;
                        foreach (string s in device2.DefaultConfig.AllowedIds)
                        {
                            if (devId.Equals(s))
                            {
                                found = true;
                            }
                        }
                        if (!found)
                        {
                            devId += " (" + device2.configInfo.AllowedIds[0] + " Compatible)";
                        }
                    }
                }
                else
                {
                    devId += " (" + ZoneFiveSoftware.Common.Visuals.CommonResources.Text.Devices.ImportJob_Status_CouldNotOpenDeviceError + ")";
                }
                result = devId;
            }
            catch (Exception)
            {
                result = Properties.Resources.Device_OpenDevice_Error;
            }
            finally
            {
                this.Close();
            }
            return result;
        }
    }
}

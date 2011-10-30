/*
Copyright (C) 2011 Gerhard Olsson

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


using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    //A generic device that should resolve to the actual device
    //There should be no methods here, all should use Device()
    class GenericDevice : GlobalsatProtocol
    {
        public GenericDevice(DeviceConfigurationInfo configInfo) : base(configInfo) { }
        public GenericDevice() : base(new FitnessDevice_Globalsat()) { }
        public override GlobalsatPacket PacketFactory { get { return GlobalsatProtocol.PacketFactoryBase; } }

        /* Autodetect device, it is up to the caller to cache the device */
        public GlobalsatProtocol Device(IJobMonitor monitor)
        {
            monitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_OpeningDevice;
            string devId = null;
            devId = base.Open();
            if (!string.IsNullOrEmpty(devId))
            {
                IList<GlobalsatProtocol> Devices = new List<GlobalsatProtocol> { new Gh625XTDevice(), new Gh625Device(), new Gb580Device(), new Gh505Device(), new Gh615Device(), new Gh561Device() };
                foreach (GlobalsatProtocol g in Devices)
                {
                    if (g.configInfo.AllowedIds != null)
                    {
                        foreach (string s in g.configInfo.AllowedIds)
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
    }
}

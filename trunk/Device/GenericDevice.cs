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
// Author: Gerhard Olsson


using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    //A generic device that should resolve to the actual device
    class GenericDevice : GhDeviceBase
    {
        public override ImportJob ImportJob(string sourceDescription, DeviceConfigurationInfo configInfo, IJobMonitor monitor, IImportResults importResults)
        {
            if (device == null) { return null; }
            return device.ImportJob(sourceDescription, configInfo, monitor, importResults);
        }

        public GhDeviceBase Device(DeviceConfigurationInfo configInfo)
        {
            string devId = null;
            devId = base.Open(configInfo);
            if (!string.IsNullOrEmpty(devId))
            {
                foreach (GhDeviceBase g in Devices)
                {
                    if (g.AllowedIds != null)
                    {
                        foreach (string s in g.AllowedIds)
                        {
                            if (devId.StartsWith(s))
                            {
                                g.CopyPort(this);
                                device = g;
                                return g;
                            }
                        }
                    }

                }
            }
            return null;
        }

        GhDeviceBase device = null;
        private IList<GhDeviceBase> Devices = new List<GhDeviceBase> { new Gh625XTDevice(), new Gh625Device(), new Gb580Device(), new Gh615Device() };
        protected override IList<int> BaudRates { get { return new List<int> { 115200, 57600 }; } }
    }
}

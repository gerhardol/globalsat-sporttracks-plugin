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
    class Gh625XTDevice : GlobalsatProtocol2
    {
        public Gh625XTDevice() : base() { }
        public Gh625XTDevice(string configInfo) : base(configInfo) { }

        public override DeviceConfigurationInfo DefaultConfig
        {
            get
            {
                DeviceConfigurationInfo info = new DeviceConfigurationInfo(new List<string> { "GH-625XT" }, new List<int> { 115200 });
                return info;
            }
        }

        public override GlobalsatPacket PacketFactory { get { return new Gh625XTPacket(); } }
        
        public override ImportJob ImportJob(string sourceDescription, IJobMonitor monitor, IImportResults importResults)
        {
            return new ImportJob_GH625XT(this, sourceDescription, monitor, importResults);
        }

        public override int SendWaypoints(IList<GlobalsatWaypoint> waypoints, IJobMonitor jobMonitor)
        {
            this.Open();
            try
            {
                int nrSentWaypoints = 0;
                foreach (GlobalsatWaypoint g in waypoints)
                {
                    GlobalsatPacket packet = PacketFactory.SendWaypoints(this.configInfo.MaxNrWaypoints, new List<GlobalsatWaypoint> { g });
                    GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);

                    // km500 no out of memory- waypoint overwritten
                    nrSentWaypoints += response.ResponseSendWaypoints();
                }
                return nrSentWaypoints;
            }
            catch
            {
                throw new Exception(Properties.Resources.Device_SendWaypoints_Error);
            }
            finally
            {
                this.Close();
            }
        }
    }
}

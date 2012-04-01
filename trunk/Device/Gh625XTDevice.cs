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
// Author: Gerhard Olsson


using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

using ZoneFiveSoftware.Common.Visuals;

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

        //Timeout when detecting
        public override int ReadTimeoutDetect { get { return 100; } }

        public override int SendWaypoints(IList<GlobalsatWaypoint> waypoints, IJobMonitor jobMonitor)
        {
            int nrSentWaypoints = 0;
            if (this.Open())
            {
                try
                {
                    foreach (GlobalsatWaypoint g in waypoints)
                    {
                        GlobalsatPacket packet = PacketFactory.SendWaypoints(this.configInfo.MaxNrWaypoints, new List<GlobalsatWaypoint> { g });
                        GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);

                        nrSentWaypoints += response.ResponseSendWaypoints();
                    }
                }
                catch (Exception ex)
                {
                    if (this.DataRecieved)
                    {
                        throw new Exception(Properties.Resources.Device_SendWaypoints_Error + ex);
                    }
                }
                finally
                {
                    this.Close();
                }
            }
            if (!this.DataRecieved)
            {
                //Normal case
                NoCommunicationError(jobMonitor);
                return 0;
            }
            return nrSentWaypoints;
        }
        public override bool RouteRequiresWaypoints { get { return false; } }
    }
}

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
using System.Text;
using System.IO.Ports;

using ZoneFiveSoftware.Common.Visuals;


namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class Gb580Device : GlobalsatProtocol2
    {
        public Gb580Device(FitnessDevice_GB580 fitnessDevice)
            : base(fitnessDevice)
        {
        }

        public override int SendRoute(IList<GlobalsatRoute> routes, IJobMonitor jobMonitor)
        {
            jobMonitor.ErrorText = ZoneFiveSoftware.SportTracks.Device.Globalsat.Properties.Resources.Device_Unsupported;
            return -1;
        }
    }
}

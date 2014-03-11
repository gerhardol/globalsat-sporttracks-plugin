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

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class Gb1000Packet : Gb580Packet
    {
        public Gb1000Packet(FitnessDevice_Globalsat device) : base(device) { }

        //Override unsupported
        public override GlobalsatPacket SendTrackStart(Train trackFile) { throw new GlobalsatProtocol.FeatureNotSupportedException(); }
        public override GlobalsatPacket SendTrackLaps(Train trackFile) { throw new GlobalsatProtocol.FeatureNotSupportedException(); }
        private int WriteTrackHeader(int offset, int noOfLaps, Header trackFile) { throw new GlobalsatProtocol.FeatureNotSupportedException(); }
        protected override int WriteTrackPointHeader(int offset, Train trackFile, int StartPointIndex, int EndPointIndex) { throw new GlobalsatProtocol.FeatureNotSupportedException(); }
        protected override int WriteTrackPoint(int offset, TrackPoint trackpoint) { throw new GlobalsatProtocol.FeatureNotSupportedException(); }

        public override GlobalsatSystemConfiguration2 ResponseGetSystemConfiguration2()
        {
            const int SystemConfiguration2 = 52;
            if (this.PacketLength < SystemConfiguration2)
            {
                ReportOffset(this.PacketLength, SystemConfiguration2);
                return null;
            }

            GlobalsatSystemConfiguration2 systemInfo = new GlobalsatSystemConfiguration2();
            systemInfo.cRecordTime = 2; //Hardcoded

            return systemInfo;
        }
    }
}

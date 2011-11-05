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
// Author: Aaron Averill


using System;
using System.Collections.Generic;
using System.Text;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class Gh615Packet : GlobalsatPacket
    {
        public new class Header : GhPacketBase.Header
        {
            //public DateTime StartTime;
            //public TimeSpan TotalTime;
            //public Int32 TotalDistanceMeters;
            public Int16 TotalCalories;
            public double MaximumSpeed;
            public byte MaximumHeartRate;
            public byte AverageHeartRate;
            //public Int16 TrackPointCount;
        }

        public class TrackFileHeader : Header
        {
            public Int16 TrackPointIndex;
        }

        public class TrackFileSection : Header
        {
            public Int16 StartPointIndex;
            public Int16 EndPointIndex;
            public IList<TrackPoint1> TrackPoints = new List<TrackPoint1>();
        }

        public IList<TrackFileHeader> UnpackTrackHeaders()
        {
            int numHeaders = this.PacketData.Length / 24;
            IList<TrackFileHeader> headers = new List<TrackFileHeader>();
            for (int i = 0; i < numHeaders; i++)
            {
                int trackStart = i*24;
                TrackFileHeader header = new TrackFileHeader();
                ReadHeader(header, trackStart);
                header.TrackPointCount = ReadInt16(trackStart + 20);
                header.TrackPointIndex = ReadInt16(trackStart + 22);
                headers.Add(header);
            }
            return headers;
        }

        public TrackFileSection UnpackTrackSection()
        {
            if (this.PacketData.Length < 26) return null;

            TrackFileSection section = new TrackFileSection();
            ReadHeader(section, 0);
            section.TrackPointCount = ReadInt16(20);
            section.StartPointIndex = ReadInt16(22);
            section.EndPointIndex = ReadInt16(24);
            int offset = 26;
            while (offset < this.PacketData.Length)
            {
                TrackPoint1 point = new TrackPoint1();
                point.Latitude = (double)ReadInt32(offset) / 1000000;
                point.Longitude = (double)ReadInt32(offset + 4) / 1000000;
                point.Altitude = ReadInt16(offset + 8);
                point.Speed = ReadInt16(offset + 10) / 3.6 / 100;
                point.HeartRate = this.PacketData[offset + 12];
                point.IntervalTime = ReadInt16(offset + 13);
                section.TrackPoints.Add(point);
                offset += 15;
            }
            return section;
        }

        private void ReadHeader(Header header, int offset)
        {
            header.StartTime = ReadDateTime(offset);
            header.TotalTime = TimeSpan.FromSeconds(((double)ReadInt32(offset + 6)) / 10);
            header.TotalDistanceMeters = ReadInt32(offset + 10);
            header.TotalCalories = ReadInt16(offset + 14);
            header.MaximumSpeed = ReadInt16(offset + 16) / 3.6 / 100;
            header.MaximumHeartRate = this.PacketData[offset + 18];
            header.AverageHeartRate = this.PacketData[offset + 19];
        }

        protected override System.Drawing.Size ScreenSize { get { return new System.Drawing.Size(128, 80); } }
        protected override int ScreenBpp { get { return 1; } }
    }
}

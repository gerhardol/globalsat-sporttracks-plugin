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
    public class Gh625Packet : GlobalsatPacket
    {
        public new class Header : GhPacketBase.Header
        {
            //public DateTime StartTime;
            //public byte LapCount;
            //public TimeSpan TotalTime;
            //public Int32 TotalDistanceMeters;
            public Int16 TotalCalories;
            public Int16 MaximumSpeed;
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
            public IList<TrackPoint> TrackPoints = new List<TrackPoint>();
            public IList<Lap> Laps = new List<Lap>();
        }

        public IList<TrackFileHeader> UnpackTrackHeaders()
        {
            int numHeaders = this.PacketLength / 31;
            IList<TrackFileHeader> headers = new List<TrackFileHeader>();
            for (int i = 0; i < numHeaders; i++)
            {
                int trackStart = i * 31;
                TrackFileHeader header = new TrackFileHeader();
                ReadHeader(header, trackStart);
                header.TrackPointCount = ReadInt16(endianFormat, this.PacketData, trackStart + 25);
                header.TrackPointIndex = ReadInt16(endianFormat, this.PacketData, trackStart + 27);
                headers.Add(header);
            }
            return headers;
        }

        public TrackFileSection UnpackTrackSectionLaps()
        {
            if (this.PacketLength < 31) return null;

            TrackFileSection section = new TrackFileSection();
            ReadHeader(section, 0);
            section.LapCount = this.PacketData[6];
            section.TrackPointCount = ReadInt16(endianFormat, this.PacketData, 25);
            section.StartPointIndex = ReadInt16(endianFormat, this.PacketData, 27);
            section.EndPointIndex = ReadInt16(endianFormat, this.PacketData, 29);

            int offset = 31;
            while (offset < this.PacketLength)
            {
                Lap lap = new Lap();
                lap.EndTime = TimeSpan.FromSeconds(((double)ReadInt32(endianFormat, this.PacketData, offset)) / 10);
                lap.LapTime = TimeSpan.FromSeconds(((double)ReadInt32(endianFormat, this.PacketData, offset + 4)) / 10);
                lap.LapDistanceMeters = ReadInt32(endianFormat, this.PacketData, offset + 8);
                lap.LapCalories = ReadInt16(endianFormat, this.PacketData, offset + 12);
                lap.MaximumSpeed = ReadInt16(endianFormat, this.PacketData, offset + 14);
                lap.MaximumHeartRate = this.PacketData[offset + 16];
                lap.AverageHeartRate = this.PacketData[offset + 17];
                //lap.StartPointIndex = ReadInt16(endianFormat, this.PacketData, 18);
                //lap.EndPointIndex = ReadInt16(endianFormat, this.PacketData, 20);
                section.Laps.Add(lap);
                offset += 22;
            }
            return section;
        }

        public TrackFileSection UnpackTrackSection()
        {
            if (this.PacketLength < 31) return null;

            TrackFileSection section = new TrackFileSection();
            ReadHeader(section, 0);
            section.TrackPointCount = ReadInt16(endianFormat, this.PacketData, 25);
            section.StartPointIndex = ReadInt16(endianFormat, this.PacketData, 27);
            section.EndPointIndex = ReadInt16(endianFormat, this.PacketData, 29);

            int offset = 31;
            while (offset < this.PacketLength)
            {
                TrackPoint point = new TrackPoint();
                point.Latitude = ReadInt32(endianFormat, this.PacketData, offset);
                point.Longitude = ReadInt32(endianFormat, this.PacketData, offset + 4);
                point.Altitude = ReadInt16(endianFormat, this.PacketData, offset + 8);
                point.Speed = ReadInt16(endianFormat, this.PacketData, offset + 10);
                point.HeartRate = this.PacketData[offset + 12];
                point.IntervalTime = ReadInt16(endianFormat, this.PacketData, offset + 13);
                section.TrackPoints.Add(point);
                offset += 15;
            }
            return section;
        }

        private void ReadHeader(Header header, int offset)
        {
            header.StartTime = ReadDateTime(this.PacketData, offset);
            header.TotalTime = TimeSpan.FromSeconds(((double)ReadInt32(endianFormat, this.PacketData, offset + 7)) / 10);
            header.TotalDistanceMeters = ReadInt32(endianFormat, this.PacketData, offset + 11);
            header.TotalCalories = ReadInt16(endianFormat, this.PacketData, offset + 15);
            header.MaximumSpeed = ReadInt16(endianFormat, this.PacketData, offset + 17);
            header.MaximumHeartRate = this.PacketData[offset + 19];
            header.AverageHeartRate = this.PacketData[offset + 20];
        }

        public override IList<GlobalsatWaypoint> ResponseWaypoints()
        {
            return new Gh625XTPacket().ResponseWaypoints(); //xxx
        }
    }
}
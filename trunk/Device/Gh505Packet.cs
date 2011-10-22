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
    class Gh505Packet : GlobalsatPacket
    {
        public class TrackFileHeader : Header
        {
            public Int16 TrackPointIndex;
        }

        public class Train : Header
        {
            public Int16 TotalCalories;
            //public double MaximumSpeed;
            public byte MaximumHeartRate;
            public byte AverageHeartRate;
            //public Int16 StartPointIndex;
            //public Int16 EndPointIndex;
            public IList<TrackPoint2> TrackPoints = new List<TrackPoint2>();
            public IList<Lap> Laps = new List<Lap>();
        }

        public IList<TrackFileHeader> UnpackTrackHeaders()
        {
            int numHeaders = this.PacketLength / 24;
            IList<TrackFileHeader> headers = new List<TrackFileHeader>();
            for (int i = 0; i < numHeaders; i++)
            {
                int trackStart = i * 24;
                TrackFileHeader header = new TrackFileHeader();
                ReadHeader(header, trackStart);
                header.TrackPointIndex = ReadInt16(trackStart + 18);
                headers.Add(header);
            }
            return headers;
        }

        public Train UnpackTrainHeader()
        {
            if (this.PacketLength < 52) return null;

            Train train = new Train();
            ReadHeader(train, 0);
            train.TotalCalories = ReadInt16(24);
            //train.MaximumSpeed = ReadInt16(26) / 3.6 / 100;
            train.MaximumHeartRate = this.PacketData[28];
            train.AverageHeartRate = this.PacketData[29];
            return train;
        }

        public IList<Lap> UnpackLaps()
        {
            if (this.PacketLength < 24) return new List<Lap>();

            IList<Lap> laps = new List<Lap>();

            int offset = 24;
            while (offset < this.PacketLength)
            {
                Lap lap = new Lap();

                lap.EndTime = TimeSpan.FromSeconds(((double)ReadInt32(offset)) / 10);
                lap.LapTime = TimeSpan.FromSeconds(((double)ReadInt32(offset + 4)) / 10);
                lap.LapDistanceMeters = ReadInt32(offset + 8);
                lap.LapCalories = ReadInt16(offset + 12);
                lap.MaximumSpeed = ReadInt16(offset + 14) / 3.6 / 100;
                lap.MaximumHeartRate = this.PacketData[offset + 16];
                lap.AverageHeartRate = this.PacketData[offset + 17];
                //lap.StartPointIndex = ReadInt16(18);
                //lap.EndPointIndex = ReadInt16(20);
                laps.Add(lap);
                offset += 36;
            }
            return laps;
        }

        public IList<TrackPoint2> UnpackTrackPoints()
        {
            if (this.PacketLength < 24) return new List<TrackPoint2>();

            IList<TrackPoint2> points = new List<TrackPoint2>();

            int offset = 24;
            while (offset < this.PacketLength)
            {
                TrackPoint2 point = new TrackPoint2();
                point.Latitude = (double)ReadInt32(offset) / 1000000;
                point.Longitude = (double)ReadInt32(offset + 4) / 1000000;
                point.Altitude = ReadInt16(offset + 8);
                point.Speed = ReadInt16(offset + 10) / 3.6 / 100;
                point.HeartRate = this.PacketData[offset + 12];
                point.IntervalTime = ReadInt32(offset + 16);
                point.Cadence = ReadInt16(offset + 20);
                point.Power = ReadInt16(offset + 24);
                points.Add(point);
                offset += 28;
            }
            return points;
        }

        private void ReadHeader(Header header, int offset)
        {
            header.StartTime = ReadDateTime(this.PacketData, offset).ToUniversalTime();
            header.TrackPointCount = ReadInt16(offset + 6);
            header.TotalTime = TimeSpan.FromSeconds(((double)ReadInt32(offset + 8)) / 10);
            header.TotalDistanceMeters = ReadInt32(offset + 12);
            header.LapCount = ReadInt16(offset + 16);
        }

        public int LocationLength { get { return 20; } }
        public override IList<GlobalsatWaypoint> ResponseGetWaypoints()
        {
            int nrWaypoints = PacketLength / 20;
            IList<GlobalsatWaypoint> waypoints = new List<GlobalsatWaypoint>(nrWaypoints);

            for (int i = 0; i < nrWaypoints; i++)
            {
                int index = i * 20;

                string waypointName = ByteArr2String(PacketData, index, 6);
                int iconNr = (int)PacketData[index + 7];
                short altitude = ReadInt16(index + 8);
                // 10-11 ?
                double latitude = (double)ReadInt32(index + 12) / 1000000.0;
                double longitude = (double)ReadInt32(index + 16) / 1000000.0;

                GlobalsatWaypoint waypoint = new GlobalsatWaypoint(waypointName, iconNr, altitude, latitude, longitude);
                waypoints.Add(waypoint);
            }

            return waypoints;
        }

        protected override bool endianFormat { get { return false; } } //little endian
        protected override int ScreenBpp { get { return 1; } }
    }
}

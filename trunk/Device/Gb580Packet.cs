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
    class Gb580Packet : GlobalsatPacket
    {
        //public class TrackFileHeader : Header
        //{
        //    public Int16 TrackPointIndex;
        //}

        public class Train : Header
        {
            public Int16 IndexStartPt;
            public Int16 LapIndexEndPt;
            public Int16 TotalCalories;
            public Int16 MaximumSpeed;
            public byte MaximumHeartRate;
            public byte AverageHeartRate;
        //    public Int16 StartPointIndex;
        //    public Int16 EndPointIndex;
            public IList<TrackPoint3> TrackPoints = new List<TrackPoint3>();
            public IList<Lap> Laps = new List<Lap>();
        }

        public IList<Train> UnpackTrainHeaders()
        {
            int numTrains = this.PacketLength / 24;
            IList<Train> trains = new List<Train>();
            for (int i = 0; i < numTrains; i++)
            {
                int trackStart = i * 24;
                Train train = new Train();
                ReadHeader(train, trackStart);
                train.IndexStartPt = ReadInt16(endianFormat, this.PacketData, trackStart + 18);
                train.LapIndexEndPt = ReadInt16(endianFormat, this.PacketData, trackStart + 20);
                trains.Add(train);
            }
            return trains;
        }

        public Train UnpackTrainHeader()
        {
            if (this.PacketLength < 52) return null;

            Train train = new Train();
            ReadHeader(train, 0);
            train.TotalCalories = ReadInt16(endianFormat, this.PacketData, 24);
            train.MaximumSpeed = ReadInt16(endianFormat, this.PacketData, 28);
            train.MaximumHeartRate = this.PacketData[32];
            train.AverageHeartRate = this.PacketData[33];
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

                lap.EndTime = TimeSpan.FromSeconds(((double)ReadInt32(endianFormat, this.PacketData, offset)) / 10);
                lap.LapTime = TimeSpan.FromSeconds(((double)ReadInt32(endianFormat, this.PacketData, offset + 4)) / 10);
                lap.LapDistanceMeters = ReadInt32(endianFormat, this.PacketData, offset + 8);
                lap.LapCalories = ReadInt16(endianFormat, this.PacketData, offset + 12);
                lap.MaximumSpeed = ReadInt16(endianFormat, this.PacketData, offset + 16);
                lap.MaximumHeartRate = this.PacketData[offset + 20];
                lap.AverageHeartRate = this.PacketData[offset + 21];
                //lap.StartPointIndex = ReadInt16(endianFormat, this.PacketData, 18);
                //lap.EndPointIndex = ReadInt16(endianFormat, this.PacketData, 20);
                laps.Add(lap);
                offset += 40;
            }
            return laps;
        }

        public IList<TrackPoint3> UnpackTrackPoints()
        {
            if (this.PacketLength < 24) return new List<TrackPoint3>();

            IList<TrackPoint3> points = new List<TrackPoint3>();

            int offset = 24;
            while (offset < this.PacketLength)
            {
                TrackPoint3 point = new TrackPoint3();
                point.Latitude = ReadInt32(endianFormat, this.PacketData, offset);
                point.Longitude = ReadInt32(endianFormat, this.PacketData, offset + 4);
                point.Altitude = ReadInt32(endianFormat, this.PacketData, offset + 8);
                point.Speed = ReadInt16(endianFormat, this.PacketData, offset + 12);
                point.HeartRate = this.PacketData[offset + 16];
                point.IntervalTime = ReadInt32(endianFormat, this.PacketData, offset + 20);
                point.Cadence = ReadInt16(endianFormat, this.PacketData, offset + 24);
                point.Power = ReadInt16(endianFormat, this.PacketData, offset + 28);
                points.Add(point);
                offset += 32;
            }
            return points;
        }

        private void ReadHeader(Header header, int offset)
        {
            header.StartTime = ReadDateTime(this.PacketData, offset).ToUniversalTime();
            header.TrackPointCount = ReadInt16(endianFormat, this.PacketData, offset + 6);
            header.TotalTime = TimeSpan.FromSeconds(((double)ReadInt32(endianFormat, this.PacketData, offset + 8)) / 10);
            header.TotalDistanceMeters = ReadInt32(endianFormat, this.PacketData, offset + 12);
            header.LapCount = ReadInt16(endianFormat, this.PacketData, offset + 16);
        }

        public override IList<GlobalsatWaypoint> ResponseWaypoints()
        {
            return new Gh505Packet().ResponseWaypoints(); //xxx
        }

        protected override bool endianFormat { get { return false; } } //little endian
    }
}

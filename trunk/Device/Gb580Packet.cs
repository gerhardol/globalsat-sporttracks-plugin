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
    class Gb580Packet : Gh505Packet
    {
        //TODO: merge w Gh505 usage
        //public class TrackFileHeader : Header
        //{
        //    public Int16 TrackPointIndex;
        //}

        public new class Train : Header
        {
            public Int16 IndexStartPt;
            public Int16 LapIndexEndPt;
            public Int16 TotalCalories;
            //public double MaximumSpeed;
            public byte MaximumHeartRate;
            public byte AverageHeartRate;
            //public Int16 StartPointIndex;
            //public Int16 EndPointIndex;
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
                train.IndexStartPt = ReadInt16(trackStart + 18);
                train.LapIndexEndPt = ReadInt16(trackStart + 20);
                trains.Add(train);
            }
            return trains;
        }

        public new Train UnpackTrainHeader()
        {
            if (this.PacketLength < 52) return null;

            Train train = new Train();
            ReadHeader(train, 0);
            train.TotalCalories = ReadInt16(24);
            //train.MaximumSpeed = ReadInt16(28);
            train.MaximumHeartRate = this.PacketData[32];
            train.AverageHeartRate = this.PacketData[33];
            return train;
        }

        public new IList<Lap> UnpackLaps()
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
                //lap.MaximumSpeed = ReadInt16(offset + 16) / 3.6 / 100;
                lap.MaximumHeartRate = this.PacketData[offset + 20];
                lap.AverageHeartRate = this.PacketData[offset + 21];
                //lap.StartPointIndex = ReadInt16(18);
                //lap.EndPointIndex = ReadInt16(20);
                laps.Add(lap);
                offset += 40;
            }
            return laps;
        }

        public new IList<TrackPoint3> UnpackTrackPoints()
        {
            if (this.PacketLength < 24) return new List<TrackPoint3>();

            IList<TrackPoint3> points = new List<TrackPoint3>();

            int offset = 24;
            while (offset < this.PacketLength)
            {
                TrackPoint3 point = new TrackPoint3();
                point.Latitude = (double)ReadInt32(offset) / 1000000;
                point.Longitude = (double)ReadInt32(offset + 4) / 1000000;
                point.Altitude = ReadInt32(offset + 8);
                point.Speed = ReadInt16(offset + 12) / 3.6 / 100;
                point.HeartRate = this.PacketData[offset + 16];
                point.IntervalTime = ReadInt32(offset + 20);
                point.Cadence = ReadInt16(offset + 24);
                point.Power = ReadInt16(offset + 28);
                points.Add(point);
                offset += 32;
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

        //public override IList<GlobalsatWaypoint> ResponseGetWaypoints()
        //{
        //    return new Gh505Packet().ResponseGetWaypoints();
        //}

        protected override bool endianFormat { get { return false; } } //little endian
        protected override System.Drawing.Size ScreenSize { get { return new System.Drawing.Size(128, 128); } }

        protected override int GetWptOffset { get { return 2; } }
        protected override int SendWptOffset { get { return 2; } }
        public override int TrackPointsPerSection { get { return 73; } }
    }
}
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

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class Gh625XTPacket : GlobalsatPacket
    {

        const int dbTrainHeaderLength = 29;
        const int dbTrainHeaderCType = 28;
        const int dbTrainHeadLength = 58;
        const int dbLapLength = 41;
        const int dbTrackPoint4Length = 25;

        public class Train : Header
        {
            //public Int16 StartPointIndex;
            //public Int16 EndPointIndex;
            //public bool Multisport;
            public Int16 TotalCalories;
            //public double MaximumSpeed;
            public byte MaximumHeartRate;
            public byte AverageHeartRate;
            public Int16 TotalAscend;
            public Int16 TotalDescend;
            //public Int16 MinimumAltitude;
            //public Int16 MaximumAltitude;
            public Int16 AverageCadence;
            public Int16 MaximumCadence;
            public Int16 AveragePower;
            public Int16 MaximumPower;
            //byte Sport1;
            //byte Sport2;
            //byte Sport3;
            //byte Sport4;
            //byte Sport5;

            public IList<TrackPoint4> TrackPoints = new List<TrackPoint4>();
            public IList<Lap> Laps = new List<Lap>();
        }

        public class TrackFileHeader : Header
        {
            public Int32 TrackPointIndex;
        }

        //Both for DB_TRAINHEADER and DB_TRAIN
        private void ReadHeader(Header header, int offset)
        {
            header.StartTime = ReadDateTime(this.PacketData, offset).ToUniversalTime();
            header.TrackPointCount = ReadInt32(offset + 6);
            header.TotalTime = TimeSpan.FromSeconds(((double)ReadInt32(offset + 10)) / 10);
            header.TotalDistanceMeters = ReadInt32(offset + 14);
            header.LapCount = ReadInt16(offset + 18);
        }

        public IList<TrackFileHeader> UnpackTrackHeaders()
        {
            int numHeaders = this.PacketLength / dbTrainHeaderLength;
            IList<TrackFileHeader> headers = new List<TrackFileHeader>();
            for (int i = 0; i < numHeaders; i++)
            {
                int trackStart = i * dbTrainHeaderLength;
                TrackFileHeader header = new TrackFileHeader();
                ReadHeader(header, trackStart);
                header.TrackPointIndex = ReadInt32(trackStart + 20);
                headers.Add(header);
            }
            return headers;
        }

        public Train UnpackTrainHeader()
        {
            if (this.PacketLength < dbTrainHeadLength) return null;

            Train train = new Train();
            ReadHeader(train, 0);
            train.TotalCalories = ReadInt16(29);
            //train.MaximumSpeed = ReadInt32(31)*3.6/100;
            train.MaximumHeartRate = this.PacketData[35];
            train.AverageHeartRate = this.PacketData[36];
            train.TotalAscend = ReadInt16(37);
            train.TotalDescend = ReadInt16(39);
            //train.MinimumAltitude = ReadInt16(41);
            //train.MaximumAltitude = ReadInt16(43);
            train.AverageCadence = ReadInt16(45);
            train.MaximumCadence = ReadInt16(47);
            train.AveragePower = ReadInt16(49);
            train.MaximumPower = ReadInt16(51);
            //5bytes of SportType
            return train;
        }

        public IList<Lap> UnpackLaps()
        {
            if (this.PacketLength < dbTrainHeaderLength ||
                this.PacketData[dbTrainHeaderCType] != HeaderTypeLaps)
            {
                return new List<Lap>();
            }
            IList<Lap> laps = new List<Lap>();

            int offset = dbTrainHeaderLength;
            while (offset < this.PacketLength)
            {
                Lap lap = new Lap();

                lap.EndTime = TimeSpan.FromSeconds(((double)ReadInt32(offset)) / 10);
                lap.LapTime = TimeSpan.FromSeconds(((double)ReadInt32(offset + 4)) / 10);
                lap.LapDistanceMeters = ReadInt32(offset + 8);
                lap.LapCalories = ReadInt16(offset + 12);
                lap.MaximumSpeed = ReadInt32(offset + 14)/3.6/100;
                lap.MaximumHeartRate = this.PacketData[offset + 18];
                lap.AverageHeartRate = this.PacketData[offset + 19];
                lap.MinimumAltitude = ReadInt16(offset + + 20);
                lap.MaximumAltitude = ReadInt16(offset + 22);
                lap.AverageCadence = ReadInt16(offset + 24);
                lap.MaximumCadence = ReadInt16(offset + 26);
                lap.AveragePower = ReadInt16(offset + 28);
                lap.MaximumPower = ReadInt16(offset + 30);
                //byte multisport
                //lap.StartPointIndex = ReadInt16(offset + 33);
                //lap.EndPointIndex = ReadInt16(offset + 37);
                laps.Add(lap);
                offset += dbLapLength;
            }
            return laps;
        }

        public IList<TrackPoint4> UnpackTrackPoints()
        {
            if (this.PacketLength < dbTrainHeaderLength ||
                this.PacketData[dbTrainHeaderCType] != HeaderTypeTrackPoints)
            {
                return new List<TrackPoint4>();
            }

            IList<TrackPoint4> points = new List<TrackPoint4>();

            int offset = dbTrainHeaderLength;
            while (offset < this.PacketLength)
            {
                TrackPoint4 point = new TrackPoint4();
                point.Latitude = (double)ReadInt32(offset) / 1000000;
                point.Longitude = (double)ReadInt32(offset + 4) / 1000000;
                point.Altitude = ReadInt16(offset + 8);
                point.Speed = ReadInt32(offset + 10) / 3.6 / 100;
                point.HeartRate = this.PacketData[offset + 14];
                point.IntervalTime = ReadInt32(offset + 15);
                point.Cadence = ReadInt16(offset + 19);
                //point.PowerCadence = ReadInt16(offset + 21);
                point.Power = ReadInt16(offset + 23);
                points.Add(point);
                offset += dbTrackPoint4Length;
            }
            return points;
        }

        public int LocationLength { get { return 18; } }

        public override IList<GlobalsatWaypoint> ResponseWaypoints()
        {
            int nrWaypoints = PacketLength / LocationLength;
            IList<GlobalsatWaypoint> waypoints = new List<GlobalsatWaypoint>(nrWaypoints);

            for (int i = 0; i < nrWaypoints; i++)
            {
                int index = i * LocationLength;

                string waypointName = ByteArr2String(PacketData, index, 6);
                int iconNr = (int)PacketData[index + 7];
                short altitude = ReadInt16(index + 8);
                int latitudeInt = ReadInt32(index + 10);
                int longitudeInt = ReadInt32(index + 14);
                double latitude = (double)latitudeInt / 1000000.0;
                double longitude = (double)longitudeInt / 1000000.0;

                GlobalsatWaypoint waypoint = new GlobalsatWaypoint(waypointName, iconNr, altitude, latitude, longitude);
                waypoints.Add(waypoint);
            }

            return waypoints;
        }
    }
}

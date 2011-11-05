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

            public IList<TrackPoint> TrackPoints = new List<TrackPoint>();
            public IList<Lap> Laps = new List<Lap>();
        }

        public class TrackFileHeader : Header
        {
            public Int32 TrackPointIndex;
        }

        //Both for DB_TRAINHEADER and DB_TRAIN
        private void ReadHeader(Header header, int offset)
        {
            header.StartTime = ReadDateTime(offset).ToUniversalTime();
            header.TrackPointCount = ReadInt32(offset + 6);
            header.TotalTime = TimeSpan.FromSeconds(((double)ReadInt32(offset + 10)) / 10);
            header.TotalDistanceMeters = ReadInt32(offset + 14);
            header.LapCount = ReadInt16(offset + 18);
            //4byte ptrec
            //4 byte laprec
            //29: cType
        }

        public IList<TrackFileHeader> UnpackTrackHeaders()
        {
            int numHeaders = this.PacketLength / TrackHeaderLength;
            IList<TrackFileHeader> headers = new List<TrackFileHeader>();
            for (int i = 0; i < numHeaders; i++)
            {
                int trackStart = i * TrackHeaderLength;
                TrackFileHeader header = new TrackFileHeader();
                ReadHeader(header, trackStart);
                header.TrackPointIndex = ReadInt32(trackStart + 20);
                headers.Add(header);
            }
            return headers;
        }

        public Train UnpackTrainHeader()
        {
            if (this.PacketLength < TrainDataHeaderLength) return null;

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
            //5 bytes of SportType
            return train;
        }

        public IList<Lap> UnpackLaps()
        {
            if (this.PacketLength < TrackHeaderLength ||
                this.PacketData[dbTrainHeaderCType] != HeaderTypeLaps)
            {
                return new List<Lap>();
            }
            IList<Lap> laps = new List<Lap>();

            int offset = TrackHeaderLength;
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
                offset += TrackLapLength;
            }
            return laps;
        }

        public IList<TrackPoint> UnpackTrackPoints()
        {
            if (this.PacketLength < TrackHeaderLength ||
                this.PacketData[dbTrainHeaderCType] != HeaderTypeTrackPoints)
            {
                return new List<TrackPoint>();
            }

            IList<TrackPoint> points = new List<TrackPoint>();

            int offset = TrackHeaderLength;
            while (offset < this.PacketLength)
            {
                TrackPoint point = new TrackPoint();
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
                offset += TrackPointLength;
            }
            return points;
        }

        //Trackstart without laps
        public override GlobalsatPacket SendTrackStart(TrackFileBase trackFile)
        {
            Int16 nrLaps = 1;
            Int16 totalLength = (Int16)TrainDataHeaderLength;
            this.InitPacket(CommandSendTrackStart, totalLength);

            int offset = 0;

            offset += WriteTrackHeader(offset, nrLaps, trackFile);

            offset += this.Write(offset, trackFile.TotalCalories);
            offset += this.Write32(offset, trackFile.MaximumSpeed);
            this.PacketData[offset++] = (byte)trackFile.MaximumHeartRate;
            this.PacketData[offset++] = (byte)trackFile.AverageHeartRate;
            offset += this.Write(offset, trackFile.TotalAscent);
            offset += this.Write(offset, trackFile.TotalDescent);

            offset += this.Write(offset, 0); // min altitude
            offset += this.Write(offset, 0); // max altitude
            offset += this.Write(offset, 0); // avg cadence
            offset += this.Write(offset, 0); // best cadence
            offset += this.Write(offset, 0); // avg power
            offset += this.Write(offset, 0); // max power
            this.PacketData[offset++] = 0; // sport 1
            this.PacketData[offset++] = 0; // sport 2
            this.PacketData[offset++] = 0; // sport 3
            this.PacketData[offset++] = 0; // sport 4
            this.PacketData[offset++] = 0; // sport 5

            CheckOffset(totalLength, offset);
            return this;
        }

        // DB_TRAIN_HEADER + DB_LAP
        public override GlobalsatPacket SendTrackLaps(TrackFileBase trackFile)
        {
            const Int16 nrLaps = 1;
            Int16 totalLength = (Int16)(TrackHeaderLength + nrLaps * TrackLapLength);
            this.InitPacket(CommandSendTrackSection, totalLength);

            int offset = 0;

            offset += WriteTrackHeader(offset, nrLaps, trackFile);

            // send only one lap
            int totalTimeSecondsTimes10 = (int)(trackFile.TotalTime.TotalMilliseconds / 100);
            offset += this.Write32(offset, totalTimeSecondsTimes10);
            offset += this.Write32(offset, totalTimeSecondsTimes10);
            offset += this.Write32(offset, trackFile.TotalDistanceMeters);
            offset += this.Write(offset, trackFile.TotalCalories);
            offset += this.Write32(offset, trackFile.MaximumSpeed);
            this.PacketData[offset++] = (byte)trackFile.MaximumHeartRate;
            this.PacketData[offset++] = (byte)trackFile.AverageHeartRate;

            offset += Write(offset, 0); // min altitude
            offset += Write(offset, 0); // max altitude
            offset += Write(offset, 0); // avg cadence
            offset += Write(offset, 0); // best cadence
            offset += Write(offset, 0); // avg power
            offset += Write(offset, 0); // max power
            this.PacketData[offset++] = (byte)0; // multisport index

            // start/end index
            offset += Write32(offset, 0);
            offset += Write32(offset, (Int16)(trackFile.TrackPointCount - 1));

            CheckOffset(totalLength, offset);
            return this;
        }

        //TrackStart/TrackLap/TrackPoint share a common header
        private int WriteTrackHeader(int offset, int noOfLaps, TrackFileBase trackFile)
        {
            offset += this.Write(offset, trackFile.StartTime);

            offset += this.Write32(offset, trackFile.TrackPointCount);
            int totalTimeSecondsTimes10 = (int)(trackFile.TotalTime.TotalMilliseconds / 100);
            offset += this.Write32(offset, totalTimeSecondsTimes10);
            offset += this.Write32(offset, trackFile.TotalDistanceMeters);
            offset += this.Write(offset, (short)noOfLaps);

            //unused in some headers
            offset += this.Write32(offset, 0);
            offset += this.Write32(offset, 0);

            this.PacketData[offset++] = 0; //Multisport

            return CheckOffset(TrackHeaderLength, offset);
        }

        protected override int WriteTrackPointHeader(int offset, TrackFileSectionSend trackFile)
        {
            offset += WriteTrackHeader(offset, 1, trackFile);

            //unused fields in some headers
            this.Write(offset - 9, trackFile.StartPointIndex);
            this.Write(offset - 5, trackFile.EndPointIndex);
            return CheckOffset(TrackHeaderLength, offset);
        }

        protected override int WriteTrackPoint(int offset, TrackPointSend trackpoint)
        {
            int startOffset = offset;
            offset += this.Write32(offset, trackpoint.Latitude);
            offset += this.Write32(offset, trackpoint.Longitude);
            offset += this.Write(offset, trackpoint.Altitude);
            offset += this.Write32(offset, trackpoint.Speed);
            this.PacketData[offset++] = (byte)trackpoint.HeartRate;
            offset += this.Write32(offset, trackpoint.IntervalTime);
            offset += this.Write(offset, 0); // cadence
            offset += this.Write(offset, 0); // power cadence
            offset += this.Write(offset, 0); // power
            return CheckOffset(TrackPointLength, offset - startOffset);
        }

        const int dbTrainHeaderCType = 28; //type of packet in train header

        public override int TrackPointsPerSection { get { return 73; } }
        protected override int TrackHeaderLength { get { return 29; } }
        protected override int TrainDataHeaderLength { get { return 58; } }
        protected override int TrackLapLength { get { return 41; } }
        protected override int TrackPointLength { get { return 25; } }
    }
}

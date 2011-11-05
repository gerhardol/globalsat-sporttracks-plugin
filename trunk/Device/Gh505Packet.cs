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
            public IList<TrackPoint> TrackPoints = new List<TrackPoint>();
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
            if (this.PacketLength < TrainDataHeaderLength) return null;

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

        public IList<TrackPoint> UnpackTrackPoints()
        {
            if (this.PacketLength < 24) return new List<TrackPoint>();

            IList<TrackPoint> points = new List<TrackPoint>();

            int offset = 24;
            while (offset < this.PacketLength)
            {
                TrackPoint point = new TrackPoint();
                point.Latitude = (double)ReadInt32(offset) / 1000000;
                point.Longitude = (double)ReadInt32(offset + 4) / 1000000;
                point.Altitude = ReadInt16(offset + 8);
                point.Speed = ReadInt16(offset + 10) / 3.6 / 100;
                point.HeartRate = this.PacketData[offset + 12];
                point.IntervalTime = ReadInt32(offset + 16);
                point.Cadence = ReadInt16(offset + 20);
                point.Power = ReadInt16(offset + 24);
                points.Add(point);
                offset += TrackPointLength;
            }
            return points;
        }

        private void ReadHeader(Header header, int offset)
        {
            header.StartTime = ReadDateTime(offset).ToUniversalTime();
            header.TrackPointCount = ReadInt16(offset + 6);
            header.TotalTime = TimeSpan.FromSeconds(((double)ReadInt32(offset + 8)) / 10);
            header.TotalDistanceMeters = ReadInt32(offset + 12);
            header.LapCount = ReadInt16(offset + 16);
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
            offset += this.Write(offset, trackFile.MaximumSpeed);
            this.PacketData[offset++] = (byte)trackFile.MaximumHeartRate;
            this.PacketData[offset++] = (byte)trackFile.AverageHeartRate;
            offset += this.Write(offset, trackFile.TotalAscent);
            offset += this.Write(offset, trackFile.TotalDescent);

            offset += this.Write(offset, nrLaps); // min altitude
            offset += this.Write(offset, nrLaps); // max altitude
            offset += this.Write(offset, nrLaps); // avg cadence
            offset += this.Write(offset, nrLaps); // best cadence
            offset += this.Write(offset, nrLaps); // avg power
            offset += this.Write(offset, nrLaps); // max power
            this.PacketData[offset++] = 0; // sport 1  - 0x4 ??
            this.PacketData[offset++] = 0; // sport 2
            this.PacketData[offset++] = 0; // sport 3
            this.PacketData[offset++] = 0; // sport 4
            this.PacketData[offset++] = 0; // sport 5
            offset += 1; //pad

            CheckOffset(totalLength, offset);
            return this;
        }

        // DB_TRAIN_HEADER + DB_LAP 580
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
            offset += this.Write(offset, trackFile.MaximumSpeed);
            this.PacketData[offset++] = (byte)trackFile.MaximumHeartRate;
            this.PacketData[offset++] = (byte)trackFile.AverageHeartRate;

            offset += Write(offset, 0); // min altitude
            offset += Write(offset, 0); // max altitude
            offset += Write(offset, 0); // avg cadence
            offset += Write(offset, 0); // best cadence
            offset += Write(offset, 0); // avg power
            offset += Write(offset, 0); // max power
            this.PacketData[offset++] = (byte)0; // multisport index
            offset += 1; // pad len=36

            // start/end index
            offset += Write(offset, 0);
            offset += Write(offset, (short)(trackFile.TrackPointCount - 1));

            CheckOffset(totalLength, offset);
            return this;
        }

        private int WriteTrackHeader(int offset, int noOfLaps, TrackFileBase trackFile)
        {
            int startOffset = offset;
            offset += this.Write(offset, trackFile.StartTime);

            offset += this.Write(offset, trackFile.TrackPointCount);
            int totalTimeSecondsTimes10 = (int)(trackFile.TotalTime.TotalMilliseconds / 100);
            offset += this.Write32(offset, totalTimeSecondsTimes10);
            offset += this.Write32(offset, trackFile.TotalDistanceMeters);
            offset += this.Write(offset, (short)noOfLaps);

            //unused in some headers
            offset += this.Write(offset, 0);
            offset += this.Write(offset, 0);

            this.PacketData[offset++] = 0; //Multisport
            this.PacketData[offset++] = 0; //pad

            return CheckOffset(TrackHeaderLength, offset - startOffset);
        }

        protected override int WriteTrackPointHeader(int offset, TrackFileSectionSend trackFile)
        {
            int startOffset = offset;
            offset += WriteTrackHeader(offset, 1, trackFile);

            //unused fields in some headers
            this.Write(offset - 6, trackFile.StartPointIndex);
            this.Write(offset - 4, trackFile.EndPointIndex);

            return CheckOffset(TrackHeaderLength, offset - startOffset);
        }

        protected override int WriteTrackPoint(int offset, TrackPointSend trackpoint)
        {
            int startOffset = offset;
            offset += this.Write32(offset, trackpoint.Latitude);
            offset += this.Write32(offset, trackpoint.Longitude);
            offset += this.Write(offset, trackpoint.Altitude);
            offset += this.Write32(offset, trackpoint.Speed);
            this.PacketData[offset++] = (byte)trackpoint.HeartRate;
            offset += 3; //padding
            offset += this.Write32(offset, trackpoint.IntervalTime);
            offset += this.Write(offset, 0); // cadence
            offset += this.Write(offset, 0); // power cadence
            offset += this.Write(offset, 0); // power

            return CheckOffset(TrackPointLength, offset - startOffset);
        }

        protected override bool endianFormat { get { return false; } } //little endian

        protected override int ScreenBpp { get { return 1; } }
        protected override bool ScreenRowCol { get { return false; } }

        protected override int GetWptOffset { get { return 2; } }
        protected override int SendWptOffset { get { return 2; } }

        public override int TrackPointsPerSection { get { return 73; } }
        protected override int TrackHeaderLength { get { return 24; } }
        protected override int TrainDataHeaderLength { get { return 52; } }
        protected override int TrackLapLength { get { return 36; } }
        protected override int TrackPointLength { get { return 28; } }
    }
}

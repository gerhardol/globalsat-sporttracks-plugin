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
    class Gb580Packet : GlobalsatPacket2
    {
        private void ReadHeader(Header header, int offset)
        {
            header.StartTime = ReadDateTime(offset).ToUniversalTime();
            header.TrackPointCount = ReadInt16(offset + 6);
            header.TotalTime = TimeSpan.FromSeconds(FromGlobTime(ReadInt32(offset + 8)));
            header.TotalDistanceMeters = ReadInt32(offset + 12);
            header.LapCount = ReadInt16(offset + 16);
        }

        public override IList<TrackFileHeader> UnpackTrackHeaders()
        {
            int numTrains = this.PacketLength / TrackHeaderLength;
            IList<TrackFileHeader> trains = new List<TrackFileHeader>();
            for (int i = 0; i < numTrains; i++)
            {
                int trackStart = i * TrackHeaderLength;
                TrackFileHeader train = new TrackFileHeader();
                ReadHeader(train, trackStart);
                train.TrackPointIndex = ReadInt16(trackStart + 18);
                //train.LapIndexEndPt = ReadInt16(trackStart + 20);
                trains.Add(train);
            }
            return trains;
        }

        public override Train UnpackTrainHeader()
        {
            if (this.PacketLength < TrainDataHeaderLength)
            {
                ReportOffset(this.PacketLength, TrackHeaderLength);
                return null;
            }

            Train train = new Train();
            ReadHeader(train, 0);
            train.TotalCalories = ReadInt16(24);
            //train.MaximumSpeed = ReadInt32(28); Guessing 4byte
            train.MaximumHeartRate = this.PacketData[32];
            train.AverageHeartRate = this.PacketData[33];
            //TODO: Guesses
            train.TotalAscend = ReadInt16(34);
            train.TotalDescend = ReadInt16(36);
            //train.MinimumAltitude = ReadInt16(38);
            //train.MaximumAltitude = ReadInt16(40);
            train.AverageCadence = ReadInt16(42);
            train.MaximumCadence = ReadInt16(44);
            train.AveragePower = ReadInt16(46);
            train.MaximumPower = ReadInt16(48);
            //5 bytes of SportType
            return train;
        }

        public override IList<Lap> UnpackLaps()
        {
            if (this.PacketLength < TrackHeaderLength ||
                this.GetTrainContent() != HeaderTypeLaps)
            {
                ReportOffset(this.PacketLength, TrackHeaderLength);
                return new List<Lap>();
            }

            IList<Lap> laps = new List<Lap>();

            int offset = TrackHeaderLength;
            while (offset <= this.PacketLength - TrackLapLength)
            {
                Lap lap = new Lap();

                lap.EndTime = TimeSpan.FromSeconds(FromGlobTime(ReadInt32(offset)));
                lap.LapTime = TimeSpan.FromSeconds(FromGlobTime(ReadInt32(offset + 4)));
                lap.LapDistanceMeters = ReadInt32(offset + 8);
                lap.LapCalories = ReadInt16(offset + 12);
                //lap.MaximumSpeed = FromGlobSpeed(ReadInt32(offset + 16)); Guessing 4bytes
                lap.MaximumHeartRate = this.PacketData[offset + 20];
                lap.AverageHeartRate = this.PacketData[offset + 21];
                //lap.StartPointIndex = ReadInt16(18);
                //lap.EndPointIndex = ReadInt16(20);
                laps.Add(lap);
                offset += TrackLapLength;
            }
            CheckOffset(this.PacketLength, offset);
            return laps;
        }

        public override IList<TrackPoint> UnpackTrackPoints()
        {
            if (this.PacketLength < TrackHeaderLength ||
                this.GetTrainContent() != HeaderTypeTrackPoints)
            {
                ReportOffset(this.PacketLength, TrackHeaderLength);
                return new List<TrackPoint>();
            }

            IList<TrackPoint> points = new List<TrackPoint>();

            int offset = TrackHeaderLength;
            while (offset <= this.PacketLength - TrackPointLength)
            {
                TrackPoint point = new TrackPoint();
                point.Latitude = (double)ReadLatLon(offset);
                point.Longitude = (double)ReadLatLon(offset + 4);
                point.Altitude = ReadInt32(offset + 8);
                point.Speed = FromGlobSpeed(ReadInt16(offset + 12));
                point.HeartRate = this.PacketData[offset + 16];
                point.IntervalTime = FromGlobTime(ReadInt32(offset + 20));
                point.Cadence = ReadInt16(offset + 24);
                point.PowerCadence = ReadInt16(offset + 26);
                point.Power = ReadInt16(offset + 28);
                points.Add(point);
                offset += TrackPointLength;
            }
            CheckOffset(this.PacketLength, offset);
            return points;
        }

        //Trackstart without laps
        public override GlobalsatPacket SendTrackStart(Train trackFile)
        {
            Int16 nrLaps = 1;
            Int16 totalLength = (Int16)TrainDataHeaderLength;
            this.InitPacket(CommandSendTrackStart, totalLength);

            int offset = 0;

            offset += WriteTrackHeader(offset, nrLaps, trackFile);

            offset += this.Write(offset, trackFile.TotalCalories);
            offset += this.Write32(offset, ToGlobSpeed(trackFile.MaximumSpeed));
            this.PacketData[offset++] = (byte)trackFile.MaximumHeartRate;
            this.PacketData[offset++] = (byte)trackFile.AverageHeartRate;
            offset += this.Write(offset, trackFile.TotalAscend);
            offset += this.Write(offset, trackFile.TotalDescend);

            offset += this.Write(offset, 0); // min altitude
            offset += this.Write(offset, 0); // max altitude
            offset += this.Write(offset, 0); // avg cadence
            offset += this.Write(offset, 0); // best cadence
            offset += this.Write(offset, 0); // avg power
            offset += this.Write(offset, 0); // max power
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
        public override GlobalsatPacket SendTrackLaps(Train trackFile)
        {
            const Int16 nrLaps = 1;
            Int16 totalLength = (Int16)(TrackHeaderLength + nrLaps * TrackLapLength);
            this.InitPacket(CommandSendTrackSection, totalLength);

            int offset = 0;

            offset += WriteTrackHeader(offset, nrLaps, trackFile);
            this.PacketData[TrainHeaderCTypeOffset] = HeaderTypeLaps;

            // send only one lap
            int totalTimeSecondsTimes10 = ToGlobTime(trackFile.TotalTime.TotalSeconds);
            offset += this.Write32(offset, totalTimeSecondsTimes10);
            offset += this.Write32(offset, totalTimeSecondsTimes10);
            offset += this.Write32(offset, (Int32)trackFile.TotalDistanceMeters);
            offset += this.Write(offset, trackFile.TotalCalories);
            offset += this.Write32(offset, ToGlobSpeed(trackFile.MaximumSpeed));
            this.PacketData[offset++] = (byte)trackFile.MaximumHeartRate;
            this.PacketData[offset++] = (byte)trackFile.AverageHeartRate;

            //Remaining fields are guesses only
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
            offset += Write(offset, (Int16)(trackFile.TrackPointCount - 1));

            CheckOffset(totalLength, offset);
            return this;
        }

        private int WriteTrackHeader(int offset, int noOfLaps, Header trackFile)
        {
            int startOffset = offset;
            offset += this.Write(offset, trackFile.StartTime.ToLocalTime());

            offset += this.Write(offset, (Int16)trackFile.TrackPointCount);
            int totalTimeSecondsTimes10 = ToGlobTime(trackFile.TotalTime.TotalSeconds);
            offset += this.Write32(offset, totalTimeSecondsTimes10);
            offset += this.Write32(offset, (Int32)trackFile.TotalDistanceMeters);
            offset += this.Write(offset, (short)noOfLaps);

            //unused in some headers
            offset += this.Write(offset, 0);
            offset += this.Write(offset, 0);

            this.PacketData[offset++] = 0; //Multisport
            this.PacketData[offset++] = 0; //pad

            return CheckOffset(TrackHeaderLength, offset - startOffset);
        }

        protected override int WriteTrackPointHeader(int offset, Train trackFile, int StartPointIndex, int EndPointIndex)
        {
            int startOffset = offset;
            offset += WriteTrackHeader(offset, 1, trackFile);
            this.PacketData[TrainHeaderCTypeOffset] = HeaderTypeTrackPoints;

            this.Write(offset - 6, (Int16)StartPointIndex);
            this.Write(offset - 4, (Int16)EndPointIndex);

            return CheckOffset(TrackHeaderLength, offset - startOffset);
        }

        protected override int WriteTrackPoint(int offset, TrackPoint trackpoint)
        {
            int startOffset = offset;
            offset += this.Write32(offset, ToGlobLatLon(trackpoint.Latitude));
            offset += this.Write32(offset, ToGlobLatLon(trackpoint.Longitude));
            offset += this.Write32(offset, (Int32)trackpoint.Altitude);
            offset += this.Write(offset, ToGlobSpeed(trackpoint.Speed));
            offset += 2;//padding
            this.PacketData[offset++] = (byte)trackpoint.HeartRate;
            offset += 3;
            offset += this.Write32(offset, ToGlobTime(trackpoint.IntervalTime));
            offset += this.Write(offset, 0); // cadence
            offset += this.Write(offset, 0); // power cadence
            offset += this.Write(offset, 0); // power
            offset += 2; //padding

            return CheckOffset(TrackPointLength, offset - startOffset);
        }

        protected override bool IsLittleEndian { get { return true; } }
        protected override System.Drawing.Size ScreenSize { get { return new System.Drawing.Size(128, 128); } }

        protected override int GetWptOffset { get { return 2; } }
        protected override int SendWptOffset { get { return 2; } }

        public override int TrackPointsPerSection { get { return 63; } }
        protected override int TrackHeaderLength { get { return 24; } }
        protected override int TrainDataHeaderLength { get { return 52; } }
        protected override int TrackLapLength { get { return 40; } }
        protected override int TrackPointLength { get { return 32; } }
        protected override int TrainHeaderCTypeOffset { get { return TrackHeaderLength - 2; } }
    }
}

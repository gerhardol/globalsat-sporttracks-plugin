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
    class Gh505Packet : GlobalsatPacket2
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
            int numHeaders = this.PacketLength / TrackHeaderLength;
            IList<TrackFileHeader> headers = new List<TrackFileHeader>();
            for (int i = 0; i < numHeaders; i++)
            {
                int trackStart = i * TrackHeaderLength;
                TrackFileHeader header = new TrackFileHeader();
                ReadHeader(header, trackStart);
                header.TrackPointIndex = ReadInt16(trackStart + 18);
                headers.Add(header);
            }
            return headers;
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
            //train.MaximumSpeed = FromGlobSpeed(ReadInt16(26));
            train.MaximumHeartRate = this.PacketData[28];
            train.AverageHeartRate = this.PacketData[29];
            //TODO: Guesses
            train.TotalAscend = ReadInt16(30);
            train.TotalDescend = ReadInt16(32);
            //train.MinimumAltitude = ReadInt16(34);
            //train.MaximumAltitude = ReadInt16(36);
            train.AverageCadence = ReadInt16(38);
            train.MaximumCadence = ReadInt16(40);
            train.AveragePower = ReadInt16(42);
            train.MaximumPower = ReadInt16(44);
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
                lap.MaximumSpeed = FromGlobSpeed(ReadInt16(offset + 14));
                lap.MaximumHeartRate = this.PacketData[offset + 16];
                lap.AverageHeartRate = this.PacketData[offset + 17];
                lap.MinimumAltitude = ReadInt16(offset + 18);
                lap.MaximumAltitude = ReadInt16(offset + 20);
                lap.AverageCadence = ReadInt16(offset + 22);
                lap.MaximumCadence = ReadInt16(offset + 24);
                lap.AveragePower = ReadInt16(offset + 26);
                lap.MaximumPower = ReadInt16(offset + 28);
                //byte multisport
                //pad?
                //lap.StartPointIndex = ReadInt16(31);
                //lap.EndPointIndex = ReadInt16(33);
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
                point.Altitude = ReadInt16(offset + 8);
                point.Speed = FromGlobSpeed(ReadInt16(offset + 10));
                point.HeartRate = this.PacketData[offset + 12];
                point.IntervalTime = FromGlobTime(ReadInt32(offset + 16));
                point.Cadence = ReadInt16(offset + 20);
                //point.PowerCadence = ReadInt16(offset + 22);
                point.Power = ReadInt16(offset + 24);
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
            offset += this.Write(offset, ToGlobSpeed(trackFile.MaximumSpeed));
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

        // DB_TRAIN_HEADER + DB_LAP
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
            offset += this.Write(offset, ToGlobSpeed(trackFile.MaximumSpeed));
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
            offset += this.Write(offset, (Int16)trackpoint.Altitude);
            offset += this.Write32(offset, ToGlobSpeed(trackpoint.Speed));
            this.PacketData[offset++] = (byte)trackpoint.HeartRate;
            offset += 3; //padding
            offset += this.Write32(offset, ToGlobTime(trackpoint.IntervalTime));
            offset += this.Write(offset, 0); // cadence
            offset += this.Write(offset, 0); // power cadence
            offset += this.Write(offset, 0); // power

            return CheckOffset(TrackPointLength, offset - startOffset);
        }

        protected override bool IsLittleEndian { get { return true; } }

        protected override int ScreenBpp { get { return 1; } }
        protected override bool ScreenRowCol { get { return false; } }

        protected override int GetWptOffset { get { return 2; } }
        protected override int SendWptOffset { get { return 2; } }

        public override int TrackPointsPerSection { get { return 73; } }
        protected override int TrackHeaderLength { get { return 24; } }
        protected override int TrainDataHeaderLength { get { return 52; } }
        protected override int TrackLapLength { get { return 36; } }
        protected override int TrackPointLength { get { return 28; } }
        protected override int TrainHeaderCTypeOffset { get { return TrackHeaderLength - 2; } }
    }
}

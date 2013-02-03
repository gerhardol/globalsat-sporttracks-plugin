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
        public Gb580Packet(FitnessDevice_Globalsat device) : base(device) { }

        private int ReadHeader(Header header, int offset)
        {
            header.StartTime = ReadDateTime(offset).ToUniversalTime().AddHours(this.FitnessDevice.configInfo.HoursAdjustment);
            header.TrackPointCount = ReadInt16(offset + 6);
            header.TotalTime = TimeSpan.FromSeconds(FromGlobTime(ReadInt32(offset + 8)));
            header.TotalDistanceMeters = ReadInt32(offset + 12);
            header.LapCount = ReadInt16(offset + 16);
            return 18;
        }

        public override IList<TrackFileHeader> UnpackTrackHeaders()
        {
            int numTrains = this.PacketLength / TrackHeaderLength;
            IList<TrackFileHeader> trains = new List<TrackFileHeader>();
            int offset = 0;
            for (int i = 0; i < numTrains; i++)
            {
                TrackFileHeader train = new TrackFileHeader();
                ReadHeader(train, offset);
                train.TrackPointIndex = ReadInt16(offset + 18);
                //20 listed as TrainLap record index too
                //train.LapIndexEndPt = ReadInt16(offset + 20);
                //cDataType or MultiSport 22
                //pad 23
                offset += TrackHeaderLength;
                trains.Add(train);
            }
            CheckOffset(this.PacketLength, offset);
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
            train.TotalCalories = ReadInt16(24); //2 pad
            train.MaximumSpeed = FromGlobSpeed(ReadInt32(28));
            train.MaximumHeartRate = this.PacketData[32];
            train.AverageHeartRate = this.PacketData[33];
            train.TotalAscend = ReadInt16(34);
            train.TotalDescend = ReadInt16(36);
            //train.MinimumAltitude = ReadInt16(38);
            //train.MaximumAltitude = ReadInt16(40);
            train.AverageCadence = ReadInt16(42);
            train.MaximumCadence = ReadInt16(44);
            train.AveragePower = ReadInt16(46);
            train.MaximumPower = ReadInt16(48);
            //5 bytes of SportType, pad
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
                lap.LapCalories = ReadInt16(offset + 12); //pad 2
                lap.MaximumSpeed = FromGlobSpeed(ReadInt32(offset + 16));
                lap.MaximumHeartRate = this.PacketData[offset + 20];
                lap.AverageHeartRate = this.PacketData[offset + 21];
                lap.MinimumAltitude = ReadInt16(offset + 22);
                lap.MaximumAltitude = ReadInt16(offset + 24);
                lap.AverageCadence = ReadInt16(offset + 26);
                lap.MaximumCadence = ReadInt16(offset + 28);
                lap.AveragePower = ReadInt16(offset + 30);
                lap.MaximumPower = ReadInt16(offset + 32);
                //byte multisport
                //pad
                //lap.StartPointIndex = ReadInt16(36);
                //lap.EndPointIndex = ReadInt16(38);
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
                point.Altitude = ReadInt16(offset + 8); //2 byte padding
                point.Speed = FromGlobSpeed(ReadInt32(offset + 12));
                point.HeartRate = this.PacketData[offset + 16]; //3 byte padding
                point.IntervalTime = FromGlobTime(ReadInt32(offset + 20));
                point.Cadence = ReadInt16(offset + 24);
                point.PowerCadence = ReadInt16(offset + 26);
                point.Power = ReadInt16(offset + 28);
                //2 byte padding
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

            offset += this.Write(offset, trackFile.TotalCalories); offset += 2;
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

            offset += this.Write(offset, trackFile.TotalCalories); offset += 2;
            offset += this.Write32(offset, ToGlobSpeed(trackFile.MaximumSpeed));
            this.PacketData[offset++] = (byte)trackFile.MaximumHeartRate;
            this.PacketData[offset++] = (byte)trackFile.AverageHeartRate;

            offset += Write(offset, 0); // min altitude
            offset += Write(offset, 0); // max altitude
            offset += Write(offset, 0); // avg cadence
            offset += Write(offset, 0); // best cadence
            offset += Write(offset, 0); // avg power
            offset += Write(offset, 0); // max power
            this.PacketData[offset++] = (byte)0; // multisport index
            offset += 1; // pad

            // start/end index
            offset += Write(offset, 0);
            offset += Write(offset, (Int16)(trackFile.TrackPointCount - 1));

            CheckOffset(totalLength, offset);
            return this;
        }

        private int WriteTrackHeader(int offset, int noOfLaps, Header trackFile)
        {
            int startOffset = offset;
            offset += this.Write(offset, trackFile.StartTime.ToLocalTime().AddHours(-this.FitnessDevice.configInfo.HoursAdjustment));

            offset += this.Write(offset, (Int16)trackFile.TrackPointCount);
            int totalTimeSecondsTimes10 = ToGlobTime(trackFile.TotalTime.TotalSeconds);
            offset += this.Write32(offset, totalTimeSecondsTimes10);
            offset += this.Write32(offset, (Int32)trackFile.TotalDistanceMeters);
            offset += this.Write(offset, (short)noOfLaps);

            //index unused in some headers
            offset += this.Write(offset, 0);
            offset += this.Write(offset, 0);

            this.PacketData[offset++] = 0; //cDataType or Multisport
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
            offset += this.Write(offset, (Int16)trackpoint.Altitude); offset += 2;
            offset += this.Write32(offset, ToGlobSpeed(trackpoint.Speed));
            this.PacketData[offset++] = (byte)trackpoint.HeartRate;
            offset += 3;
            offset += this.Write32(offset, ToGlobTime(trackpoint.IntervalTime));
            offset += this.Write(offset, 0); // cadence
            offset += this.Write(offset, 0); // power cadence
            offset += this.Write(offset, 0); // power
            offset += 2; // padding

            return CheckOffset(TrackPointLength, offset - startOffset);
        }

        public override GlobalsatSystemConfiguration2 ResponseGetSystemConfiguration2()
        {
            const int SystemConfiguration2 = 404;
            if (this.PacketLength < SystemConfiguration2)
            {
                ReportOffset(this.PacketLength, SystemConfiguration2);
                return null;
            }

            GlobalsatSystemConfiguration2 systemInfo = new GlobalsatSystemConfiguration2();
            systemInfo.ScreenOrientation = this.PacketData[394];
            systemInfo.cRecordTime = this.PacketData[326];

            return systemInfo;
        }

        protected override bool IsLittleEndian { get { return true; } }

        protected override int WptLatLonOffset { get { return 2; } }

        public override int TrackPointsPerSection { get { return 63; } }
        protected override int TrackHeaderLength { get { return 24; } }
        protected override int TrainDataHeaderLength { get { return 56; } }
        protected override int TrackLapLength { get { return 40; } }
        protected override int TrackPointLength { get { return 32; } }
        protected override int TrainHeaderCTypeOffset { get { return TrackHeaderLength - 2; } }

        protected override int SystemConfigWaypointCountOffset { get { return 66; } }
        protected override int SystemConfigPcRouteCountOffset { get { return 74; } }
    }
}

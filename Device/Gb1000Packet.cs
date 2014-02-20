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
    class Gb1000Packet : Gb580Packet
    {
        public Gb1000Packet(FitnessDevice_Globalsat device) : base(device) { }
/*
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
        */
        //Trackstart without laps
        public override GlobalsatPacket SendTrackStart(Train trackFile) { throw new GlobalsatProtocol.FeatureNotSupportedException(); }

        // DB_TRAIN_HEADER + DB_LAP 580
        public override GlobalsatPacket SendTrackLaps(Train trackFile) { throw new GlobalsatProtocol.FeatureNotSupportedException(); }
        private int WriteTrackHeader(int offset, int noOfLaps, Header trackFile) { throw new GlobalsatProtocol.FeatureNotSupportedException(); }
        protected override int WriteTrackPointHeader(int offset, Train trackFile, int StartPointIndex, int EndPointIndex) { throw new GlobalsatProtocol.FeatureNotSupportedException(); }
        protected override int WriteTrackPoint(int offset, TrackPoint trackpoint) { throw new GlobalsatProtocol.FeatureNotSupportedException(); }

        public override GlobalsatSystemConfiguration2 ResponseGetSystemConfiguration2()
        {
            const int SystemConfiguration2 = 52;
            if (this.PacketLength < SystemConfiguration2)
            {
                ReportOffset(this.PacketLength, SystemConfiguration2);
                return null;
            }

            GlobalsatSystemConfiguration2 systemInfo = new GlobalsatSystemConfiguration2();
            systemInfo.cRecordTime = 2; //Hardcoded

            return systemInfo;
        }
/*
        protected override bool IsLittleEndian { get { return true; } }

        protected override int WptLatLonOffset { get { return 2; } }

        public override int TrackPointsPerSection { get { return 63; } }
        protected override int TrackHeaderLength { get { return 24; } }
        protected override int TrainDataHeaderLength { get { return 56; } }
        protected override int TrackLapLength { get { return 40; } }
        protected override int TrackPointLength { get { return 32; } }
        protected override int TrainHeaderCTypeOffset { get { return TrackHeaderLength - 2; } }
*/
        //protected override int SystemConfigWaypointCountOffset { get { return 66; } }
        //protected override int SystemConfigPcRouteCountOffset { get { return 74; } }
    }
}

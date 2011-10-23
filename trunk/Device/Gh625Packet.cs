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
    public class Gh625Packet : GlobalsatPacket
    {
        public new class Header : GhPacketBase.Header
        {
            //public DateTime StartTime;
            //public byte LapCount;
            //public TimeSpan TotalTime;
            //public Int32 TotalDistanceMeters;
            public Int16 TotalCalories;
            public double MaximumSpeed;
            public byte MaximumHeartRate;
            public byte AverageHeartRate;
            //public Int16 TrackPointCount;
        }

        public class TrackFileHeader : Header
        {
            public Int16 TrackPointIndex;
        }

        public class TrackFileSection : Header
        {
            public Int16 StartPointIndex;
            public Int16 EndPointIndex;
            public IList<TrackPoint> TrackPoints = new List<TrackPoint>();
            public IList<Lap> Laps = new List<Lap>();
        }

        public IList<TrackFileHeader> UnpackTrackHeaders()
        {
            int numHeaders = this.PacketLength / 31;
            IList<TrackFileHeader> headers = new List<TrackFileHeader>();
            for (int i = 0; i < numHeaders; i++)
            {
                int trackStart = i * 31;
                TrackFileHeader header = new TrackFileHeader();
                ReadHeader(header, trackStart);
                header.TrackPointCount = ReadInt16(trackStart + 25);
                header.TrackPointIndex = ReadInt16(trackStart + 27);
                headers.Add(header);
            }
            return headers;
        }

        public TrackFileSection UnpackTrackSectionLaps()
        {
            if (this.PacketLength < 31) return null;

            TrackFileSection section = new TrackFileSection();
            ReadHeader(section, 0);
            section.LapCount = this.PacketData[6];
            section.TrackPointCount = ReadInt16(25);
            section.StartPointIndex = ReadInt16(27);
            section.EndPointIndex = ReadInt16(29);

            int offset = 31;
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
                section.Laps.Add(lap);
                offset += 22;
            }
            return section;
        }

        public TrackFileSection UnpackTrackSection()
        {
            if (this.PacketLength < 31) return null;

            TrackFileSection section = new TrackFileSection();
            ReadHeader(section, 0);
            section.TrackPointCount = ReadInt16(25);
            section.StartPointIndex = ReadInt16(27);
            section.EndPointIndex = ReadInt16(29);

            int offset = 31;
            while (offset < this.PacketLength)
            {
                TrackPoint point = new TrackPoint();
                point.Latitude = (double)ReadInt32(offset) / 1000000;
                point.Longitude = (double)ReadInt32(offset + 4) / 1000000;
                point.Altitude = ReadInt16(offset + 8);
                point.Speed = ReadInt16(offset + 10) / 3.6 / 100;
                point.HeartRate = this.PacketData[offset + 12];
                point.IntervalTime = ReadInt16(offset + 13);
                section.TrackPoints.Add(point);
                offset += 15;
            }
            return section;
        }

        private void ReadHeader(Header header, int offset)
        {
            header.StartTime = ReadDateTime(this.PacketData, offset);
            header.TotalTime = TimeSpan.FromSeconds(((double)ReadInt32(offset + 7)) / 10);
            header.TotalDistanceMeters = ReadInt32(offset + 11);
            header.TotalCalories = ReadInt16(offset + 15);
            header.MaximumSpeed = ReadInt16(offset + 17) / 3.6 / 100;
            header.MaximumHeartRate = this.PacketData[offset + 19];
            header.AverageHeartRate = this.PacketData[offset + 20];
        }

        public override IList<GlobalsatWaypoint> ResponseGetWaypoints()
        {
            return new Gh625XTPacket().ResponseGetWaypoints();
        }
        public override GlobalsatSystemInformation ResponseGetSystemInformation()
        {
            string deviceName = ByteArr2String(0, 20 + 1);

            int versionInt = ReadInt16(21);
            double version = (double)versionInt / 100.0;

            // 23-24: version hex ? - update code/flag

            string firmware = ByteArr2String(25, 16 + 1);

            string userName = ByteArr2String(42, 10 + 1);

            bool isFemale = PacketData[53] != 0x00;
            int age = PacketData[54];

            int weightPounds = ReadInt16(55);
            int weightKg = ReadInt16(57);

            int heightInches = ReadInt16(59);
            int heightCm = ReadInt16(61);

            int waypointCount = PacketData[63];
            int trainCount = PacketData[64];
            int manualRouteCount = PacketData[65];

            int birthYear = ReadInt16(66);
            int birthMonth = PacketData[68] + 1;
            int birthDay = PacketData[69];
            DateTime birthDate = new DateTime(birthYear, birthMonth, birthDay);

            int pcRouteCount = 0;
            int courseCount = 0;
            try
            {
                pcRouteCount = PacketData[70];
                courseCount = PacketData[71];
            }
            catch { }

            GlobalsatSystemInformation systemInfo = new GlobalsatSystemInformation(deviceName, version, firmware, userName, isFemale, age, weightPounds, weightKg, heightInches, heightCm, birthDate, 
                waypointCount, trainCount, manualRouteCount, pcRouteCount, courseCount);

            return systemInfo;
        }

        public override GlobalsatSystemConfiguration ResponseGetSystemConfiguration()
        {
            string userName = ByteArr2String(0, 10 + 1);

            bool isFemale = PacketData[11] != 0x00;
            int age = (int)PacketData[12];

            int weightPounds = ReadInt16(13);
            int weightKg = ReadInt16(15);

            int heightInches = ReadInt16(17);
            int heightCm = ReadInt16(19);

            int birthYear = ReadInt16(21);
            int birthMonth = (int)PacketData[23] + 1;
            int birthDay = (int)PacketData[24];
            DateTime birthDate = new DateTime(birthYear, birthMonth, birthDay);

            int languageIndex = (int)PacketData[25];
            int timezoneIndex = (int)PacketData[26];
            int utcOffsetTimeIndex = (int)PacketData[27];
            bool summertime = PacketData[28] != 0x00;
            bool isTimeFormat24h = PacketData[29] != 0x00;
            int unitIndex = (int)PacketData[30];
            int beeperIndex = (int)PacketData[31];
            bool waasOn = PacketData[32] != 0x00;
            int recordSamplingIndex = (int)PacketData[33];
            int recordSamplingCustomTime = (int)PacketData[34];
            int sportTypeIndex = (int)PacketData[35];
            int timeAlertIndex = (int)PacketData[36];
            int timeAlertInterval = ReadInt32(37); // 0.1 secs
            int distanceAlertIndex = (int)PacketData[41];
            int distanceAlertInterval = ReadInt32(42); // cm
            bool fastSpeedAlertOn = PacketData[46] != 0x00;
            int fastSpeedMiles = ReadInt32(47);
            int fastSpeedKm = ReadInt32(51);
            int fastSpeedKnots = ReadInt32(55);
            bool slowSpeedAlertOn = PacketData[59] != 0x00;
            int slowSpeedMiles = ReadInt32(60);
            int slowSpeedKm = ReadInt32(64);
            int slowSpeedKnots = ReadInt32(68);
            bool fastPaceAlertOn = PacketData[72] != 0x00;
            int fastPace = ReadInt32(73); // by units in byte 30
            bool slowPaceAlertOn = PacketData[77] != 0x00;
            int slowPace = ReadInt32(78); // by units in byte 30
            int autoPauseIndex = (int)PacketData[82];
            int pauseSpeedMiles = ReadInt32(83);
            int pauseSpeedKm = ReadInt32(87);
            int pauseSpeedKnots = ReadInt32(91);
            bool calculateCalorieByHeartrate = PacketData[95] != 0x00;
            bool heartMaxAlertOn = PacketData[96] != 0x00;
            int maxHeartrate = (int)PacketData[97];
            bool heartMinAlertOn = PacketData[98] != 0x00;
            int minHeartrate = (int)PacketData[99];
            int coordinationIndex = (int)PacketData[100];
            int sleepModeIndex = (int)PacketData[101];
            int heartAlertLevelIndex = (int)PacketData[102];
            int trainingLevelIndex = (int)PacketData[103];
            int declinationIndex = (int)PacketData[104];
            int declinationManualValue = ReadInt32(105);
            int autoLapIndex = (int)PacketData[109];
            int autoLapDistance = ReadInt32(110);
            int autoLapTime = ReadInt32(114);
            int extraWeightPounds = ReadInt16(118);
            int extraWeightKg = ReadInt16(120);
            int heartZoneLow = (int)PacketData[122];
            int heartZoneHigh = (int)PacketData[123];
            int switchDistanceIndex = (int)PacketData[124];
            bool switchCorrectionOn = PacketData[125] != 0x00;

            GlobalsatSystemConfiguration systemInfo = new GlobalsatSystemConfiguration();

            return systemInfo;
        }

        protected override System.Drawing.Size ScreenSize { get { return new System.Drawing.Size(128, 80); } }
        protected override int ScreenBpp { get { return 1; } }
    }
}
/*
 *  Globalsat/Keymaze SportTracks Plugin
 *  Copyright 2009 John Philip 
 * 
 *  This software may be used and distributed according to the terms of the
 *  GNU Lesser General Public License version 2 or any later version.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

using ZoneFiveSoftware.Common.Data;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    public class GlobalsatPacket : GhPacketBase
    {
        private static string InvalidOperation = "Invalid Operation";

        public class GlobalsatSystemConfiguration
        {
            public string UserName;
            public bool IsFemale;
            public int Age;
            public int WeightPounds;
            public int WeightKg;
            public int HeightInches;
            public int HeightCm;
            public DateTime BirthDate;

            public string DeviceName;
            public double Version;
            public string Firmware;
            public int WaypointCount;
            public int TrackpointCount;
            public int ManualRouteCount;
            public int PcRouteCount;
            public int CourseeCount;

            public GlobalsatSystemConfiguration(string deviceName, double version, string firmware,
                string userName, bool isFemale, int age, int weightPounds, int weightKg, int heightInches, int heightCm, DateTime birthDate,
                int waypointCount, int trackpointCount, int manualRouteCount, int pcRouteCount, int courseCount)
            {
                this.UserName = userName;
                this.IsFemale = isFemale;
                this.Age = age;
                this.WeightPounds = weightPounds;
                this.WeightKg = weightKg;
                this.HeightInches = heightInches;
                this.HeightCm = heightCm;
                this.BirthDate = birthDate;
                this.DeviceName = deviceName;
                this.Version = version;
                this.Firmware = firmware;
                this.WaypointCount = waypointCount;
                this.TrackpointCount = trackpointCount;
                this.ManualRouteCount = manualRouteCount;
                this.PcRouteCount = pcRouteCount;
                this.CourseeCount = courseCount;
            }

            public GlobalsatSystemConfiguration(string deviceName, string firmware,
                int waypointCount, int pcRouteCount)
            {
                this.DeviceName = deviceName;
                this.Firmware = firmware;
                this.WaypointCount = waypointCount;
                this.PcRouteCount = pcRouteCount;
            }
        }

        public class GlobalsatSystemConfiguration2
        {
            public string UserName;
            public bool IsFemale;
            public int Age;
            public int WeightPounds;
            public int WeightKg;
            public int HeightInches;
            public int HeightCm;
            public DateTime BirthDate;

            public GlobalsatSystemConfiguration2()
            {
            }

            public GlobalsatSystemConfiguration2(string deviceName, double version, string firmware,
                string userName, bool isFemale, int age, int weightPounds, int weightKg, int heightInches, int heightCm, DateTime birthDate,
                int waypointCount, int trackpointCount, int manualRouteCount, int pcRouteCount)
            {
                this.UserName = userName;
                this.IsFemale = isFemale;
                this.Age = age;
                this.WeightPounds = weightPounds;
                this.WeightKg = weightKg;
                this.HeightInches = heightInches;
                this.HeightCm = heightCm;
                this.BirthDate = birthDate;
            }
        }

        public GlobalsatPacket GetWhoAmI()
        {
            InitPacket(CommandWhoAmI, 0);
            return this;
        }

        public GlobalsatPacket GetSystemConfiguration()
        {
            InitPacket(CommandGetSystemInformation, 0);
            return this;
        }

        public virtual GlobalsatSystemConfiguration ResponseGetSystemConfiguration()
        {
            string deviceName = ByteArr2String(0, 20 + 1);
            string firmware = ByteArr2String(21, 16 + 1);
            int waypointCount = (int)PacketData[38];
            int pcRouteCount = (int)PacketData[39];

            GlobalsatSystemConfiguration systemInfo = new GlobalsatSystemConfiguration(deviceName, firmware, waypointCount, pcRouteCount);

            return systemInfo;
        }

        public GlobalsatPacket GetSystemConfiguration2()
        {
            InitPacket(CommandGetSystemConfiguration, 0);
            return this;
        }

        public virtual GlobalsatSystemConfiguration2 ResponseGetSystemConfiguration2() { throw new Exception(InvalidOperation); }

        //public virtual GlobalsatPacket SetSystemInformation(byte[] data) { throw new Exception(InvalidOperation); }
        public virtual GlobalsatPacket SetSystemConfiguration2(byte[] data)
        {
            InitPacket(CommandSetSystemConfiguration, (Int16)data.Length);

            for (int i = 0; i < data.Length; i++)
            {
                this.PacketData[i] = data[i];
            }

            return this;
        }

        public GlobalsatPacket GetNextTrackSection()
        {
            InitPacket(CommandGetNextTrackSection, 0);
            return this;
        }

        public GlobalsatPacket GetTrackFileHeaders()
        {
            InitPacket(CommandGetTrackFileHeaders, 0);
            return this;
        }
        public GlobalsatPacket GetTrackFileSections(IList<Int16> trackPointIndexes)
        {
            this.InitPacket(CommandGetTrackFileSections, (Int16)(2 + trackPointIndexes.Count * 2));
            this.Write(0, (Int16)trackPointIndexes.Count);
            int offset = 2;
            foreach (Int16 index in trackPointIndexes)
            {
                this.Write(offset, index);
                offset += 2;
            }
            return this;
        }

        public GlobalsatPacket GetWaypoints()
        {
            InitPacket(CommandGetWaypoints, 0);
            return this;
        }

        public virtual IList<GlobalsatWaypoint> ResponseGetWaypoints()
        {
            int nrWaypoints = PacketLength / (LocationLength + GetWptOffset);
            IList<GlobalsatWaypoint> waypoints = new List<GlobalsatWaypoint>(nrWaypoints);

            for (int i = 0; i < nrWaypoints; i++)
            {
                int index = i * (LocationLength + GetWptOffset);

                string waypointName = ByteArr2String(index, 6);
                int iconNr = (int)PacketData[index + 7];
                short altitude = ReadInt16(index + 8);
                int latitudeInt = ReadInt32(index + 10 + GetWptOffset);
                int longitudeInt = ReadInt32(index + 14 + GetWptOffset);
                double latitude = (double)latitudeInt / 1000000.0;
                double longitude = (double)longitudeInt / 1000000.0;

                GlobalsatWaypoint waypoint = new GlobalsatWaypoint(waypointName, iconNr, altitude, latitude, longitude);
                waypoints.Add(waypoint);
            }

            return waypoints;
        }

        public virtual GlobalsatPacket SendWaypoints(int MaxNrWaypoints, IList<GlobalsatWaypoint> waypoints)
        {
            int nrWaypointsLength = 2;
            int waypointNameLength = 6;

            int nrWaypoints = Math.Min(MaxNrWaypoints, waypoints.Count);

            Int16 totalLength = (Int16)(nrWaypointsLength + SendWptOffset + nrWaypoints * (LocationLength + SendWptOffset));
            this.InitPacket(CommandSendWaypoint, totalLength);

            int offset = 0;

            this.Write(offset, (short)nrWaypoints);
            offset += 2;

            offset += SendWptOffset; //pad -some only?

            int waypOffset = offset;
            for (int i = 0; i < nrWaypoints; i++)
            {
                GlobalsatWaypoint waypoint = waypoints[i];
                offset = waypOffset + i * (LocationLength + 2);

                this.Write(offset, waypointNameLength + 1, waypoint.WaypointName);
                offset += waypointNameLength+1;

                this.PacketData[offset] = (byte)waypoint.IconNr;
                offset++;

                this.Write(offset, waypoint.Altitude);
                offset += 2;

                offset += SendWptOffset; //pad?

                int latitude = (int)Math.Round(waypoint.Latitude * 1000000);
                int longitude = (int)Math.Round(waypoint.Longitude * 1000000);

                this.Write32(offset, latitude);
                this.Write32(offset + 4, longitude);
            }
            return this;
        }

        public virtual int ResponseSendWaypoints()
        {
            //TODO: Size check
            int nrSentWaypoints = this.ReadInt16(0);
            return nrSentWaypoints;
        }

        public virtual GlobalsatPacket DeleteAllWaypoints()
        {
            this.InitPacket(CommandDeleteWaypoints, 2);
            this.Write(0, (Int16)100);
            return this;
        }

        public virtual GlobalsatPacket DeleteWaypoints(int MaxNrWaypoints, IList<GlobalsatWaypoint> waypoints)
        {
            int nrWaypointsLength = 2;
            int waypointNameLength = 6;

            int nrWaypoints = Math.Min(MaxNrWaypoints, waypoints.Count);

            Int16 totalLength = (Int16)(nrWaypointsLength + nrWaypoints * 7);
            this.InitPacket(CommandDeleteWaypoints, totalLength);

            int offset = 0;

            this.Write(offset, (short)nrWaypoints);
            offset += 2;

            for (int i = 0; i < nrWaypoints && nrWaypoints < MaxNrWaypoints; i++)
            {
                string waypointName = waypoints[i].WaypointName;

                this.Write(offset, waypointNameLength + 1, waypointName);
                offset += waypointNameLength+1;
            }

            return this;
        }

        public virtual GlobalsatPacket SendRoute(GlobalsatRoute route)
        {
            int maxRouteNameLenght = GlobalsatRoute.MaxRouteNameLength;
            int nrPointsLenght = 1;

            byte nrPoints = (byte)Math.Min(0xFF, route.wpts.Count);
            string routeName = route.Name.Substring(0, Math.Min(maxRouteNameLenght, route.Name.Length));
            Int16 totalLength = (Int16)(1 + (maxRouteNameLenght + 1) + nrPointsLenght + nrPoints * 8); // save a byte for the ending null char
            this.InitPacket(CommandSendRoute, totalLength);

            int offset = 0;
            this.Write(0, maxRouteNameLenght + 1, routeName);
            offset += maxRouteNameLenght + 1;

            this.PacketData[offset++] = nrPoints;

            for (int i = 0; i < nrPoints; i++)
            {
                int latitude = (int)Math.Round(route.wpts[i].Latitude * 1000000);
                int longitude = (int)Math.Round(route.wpts[i].Longitude * 1000000);

                Write32(offset, latitude);
                Write32(offset + 4, longitude);

                offset += 8;
            }

            return this;
        }

        public virtual GlobalsatPacket SendTrackStart(TrackFileBase trackFile)
        {
            Int16 nrLaps = 1;
            Int16 totalLength = (Int16)(31 + nrLaps * 22); //xxx 505 52?
            this.InitPacket(CommandSendTrackStart, totalLength);

            int offset = 0;

            int year = (int)Math.Max(0, trackFile.StartTime.Year - 2000);
            this.PacketData[offset++] = (byte)year;
            this.PacketData[offset++] = (byte)trackFile.StartTime.Month;
            this.PacketData[offset++] = (byte)trackFile.StartTime.Day;
            this.PacketData[offset++] = (byte)trackFile.StartTime.Hour;
            this.PacketData[offset++] = (byte)trackFile.StartTime.Minute;
            this.PacketData[offset++] = (byte)trackFile.StartTime.Second;

            //xxx 625 - 505 differs
            this.PacketData[offset++] = (byte)nrLaps;
            int totalTimeSecondsTimes10 = (int)(trackFile.TotalTime.TotalMilliseconds / 100);
            this.Write32(offset, totalTimeSecondsTimes10); offset += 4;
            this.Write32(offset, trackFile.TotalDistanceMeters); offset += 4;
            this.Write(offset, trackFile.TotalCalories); offset += 2;
            this.Write(offset, trackFile.MaximumSpeed); offset += 2;
            this.PacketData[offset++] = (byte)trackFile.MaximumHeartRate;
            this.PacketData[offset++] = (byte)trackFile.AverageHeartRate;
            this.Write(offset, trackFile.TotalAscent); offset += 2;
            this.Write(offset, trackFile.TotalDescent); offset += 2;
            this.Write(offset, trackFile.TrackPointCount); offset += 2;

            // unused fields
            this.Write(offset, 0); offset += 2;
            this.Write(offset, 0); offset += 2;

            // send only one lap
            this.Write32(offset, totalTimeSecondsTimes10); offset += 4;
            this.Write32(offset, totalTimeSecondsTimes10); offset += 4;
            this.Write32(offset, trackFile.TotalDistanceMeters); offset += 4;
            this.Write(offset, trackFile.TotalCalories); offset += 2;
            this.Write(offset, trackFile.MaximumSpeed); offset += 2;
            this.PacketData[offset++] = (byte)trackFile.MaximumHeartRate;
            this.PacketData[offset++] = (byte)trackFile.AverageHeartRate;

            // start/end index
            this.Write(offset, 0); offset += 2;
            this.Write(offset, (short)(trackFile.TrackPointCount - 1)); offset += 2;

            if (offset == totalLength)
            {
            }
            return this;
        }

        public virtual GlobalsatPacket SendTrackSection(TrackFileSectionSend trackFile)
        {
            int trackPointCount = trackFile.EndPointIndex - trackFile.StartPointIndex + 1;

            int nrLaps = 1;
            Int16 totalLength = (Int16)(31 + trackPointCount * 15);
            this.InitPacket(CommandSendTrackSection, totalLength);
            int offset = 0;

            int year = (int)Math.Max(0, trackFile.StartTime.Year - 2000);
            this.PacketData[offset++] = (byte)year;
            this.PacketData[offset++] = (byte)trackFile.StartTime.Month;
            this.PacketData[offset++] = (byte)trackFile.StartTime.Day;
            this.PacketData[offset++] = (byte)trackFile.StartTime.Hour;
            this.PacketData[offset++] = (byte)trackFile.StartTime.Minute;
            this.PacketData[offset++] = (byte)trackFile.StartTime.Second;

            this.PacketData[offset++] = (byte)nrLaps;
            int totalTimeSecondsTimes10 = (int)(trackFile.TotalTime.TotalMilliseconds / 100);
            this.Write32(offset, totalTimeSecondsTimes10); offset += 4;
            this.Write32(offset, trackFile.TotalDistanceMeters); offset += 4;
            this.Write(offset, trackFile.TotalCalories); offset += 2;
            this.Write(offset, trackFile.MaximumSpeed); offset += 2;
            this.PacketData[offset++] = (byte)trackFile.MaximumHeartRate;
            this.PacketData[offset++] = (byte)trackFile.AverageHeartRate;
            this.Write(offset, trackFile.TotalAscent); offset += 2;
            this.Write(offset, trackFile.TotalDescent); offset += 2;
            this.Write(offset, trackFile.TrackPointCount); offset += 2;

            // unused fields
            this.Write(offset, trackFile.StartPointIndex); offset += 2;
            this.Write(offset, trackFile.EndPointIndex); offset += 2;

            foreach (TrackPointSend trackpoint in trackFile.TrackPoints)
            {
                this.Write32(offset, trackpoint.Latitude); offset += 4;
                this.Write32(offset, trackpoint.Longitude); offset += 4;
                this.Write(offset, trackpoint.Altitude); offset += 2;
                this.Write(offset, trackpoint.Speed); offset += 2;
                this.PacketData[offset++] = (byte)trackpoint.HeartRate;
                this.Write(offset, trackpoint.IntervalTime); offset += 2;
            }

            if (offset == totalLength)
            {
            }

            return this;
        }

        public GlobalsatPacket GetScreenshot()
        {
            InitPacket(CommandGetScreenshot, 0);
            return this;
        }

        public virtual System.Drawing.Bitmap ResponseScreenshot() { return GlobalsatBitmap.GetBitmap(this.ScreenBpp, this.ScreenSize, this.ScreenRowCol, this.PacketData); }

    //Packetsizes
        protected virtual int LocationLength { get { return 18; } }
        protected virtual int GetWptOffset { get { return 0; } }
        protected virtual int SendWptOffset { get { return 0; } }
        public virtual int TrackPointsPerSection { get { return 136; } }
    }
}

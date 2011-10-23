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

using ZoneFiveSoftware.Common.Data.GPS;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    public class GlobalsatPacket : GhPacketBase
    {
        public GlobalsatPacket() { }
        public GlobalsatPacket(byte commandId) : base(commandId, 0) { }
        public GlobalsatPacket(byte commandId, Int16 len) : base(commandId, len) { }

        private static string InvalidOperation = "Invalid Operation";

        public class GlobalsatSystemInformation
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

            public GlobalsatSystemInformation(string deviceName, double version, string firmware,
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

            public GlobalsatSystemInformation(string deviceName, string firmware,
                int waypointCount, int pcRouteCount)
            {
                this.DeviceName = deviceName;
                this.Firmware = firmware;
                this.WaypointCount = waypointCount;
                this.PcRouteCount = pcRouteCount;
            }
        }

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

            public GlobalsatSystemConfiguration()
            {
            }

            public GlobalsatSystemConfiguration(string deviceName, double version, string firmware,
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

        //Implemented in DeviceBase
        //public GlobalsatPacket GetWhoAmI()
        //{
        //    return new GlobalsatPacket(CommandWhoAmI);
        //}

        public GlobalsatPacket GetSystemInformation()
        {
            return new GlobalsatPacket(CommandGetSystemInformation);
        }

        public virtual GlobalsatSystemInformation ResponseGetSystemInformation()
        {
            string deviceName = ByteArr2String(PacketData, 0, 20 + 1);
            string firmware = ByteArr2String(PacketData, 21, 16 + 1);
            int waypointCount = (int)PacketData[38];
            int pcRouteCount = (int)PacketData[39];

            GlobalsatSystemInformation systemInfo = new GlobalsatSystemInformation(deviceName, firmware, waypointCount, pcRouteCount);

            return systemInfo;
        }

        public GlobalsatPacket GetSystemConfiguration()
        {
            return new GlobalsatPacket(CommandGetSystemConfiguration);
        }

        public virtual GlobalsatSystemConfiguration ResponseGetSystemConfiguration() { throw new Exception(InvalidOperation); }

        public GlobalsatPacket GetNextSection()
        {
            return new GlobalsatPacket(CommandGetNextSection);
        }

        public GlobalsatPacket GetTrackFileHeaders()
        {
            return new GlobalsatPacket(CommandGetTrackFileHeaders);
        }
        public GlobalsatPacket GetTrackFileSections(IList<Int16> trackPointIndexes)
        {
            GlobalsatPacket packet = new GlobalsatPacket(CommandGetTrackFileSections, (Int16)(2 + trackPointIndexes.Count * 2));
            packet.Write(0, (Int16)trackPointIndexes.Count);
            int offset = 2;
            foreach (Int16 index in trackPointIndexes)
            {
                packet.Write(offset, index);
                offset += 2;
            }
            return packet;
        }

        public GlobalsatPacket GetScreenshot()
        {
            return new GlobalsatPacket(CommandGetScreenshot);
        }

        //public virtual GlobalsatPacket GetDeviceIdentification() { throw new Exception(InvalidOperation); }
        //public abstract GlobalsatPacket GetSystemInformation();
        //public abstract GlobalsatPacket GetSystemConfiguration();

        public virtual IList<GlobalsatWaypoint> ResponseGetWaypoints()
        {
            return new List<GlobalsatWaypoint>();
        }

        public virtual int GetSentWaypoints()
        {
            //TODO: Size check
            int nrSentWaypoints = this.ReadInt16(0);

            return nrSentWaypoints;
        }

        public virtual GlobalsatPacket DeleteAllWaypoints()
        {
            GlobalsatPacket packet = new GlobalsatPacket(CommandDeleteWaypoints, 2);
            packet.Write(0, (Int16)100);
            return packet;
        }

        public virtual GlobalsatPacket DeleteWaypoints(int MaxNrWaypoints, IList<GlobalsatWaypoint> waypoints)
        {
            int nrWaypointsLength = 2;
            int waypointNameLength = 6;

            int nrWaypoints = Math.Min(MaxNrWaypoints, waypoints.Count);

            Int16 totalLength = (Int16)(nrWaypointsLength + nrWaypoints * 7);
            GlobalsatPacket packet = new GlobalsatPacket(CommandDeleteWaypoints, totalLength);

            int offset = 0;

            packet.Write(offset, (short)nrWaypoints);
            offset += 2;

            for (int i = 0; i < nrWaypoints && nrWaypoints < MaxNrWaypoints; i++)
            {
                string waypointName = waypoints[i].WaypointName;

                for (int j = 0; j < waypointNameLength; j++)
                {
                    byte c = (byte)(j < waypointName.Length ? waypointName[j] : '\0');
                    packet.PacketData[offset + j] = c;
                }
                offset += waypointNameLength;
                packet.PacketData[offset] = 0; // adds ending \0
                offset++;
            }

            return packet;
        }

        public virtual GlobalsatPacket SendRoute(GlobalsatRoute route) { throw new Exception(InvalidOperation); }
        public virtual GlobalsatPacket SendWaypoints(IList<GlobalsatWaypoint> waypoints) { throw new Exception(InvalidOperation); }
        public virtual GlobalsatPacket SetSystemInformation(byte[] data) { throw new Exception(InvalidOperation); }
        public virtual GlobalsatPacket SetSystemConfiguration(byte[] data) { throw new Exception(InvalidOperation); }
        public virtual IList<GlobalsatPacket> SendTrack(IGPSRoute gpsRoute) { throw new Exception(InvalidOperation); }

        public virtual System.Drawing.Bitmap ResponseScreenshot() { return GlobalsatBitmap.GetBitmap(this.ScreenBpp, this.ScreenSize, this.PacketData); }
    }
}

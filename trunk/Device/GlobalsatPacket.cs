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

        //Implemented in DeviceBase
        //public GlobalsatPacket GetWhoAmI()
        //{
        //    return new GlobalsatPacket(CommandWhoAmI);
        //}

        public GlobalsatPacket GetSystemInformation()
        {
            return new GlobalsatPacket(CommandGetSystemInformation);
        }

        public GlobalsatPacket GetSystemConfiguration2()
        {
            return new GlobalsatPacket(CommandGetSystemConfiguration);
        }

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
        //public abstract GlobalsatPacket GetWaypoints();
        //public abstract GlobalsatPacket DeleteAllWaypoints();
        public virtual GlobalsatProtocol.GlobalsatSystemInformation ResponseSystemInformation() { throw new Exception(InvalidOperation); }
        public virtual IList<GlobalsatWaypoint> ResponseWaypoints()
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


        public virtual GlobalsatPacket DeleteWaypoints(int MaxNrWaypoints, IList<string> waypointNames)
        {
            int nrWaypointsLength = 2;
            int waypointNameLength = 6;

            int nrWaypoints = Math.Min(MaxNrWaypoints, waypointNames.Count);

            Int16 totalLength = (Int16)(nrWaypointsLength + nrWaypoints * 7);
            GlobalsatPacket packet = new GlobalsatPacket(CommandDeleteWaypoints, totalLength);

            int offset = 0;

            packet.Write(offset, (short)nrWaypoints);
            offset += 2;

            for (int i = 0; i < nrWaypoints && nrWaypoints < MaxNrWaypoints; i++)
            {
                string waypointName = waypointNames[i];

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
        public virtual GlobalsatPacket SendWaypoints(List<GlobalsatWaypoint> waypoints) { throw new Exception(InvalidOperation); }
        public virtual GlobalsatPacket DeleteWaypoints(List<string> waypointNames) { throw new Exception(InvalidOperation); }
        public virtual GlobalsatPacket SetSystemInformation(byte[] data) { throw new Exception(InvalidOperation); }
        public virtual GlobalsatPacket SetSystemConfiguration(byte[] data) { throw new Exception(InvalidOperation); }
        public virtual List<GlobalsatPacket> SendTrack(IGPSRoute gpsRoute) { throw new Exception(InvalidOperation); }

        public virtual System.Drawing.Bitmap ResponseScreenshot() { return GlobalsatBitmap.GetBitmap(this.ScreenBpp, this.ScreenSize, this.PacketData); }
    }
}

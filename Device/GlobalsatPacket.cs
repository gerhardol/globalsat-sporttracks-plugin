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
using System.Windows.Forms;


namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    public class GlobalsatPacket : GhPacketBase
    {
        //private static string InvalidOperation = "Invalid Operation";

        private static bool showOffsetPopup = true;
        protected void ReportOffset(int headerLen, int offset)
        {
            if (showOffsetPopup)
            {
                //Debug, show popup
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
                System.Diagnostics.StackFrame[] stFrames = st.GetFrames();
                string trace = "";
                for (int i = 1; i < stFrames.Length && i < 6; i++)
                {
                    //TODO: Nicer formatting
                    trace += stFrames[i].ToString() + System.Environment.NewLine;
                }

                string s = string.Format("Error occurred, unexpected offsets: {0}({1}).{2}{3}{2}Report to plugin maintainer.{2}To ignore further errors, press {4}",
                        headerLen, offset, System.Environment.NewLine, trace, DialogResult.Yes);

                if (MessageBox.Show(s, "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    showOffsetPopup = false;
                }
            }
        }
        protected int CheckOffset(int headerLen, int offset)
        {
            if(headerLen != offset)
            {
                ReportOffset(headerLen, offset);
            }
            return headerLen;
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
            //No reasonable check for CheckOffset()
            string deviceName = ByteArr2String(0, 20 + 1);
            string firmware = ByteArr2String(25, 16 + 1); //21wo version
            int waypointCount = (int)PacketData[63]; //38 in 615?
            int pcRouteCount = 0;
            if (this.PacketLength > 70)
            {
                pcRouteCount = (int)PacketData[70]; //39 in 615?
            }
            GlobalsatSystemConfiguration systemInfo = new GlobalsatSystemConfiguration(deviceName, firmware, waypointCount, pcRouteCount);

            return systemInfo;
        }

        public GlobalsatPacket GetSystemConfiguration2()
        {
            InitPacket(CommandGetSystemConfiguration, 0);
            return this;
        }

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

        //Currently unused, info is not decoded
        //public virtual GlobalsatSystemConfiguration2 ResponseGetSystemConfiguration2() { throw new Exception(InvalidOperation); }

        public GlobalsatPacket GetWaypoints()
        {
            InitPacket(CommandGetWaypoints, 0);
            return this;
        }

        public virtual IList<GlobalsatWaypoint> ResponseGetWaypoints()
        {
            //No reasonable CheckOffset()
            int nrWaypoints = PacketLength / LocationLength;
            IList<GlobalsatWaypoint> waypoints = new List<GlobalsatWaypoint>(nrWaypoints);
            
            for (int i = 0; i < nrWaypoints; i++)
            {
                int index = i * LocationLength;

                string waypointName = ByteArr2String(index, 6);
                int iconNr = (int)PacketData[index + 7];
                short altitude = ReadInt16(index + 8);
                double latitude = ReadLatLon(index + 10 + WptLatLonOffset);
                double longitude = ReadLatLon(index + 14 + WptLatLonOffset);

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

            //Same WptLatLonOffset is used when padding - could differ in other devices
            Int16 totalLength = (Int16)(nrWaypointsLength + WptLatLonOffset + nrWaypoints * LocationLength);
            this.InitPacket(CommandSendWaypoint, totalLength);

            int offset = 0;

            offset += this.Write(offset, (short)nrWaypoints);

            offset += WptLatLonOffset; //pad -some only

            int waypOffset = offset;
            for (int i = 0; i < nrWaypoints; i++)
            {
                GlobalsatWaypoint waypoint = waypoints[i];
                offset = waypOffset + i * LocationLength;

                //Points to be added requires names
                if (string.IsNullOrEmpty(waypoint.WaypointName))
                {
                    waypoint.WaypointName = "WR" + sendIdentification++;
                }

                offset += this.Write(offset, waypointNameLength + 1, waypoint.WaypointName);

                this.PacketData[offset++] = (byte)waypoint.IconNr;

                offset += this.Write(offset, waypoint.Altitude);

                offset += WptLatLonOffset; //pad
                offset += this.Write32(offset, ToGlobLatLon(waypoint.Latitude));
                offset += this.Write32(offset, ToGlobLatLon(waypoint.Longitude));
            }
            CheckOffset(totalLength, offset);
            return this;
        }

        public virtual int ResponseSendWaypoints()
        {
            if (PacketLength < 2)
            {
                ReportOffset(PacketLength, 2);
            }
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

            for (int i = 0; i < nrWaypoints && nrWaypoints <= MaxNrWaypoints; i++)
            {
                string waypointName = waypoints[i].WaypointName;

                offset += this.Write(offset, waypointNameLength + 1, waypointName);
            }

            CheckOffset(totalLength, offset);
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
            Int16 totalLength = (Int16)(Int16)(2 + trackPointIndexes.Count * 2);
            this.InitPacket(CommandGetTrackFileSections, totalLength);
            this.Write(0, (Int16)trackPointIndexes.Count);
            int offset = 2;
            foreach (Int16 index in trackPointIndexes)
            {
                this.Write(offset, index);
                offset += 2;
            }
            CheckOffset(totalLength, offset);
            return this;
        }

        public virtual GlobalsatPacket SendTrackStart(Train trackFile) { throw new GlobalsatProtocol.FeatureNotSupportedException(); }
        //Not used by all devices
        public virtual GlobalsatPacket SendTrackLaps(Train trackFile) { return null; }

        public virtual GlobalsatPacket SendTrackSection(Train trackFile, int startIndex, int endIndex)
        {
            int trackPointCount = endIndex - startIndex + 1;

            Int16 totalLength = (Int16)(TrackHeaderLength + trackPointCount * TrackPointLength);
            this.InitPacket(CommandSendTrackSection, totalLength);
            int offset = 0;

            offset += WriteTrackPointHeader(offset, trackFile, startIndex, endIndex);
            for (int i = startIndex; i<=endIndex; i++)
            {
                offset += WriteTrackPoint(offset, trackFile.TrackPoints[i]);
            }

            CheckOffset(totalLength, offset);
            return this;
        }

        //Only 625M uses Train, other only Header
        protected virtual int WriteTrackPointHeader(int offset, Train trackFile, int StartPointIndex, int EndPointIndex) { throw new GlobalsatProtocol.FeatureNotSupportedException(); }
        protected virtual int WriteTrackPoint(int offset, TrackPoint trackpoint) { throw new GlobalsatProtocol.FeatureNotSupportedException(); }

        public virtual GlobalsatPacket SendRoute(GlobalsatRoute route)
        {
            const int nrPointsLength = 1;

            //Name limitations in some devives?
            //if (this is Gh625XTPacket)
            //{
            //    route.Name = route.Name.Replace('-', '_');
            //}

            byte nrPoints = (byte)Math.Min(0xFF, route.wpts.Count);
            Int16 totalLength = (Int16)((MaxRouteNameLength + 1) + nrPointsLength + nrPoints * RouteWaypointLength); // save a byte for the ending null char
            this.InitPacket(CommandSendRoute, totalLength);

            int offset = 0;
            offset += this.Write(offset, MaxRouteNameLength + 1, route.Name);

            this.PacketData[offset++] = nrPoints;

            for (int i = 0; i < nrPoints; i++)
            {
                offset += this.Write32(offset, ToGlobLatLon(route.wpts[i].Latitude));
                offset += this.Write32(offset, ToGlobLatLon(route.wpts[i].Longitude));
            }

            CheckOffset(totalLength, offset);
            return this;
        }

        public virtual GlobalsatPacket GetScreenshot()
        {
            InitPacket(CommandGetScreenshot, 0);
            return this;
        }

        public virtual System.Drawing.Bitmap ResponseGetScreenshot() { return GlobalsatBitmap.GetBitmap(this.ScreenBpp, this.ScreenSize, this.ScreenRowCol, this.RotateFlip, this.PacketData); }

        //For autogenerated names
        protected static int sendIdentification=0;

    //Packetsizes
        protected virtual int LocationLength { get { return 18 + WptLatLonOffset; } }
        protected virtual int WptLatLonOffset { get { return 0; } }

        public virtual int TrackPointsPerSection { get { return 136; } }
        //Unused: public virtual int TrackLapsPerSection { get { return 58; } }
        protected virtual int TrackHeaderLength { get { return 24; } }
        protected virtual int TrainDataHeaderLength { get { return 31; } }
        protected virtual int TrackLapLength { get { return 22; } }
        protected virtual int TrackPointLength { get { return 25; } }
        protected virtual int TrainHeaderCTypeOffset { get { return TrackHeaderLength - 1; } } //cDataType
		
        protected virtual int MaxRouteNameLength { get { return 15; } }
        protected virtual int RouteWaypointLength { get { return 8; } }
		
        //Some device related settings - accessed from packet classes
        //Could be moved to device classes, accessed with a device reference when creating packets
        protected virtual System.Drawing.Size ScreenSize { get { return new System.Drawing.Size(128, 96); } }
        protected virtual int ScreenBpp { get { return 2; } }
        protected virtual bool ScreenRowCol { get { return true; } } //Screenshot row over columns
        protected virtual System.Drawing.RotateFlipType RotateFlip { get { return System.Drawing.RotateFlipType.RotateNoneFlipNone; } }
    }


    //Newer devices
    public abstract class GlobalsatPacket2 : GlobalsatPacket
    {
        public virtual byte GetTrainContent()
        {
            return this.PacketData[TrainHeaderCTypeOffset];
        }
        public abstract IList<TrackFileHeader> UnpackTrackHeaders();
        public abstract Train UnpackTrainHeader();
        public abstract IList<Lap> UnpackLaps();
        public abstract IList<TrackPoint> UnpackTrackPoints();
    }
}

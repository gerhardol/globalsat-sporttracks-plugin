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
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class Gh615Device : GlobalsatProtocol
    {
        public Gh615Device() : base() { }
        public Gh615Device(string configInfo) : base(configInfo) { }
        
        public override DeviceConfigurationInfo DefaultConfig
        {
            get
            {
                DeviceConfigurationInfo info = new DeviceConfigurationInfo(new List<string> { "GH-615" }, new List<int> { 57600 });
                return info;
            }
        }

        public override GlobalsatPacket PacketFactory { get { return new Gh615Packet(); } }

        public override ImportJob ImportJob(string sourceDescription, IJobMonitor monitor, IImportResults importResults)
        {
            return new ImportJob_GH615(this, sourceDescription, monitor, importResults);
        }

        public IList<Gh615Packet.TrackFileHeader> ReadTrackHeaders(IJobMonitor monitor)
        {
            monitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_OpeningDevice;

            Int16[] tracks = new Int16[2];

            GlobalsatPacket getHeadersPacket = PacketFactory.GetTrackFileHeaders();
            Gh615Packet response = (Gh615Packet)SendPacket(getHeadersPacket);
            return response.UnpackTrackHeaders();
        }

        public IList<Gh615Packet.TrackFileSection> ReadTracks(IList<Gh615Packet.TrackFileHeader> tracks, IJobMonitor monitor)
        {
            if (tracks.Count == 0) return new Gh615Packet.TrackFileSection[0];

            float totalPoints = 0;
            IList<Int16> trackIndexes = new List<Int16>();
            foreach (Gh615Packet.TrackFileHeader header in tracks)
            {
                totalPoints += header.TrackPointCount;
                trackIndexes.Add(header.TrackPointIndex);
            }
            float pointsRead = 0;

            IList<Gh615Packet.TrackFileSection> trackSections = new List<Gh615Packet.TrackFileSection>();
            GlobalsatPacket getFilesPacket = PacketFactory.GetTrackFileSections(trackIndexes);
            GlobalsatPacket getNextPacket = PacketFactory.GetNextTrackSection();
            Gh615Packet data = (Gh615Packet)SendPacket(getFilesPacket);

            monitor.PercentComplete = 0;

            Gh615Packet.TrackFileSection trackSection;
            do
            {
                trackSection = data.UnpackTrackSection();
                if (trackSection != null)
                {
                    pointsRead += trackSection.EndPointIndex - trackSection.StartPointIndex + 1;

                    string statusProgress = trackSection.StartTime.ToShortDateString() + " " + trackSection.StartTime.ToShortTimeString();
                    monitor.StatusText = String.Format(CommonResources.Text.Devices.ImportJob_Status_Reading, statusProgress);
                    monitor.PercentComplete = pointsRead / totalPoints;

                    trackSections.Add(trackSection);
                    data = (Gh615Packet)SendPacket(getNextPacket);
                }
            } while (trackSection != null);

            monitor.PercentComplete = 1;
            return trackSections;
        }
        public override int SendTrack(IList<IActivity> activities, IJobMonitor jobMonitor) { throw new FeatureNotSupportedException(); }
    }
}

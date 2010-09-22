// Copyright (C) 2010 Zone Five Software
// Author: Aaron Averill
using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class Gh615Device : GhDeviceBase
    {
        public IList<Gh615Packet.TrackFileHeader> ReadTrackHeaders(IJobMonitor monitor)
        {
            monitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_OpeningDevice;

            Int16[] tracks = new Int16[2];

            byte[] getHeadersPacket = Gh615Packet.GetTrackFileHeaders();
            byte[] data = SendPacket(Port, getHeadersPacket).PacketData;
            return Gh615Packet.UnpackTrackHeaders(data);
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
            byte[] getFilesPacket = Gh615Packet.GetTrackFileSections(trackIndexes);
            byte[] getNextPacket = Gh615Packet.GetNextSection();
            byte[] data = SendPacket(Port, getFilesPacket).PacketData;

            monitor.PercentComplete = 0;

            Gh615Packet.TrackFileSection trackSection;
            do
            {
                trackSection = Gh615Packet.UnpackTrackSection(data);
                if (trackSection != null)
                {
                    pointsRead += trackSection.EndPointIndex - trackSection.StartPointIndex + 1;

                    string statusProgress = trackSection.StartTime.ToShortDateString() + " " + trackSection.StartTime.ToShortTimeString();
                    monitor.StatusText = String.Format(CommonResources.Text.Devices.ImportJob_Status_Reading, statusProgress);
                    monitor.PercentComplete = pointsRead / totalPoints;

                    trackSections.Add(trackSection);
                    data = SendPacket(Port, getNextPacket).PacketData;
                }
            } while (trackSection != null);

            monitor.PercentComplete = 1;
            return trackSections;
        }
    }
}

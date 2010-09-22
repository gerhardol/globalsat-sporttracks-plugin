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
    class Gh625Device : GhDeviceBase
    {
        public IList<Gh625Packet.TrackFileHeader> ReadTrackHeaders(IJobMonitor monitor)
        {
            monitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_OpeningDevice;

            Int16[] tracks = new Int16[2];

            byte[] getHeadersPacket = Gh625Packet.GetTrackFileHeaders();
            byte[] data = SendPacket(Port, getHeadersPacket).PacketData;
            return Gh625Packet.UnpackTrackHeaders(data);
        }

        public IList<Gh625Packet.TrackFileSection> ReadTracks(IList<Gh625Packet.TrackFileHeader> tracks, IJobMonitor monitor)
        {
            if (tracks.Count == 0) return new Gh625Packet.TrackFileSection[0];

            float totalPoints = 0;
            IList<Int16> trackIndexes = new List<Int16>();
            foreach (Gh625Packet.TrackFileHeader header in tracks)
            {
                totalPoints += header.TrackPointCount;
                trackIndexes.Add(header.TrackPointIndex);
            }
            float pointsRead = 0;

            IList<Gh625Packet.TrackFileSection> trackSections = new List<Gh625Packet.TrackFileSection>();
            byte[] getFilesPacket = Gh625Packet.GetTrackFileSections(trackIndexes);
            byte[] getNextPacket = Gh625Packet.GetNextSection();
            byte[] data = SendPacket(Port, getFilesPacket).PacketData;

            monitor.PercentComplete = 0;

            Gh625Packet.TrackFileSection trackSection;
            int numInCurrentTrain = 0;
            int readInCurrentTrain = 0;
            do
            {
                if (numInCurrentTrain == 0)
                {
                    // The section is a laps section (the first of the section)
                    trackSection = Gh625Packet.UnpackTrackSectionLaps(data);
                    if (trackSection != null)
                    {
                        numInCurrentTrain = trackSection.TrackPointCount;
                        readInCurrentTrain = 0;
                    }
                }
                else
                {
                    // The section is a GPS/HRM detail section
                    trackSection = Gh625Packet.UnpackTrackSection(data);
                    if (trackSection != null)
                    {
                        int pointsInThisSection = trackSection.EndPointIndex - trackSection.StartPointIndex + 1;
                        readInCurrentTrain += pointsInThisSection;
                        pointsRead += pointsInThisSection;

                        string statusProgress = trackSection.StartTime.ToShortDateString() + " " + trackSection.StartTime.ToShortTimeString();
                        monitor.StatusText = String.Format(CommonResources.Text.Devices.ImportJob_Status_Reading, statusProgress);
                        monitor.PercentComplete = pointsRead / totalPoints;

                        if (readInCurrentTrain >= numInCurrentTrain)
                        {
                            numInCurrentTrain = 0;
                        }
                    }
                }
                if (trackSection != null)
                {
                    trackSections.Add(trackSection);
                    data = SendPacket(Port, getNextPacket).PacketData;
                }
            } while (trackSection != null);

            monitor.PercentComplete = 1;
            return trackSections;
        }
    }
}

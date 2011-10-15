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
using System.IO.Ports;

using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class Gh625Device : GhDeviceBase
    {
        public override ImportJob ImportJob(string sourceDescription, DeviceConfigurationInfo configInfo, IJobMonitor monitor, IImportResults importResults)
        {
            return new ImportJob_GH625(this, sourceDescription, configInfo, monitor, importResults);
        }
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

        protected override IList<int> BaudRates { get { return new List<int> { 57600 }; } }
        //TODO: Find valid Id for KeyMaze
        public override IList<string> AllowedIds { get { return new List<string> { "GH-625M", "GH-625B", "KM" }; } }
    }
}

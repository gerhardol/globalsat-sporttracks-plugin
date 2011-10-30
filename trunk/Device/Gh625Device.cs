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
    public class Gh625Device : GlobalsatProtocol
    {
        public Gh625Device(DeviceConfigurationInfo configInfo) : base(configInfo) { }
        public Gh625Device() : base(new FitnessDevice_GH625()) { }

        public override GlobalsatPacket PacketFactory { get { return new Gh625Packet(); } }

        public override ImportJob ImportJob(string sourceDescription, IJobMonitor monitor, IImportResults importResults)
        {
            return new ImportJob_GH625(this, sourceDescription, monitor, importResults);
        }

        public IList<Gh625Packet.TrackFileHeader> ReadTrackHeaders(IJobMonitor monitor)
        {
            monitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_OpeningDevice;

            Int16[] tracks = new Int16[2];

            GlobalsatPacket getHeadersPacket = PacketFactory.GetTrackFileHeaders();
            Gh625Packet response = (Gh625Packet)SendPacket(getHeadersPacket);
            return response.UnpackTrackHeaders();
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
            GlobalsatPacket getFilesPacket = PacketFactory.GetTrackFileSections(trackIndexes);
            GlobalsatPacket getNextPacket = PacketFactory.GetNextTrackSection();
            Gh625Packet response = (Gh625Packet)SendPacket(getFilesPacket);

            monitor.PercentComplete = 0;

            Gh625Packet.TrackFileSection trackSection;
            int numInCurrentTrain = 0;
            int readInCurrentTrain = 0;
            do
            {
                if (numInCurrentTrain == 0)
                {
                    // The section is a laps section (the first of the section)
                    trackSection = response.UnpackTrackSectionLaps();
                    if (trackSection != null)
                    {
                        numInCurrentTrain = trackSection.TrackPointCount;
                        readInCurrentTrain = 0;
                    }
                }
                else
                {
                    // The section is a GPS/HRM detail section
                    trackSection = response.UnpackTrackSection();
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
                    response = (Gh625Packet)SendPacket(getNextPacket);
                }
            } while (trackSection != null);

            monitor.PercentComplete = 1;
            return trackSections;
        }
    }
}

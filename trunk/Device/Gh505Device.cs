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
    class Gh505Device : GlobalsatProtocol
    {
        public Gh505Device() : base() { }
        public Gh505Device(string configInfo) : base(configInfo) { }

        public override DeviceConfigurationInfo DefaultConfig
        {
            get
            {
                DeviceConfigurationInfo info = new DeviceConfigurationInfo(new List<string> { "GH-505", "GH-50" });
                return info;
            }
        }

        public override GlobalsatPacket PacketFactory { get { return new Gh505Packet(); } }

        public override ImportJob ImportJob(string sourceDescription, IJobMonitor monitor, IImportResults importResults)
        {
            return new ImportJob_GH505(this, sourceDescription, monitor, importResults);
        }

        public IList<Gh505Packet.TrackFileHeader> ReadTrackHeaders(IJobMonitor monitor)
        {
            monitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_OpeningDevice;

            GlobalsatPacket getHeadersPacket = PacketFactory.GetTrackFileHeaders();
            Gh505Packet response = (Gh505Packet)SendPacket(getHeadersPacket);
            return response.UnpackTrackHeaders();
        }

        private enum ReadMode
        {
            Header,
            Laps,
            Points
        }

        public IList<Gh505Packet.Train> ReadTracks(IList<Gh505Packet.TrackFileHeader> tracks, IJobMonitor monitor)
        {
            if (tracks.Count == 0) return new Gh505Packet.Train[0];

            float totalPoints = 0;
            IList<Int16> trackIndexes = new List<Int16>();
            foreach (Gh505Packet.TrackFileHeader header in tracks)
            {
                totalPoints += header.TrackPointCount;
                trackIndexes.Add((Int16)header.TrackPointIndex);
            }
            float pointsRead = 0;

            IList<Gh505Packet.Train> trains = new List<Gh505Packet.Train>();
            GlobalsatPacket getFilesPacket = PacketFactory.GetTrackFileSections(trackIndexes);
            GlobalsatPacket getNextPacket = PacketFactory.GetNextTrackSection();
            Gh505Packet response = (Gh505Packet)SendPacket(getFilesPacket);

            monitor.PercentComplete = 0;

            ReadMode readMode = ReadMode.Header;
            int trainLapsToRead = 0;
            int pointsToRead = 0;
            while (response.CommandId != Gh505Packet.CommandId_FINISH)
            {
                switch (readMode)
                {
                    case ReadMode.Header:
                        {
                            Gh505Packet.Train train = response.UnpackTrainHeader();
                            if (train != null)
                            {
                                trains.Add(train);
                                trainLapsToRead = train.LapCount;
                                pointsToRead = train.TrackPointCount;
                            }
                            readMode = ReadMode.Laps;
                            break;
                        }
                    case ReadMode.Laps:
                        {
                            Gh505Packet.Train currentTrain = trains[trains.Count - 1];
                            IList<Gh505Packet.Lap> laps = response.UnpackLaps();
                            foreach (Gh505Packet.Lap lap in laps) currentTrain.Laps.Add(lap);
                            trainLapsToRead -= laps.Count;
                            if (trainLapsToRead == 0)
                            {
                                readMode = ReadMode.Points;
                            }
                            break;
                        }
                    case ReadMode.Points:
                        {
                            Gh505Packet.Train currentTrain = trains[trains.Count - 1];
                            IList<GhPacketBase.TrackPoint> points = response.UnpackTrackPoints();
                            foreach (GhPacketBase.TrackPoint point in points) currentTrain.TrackPoints.Add(point);
                            pointsToRead -= points.Count;
                            pointsRead += points.Count;
                            DateTime startTime = currentTrain.StartTime.ToLocalTime();
                            string statusProgress = startTime.ToShortDateString() + " " + startTime.ToShortTimeString();
                            monitor.StatusText = String.Format(CommonResources.Text.Devices.ImportJob_Status_Reading, statusProgress);
                            monitor.PercentComplete = pointsRead / totalPoints;
                            if (pointsToRead == 0)
                            {
                                readMode = ReadMode.Header;
                            }
                            break;
                        }

                }
                response = (Gh505Packet)SendPacket(getNextPacket);
            }

            monitor.PercentComplete = 1;
            return trains;
        }
    }
}

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
    class Gb580Device : GlobalsatProtocol
    {
        public Gb580Device(DeviceConfigurationInfo configInfo) : base(configInfo) { }
        public Gb580Device() : base (new FitnessDevice_GB580()) { }

        public override GlobalsatPacket PacketFactory { get { return new Gb580Packet(); } }

        public override ImportJob ImportJob(string sourceDescription, IJobMonitor monitor, IImportResults importResults)
        {
            return new ImportJob_GB580(this, sourceDescription, monitor, importResults);
        }

        public IList<Gb580Packet.Train> ReadTrainHeaders(IJobMonitor monitor)
        {
            monitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_OpeningDevice;

            GlobalsatPacket getHeadersPacket = PacketFactory.GetTrackFileHeaders();
            Gb580Packet response = (Gb580Packet)SendPacket(getHeadersPacket);
            return response.UnpackTrainHeaders();
        }

        private enum ReadMode
        {
            Header,
            Laps,
            Points
        }

        public IList<Gb580Packet.Train> ReadTracks(IList<Gb580Packet.Train> headers, IJobMonitor monitor)
        {
            if (headers.Count == 0) return new Gb580Packet.Train[0];

            float totalPoints = 0;
            IList<Int16> trackIndexes = new List<Int16>();
            foreach (Gb580Packet.Train header in headers)
            {
                totalPoints += header.TrackPointCount;
                trackIndexes.Add(header.IndexStartPt);
            }
            float pointsRead = 0;

            IList<Gb580Packet.Train> trains = new List<Gb580Packet.Train>();
            GlobalsatPacket getFilesPacket = PacketFactory.GetTrackFileSections(trackIndexes);
            GlobalsatPacket getNextPacket = PacketFactory.GetNextSection();
            Gb580Packet response = (Gb580Packet)SendPacket(getFilesPacket);

            monitor.PercentComplete = 0;

            ReadMode readMode = ReadMode.Header;
            int trainLapsToRead = 0;
            int pointsToRead = 0;
            while (response.CommandId != Gb580Packet.CommandId_FINISH)
            {
                switch (readMode)
                {
                    case ReadMode.Header:
                        {
                            Gb580Packet.Train train = response.UnpackTrainHeader();
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
                            Gb580Packet.Train currentTrain = trains[trains.Count - 1];
                            IList<Gb580Packet.Lap> laps = response.UnpackLaps();
                            foreach (Gb580Packet.Lap lap in laps) currentTrain.Laps.Add(lap);
                            trainLapsToRead -= laps.Count;
                            if (trainLapsToRead == 0)
                            {
                                readMode = ReadMode.Points;
                            }
                            break;
                        }
                    case ReadMode.Points:
                        {
                            Gb580Packet.Train currentTrain = trains[trains.Count - 1];
                            IList<GhPacketBase.TrackPoint3> points = response.UnpackTrackPoints();
                            foreach (GhPacketBase.TrackPoint3 point in points) currentTrain.TrackPoints.Add(point);
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
                response = (Gb580Packet)SendPacket(getNextPacket);
            }

            monitor.PercentComplete = 1;
            return trains;
        }
    }
}

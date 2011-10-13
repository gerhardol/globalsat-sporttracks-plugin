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
    class Gh505Device : GhDeviceBase
    {
        public IList<Gh505Packet.TrackFileHeader> ReadTrackHeaders(IJobMonitor monitor)
        {
            monitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_OpeningDevice;

            byte[] getHeadersPacket = Gh505Packet.GetTrackFileHeaders();
            byte[] data = SendPacket(Port, getHeadersPacket).PacketData;
            return Gh505Packet.UnpackTrackHeaders(data);
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
                trackIndexes.Add(header.TrackPointIndex);
            }
            float pointsRead = 0;

            IList<Gh505Packet.Train> trains = new List<Gh505Packet.Train>();
            byte[] getFilesPacket = Gh505Packet.GetTrackFileSections(trackIndexes);
            byte[] getNextPacket = Gh505Packet.GetNextSection();
            GhPacketBase.Response response = SendPacket(Port, getFilesPacket);

            monitor.PercentComplete = 0;

            ReadMode readMode = ReadMode.Header;
            int trainLapsToRead = 0;
            int pointsToRead = 0;
            while (response.CommandId != Gh505Packet.CommandId_FINISH)
            {
                byte[] data = response.PacketData;
                switch (readMode)
                {
                    case ReadMode.Header:
                        {
                            Gh505Packet.Train train = Gh505Packet.UnpackTrainHeader(data);
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
                            IList<Gh505Packet.Lap> laps = Gh505Packet.UnpackLaps(data);
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
                            IList<GhPacketBase.TrackPoint2> points = Gh505Packet.UnpackTrackPoints(data);
                            foreach (GhPacketBase.TrackPoint2 point in points) currentTrain.TrackPoints.Add(point);
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
                response = SendPacket(Port, getNextPacket);
            }

            monitor.PercentComplete = 1;
            return trains;
        }
        public override IList<string> AllowedIds { get { return new List<string> { "GH-50" }; } }
    }
}

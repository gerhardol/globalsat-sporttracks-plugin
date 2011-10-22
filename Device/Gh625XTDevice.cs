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
// Author: Gerhard Olsson


using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class Gh625XTDevice : GlobalsatProtocol
    {
        public Gh625XTDevice(DeviceConfigurationInfo configInfo) : base(configInfo) { }
        public Gh625XTDevice() : base(new FitnessDevice_GH625XT()) { }

        public override GlobalsatPacket PacketFactory { get { return new Gh625XTPacket(); } }
        
        public override ImportJob ImportJob(string sourceDescription, IJobMonitor monitor, IImportResults importResults)
        {
            return new ImportJob_GH625XT(this, sourceDescription, monitor, importResults);
        }

        public IList<Gh625XTPacket.TrackFileHeader> ReadTrackHeaders(IJobMonitor monitor)
        {
            //monitor.StatusText = String.Format(CommonResources.Text.Devices.ImportJob_Status_Reading, CommonResources.Text.LabelActivity);

            GlobalsatPacket getHeadersPacket = new Gh625XTPacket().GetTrackFileHeaders();
            Gh625XTPacket response = (Gh625XTPacket)SendPacket(getHeadersPacket);
            return response.UnpackTrackHeaders();
        }

        private enum ReadMode
        {
            Header,
            Laps,
            Points
        }

        public IList<Gh625XTPacket.Train> ReadTracks(IList<Gh625XTPacket.TrackFileHeader> tracks, IJobMonitor monitor)
        {
            if (tracks.Count == 0) return new Gh625XTPacket.Train[0];

            float totalPoints = 0;
            IList<Int16> trackIndexes = new List<Int16>();
            foreach (Gh625XTPacket.TrackFileHeader header in tracks)
            {
                totalPoints += header.TrackPointCount;
                //track number, less than 100
                trackIndexes.Add((Int16)header.TrackPointIndex);
            }
            float pointsRead = 0;

            IList<Gh625XTPacket.Train> trains = new List<Gh625XTPacket.Train>();
            GlobalsatPacket getFilesPacket = new Gh625XTPacket().GetTrackFileSections(trackIndexes);
            GlobalsatPacket getNextPacket = new Gh625XTPacket().GetNextSection();
            Gh625XTPacket response = (Gh625XTPacket)SendPacket(getFilesPacket);

            monitor.PercentComplete = 0;

            ReadMode readMode = ReadMode.Header;
            int trainLapsToRead = 0;
            int pointsToRead = 0;
            while (response.CommandId != Gh625XTPacket.CommandId_FINISH)
            {
                switch (readMode)
                {
                    case ReadMode.Header:
                        {
                            Gh625XTPacket.Train train = response.UnpackTrainHeader();
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
                            Gh625XTPacket.Train currentTrain = trains[trains.Count - 1];
                            IList<Gh625XTPacket.Lap> laps = response.UnpackLaps();
                            foreach (Gh625XTPacket.Lap lap in laps) currentTrain.Laps.Add(lap);
                            trainLapsToRead -= laps.Count;
                            if (trainLapsToRead == 0)
                            {
                                readMode = ReadMode.Points;
                            }
                            break;
                        }
                    case ReadMode.Points:
                        {
                            Gh625XTPacket.Train currentTrain = trains[trains.Count - 1];
                            IList<GhPacketBase.TrackPoint4> points = response.UnpackTrackPoints();
                            foreach (GhPacketBase.TrackPoint4 point in points) currentTrain.TrackPoints.Add(point);
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
                response = (Gh625XTPacket)SendPacket(getNextPacket);
            }

            monitor.PercentComplete = 1;
            return trains;
        }
    }
}

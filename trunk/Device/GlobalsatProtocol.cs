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
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

using System.IO.Ports;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ZoneFiveSoftware.Common.Data.GPS;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    public abstract class GlobalsatProtocol : GhDeviceBase
    {
        public GlobalsatProtocol(DeviceConfigurationInfo configInfo) : base(configInfo) { }
        public GlobalsatProtocol(FitnessDevice_Globalsat fitDev) : base(fitDev) { }

        //The device creating the packet, used by Send()
        //Must be implemented in each device, a base version for generic cases where all protocols are the same
        //public override GlobalsatPacket PacketFactory { get { return new GlobalsatPacket(); } }
        public static GlobalsatPacket PacketFactoryBase { get { return new GlobalsatPacket(); } }

        //Import kept in separate structure
        public virtual ImportJob ImportJob(string sourceDescription, IJobMonitor monitor, IImportResults importResults)
        {
            return null;
        }

        public virtual int SendTrack(IList<IActivity> activities, BackgroundWorker worker, IJobMonitor jobMonitor)
        {
            int result = 0;
            if (worker.CancellationPending)
            {
                return result;
            }

            this.Open();
            try
            {
                foreach (IActivity activity in activities)
                {
                    IList<GlobalsatPacket> packets = SendTrack(activity);

                    int i = 0;
                    foreach (GlobalsatPacket packet in packets)
                    {
                        GlobalsatPacket response = null;
                        int nrAttempts = 0;

                        do
                        {
                            try
                            {
                                response = (GlobalsatPacket)this.SendPacket(packet);
                                break;
                            }
                            catch (Exception ex1)
                            {
                                nrAttempts++;
                                if (nrAttempts >= 3)
                                {
                                    throw ex1;
                                }
                            }
                        }
                        while (nrAttempts < 3);

                        if (response != null)
                        {
                            //Generic codes in SendPacket
                            if (response.CommandId == GhPacketBase.ResponseResendTrackSection)
                            {
                                // TODO resend
                                throw new Exception(Properties.Resources.Device_SendTrack_Error);
                            }
                            else if (response.CommandId == GhPacketBase.ResponseSendTrackFinish)
                            {
                                return result;
                            }
                            else
                            {
                                throw new Exception(Properties.Resources.Device_SendTrack_Error);
                            }
                            //TODO:
                            throw new Exception("Send track error: " + response.CommandId);
                        }

                        this.Port.Close();
                        this.Port.Open();

                        i++;
                        double progress = packets.Count <= 1 ? 100 : (double)(i * 100) / (double)(packets.Count - 1);

                        worker.ReportProgress((int)progress);

                        if (worker.CancellationPending)
                        {
                            return result;
                        }
                    }
                    result++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(Properties.Resources.Device_SendTrack_Error+ex);
            }
            finally
            {
                this.Close();
            }
            return result;
        }

        public virtual IList<GlobalsatPacket> SendTrack(IActivity activity)
        {
            IList<GlobalsatPacket> sendTrackPackets = new List<GlobalsatPacket>();

            IGPSRoute gpsRoute = activity.GPSRoute;
            GhPacketBase.TrackFileBase trackFileStart = new GhPacketBase.TrackFileBase();
            trackFileStart.TotalDistanceMeters = (int)gpsRoute.TotalDistanceMeters;
            trackFileStart.StartTime = DateTime.Now; // trackPoints.StartTime;
            trackFileStart.TrackPointCount = (short)gpsRoute.Count;
            trackFileStart.TotalTime = TimeSpan.FromSeconds(gpsRoute.TotalElapsedSeconds);

            GlobalsatPacket startPacket = PacketFactory.SendTrackStart(trackFileStart);
            sendTrackPackets.Add(startPacket);

            int nrPointsPerSection = startPacket.TrackPointsPerSection;

            for (int i = 0; i < gpsRoute.Count; i += nrPointsPerSection)
            {
                int startPoint = i;
                int endPoint = (int)Math.Min(i + nrPointsPerSection - 1, gpsRoute.Count - 1);

                List<GhPacketBase.TrackPointSend> trackpoints = new List<GhPacketBase.TrackPointSend>();
                for (int j = startPoint; j <= endPoint; j++)
                {
                    IGPSPoint point = gpsRoute[j].Value;
                    GhPacketBase.TrackPointSend trackpoint = new GhPacketBase.TrackPointSend(point.LatitudeDegrees, point.LongitudeDegrees);
                    trackpoint.Altitude = (short)point.ElevationMeters;
                    trackpoints.Add(trackpoint);
                }

                GhPacketBase.TrackFileSectionSend trackFileSection = new GhPacketBase.TrackFileSectionSend(trackFileStart);
                trackFileSection.StartPointIndex = (short)startPoint;
                trackFileSection.EndPointIndex = (short)endPoint;
                trackFileSection.TrackPoints = trackpoints;

                GlobalsatPacket pointsPacket = PacketFactory.SendTrackSection(trackFileSection);
                sendTrackPackets.Add(pointsPacket);
            }

            return sendTrackPackets;
        }

        //public virtual GlobalsatPacket.GlobalsatSystemInformation GetSystemConfiguration(IJobMonitor jobMonitor)
        //{
        //    this.Open();
        //    try
        //    {
        //        GlobalsatPacket packet = PacketFactory.GetSystemConfiguration();
        //        GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);

        //        GlobalsatPacket.GlobalsatSystemInformation systemInfo = response.ResponseGetSystemConfiguration();
        //        return systemInfo;
        //    }
        //    catch
        //    {
        //        throw;
        //        //throw new Exception(Properties.Resources.Device_GetInfo_Error);
        //    }
        //    finally
        //    {
        //        this.Close();
        //    }
        //}

        public virtual GlobalsatDeviceConfiguration GetSystemConfiguration2(IJobMonitor jobMonitor)
        {
            GlobalsatDeviceConfiguration devConfig = new GlobalsatDeviceConfiguration();

            this.Open();
            try
            {
                GlobalsatPacket packet = PacketFactory.GetSystemConfiguration();
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);

                GlobalsatPacket.GlobalsatSystemConfiguration systemInfo = response.ResponseGetSystemConfiguration();
                devConfig.DeviceName = systemInfo.DeviceName;
                //devConfig.SystemInfoData = response.PacketData;

                packet = PacketFactory.GetSystemConfiguration2();
                response = (GlobalsatPacket)this.SendPacket(packet);

                devConfig.SystemConfigData = response.PacketData;
                return devConfig;
            }
            catch
            {
                throw new Exception(Properties.Resources.Device_GetInfo_Error);
            }
            finally
            {
                this.Close();
            }
        }

        public virtual void SetSystemConfiguration2(GlobalsatDeviceConfiguration devConfig, IJobMonitor jobMonitor)
        {

            this.Open();
            try
            {
                GlobalsatPacket packet = PacketFactory.SetSystemConfiguration2(devConfig.SystemConfigData);
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);
                //No info in the response
            }
            catch
            {
                throw new Exception(Properties.Resources.Device_GetInfo_Error);
            }
            finally
            {
                this.Close();
            }
        }

        public virtual IList<GlobalsatWaypoint> GetWaypoints(IJobMonitor jobMonitor)
        {
            this.Open();
            try
            {
                GlobalsatPacket packet = PacketFactory.GetWaypoints();
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);
                IList<GlobalsatWaypoint> waypoints = response.ResponseGetWaypoints();

                return waypoints;
            }
            catch
            {
                throw new Exception(Properties.Resources.Device_GetWaypoints_Error);
            }
            finally
            {
                this.Close();
            }
        }

        public virtual int SendWaypoints(IList<GlobalsatWaypoint> waypoints, IJobMonitor jobMonitor)
        {
            this.Open();
            try
            {
                GlobalsatPacket packet = PacketFactory.SendWaypoints(this.configInfo.MaxNrWaypoints, waypoints);
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);

                // km500 no out of memory- waypoint overwritten
                int nrSentWaypoints = response.ResponseSendWaypoints();

                return nrSentWaypoints;
            }
            catch
            {
                throw new Exception(Properties.Resources.Device_SendWaypoints_Error);
            }
            finally
            {
                this.Close();
            }
        }

        public virtual void DeleteWaypoints(IList<GlobalsatWaypoint> waypointNames, IJobMonitor jobMonitor)
        {
            this.Open();
            try
            {
                GlobalsatPacket packet = PacketFactory.DeleteWaypoints(this.configInfo.MaxNrWaypoints, waypointNames);
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);
            }
            catch
            {
                throw new Exception(Properties.Resources.Device_DeleteWaypoints_Error);
            }
            finally
            {
                this.Close();
            }
        }


        public virtual void DeleteAllWaypoints(IJobMonitor jobMonitor)
        {
            this.Open();
            try
            {
                GlobalsatPacket packet = PacketFactory.DeleteAllWaypoints();
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);
            }
            catch
            {
                throw new Exception(Properties.Resources.Device_DeleteWaypoints_Error);
            }
            finally
            {
                this.Close();
            }
        }

        public virtual int SendRoute(IList<GlobalsatRoute> routes, IJobMonitor jobMonitor)
        {
            int res = 0;
            this.Open();
            try
            {
                foreach (GlobalsatRoute route in routes)
                {
                    GlobalsatPacket packet = PacketFactory.SendRoute(route);
                    GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);
                    res++;
                }
            }
            catch
            {
                throw new Exception(Properties.Resources.Device_SendRoute_Error);
            }
            finally
            {
                this.Close();
            }
            return res;
        }

        public virtual Bitmap GetScreenshot(IJobMonitor jobMonitor)
        {

            this.Open();
            try
            {
                GlobalsatPacket packet = PacketFactory.GetScreenshot();
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);
                return response.ResponseScreenshot();
            }
            catch
            {
                throw;
                throw new Exception(""); // TODO create message  Properties.Resources.Device_GetInfo_Error);
            }
            finally
            {
                this.Close();
            }
        }

        //public class GlobalsatSystemInformation
        //{

        //    public string UserName;
        //    public bool IsFemale;
        //    public int Age;
        //    public int WeightPounds;
        //    public int WeightKg;
        //    public int HeightInches;
        //    public int HeightCm;
        //    public DateTime BirthDate;

        //    public string DeviceName;
        //    public double Version;
        //    public string Firmware;
        //    public int WaypointCount;
        //    public int TrackpointCount;
        //    public int ManualRouteCount;
        //    public int PcRouteCount;



        //    public GlobalsatSystemInformation(string deviceName, double version, string firmware,
        //        string userName, bool isFemale, int age, int weightPounds, int weightKg, int heightInches, int heightCm, DateTime birthDate,
        //        int waypointCount, int trackpointCount, int manualRouteCount, int pcRouteCount)
        //    {
        //        this.UserName = userName;
        //        this.IsFemale = isFemale;
        //        this.Age = age;
        //        this.WeightPounds = weightPounds;
        //        this.WeightKg = weightKg;
        //        this.HeightInches = heightInches;
        //        this.HeightCm = heightCm;
        //        this.BirthDate = birthDate;
        //        this.DeviceName = deviceName;
        //        this.Version = version;
        //        this.Firmware = firmware;
        //        this.WaypointCount = waypointCount;
        //        this.TrackpointCount = trackpointCount;
        //        this.ManualRouteCount = manualRouteCount;
        //        this.PcRouteCount = pcRouteCount;
        //    }



        //    public GlobalsatSystemInformation(string deviceName, string firmware,
        //        int waypointCount, int pcRouteCount)
        //    {
        //        this.DeviceName = deviceName;
        //        this.Firmware = firmware;
        //        this.WaypointCount = waypointCount;
        //        this.PcRouteCount = pcRouteCount;
        //    }
        //}
    }
}
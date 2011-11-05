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
            throw new NotImplementedException();
        }

        public virtual int SendTrack(IList<IActivity> activities, IJobMonitor jobMonitor)
        {
            int result = 0;
            if (jobMonitor.Cancelled)
            {
                return result;
            }

            this.Open();
            try
            {
                foreach (IActivity activity in activities)
                {
                    IList<GlobalsatPacket> packets = SendTrackPackets(activity);

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
							//	 Console.WriteLine("------ send error 1");
                                nrAttempts++;
                                if (nrAttempts >= 3)
                                {
                                    throw ex1;
                                }
                            }
                        }
                        while (nrAttempts < 3);

                        if (response != null && response.CommandId != packet.CommandId )
                        {
                            //Generic codes in SendPacket
                            if (response.CommandId == GhPacketBase.ResponseResendTrackSection)
                            {
                                // TODO resend
							//	Console.WriteLine("------ send error 2");

                                throw new Exception(Properties.Resources.Device_SendTrack_Error);
                            }
                            else if (response.CommandId == GhPacketBase.ResponseSendTrackFinish)
                            {
                                //Done, all sent
                                break;
                            }
                            else
                            {
							//	 Console.WriteLine("------ send error 3");
                                throw new Exception(Properties.Resources.Device_SendTrack_Error);
                            }
                            //TODO:
							//	 Console.WriteLine("------ send error 4");
                            throw new Exception("Send track error: " + response.CommandId);
                        }

                        i++;
                        float progress = (float)(packets.Count <= 1 ? 1 : (double)(i) / (double)(packets.Count - 1));

                        jobMonitor.PercentComplete = progress;

                        if (jobMonitor.Cancelled)
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

        public virtual IList<GlobalsatPacket> SendTrackPackets(IActivity activity)
        {
            IList<GlobalsatPacket> sendTrackPackets = new List<GlobalsatPacket>();

            IGPSRoute gpsRoute = activity.GPSRoute;
            GhPacketBase.TrackFileBase trackFileStart = new GhPacketBase.TrackFileBase();
            trackFileStart.TotalDistanceMeters = (int)gpsRoute.TotalDistanceMeters;
            trackFileStart.StartTime = DateTime.Now; // trackPoints.StartTime;
            trackFileStart.TrackPointCount = (short)gpsRoute.Count;
            trackFileStart.TotalTime = TimeSpan.FromSeconds(gpsRoute.TotalElapsedSeconds);

			
			//Console.WriteLine("------ SendTrackStart()");
            GlobalsatPacket startPacket = PacketFactory.SendTrackStart(trackFileStart);
            sendTrackPackets.Add(startPacket);

            //Some protocols send laps in separate header
            //Use common code instead of overiding this method
            GlobalsatPacket lapPacket = PacketFactory.SendTrackLaps(trackFileStart);
            if (lapPacket != null)
            {
                sendTrackPackets.Add(lapPacket);
            }
            int nrPointsPerSection = startPacket.TrackPointsPerSection;

            for (int i = 0; i < gpsRoute.Count; i += nrPointsPerSection)
            {
                GhPacketBase.TrackFileSectionSend trackFileSection = new GhPacketBase.TrackFileSectionSend(trackFileStart);
                trackFileSection.StartPointIndex = (Int16)i;
                trackFileSection.EndPointIndex = (Int16)Math.Min(i + nrPointsPerSection - 1, gpsRoute.Count - 1);
                trackFileSection.TrackPoints = new List<GhPacketBase.TrackPointSend>();

                for (int j = trackFileSection.StartPointIndex; j <= trackFileSection.EndPointIndex; j++)
                {
                    IGPSPoint point = gpsRoute[j].Value;
                    GhPacketBase.TrackPointSend trackpoint = new GhPacketBase.TrackPointSend(point.LatitudeDegrees, point.LongitudeDegrees, (short)point.ElevationMeters);
                    trackFileSection.TrackPoints.Add(trackpoint);
                }

    			//Console.WriteLine("------ SendTrackSection()");
                GlobalsatPacket pointsPacket = PacketFactory.SendTrackSection(trackFileSection);
                sendTrackPackets.Add(pointsPacket);
            }

            return sendTrackPackets;
        }

        //Not used as a separate protocol now
        //public virtual GlobalsatPacket.GlobalsatSystemInformation GetSystemConfiguration(IJobMonitor jobMonitor)
        //{
        //    this.Open();
        //    try
        //    {
        //        GlobalsatPacket packet = PacketFactory.GetSystemConfiguration();
        //        GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);

        //        GlobalsatPacket.GlobalsatSystemConfiguration systemInfo = response.ResponseGetSystemConfiguration();
        //        return systemInfo;
        //    }
        //    catch(Exception e)
        //    {
        //        throw new Exception(Properties.Resources.Device_GetInfo_Error+e);
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

                GlobalsatSystemConfiguration systemInfo = response.ResponseGetSystemConfiguration();
                devConfig.DeviceName = systemInfo.DeviceName;
                //devConfig.SystemInfoData = response.PacketData;

                packet = PacketFactory.GetSystemConfiguration2();
                response = (GlobalsatPacket)this.SendPacket(packet);

                devConfig.SystemConfigData = response.PacketData;
                return devConfig;
            }
            catch (Exception e)
            {
                throw new Exception(Properties.Resources.Device_GetInfo_Error+e);
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
            catch (Exception e)
            {
                throw new Exception(Properties.Resources.Device_GetInfo_Error+e);
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
            catch (Exception e)
            {
                throw new Exception(Properties.Resources.Device_GetWaypoints_Error+e);
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
            catch (Exception e)
            {
                throw new Exception(Properties.Resources.Device_SendWaypoints_Error+e);
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
            catch (Exception e)
            {
                throw new Exception(Properties.Resources.Device_DeleteWaypoints_Error+e);
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
            catch (Exception e)
            {
                throw new Exception(Properties.Resources.Device_DeleteWaypoints_Error+e);
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
            catch (Exception e)
            {
                throw new Exception(Properties.Resources.Device_SendRoute_Error+e);
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
                return response.ResponseGetScreenshot();
            }
            catch (Exception e)
            {
                throw new Exception(Properties.Resources.Device_GetInfo_Error+e); 
            }
            finally
            {
                this.Close();
            }
        }
    }
}

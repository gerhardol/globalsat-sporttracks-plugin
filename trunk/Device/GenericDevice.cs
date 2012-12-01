/*
Copyright (C) 2011 Gerhard Olsson

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library. If not, see <http://www.gnu.org/licenses/>.
 */


using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

using ZoneFiveSoftware.Common.Visuals;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    //A generic device that should resolve to the actual device
    //There should be no overridden methods in this class, all should use Device()
    public class GenericDevice : GlobalsatProtocol
    {
        public GenericDevice(FitnessDevice_GsSport fitnessDevice)
            : base(fitnessDevice)
        {
            //genericDevice = fitnessDevice;
        }
        //private FitnessDevice_GsSport genericDevice;

        /* Autodetect specific device, it is up to the caller to cache the device */
        //public bool Open(IJobMonitor monitor)
        //{
        //    monitor.PercentComplete = 0;
        //    monitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_OpeningDevice;
        //    bool res = this.Open();

        //    if (!string.IsNullOrEmpty(this.devId))
        //    {
        //        //No need to translate, will just flash by
        //        monitor.StatusText = this.devId + " detected";
        //    }
        //    else
        //    {
        //        //Failed to open, set monitor.ErrorText
        //        this.NoCommunicationError(monitor);
        //        monitor.StatusText = Properties.Resources.Device_OpenDevice_Error;
        //    }
        //    return res;
        //}

        //public FitnessDevice_Globalsat xxxDevice(IJobMonitor monitor)//xxx
        //{
        //    monitor.PercentComplete = 0;
        //    monitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_OpeningDevice;
        //    if (!this.Open())
        //    {
        //        this.Close();
        //        //if (genericDevice.TryLittleEndian)
        //        {
        //            this.Open();
        //        }
        //    }

        //    FitnessDevice_Globalsat g = new FitnessDevice_GsSport();//xxx this.FitnessDevice.SpecificDevice;
        //    if (g != null)
        //    {
        //        //No need to translate, will just flash by
        //        monitor.StatusText = this.devId + " detected";
        //    }
        //    else
        //    {
        //        //Failed to open, set monitor.ErrorText
        //        this.NoCommunicationError(monitor);
        //        monitor.StatusText = Properties.Resources.Device_OpenDevice_Error;
        //    }
        //    return g;
        //}

        //public string xxxDetect()
        //{
        //    string identification = "Error";
        //    try
        //    {
        //        this.Open();
        //        FitnessDevice_Globalsat device2 = this.FitnessDevice;//xxx .Device(new JobMonitor());
        //        if (device2 != null)
        //        {
        //            if (device2.configInfo.AllowedIds == null || device2.configInfo.AllowedIds.Count == 0)
        //            {
        //                identification = this.devId + " (Globalsat Generic)";
        //            }
        //            else
        //            {
        //                bool found = false;
        //                foreach (string s in device2.configInfo.AllowedIds)
        //                {
        //                    if (this.devId.Equals(s))
        //                    {
        //                        found = true;
        //                        identification = this.devId;
        //                    }
        //                }
        //                if (!found)
        //                {
        //                    identification = this.devId + " (" + device2.configInfo.AllowedIds[0] + " Compatible)";
        //                }
        //            }
        //            identification += " on " + device2.LastValidComPort;
        //        }
        //        else
        //        {
        //            identification = this.devId + " (" + ZoneFiveSoftware.Common.Visuals.CommonResources.Text.Devices.ImportJob_Status_CouldNotOpenDeviceError + ")";
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        identification = Properties.Resources.Device_OpenDevice_Error;
        //    }
        //    finally
        //    {
        //        this.Close();
        //    }
        //    return identification;
        //}
    }
}

/*
 *  Globalsat/Keymaze SportTracks Plugin
 *  Copyright 2009 John Philip 
 * 
 *  This software may be used and distributed according to the terms of the
 *  GNU Lesser General Public License version 2 or any later version.
 */

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    [Serializable]
    public class GlobalsatDeviceConfiguration
    {
        public string DeviceName;
        public byte[] SystemConfigData;

        public void Save(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GlobalsatDeviceConfiguration));

            TextWriter writer = new StreamWriter(filename);
            serializer.Serialize(writer, this);
            writer.Close();
        }

        public static GlobalsatDeviceConfiguration Load(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GlobalsatDeviceConfiguration));

            TextReader reader = new StreamReader(filename);
            GlobalsatDeviceConfiguration configData = (GlobalsatDeviceConfiguration)serializer.Deserialize(reader);
            reader.Close();

            return configData;
        }
    }

    //Details unused
    public class GlobalsatSystemConfiguration
    {
        //public string UserName;
        //public bool IsFemale;
        //public int Age;
        //public int WeightPounds;
        //public int WeightKg;
        //public int HeightInches;
        //public int HeightCm;
        //public DateTime BirthDate;

        public string DeviceName;
        //public double Version;
        public string Firmware;
        public int WaypointCount;
        //public int TrackpointCount;
        //public int ManualRouteCount;
        public int PcRouteCount;
        //public int CourseCount;

        //public GlobalsatSystemConfiguration(string deviceName, double version, string firmware,
        //    string userName, bool isFemale, int age, int weightPounds, int weightKg, int heightInches, int heightCm, DateTime birthDate,
        //    int waypointCount, int trackpointCount, int manualRouteCount, int pcRouteCount, int courseCount)
        //{
        //    this.UserName = userName;
        //    this.IsFemale = isFemale;
        //    this.Age = age;
        //    this.WeightPounds = weightPounds;
        //    this.WeightKg = weightKg;
        //    this.HeightInches = heightInches;
        //    this.HeightCm = heightCm;
        //    this.BirthDate = birthDate;
        //    this.DeviceName = deviceName;
        //    this.Version = version;
        //    this.Firmware = firmware;
        //    this.WaypointCount = waypointCount;
        //    this.TrackpointCount = trackpointCount;
        //    this.ManualRouteCount = manualRouteCount;
        //    this.PcRouteCount = pcRouteCount;
        //    this.CourseCount = courseCount;
        //}

        public GlobalsatSystemConfiguration(string deviceName, string firmware,
            int waypointCount, int pcRouteCount)
        {
            this.DeviceName = deviceName;
            this.Firmware = firmware;
            this.WaypointCount = waypointCount;
            this.PcRouteCount = pcRouteCount;
        }
    }

    //Partly used
    public class GlobalsatSystemConfiguration2
    {
    //    public string UserName;
    //    public bool IsFemale;
    //    public int Age;
    //    public int WeightPounds;
    //    public int WeightKg;
    //    public int HeightInches;
    //    public int HeightCm;
    //    public DateTime BirthDate;
        public byte ScreenOrientation = 0;

    //    public GlobalsatSystemConfiguration2(string deviceName, double version, string firmware,
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
    //    }
    }

}

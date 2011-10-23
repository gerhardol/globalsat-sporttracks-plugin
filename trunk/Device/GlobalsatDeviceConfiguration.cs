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
}

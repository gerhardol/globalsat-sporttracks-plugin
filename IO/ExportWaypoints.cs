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
using System.IO;
using System.Xml;
using System.Xml.Serialization;

#if WAYPOINTSPLUGIN
using KeymazePlugin.Device;
//using KeymazePlugin.Helper;
#else
using ZoneFiveSoftware.SportTracks.Device.Globalsat;
#endif


namespace KeymazePlugin.IO
{
    class ExportWaypoints
    {
        public static Stream ExportGpxWaypointsStream(IList<GlobalsatWaypoint> waypoints)
        {
            gpxType gpxFile = new gpxType();
            //gpxFile.creator = "SportTracks KeymazePlugin";

            List<wptType> wptList = new List<wptType>();

            if (waypoints != null && waypoints.Count > 0)
            {
                foreach (GlobalsatWaypoint waypoint in waypoints)
                {
                    wptType wpt = new wptType();
                    wpt.name = waypoint.WaypointName;
                    wpt.lat = (decimal)waypoint.Latitude;
                    wpt.lon = (decimal)waypoint.Longitude;
                    wpt.ele = (decimal)waypoint.Altitude;
                    wpt.type = waypoint.IconNr.ToString();

                    wptList.Add(wpt);
                }
            }

            gpxFile.wpt = wptList.ToArray();

            XmlSerializer serializer = new XmlSerializer(gpxFile.GetType());

            Stream writer = new MemoryStream();
            serializer.Serialize(writer, gpxFile);
            //writer.Close();
            return writer;
        }

        /// <summary>
        /// Copies the contents of input to output. Doesn't close either stream.
        /// </summary>
        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[input.Length];
            int len;
            input.Position = 0;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        public static void ExportGpxWaypoints(string filename, IList<GlobalsatWaypoint> waypoints)
        {
            Stream gpx = ExportGpxWaypointsStream(waypoints);
            using (Stream file = File.OpenWrite(filename))
            {
                CopyStream(gpx, file);
            }
            gpx.Close();
        }
    }
}

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
using KeymazePlugin.Helper;
using Utils;
#else
using ZoneFiveSoftware.SportTracks.Device.Globalsat;
#endif
using ZoneFiveSoftware.Common.Data.Measurement;

namespace KeymazePlugin.IO
{
#if !WAYPOINTSPLUGIN
    class Utils
    {
        public static double StringToDouble(string str)
        {
            str = str.Replace(',', '.');
            return double.Parse(str, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        }

        //public static string DoubleToString(double d)
        //{
        //    d = Math.Round(d * 1000000) / 1000000.0;
        //    return d.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        //}
    }
#endif

    class ImportWaypoints
    {
        public static IList<GlobalsatWaypoint> ImportStreamGpxWaypoints(Stream file)
        {
            XmlTextReader xmlReader = null;
            gpxType gpxFile = null;
            file.Position = 0;
            try
            {
                xmlReader = new XmlTextReader(file);
                XmlSerializer serializer = new XmlSerializer(typeof(gpxType));
                gpxFile = (gpxType)serializer.Deserialize(xmlReader);
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
            }

            IList<GlobalsatWaypoint> waypoints = new List<GlobalsatWaypoint>();

            if (gpxFile != null && gpxFile.wpt != null && gpxFile.wpt.Length > 0)
            {
                foreach (wptType wpt in gpxFile.wpt)
                {
                    int iconNr = 0;
                    try
                    {
                        iconNr = int.Parse(wpt.type);
                    }
                    catch { }

                    GlobalsatWaypoint waypoint = new GlobalsatWaypoint(wpt.name, iconNr, (short)wpt.ele, (double)wpt.lat, (double)wpt.lon);
                    waypoints.Add(waypoint);
                }
            }
            else if (gpxFile != null && gpxFile.trk != null && gpxFile.trk.Length > 0 && gpxFile.trk[0].trkseg != null && gpxFile.trk[0].trkseg.Length > 0)
            {
                string trackName = gpxFile.trk[0].name;
                wptType[] trackPoints = gpxFile.trk[0].trkseg[0].trkpt;

                string numberFormat = "";
                for (int x = 1; x <= trackPoints.Length; x *= 10)
                {
                    numberFormat += "0";
                }
                int subLenght = 6 - Math.Min(6, numberFormat.Length);
                subLenght = Math.Min(trackName.Length, subLenght);

                int pointNr = 0;
                foreach (wptType trkPoint in trackPoints)
                {
                    pointNr++;

                    string name = trackName.Substring(0, subLenght);
                    name += string.Format("{0:" + numberFormat + "}", pointNr);
                    GlobalsatWaypoint waypoint = new GlobalsatWaypoint(name, 0, 0, (double)trkPoint.lat, (double)trkPoint.lon);

                    waypoint.Altitude = (short)trkPoint.ele;

                    waypoints.Add(waypoint);
                }
            }

            return waypoints;
        }

        public static IList<GlobalsatWaypoint> ImportGpxWaypoints(string filename)
        {
            return ImportStreamGpxWaypoints((new StreamReader(filename)).BaseStream);
        }

        public static IList<GlobalsatWaypoint> ImportKmlWaypoints(string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            XmlElement root = doc.DocumentElement;
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("x", root.NamespaceURI);

            XmlNode documentNode = root.SelectSingleNode("x:Document", nsmgr);
            if (documentNode == null)
            {
                documentNode = root;
            }

            XmlNodeList placemarkNodes = documentNode.SelectNodes("x:Placemark", nsmgr);
            if (placemarkNodes.Count == 0)
            {
                /*
                XmlNode folderNode = documentNode.SelectSingleNode("x:Folder", nsmgr);
                if (folderNode != null)
                {
                    placemarkNodes = folderNode.SelectNodes("x:Placemark", nsmgr);
                }
                */

                XmlNode folderNode = null;
                while ((folderNode = documentNode.SelectSingleNode("x:Folder", nsmgr)) != null &&
                    (placemarkNodes = folderNode.SelectNodes("x:Placemark", nsmgr)).Count == 0)
                {
                    documentNode = folderNode;
                }

            }

            IList<GlobalsatWaypoint> waypoints = new List<GlobalsatWaypoint>();

            foreach (XmlNode placemarkNode in placemarkNodes)
            {
                XmlNode nameNode = placemarkNode.SelectSingleNode("x:name", nsmgr);

                string waypointName = "";
                if (nameNode != null)
                {
                    waypointName = nameNode.InnerText;
                }

                XmlNode pointNode = placemarkNode.SelectSingleNode("x:Point", nsmgr);
                if (pointNode == null)
                {
                    // import waypoints from track

                    try
                    {
                        XmlNode lineStringNode = placemarkNode.SelectSingleNode("x:LineString", nsmgr);

                        XmlNode multiGeometryNode = placemarkNode.SelectSingleNode("x:MultiGeometry", nsmgr);
                        if (lineStringNode == null && multiGeometryNode != null)
                        {
                            lineStringNode = multiGeometryNode.SelectSingleNode("x:LineString", nsmgr);
                        }

                        if (lineStringNode == null)
                        {
                            continue;
                        }

                        XmlNode coordinatesNodeTrack = lineStringNode.SelectSingleNode("x:coordinates", nsmgr);
                        if (coordinatesNodeTrack == null)
                        {
                            continue;
                        }


                        string coordinatesTextTrack = coordinatesNodeTrack.InnerText.Trim();

                        // cordinates including altitude are separated by line breaks
                        string[] pointCoordinates = coordinatesTextTrack.Split(new char[] { '\n' });

                        // otherwise are separated by spaces
                        if (pointCoordinates.Length <= 1)
                        {
                            pointCoordinates = coordinatesTextTrack.Split(new char[] { ' ' });
                        }


                        string numberFormat = "";
                        for (int x = 1; x <= pointCoordinates.Length; x *= 10)
                        {
                            numberFormat += "0";
                        }
                        int subLenght = 6 - Math.Min(6, numberFormat.Length);
                        subLenght = Math.Min(waypointName.Length, subLenght);

                        int pointNr = 0;
                        foreach (string coordinates in pointCoordinates)
                        {
                            pointNr++;
                            try
                            {
                                string[] coordArray = coordinates.Split(new char[] { ',' });


                                double longitude = Utils.StringToDouble(coordArray[0]);
                                double latitude = Utils.StringToDouble(coordArray[1]);


                                string name = waypointName.Substring(0, subLenght);
                                name += string.Format("{0:" + numberFormat + "}", pointNr);
                                GlobalsatWaypoint waypoint = new GlobalsatWaypoint(name, 0, 0, latitude, longitude);

                                try
                                {
                                    short altitude = (short)Utils.StringToDouble(coordArray[2]);
                                    waypoint.Altitude = altitude;
                                }
                                catch { }

                                waypoints.Add(waypoint);

                            }
                            catch { }
                        }

                    }
                    catch { }

                    continue;
                }

                // placemark
                XmlNode coordinatesNode = pointNode.SelectSingleNode("x:coordinates", nsmgr);
                if (coordinatesNode == null)
                {
                    continue;
                }

                string coordinatesText = coordinatesNode.InnerText.Trim();

                try
                {
                    string[] coordArray = coordinatesText.Split(new char[] { ',' });

                    double longitude = Utils.StringToDouble(coordArray[0]);
                    double latitude = Utils.StringToDouble(coordArray[1]);
                    short altitude = (short)Utils.StringToDouble(coordArray[2]);

                    GlobalsatWaypoint waypoint = new GlobalsatWaypoint(waypointName, 0, altitude, latitude, longitude);
                    waypoints.Add(waypoint);

                    //break;
                }
                catch { }

            }
            return waypoints;
        }

        public static IList<GlobalsatWaypoint> ImportWptWaypoints(string filename)
        {
            IList<GlobalsatWaypoint> waypoints = new List<GlobalsatWaypoint>();

            string[] lines = File.ReadAllLines(filename, ASCIIEncoding.Default);

            for (int i = 4; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] waypointFields = line.Split(new char[] { ',' });
                for (int j = 0; j < waypointFields.Length; j++)
                {
                    waypointFields[j] = waypointFields[j].Replace((char)209, ',').Trim();
                }

                try
                {
                    string name = waypointFields[1];
                    double latitude = Utils.StringToDouble(waypointFields[2]);
                    double longitude = Utils.StringToDouble(waypointFields[3]);
                    int icon = 0;// (int)Utils.StringToDouble(waypointFields[5]);

                    double altitudeMeters = 0;

                    try
                    {
                        double altitudeFeet = Utils.StringToDouble(waypointFields[14]);
                        if (altitudeFeet == -777)
                        {
                            altitudeFeet = 0;
                        }
                        altitudeMeters = Length.Convert(altitudeFeet, Length.Units.Foot, Length.Units.Meter);
                    }
                    catch { }

                    GlobalsatWaypoint waypoint = new GlobalsatWaypoint(name, icon, (short)altitudeMeters, latitude, longitude);
                    waypoints.Add(waypoint);
                }
                catch { }

               
            }

            return waypoints;
        }

        public static IList<GlobalsatWaypoint> ImportLocWaypoints(string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            ///XmlElement root = doc.DocumentElement;
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("x", doc.NamespaceURI);

            XmlNode locNode = doc.SelectSingleNode("x:loc", nsmgr);
            if (locNode == null) return null;

            XmlNodeList waypointNodes = locNode.SelectNodes("x:waypoint", nsmgr);
            if (waypointNodes == null) return null;


            IList<GlobalsatWaypoint> waypoints = new List<GlobalsatWaypoint>();

            foreach (XmlNode waypointNode in waypointNodes)
            {

                try
                {
                    XmlNode nameNode = waypointNode.SelectSingleNode("x:name", nsmgr);

                    string waypointName = nameNode.InnerText;

                    XmlNode coordNode = waypointNode.SelectSingleNode("x:coord", nsmgr);

                    double longitude = Utils.StringToDouble(coordNode.Attributes["lon"].Value);
                    double latitude = Utils.StringToDouble(coordNode.Attributes["lat"].Value);

                    GlobalsatWaypoint waypoint = new GlobalsatWaypoint(waypointName, 0, 0, latitude, longitude);
                    waypoints.Add(waypoint);
                }
                catch { }
            }
            return waypoints;
        }
    }

    class ImportRoutes
    {
        public static IList<GlobalsatRoute> ImportStreamGpxRoutes(Stream file)
        {
            XmlTextReader xmlReader = null;
            gpxType gpxFile = null;
            file.Position = 0;
            try
            {
                xmlReader = new XmlTextReader(file);
                XmlSerializer serializer = new XmlSerializer(typeof(gpxType));
                gpxFile = (gpxType)serializer.Deserialize(xmlReader);
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
            }

            IList<GlobalsatRoute> routes = new List<GlobalsatRoute>();

            if (gpxFile != null && gpxFile.rte != null && gpxFile.rte.Length > 0)
            {
                foreach (rteType rte in gpxFile.rte)
                {
                    if (rte.rtept.Length > 0)
                    {
                        IList<GlobalsatWaypoint> wpts = new List<GlobalsatWaypoint>();
                        foreach (wptType wpt in rte.rtept)
                        {
                            //Icon not decoded from symbol
                            wpts.Add(new GlobalsatWaypoint(wpt.name, 0, (short)wpt.ele, (double)wpt.lat, (double)wpt.lon));
                        }
                        routes.Add(new GlobalsatRoute(rte.name, wpts));
                    }
                }
            }
            else if (gpxFile != null && gpxFile.trk != null && gpxFile.trk.Length > 0)
            {
                foreach (trkType trk in gpxFile.trk)
                {
                    if (trk.trkseg != null && trk.trkseg.Length > 0)
                    {
                        IList<GlobalsatWaypoint> wpts = new List<GlobalsatWaypoint>();
                        string trackName = trk.name;
                        wptType[] trackPoints = trk.trkseg[0].trkpt;

                        string numberFormat = "";
                        for (int x = 1; x <= trackPoints.Length; x *= 10)
                        {
                            numberFormat += "0";
                        }
                        int subLenght = 6 - Math.Min(6, numberFormat.Length);
                        subLenght = Math.Min(trackName.Length, subLenght);

                        int pointNr = 0;
                        foreach (wptType trkPoint in trackPoints)
                        {
                            pointNr++;

                            string name = trackName.Substring(0, subLenght);
                            name += string.Format("{0:" + numberFormat + "}", pointNr);
                            GlobalsatWaypoint waypoint = new GlobalsatWaypoint(name, 0, (short)trkPoint.ele, (double)trkPoint.lat, (double)trkPoint.lon);

                            wpts.Add(waypoint);
                        }
                        routes.Add(new GlobalsatRoute(trackName, wpts));
                    }
                }
            }

            return routes;
        }
    }
}

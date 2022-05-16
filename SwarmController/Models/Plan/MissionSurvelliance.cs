﻿using GMap.NET;
using GMap.NET.WindowsPresentation;
using SwarmController.Markers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SwarmController.Models.Plan
{
    public class MissionSurvelliance : MissionBase
    {
        public PointLatLng topLeftCorner;
        public PointLatLng bottomUpCorner;

        public List<Route> routes;

        public MissionSurvelliance(int missionID)
        {
            this.missionID = missionID;
            routes = new List<Route>();
            assignedDrones = new List<int>();
            tcpClients = new List<TcpClient>();
        }

        public void createRoutes()
        {
            PointLatLng home = new PointLatLng(0,0);
            MissionItem missionItem = new MissionItem();
            GMapMarker gmapMarker = new GMapMarker(home);
            gmapMarker.Shape = new MissionItemMarker();
            gmapMarker.Offset = new Point(-10, -20);
            missionItem.marker = gmapMarker;
            missionItem.itemOrder = 0;
            missionItem.altitude = 5;

            Route route1 = new Route();
            List<PointLatLng> gMapRoutePoints1 = new List<PointLatLng>();
            PointLatLng pointLatLng1 = new PointLatLng(-35.3629110, 149.1613340);
            MissionItem missionItem1 = new MissionItem();
            GMapMarker gmapMarker1 = new GMapMarker(pointLatLng1);
            gmapMarker1.Shape = new MissionItemMarker();
            gmapMarker1.Offset = new Point(-10, -20);
            missionItem1.marker = gmapMarker1;
            missionItem1.itemOrder = 1;
            missionItem1.altitude = 5;
            gMapRoutePoints1.Add(home);
            gMapRoutePoints1.Add(pointLatLng1);
            route1.missionID = this.missionID;
            route1.gMapRoute = new GMapRoute(gMapRoutePoints1);
            route1.missionItems.Add(missionItem); // home
            route1.missionItems.Add(missionItem1);
            routes.Add(route1);

            Route route2 = new Route();
            List<PointLatLng> gMapRoutePoints2 = new List<PointLatLng>();
            PointLatLng pointLatLng2 = new PointLatLng(-35.3629110, 149.1613340);
            MissionItem missionItem2 = new MissionItem();
            GMapMarker gmapMarker2 = new GMapMarker(pointLatLng2);
            gmapMarker2.Shape = new MissionItemMarker();
            gmapMarker2.Offset = new Point(-10, -20);
            missionItem2.marker = gmapMarker2;
            missionItem2.itemOrder = 1;
            missionItem2.altitude = 5;
            gMapRoutePoints2.Add(home);
            gMapRoutePoints2.Add(pointLatLng2);
            route2.missionID = this.missionID;
            route2.gMapRoute = new GMapRoute(gMapRoutePoints2);
            route2.missionItems.Add(missionItem); // home
            route2.missionItems.Add(missionItem2);
            routes.Add(route2);

            Route route3 = new Route();
            List<PointLatLng> gMapRoutePoints3 = new List<PointLatLng>();
            PointLatLng pointLatLng3 = new PointLatLng(-35.3629110, 149.1613340);
            MissionItem missionItem3 = new MissionItem();
            GMapMarker gmapMarker3 = new GMapMarker(pointLatLng3);
            gmapMarker3.Shape = new MissionItemMarker();
            gmapMarker3.Offset = new Point(-10, -20);
            missionItem3.marker = gmapMarker3;
            missionItem3.itemOrder = 1;
            missionItem3.altitude = 5;
            gMapRoutePoints3.Add(home);
            gMapRoutePoints3.Add(pointLatLng3);
            route3.missionID = this.missionID;
            route3.gMapRoute = new GMapRoute(gMapRoutePoints3);
            route3.missionItems.Add(missionItem); // home
            route3.missionItems.Add(missionItem3);
            routes.Add(route3);
        }

        //public void uploadMissionToDrones()
        //{

        //}
    }
}

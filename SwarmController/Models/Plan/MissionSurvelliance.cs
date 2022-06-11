using GMap.NET;
using GMap.NET.WindowsPresentation;
using SwarmController.Markers;
using SwarmController.Models.Swarm;
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
        public List<PointLatLng> pointsToSurveillance { get; set; }

        public MissionSurvelliance(int missionID)
        {
            this.missionID = missionID;
            this.droneIdInMissionCounter = 0;
            routes = new List<Route>();
            drones = new List<Drone>();
            assignedDronePorts = new List<int>();
            pointsToSurveillance = new List<PointLatLng>();
        }

        public void createRoutes()
        {
            PointLatLng home = new PointLatLng(0,0);
            MissionItem homeItem = new MissionItem();
            GMapMarker gmapMarker = new GMapMarker(home);
            gmapMarker.Shape = new MissionItemMarker();
            gmapMarker.Offset = new Point(-10, -20);
            homeItem.marker = gmapMarker;
            homeItem.itemOrder = 0;
            homeItem.altitude = 5;

            for(int i = 0; i < numberOfDronesInMission; i++)
            {
                Route route = new Route();
                List<PointLatLng> gMapRoutePoints = new List<PointLatLng>();
                MissionItem missionItem = new MissionItem();

                GMapMarker gmapMarker1 = new GMapMarker(pointsToSurveillance[i]);
                gmapMarker1.Shape = new MissionItemMarker();
                gmapMarker1.Offset = new Point(-10, -20);
                missionItem.marker = gmapMarker1;
                missionItem.itemOrder = 1;
                missionItem.altitude = 5;

                gMapRoutePoints.Add(home);
                gMapRoutePoints.Add(pointsToSurveillance[i]);
                route.missionID = missionID;
                route.gMapRoute = new GMapRoute(gMapRoutePoints);
                route.missionItems.Add(homeItem); // home
                route.missionItems.Add(missionItem);
                routes.Add(route);
            }
            
        }

    }
}

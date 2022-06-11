using GMap.NET;
using GMap.NET.WindowsPresentation;
using SwarmController.Helper;
using SwarmController.Markers;
using SwarmController.Models.Swarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SwarmController.Models.Plan
{
    public class MissionKamikaze : MissionBase
    {
        public PointLatLng airDefenceLocation { get; set; }

        public MissionKamikaze(int missionID)
        {
            this.missionID = missionID;
            this.droneIdInMissionCounter = 0;
            routes = new List<Route>(); // 0 right, 1 left, 2 center
            drones = new List<Drone>();
            assignedDronePorts = new List<int>();
        }

        public void createRoutes()
        {
            PointLatLng home = new PointLatLng(0, 0);
            MissionItem homeItem = new MissionItem();
            GMapMarker gmapMarker = new GMapMarker(home);
            gmapMarker.Shape = new AirDefenceMarker();
            gmapMarker.Offset = new Point(-25, -25);
            homeItem.marker = gmapMarker;
            homeItem.itemOrder = 0;
            homeItem.altitude = 5;

            for (int i = 0; i < numberOfDronesInMission; i++)
            {
                if(i == 0) // right decoy coords
                {
                    Route route = new Route();
                    List<PointLatLng> gMapRoutePoints = new List<PointLatLng>();
                    MissionItem missionItem1 = new MissionItem();
                    MissionItem missionItem2 = new MissionItem();

                    // right
                    PointLatLng rightDecoyCoords = MeasurementTool.FindPointAtDistanceFrom(airDefenceLocation,
                                                                                           45,
                                                                                           0.1);

                    GMapMarker gmapMarker1 = new GMapMarker(rightDecoyCoords);
                    gmapMarker1.Shape = new DecoyMarker();
                    gmapMarker1.Offset = new Point(-25, -25);
                    missionItem1.marker = gmapMarker1;
                    missionItem1.itemOrder = 1;
                    missionItem1.altitude = 5;

                    // center
                    GMapMarker gmapMarker2 = new GMapMarker(airDefenceLocation);
                    gmapMarker2.Shape = new MissionItemMarker();
                    gmapMarker2.Offset = new Point(-25, -25);
                    missionItem2.marker = gmapMarker2;
                    missionItem2.itemOrder = 2;
                    missionItem2.altitude = 5;

                    gMapRoutePoints.Add(home);
                    gMapRoutePoints.Add(rightDecoyCoords);
                    gMapRoutePoints.Add(airDefenceLocation);
                    route.missionID = missionID;
                    route.gMapRoute = new GMapRoute(gMapRoutePoints);
                    route.missionItems.Add(homeItem); // home
                    route.missionItems.Add(missionItem1);
                    route.missionItems.Add(missionItem2);
                    routes.Add(route);
                }
                else if(i == 1) // left decoy coords
                {
                    Route route = new Route();
                    List<PointLatLng> gMapRoutePoints = new List<PointLatLng>();
                    MissionItem missionItem1 = new MissionItem();
                    MissionItem missionItem2 = new MissionItem();

                    // left
                    PointLatLng rightDecoyCoords = MeasurementTool.FindPointAtDistanceFrom(airDefenceLocation,
                                                                                           200,
                                                                                           0.1);

                    GMapMarker gmapMarker1 = new GMapMarker(rightDecoyCoords);
                    gmapMarker1.Shape = new DecoyMarker();
                    gmapMarker1.Offset = new Point(-25, -25);
                    missionItem1.marker = gmapMarker1;
                    missionItem1.itemOrder = 1;
                    missionItem1.altitude = 5;

                    // target
                    GMapMarker gmapMarker2 = new GMapMarker(airDefenceLocation);
                    gmapMarker2.Shape = new MissionItemMarker();
                    gmapMarker2.Offset = new Point(-25, -25);
                    missionItem2.marker = gmapMarker2;
                    missionItem2.itemOrder = 2;
                    missionItem2.altitude = 5;

                    gMapRoutePoints.Add(home);
                    gMapRoutePoints.Add(rightDecoyCoords);
                    gMapRoutePoints.Add(airDefenceLocation);
                    route.missionID = missionID;
                    route.gMapRoute = new GMapRoute(gMapRoutePoints);
                    route.missionItems.Add(homeItem); // home
                    route.missionItems.Add(missionItem1);
                    route.missionItems.Add(missionItem2);
                    routes.Add(route);
                }
                else // target coords
                {
                    Route route = new Route();
                    List<PointLatLng> gMapRoutePoints = new List<PointLatLng>();
                    MissionItem missionItem = new MissionItem();

                    // target
                    GMapMarker gmapMarker1 = new GMapMarker(airDefenceLocation);
                    gmapMarker1.Shape = new AirDefenceMarker();
                    gmapMarker1.Offset = new Point(-25, -25);
                    missionItem.marker = gmapMarker1;
                    missionItem.itemOrder = 1;
                    missionItem.altitude = 5;

                    gMapRoutePoints.Add(home);
                    gMapRoutePoints.Add(airDefenceLocation);
                    route.missionID = missionID;
                    route.gMapRoute = new GMapRoute(gMapRoutePoints);
                    route.missionItems.Add(homeItem); // home
                    route.missionItems.Add(missionItem);
                    routes.Add(route);
                }


            }


        }
    }
}

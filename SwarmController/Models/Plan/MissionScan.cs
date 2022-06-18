using GMap.NET;
using GMap.NET.WindowsPresentation;
using SwarmController.Helper;
using SwarmController.Markers;
using SwarmController.Models.Swarm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SwarmController.Models.Plan
{
    public class MissionScan : MissionBase
    {
        public PointLatLng Corner1 { get; set; }
        public PointLatLng Corner2 { get; set; }

        public double drone_scan_lenght = 0.0002;

        public MissionScan(int missionID)
        {
            this.missionID = missionID;
            this.droneIdInMissionCounter = 0;
            routes = new List<Route>(); // 0 right, 1 left, 2 center
            drones = new List<Drone>();
            assignedDronePorts = new List<int>();
        }

        public void createRoutes()
        {


            if (Corner2.Lat > Corner1.Lat)
            {
                PointLatLng temp = new PointLatLng(Corner2.Lat, Corner1.Lng);
                PointLatLng temp2 = new PointLatLng(Corner1.Lat, Corner2.Lng);
                Corner2 = temp2;
                Corner1 = temp;
            }
            if (Corner2.Lng > Corner1.Lng)
            {
                PointLatLng temp = new PointLatLng(Corner1.Lat, Corner2.Lng);
                PointLatLng temp2 = new PointLatLng(Corner2.Lat, Corner1.Lng);
                Corner2 = temp2;
                Corner1 = temp;
            }

            // int task_count = Convert.ToInt32((Corner1.Lng - Corner2.Lng) / (2* drone_scan_lenght));
            // int iter_count = (task_count / numberOfDronesInMission)+1;


            PointLatLng home = new PointLatLng(0, 0);
            MissionItem homeItem = new MissionItem();
            GMapMarker gmapMarker = new GMapMarker(home);
            gmapMarker.Shape = new MissionItemMarker();
            gmapMarker.Offset = new Point(-10, -20);
            homeItem.marker = gmapMarker;
            homeItem.itemOrder = 0;
            homeItem.altitude = 5;

            for (int i = 0; i < numberOfDronesInMission; i++)
            {
                Route route = new Route();
                List<PointLatLng> gMapRoutePoints = new List<PointLatLng>();
                gMapRoutePoints.Add(home);
                route.missionID = missionID;
                route.missionItems.Add(homeItem);


                double Long_iter = Corner2.Lng + (drone_scan_lenght * i);
                int item_order_count = 1;
                while (Long_iter < Corner1.Lng)
                {
                    MissionItem Mis_item_temp = new MissionItem();
                    MissionItem Mis_item_temp2 = new MissionItem();
                    PointLatLng Start_pos;
                    PointLatLng End_pos;

          

                    if ((item_order_count / 2) % 2 == 0)
                    {
                        Start_pos = new PointLatLng(Corner2.Lat, Long_iter);
                        End_pos = new PointLatLng(Corner1.Lat, Long_iter);
                    }
                    else
                    {
                        Start_pos = new PointLatLng(Corner1.Lat, Long_iter);
                        End_pos = new PointLatLng(Corner2.Lat, Long_iter);
                    }

                    //Debug.WriteLine($"Drone: {i}, Mission Order {item_order_count}");
                    //Debug.WriteLine($"Start: {Start_pos.Lat}, ---- {Start_pos.Lng}");
                    //Debug.WriteLine($"End: {End_pos.Lat}, ---- {End_pos.Lng}");

                    GMapMarker gmapMarker1 = new GMapMarker(Start_pos);
                    gmapMarker1.Shape = new MissionItemMarker();
                    gmapMarker1.Offset = new Point(-25, -25);
                    Mis_item_temp.marker = gmapMarker1;
                    Mis_item_temp.itemOrder = item_order_count;
                    Mis_item_temp.altitude = 5;
                    gMapRoutePoints.Add(Start_pos);
                    route.missionItems.Add(Mis_item_temp);
                    item_order_count++;


                    GMapMarker gmapMarker2 = new GMapMarker(End_pos);
                    gmapMarker2.Shape = new MissionItemMarker();
                    gmapMarker2.Offset = new Point(-25, -25);
                    Mis_item_temp2.marker = gmapMarker2;
                    Mis_item_temp2.itemOrder = item_order_count;
                    Mis_item_temp2.altitude = 5;
                    gMapRoutePoints.Add(End_pos);
                    route.missionItems.Add(Mis_item_temp2);
                    item_order_count++;

                    Long_iter = Long_iter + (drone_scan_lenght * numberOfDronesInMission);
                }
                route.gMapRoute = new GMapRoute(gMapRoutePoints);
                routes.Add(route);
            }

        }
    }
}
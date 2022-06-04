using GMap.NET;
using GMap.NET.WindowsPresentation;
using SwarmController.Markers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SwarmController.Models.Plan;

namespace SwarmController.Models.Swarm
{
    public class Drone
    {
        public int port { get; set; }
        public TcpClient tcpClient { get; set; }

        private int _missionID;
        public int missionID
        {
            get
            {
                return _missionID;
            }
            set
            {
                _missionID = value;

                SwarmManager sM = SwarmManager.getSwarmManager();
                if (_missionID == -1)
                {
                    sM.availableNumberOfDrones++;
                }
                else
                {
                    sM.availableNumberOfDrones--;
                }
            }
        }

        private double _lat;
        public double lat
        {
            get
            {
                return _lat;
            }
            set
            {
                _lat = value;
                //droneMarker.Position = new PointLatLng(_lat, lng);
            }
        }

        private double _lng;
        public double lng
        {
            get
            {
                return _lng;
            }
            set
            {
                _lng = value;
                //droneMarker.Position = new PointLatLng(lat, _lng);
            }
        }
        public double alt;

        public double roll;
        public double yaw;
        public double pitch;

        public GMapMarker droneMarker { get; set; }

        private int _droneID;
        public int droneID
        {
            get
            {
                _droneID = (this.port - 5762) / 10;
                return _droneID;
            }
            set
            {
                _droneID = value;
            }
        }

        private int _droneIdInMission;
        public int droneIdInMission
        {
            get
            {
                return _droneIdInMission;
            }
            set
            {
                _droneIdInMission = value;
            }
        }

        private bool _availability;
        public bool availability
        {
            get
            {
                return _availability;
            }
            set
            {
                _availability = value;
                if(_availability == false)
                {
                    //PlanController pC = PlanController.getPlanController();
                    //(pC.allMissions[missionID] as MissionSurvelliance).ReAssignDrone(droneID);
                    SwarmManager sM = SwarmManager.getSwarmManager();
                    sM.startMissionForOneDrone(this);
                }
            }
        }

        public Drone(int port, int missionID)
        {
            droneMarker = new GMapMarker(new PointLatLng(0, 0));
            droneMarker.Shape = new DroneMarker(port);
            droneMarker.Offset = new Point(-15, -15);

            this.port       = port;
            this.missionID  = missionID;
            this.lat        = 0;
            this.lng        = 0;
            this.alt        = 0;
            this.roll       = 0;
            this.yaw        = 0;
            this.pitch      = 0;

            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect("127.0.0.1", port);

                this.availability = true;
            }
            catch
            {
                this.availability = false;
            }
        }

        public override string ToString()
        {
            return $"id: {droneID} - port {port}";
        }
    }

}

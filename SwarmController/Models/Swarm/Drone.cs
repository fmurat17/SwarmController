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

namespace SwarmController.Models.Swarm
{
    public class Drone
    {
        public int port { get; set; }
        public TcpClient tcpClient { get; set; }
        public int missionID { get; set; }

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
                _droneID = (this.port - 5763) / 10;
                return _droneID;
            }
            set
            {
                _droneID = value;
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
            }
        }

        public Drone(int port, int missionID)
        {
            droneMarker = new GMapMarker(new PointLatLng(0, 0));
            droneMarker.Shape = new DroneMarker();
            droneMarker.Offset = new Point(-15, -15);

            this.port       = port;
            this.missionID  = missionID;
            this.lat        = 0;
            this.lng        = 0;
            this.alt        = 0;
            this.roll       = 0;
            this.yaw        = 0;
            this.pitch      = 0;
            this.availability = true;

            tcpClient = new TcpClient();
            tcpClient.Connect("127.0.0.1", port);
        }
    }

}

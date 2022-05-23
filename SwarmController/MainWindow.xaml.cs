﻿using GMap.NET;
using GMap.NET.WindowsPresentation;
using Haberlesme;
using SwarmController.Enums;
using SwarmController.Markers;
using SwarmController.Models.Plan;
using SwarmController.Models.Swarm;
using SwarmController.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SwarmController
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ClickFunction clickFunction;
        public int surveillanceClickCounter = 0;
        public List<PointLatLng> corners;
        public MissionSurvelliance ms;
        public int missionIDCounter;
        public LogViewModel logViewModel = new LogViewModel();
        public List<GMapMarker> droneMarkers = new List<GMapMarker>();

        SwarmManager sm = SwarmManager.getSwarmManager();
        PlanController pc = PlanController.getPlanController();

        public MainWindow()
        {
            InitializeComponent();
            lv_Log.DataContext = logViewModel;

            //btn_CreateDrones_Click(null, null);

            sm.createDrones();

            createDroneMarkers();
            UpdateAllDronesInMap();

            sm.InitListenAllDrones();
        }

        private void createDroneMarkers()
        {
            int totalNumberOfDrones = sm.totalNumberOfDrones;

            for(int i = 0; i < totalNumberOfDrones; i++)
            {
                mapView.Markers.Add(sm.allDrones[i].droneMarker);
            }
        }

        private void UpdateAllDronesInMap()
        {
            for (int i = 0; i < sm.allDrones.Count; i++)
            {
                int k = i;
                Thread thread = new Thread(() => updateDroneInMap(sm.allDrones[k]));
                thread.Start();
            }
        }

        private void updateDroneInMap(Drone drone)
        {
            while (true)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    mapView.Markers.Remove(drone.droneMarker);

                    drone.droneMarker.Position = new PointLatLng(drone.lat, drone.lng);

                    mapView.Markers.Add(drone.droneMarker);
                }));

                //Dispatcher.Invoke(new Action(() => {
                //    mapView.Markers.Remove(drone.droneMarker);

                //    drone.droneMarker.Position = new PointLatLng(drone.lat, drone.lng);

                //    mapView.Markers.Add(drone.droneMarker);
                //}));

                Thread.Sleep(1000);
            }
        }

        private void mapView_Loaded(object sender, RoutedEventArgs e)
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            //mapView.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance;
            mapView.MapProvider = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;
            mapView.MinZoom = 1;
            mapView.MaxZoom = 24;
            mapView.Zoom = 2;
            mapView.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
            mapView.CanDragMap = true;
            mapView.DragButton = MouseButton.Right;
            mapView.IgnoreMarkerOnMouseWheel = true;
        }

        private void mapView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(clickFunction == ClickFunction.CreateMission)
            {
                Point mousePos = e.GetPosition(mapView);
                double lat = mapView.FromLocalToLatLng((int)mousePos.X, (int)mousePos.Y).Lat;
                double lng = mapView.FromLocalToLatLng((int)mousePos.X, (int)mousePos.Y).Lng;
                PointLatLng coords = new PointLatLng(lat, lng);
                Debug.WriteLine($"lat: {lat}, lng: {lng}");

                surveillanceClickCounter++;

                corners.Add(coords);

                MissionItem missionItem = new MissionItem();
                GMapMarker gmapMarker = new GMapMarker(coords);
                gmapMarker.Shape = new MissionItemMarker();
                gmapMarker.Offset = new Point(-10, -20); // -15,-15 dronemarker
                missionItem.marker = gmapMarker;
                mapView.Markers.Add(gmapMarker);

                if (surveillanceClickCounter == 1)
                {
                    ms.firstPoint = coords;
                    addLog("first point selected");
                }
                else
                {
                    ms.secondPoint = coords;
                    ms.numberOfDronesInMission = 2;
                    //pc.currentMission = ms;
                    sm.currentMission = ms;
                    pc.allMissions.Add(ms);
                    addLog("second point selected");
                    ms.createRoutes();

                    surveillanceClickCounter = 0;
                    corners.Clear();
                    clickFunction = ClickFunction.Select;

                }
            }

        }

        private void btn_Surveillance_Click(object sender, RoutedEventArgs e)
        {
            clickFunction = ClickFunction.CreateMission;
            corners = new List<PointLatLng>();
            ms = new MissionSurvelliance(missionIDCounter++);
            MessageBox.Show("Mark fields to watch");
        }

        public void addLog(string log_string)
        {
            logViewModel.logList.Insert(0, log_string);
        }

        private void btn_StartMission_Click(object sender, RoutedEventArgs e)
        {
            sm.startMission();
        }

        private void btn_CreateDrones_Click(object sender, RoutedEventArgs e)
        {
            Thread createDronesThread = new Thread(() => CreateDrones(3));
            createDronesThread.Start();
        }

        private void CreateDrones(int numberOfDrones)
        {
            Process bashProcess = new Process();

            bashProcess.StartInfo.FileName = @"D:\cygwin64\bin\bash.exe";
            bashProcess.StartInfo.Arguments = $"--login -i -l -c 'cd ardupilot/ArduCopter; ../Tools/autotest/sim_vehicle.py -n{numberOfDrones}'";
            bashProcess.StartInfo.UseShellExecute = true;

            bashProcess.Start();
            bashProcess.WaitForExit();
        }

        private void mapView_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(mapView);
            double lat = mapView.FromLocalToLatLng((int)mousePos.X, (int)mousePos.Y).Lat;
            double lng = mapView.FromLocalToLatLng((int)mousePos.X, (int)mousePos.Y).Lng;

            tb_Lat.Text = Math.Round(lat, 6).ToString();
            tb_Lng.Text = Math.Round(lng, 6).ToString();
        }

        private void btn_close63_Click(object sender, RoutedEventArgs e)
        {
            Drone drone = sm.getDroneByMissionIdAndPort(ms.missionID, 5763);
            drone.isClosedForever = true;
            drone.availability = false;
        }

        private void btn_close73_Click(object sender, RoutedEventArgs e)
        {
            Drone drone = sm.getDroneByMissionIdAndPort(ms.missionID, 5773);
            drone.isClosedForever = true;
            drone.availability = false;
        }

        private void btn_close83_Click(object sender, RoutedEventArgs e)
        {
            Drone drone = sm.getDroneByMissionIdAndPort(ms.missionID, 5783);
            drone.isClosedForever = true;
            drone.availability = false;
        }
    }
}


// TODO:
//plan, swarm ve log için altyapı kuruldu. ++
//oluşturulan plan dronelara dağıtılsın ve yüklensim ++
//drone'ları dinle ++
//drone'ları uçur ++
//begininvoke yerine invoke kullan, güncelleme hızını artır ++ begininvoke devamke şimdilik
//optimize edilecek bir şey varsa optimize et
//bağlantı kes ve yeni rota oluşturup gönder
//sm.startMission(); parametre olarak görevi almalı
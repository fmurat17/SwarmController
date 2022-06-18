using GMap.NET;
using GMap.NET.WindowsPresentation;
using Haberlesme;
using SwarmController.Enums;
using SwarmController.Markers;
using SwarmController.Models.Log;
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
        public int scanCornerCounter = 0;
        public List<PointLatLng> corners;
        public MissionBase ms;
        public int missionIDCounter;
        public LogViewModel logViewModel = new LogViewModel();
        public MissionNamesViewModel missionNamesViewModel = new MissionNamesViewModel();
        public DroneInfoCardListViewModel droneInfoCardListViewModel = new DroneInfoCardListViewModel();
        public List<GMapMarker> droneMarkers = new List<GMapMarker>();

        public int desiredNumberOfDronesInMission = 0;

        SwarmManager sm = SwarmManager.getSwarmManager();
        PlanController pc = PlanController.getPlanController();
        LogManager lM = LogManager.getLogManager();

        public MainWindow()
        {
            InitializeComponent();

            lv_Log.DataContext = logViewModel;
            cmb_missionNames.DataContext = missionNamesViewModel;
            droneInfoCard.DataContext = droneInfoCardListViewModel;
            sp_droneNumbers.DataContext = sm.droneNumbersViewModel;
            //sv_droneInfo.DataContext = droneInfoCardListViewModel;

            //btn_CreateDrones_Click(null, null);
            createDronesViewModel();
            sm.createDrones();
            
            createDroneMarkers();
            InitDispatcherUIUpdaterThread();

            sm.InitListenAllDrones();
        }

        private void createDronesViewModel()
        {
            for(int i = 0; i < sm.totalNumberOfDrones; i++)
            {
                droneInfoCardListViewModel.droneInfoList.Add(new DroneInfoCardViewModel(0,0,0,0,0,0,"",0,""));
            }
        }

        private void createDroneMarkers()
        {
            int totalNumberOfDrones = sm.totalNumberOfDrones;

            for(int i = 0; i < totalNumberOfDrones; i++)
            {
                mapView.Markers.Add(sm.allDrones[i].droneMarker);
            }
        }

        private void InitDispatcherUIUpdaterThread()
        {
            Thread dispatcherUIUpdaterThread = new Thread(() => DispatcherUIUpdaterThread());
            dispatcherUIUpdaterThread.Start();
        }

        private void DispatcherUIUpdaterThread()
        {
            while (true)
            {
                //Dispatcher.BeginInvoke(new Action(() =>
                Dispatcher.Invoke(new Action(() =>
                {
                    // update drones in map
                    for (int i = 0; i < sm.allDrones.Count; i++)
                    {

                        Drone drone = sm.allDrones[i];
                        MissionBase mission = sm.getMissionByPort(drone.port);

                        DroneInfoCardViewModel droneInfoCard = new DroneInfoCardViewModel(drone.roll,
                                                                                          drone.yaw,
                                                                                          drone.pitch,
                                                                                          drone.lat,
                                                                                          drone.lng,
                                                                                          drone.alt,
                                                                                          drone.connectionColor,
                                                                                          drone.port,
                                                                                          mission.ToString());

                        droneInfoCardListViewModel.droneInfoList[i] = droneInfoCard;

                        mapView.Markers.Remove(drone.droneMarker);
                        drone.droneMarker.Position = new PointLatLng(drone.lat, drone.lng);
                        mapView.Markers.Add(drone.droneMarker);
                    }
                    
                    // update log viewer
                    while (lM.logList.Count != 0)
                    {
                        string log_string = lM.logList.Dequeue();
                        logViewModel.logList.Insert(0, log_string);
                    }

                    // update drone status colors
                    while(sm.droneStatusColorQueue.Count != 0)
                    {
                        Dictionary<Drone, string> droneWithColor = sm.droneStatusColorQueue.Dequeue();

                        (droneWithColor.First().Key.droneMarker.Shape as DroneMarker).port_status_color.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(droneWithColor.First().Value);
                    }

                    // update drone numbers
                    tb_inMissionNumberOfDrones.Text = sm.droneNumbersViewModel.inMissionNumberOfDrones.ToString();
                    tb_availableNumberOfDrones.Text = sm.droneNumbersViewModel.availableNumberOfDrones.ToString();
                }));

                Thread.Sleep(200);
            }
        }

        private void mapView_Loaded(object sender, RoutedEventArgs e)
        {
            //mapView.CacheLocation = @"C:\Users\User\AppData\Local\GMap.NET";
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
            if(clickFunction == ClickFunction.CreateMissionSurveillance)
            {
                Point mousePos = e.GetPosition(mapView);
                double lat = mapView.FromLocalToLatLng((int)mousePos.X, (int)mousePos.Y).Lat;
                double lng = mapView.FromLocalToLatLng((int)mousePos.X, (int)mousePos.Y).Lng;
                PointLatLng coords = new PointLatLng(lat, lng);
                Debug.WriteLine($"lat: {lat}, lng: {lng}");

                surveillanceClickCounter++;

                MissionItem missionItem = new MissionItem();
                GMapMarker gmapMarker = new GMapMarker(coords);
                gmapMarker.Shape = new MissionItemMarker();
                gmapMarker.Offset = new Point(-10, -20); // -15,-15 dronemarker
                missionItem.marker = gmapMarker;
                mapView.Markers.Add(gmapMarker);

                (ms as MissionSurvelliance).pointsToSurveillance.Add(coords);

                if (surveillanceClickCounter == ms.numberOfDronesInMission)
                {
                    string missionName = "SVR_" + tb_missionName.Text;
                    ms.missionName = missionName;
                    missionNamesViewModel.missionNames.Add(missionName);

                    pc.allMissions.Add(ms);
                    (ms as MissionSurvelliance).createRoutes();

                    surveillanceClickCounter = 0;
                    clickFunction = ClickFunction.Select;
                }
            }else if (clickFunction == ClickFunction.CreateMissionKamikaze)
            {
                Point mousePos = e.GetPosition(mapView);
                double lat = mapView.FromLocalToLatLng((int)mousePos.X, (int)mousePos.Y).Lat;
                double lng = mapView.FromLocalToLatLng((int)mousePos.X, (int)mousePos.Y).Lng;
                PointLatLng targetCoords = new PointLatLng(lat, lng);
                Debug.WriteLine($"lat: {lat}, lng: {lng}");

                GMapMarker gmapMarker = new GMapMarker(targetCoords);
                gmapMarker.Shape = new AirDefenceMarker();
                gmapMarker.Offset = new Point(-25, -25);
                mapView.Markers.Add(gmapMarker);

                string missionName = "KMZ_" + tb_missionName.Text;
                ms.missionName = missionName;
                missionNamesViewModel.missionNames.Add(missionName);

                pc.allMissions.Add(ms);
                (ms as MissionKamikaze).airDefenceLocation = targetCoords;
                (ms as MissionKamikaze).createRoutes();

                clickFunction = ClickFunction.Select;
            }
            else if (clickFunction == ClickFunction.CreateMissionScan)
            {
                Point mousePos = e.GetPosition(mapView);
                double lat = mapView.FromLocalToLatLng((int)mousePos.X, (int)mousePos.Y).Lat;
                double lng = mapView.FromLocalToLatLng((int)mousePos.X, (int)mousePos.Y).Lng;
                PointLatLng cornerCoord = new PointLatLng(lat, lng);
                Debug.WriteLine($"lat: {lat}, lng: {lng}");

                GMapMarker gmapMarker = new GMapMarker(cornerCoord);
                gmapMarker.Shape = new MissionItemMarker();
                gmapMarker.Offset = new Point(-25, -25);
                mapView.Markers.Add(gmapMarker);

                if (scanCornerCounter == 0) (ms as MissionScan).Corner1 = cornerCoord;
                else (ms as MissionScan).Corner2 = cornerCoord;
                scanCornerCounter++;

                if (scanCornerCounter == 2)
                {
                    string missionName = "SCN_" + tb_missionName.Text;
                    ms.missionName = missionName;
                    missionNamesViewModel.missionNames.Add(missionName);

                    pc.allMissions.Add(ms);
                    (ms as MissionScan).createRoutes();

                    scanCornerCounter = 0;
                    clickFunction = ClickFunction.Select;
                }
            }
        

    }


        private void btn_Surveillance_Click(object sender, RoutedEventArgs e)
        {
            int desiredNumberOfDronesInTheMission = int.Parse(tb_numberOfDronesInMission.Text.ToString());
            if(desiredNumberOfDronesInTheMission > sm.availableNumberOfDrones)
            {
                MessageBox.Show($"There is only {sm.availableNumberOfDrones} drones available!",
                                "Mission Creation Failed");

                return;
            }

            clickFunction = ClickFunction.CreateMissionSurveillance;
            ms = new MissionSurvelliance(missionIDCounter++);
            ms.numberOfDronesInMission = desiredNumberOfDronesInTheMission;
            MessageBox.Show("Mark fields to watch");
        }
        private void btn_Kamikaze_Click(object sender, RoutedEventArgs e)
        {
            int desiredNumberOfDronesInTheMission = int.Parse(tb_numberOfDronesInMission.Text.ToString());

            if (desiredNumberOfDronesInTheMission > sm.availableNumberOfDrones)
            {
                MessageBox.Show($"There is only {sm.availableNumberOfDrones} drones available!",
                                "Mission Creation Failed");

                return;
            }else if(desiredNumberOfDronesInTheMission < 3)
            {
                MessageBox.Show($"At least 3 drones are needed for this mission!",
                                "Mission Creation Failed");

                return;
            }

            clickFunction = ClickFunction.CreateMissionKamikaze;
            ms = new MissionKamikaze(missionIDCounter++);
            ms.numberOfDronesInMission = desiredNumberOfDronesInTheMission;
            MessageBox.Show("Mark target to destroy");
        }

        private void btn_Scan_Click(object sender, RoutedEventArgs e)
        {
            int desiredNumberOfDronesInTheMission = int.Parse(tb_numberOfDronesInMission.Text.ToString());

            if (desiredNumberOfDronesInTheMission > sm.availableNumberOfDrones)
            {
                MessageBox.Show($"There is only {sm.availableNumberOfDrones} drones available!",
                                "Mission Creation Failed");

                return;
            }

            clickFunction = ClickFunction.CreateMissionScan;
            ms = new MissionScan(missionIDCounter++);
            ms.numberOfDronesInMission = desiredNumberOfDronesInTheMission;
            MessageBox.Show("Mark corners to scan");
        }

        private void btn_StartMission_Click(object sender, RoutedEventArgs e)
        {
            sm.InitStartMission(cmb_missionNames.SelectedItem.ToString());
        }
        private void mapView_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(mapView);
            double lat = mapView.FromLocalToLatLng((int)mousePos.X, (int)mousePos.Y).Lat;
            double lng = mapView.FromLocalToLatLng((int)mousePos.X, (int)mousePos.Y).Lng;

            tb_Lat.Text = Math.Round(lat, 6).ToString();
            tb_Lng.Text = Math.Round(lng, 6).ToString();
        }

        //private void btn_CreateDrones_Click(object sender, RoutedEventArgs e)
        //{
        //    Thread createDronesThread = new Thread(() => CreateDrones(3));
        //    createDronesThread.Start();
        //}

        //private void CreateDrones(int numberOfDrones)
        //{
        //    Process bashProcess = new Process();

        //    bashProcess.StartInfo.FileName = @"D:\cygwin64\bin\bash.exe";
        //    bashProcess.StartInfo.Arguments = $"--login -i -l -c 'cd ardupilot/ArduCopter; ../Tools/autotest/sim_vehicle.py -n{numberOfDrones}'";
        //    bashProcess.StartInfo.UseShellExecute = true;

        //    bashProcess.Start();
        //    bashProcess.WaitForExit();
        //}


        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    Application.Current.MainWindow = this;
        //}
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
using GMap.NET;
using GMap.NET.WindowsPresentation;
using SwarmController.Enums;
using SwarmController.Markers;
using SwarmController.Models.Plan;
using SwarmController.Models.Swarm;
using SwarmController.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        SwarmManager sm = SwarmManager.getSwarmManager();
        PlanController pc = PlanController.getPlanController();

        public MainWindow()
        {
            InitializeComponent();
            lv_Log.DataContext = logViewModel;
        }


        private void mapView_Loaded(object sender, RoutedEventArgs e)
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            mapView.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance;
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
                gmapMarker.Offset = new Point(-10, -20);
                missionItem.marker = gmapMarker;
                mapView.Markers.Add(gmapMarker);

                if (surveillanceClickCounter == 1)
                {
                    ms.topLeftCorner = coords;
                    addLog("topLeftCorner selected");
                }
                else
                {
                    ms.bottomUpCorner = coords;
                    ms.numberOfDronesInMission = 3;
                    //pc.currentMission = ms;
                    sm.currentMission = ms;
                    pc.allMissions.Add(ms);
                    addLog("bottomUpCorner selected");

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
            MessageBox.Show("Choose TopLeft and BottomRight positions of field!");
        }

        private void addLog(string log_string)
        {
            logViewModel.logList.Insert(0, log_string);
        }

        private void btn_StartMission_Click(object sender, RoutedEventArgs e)
        {
            sm.startMission();
        }
    }
}

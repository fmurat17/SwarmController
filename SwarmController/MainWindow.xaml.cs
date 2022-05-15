using GMap.NET;
using GMap.NET.WindowsPresentation;
using SwarmController.Enums;
using SwarmController.Markers;
using SwarmController.Models.Plan;
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

        public MainWindow()
        {
            InitializeComponent();
        }

        private void mapView_Loaded(object sender, RoutedEventArgs e)
        {
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            mapView.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance;
            mapView.MinZoom = 1;
            mapView.MaxZoom = 24;
            mapView.Zoom = 2;
            mapView.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
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
                Debug.WriteLine($"lat: {lat}, lng: {lng}");

                surveillanceClickCounter++;

                if(surveillanceClickCounter == 1)
                {
                    MissionItem missionItem = new MissionItem();
                    GMapMarker gmapMarker = new GMapMarker(new PointLatLng(lat, lng));
                    gmapMarker.Shape = new MissionItemMarker();
                    gmapMarker.Offset = new Point(-10, -20);
                    missionItem.marker = gmapMarker;
                    mapView.Markers.Add(gmapMarker);
                }
                else
                {
                    MissionItem missionItem = new MissionItem();
                    GMapMarker gmapMarker = new GMapMarker(new PointLatLng(lat, lng));
                    gmapMarker.Shape = new MissionItemMarker();
                    gmapMarker.Offset = new Point(-10, -20);
                    missionItem.marker = gmapMarker;
                    mapView.Markers.Add(gmapMarker);

                    surveillanceClickCounter = 0;


                }

                
            }

        }

        private void btn_Surveillance_Click(object sender, RoutedEventArgs e)
        {
            clickFunction = ClickFunction.CreateMission;
            MessageBox.Show("Choose TopLeft and BottomRight positions of field!");
        }

    }
}

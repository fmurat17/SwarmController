using GMap.NET;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwarmController.Models.Plan
{
    public class MissionItem
    {
        public int itemOrder { get; set; }
        public GMapMarker marker { get; set; }
        public double altitude { get; set; }
        public PointLatLng koordinat
        {
            get
            {
                return marker.Position;
            }
            set
            {

            }
        }
    }
}

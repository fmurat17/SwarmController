using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwarmController.ViewModels
{
    public class DroneInfoCardViewModel : ViewModelBase
    {

        public double roll { get; set; }
        public double yaw { get; set; }
        public double pitch { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public double alt { get; set; }
        public string connectionColor { get; set; }

        public DroneInfoCardViewModel(double roll, double yaw, double pitch, double lat, double lng, double alt, string connectionColor)
        {
            this.roll = roll;
            this.yaw = yaw;
            this.pitch = pitch;
            this.lat = lat;
            this.lng = lng;
            this.alt = alt;
            this.connectionColor = connectionColor;
        }
        
    }
}

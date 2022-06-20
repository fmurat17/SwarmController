using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SwarmController.Models.Plan
{
    public class Route
    {
        public GMapRoute gMapRoute;
        public List<MissionItem> missionItems;
        public int missionID;
        public List<SolidColorBrush> colors;

        public Route()
        {
            missionItems = new List<MissionItem>();
            colors = new List<SolidColorBrush>();
            colors.Add(Brushes.Green);
            colors.Add(Brushes.Blue);
            colors.Add(Brushes.Red);
        }
    }
}

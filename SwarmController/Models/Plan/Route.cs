using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwarmController.Models.Plan
{
    public class Route
    {
        public GMapRoute rota;
        public List<MissionItem> missionItems;

        public Route()
        {
            missionItems = new List<MissionItem>();
        }
    }
}

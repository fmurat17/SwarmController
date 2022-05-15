using GMap.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwarmController.Models.Plan
{
    public class MissionSurvelliance : MissionBase
    {
        public PointLatLng topLeftCorner;
        public PointLatLng bottomUpCorner;

        List<Route> routes;

        public MissionSurvelliance(int missionID)
        {
            this.missionID = missionID;
            routes = new List<Route>();
        }

        public void createRoutes()
        {

        }

        public void uploadMissionToDrones()
        {

        }
    }
}

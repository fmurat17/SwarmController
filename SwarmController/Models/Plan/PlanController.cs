using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwarmController.Models.Plan
{
    public class PlanController
    {
        private static PlanController planController = null;

        public static PlanController getPlanController()
        {
            if (planController == null)
            {
                planController = new PlanController();
            }

            return planController;
        }


        public MissionItem selectedMissionItem = null;
        public Route selectedRoute = null;
        //public MissionBase currentMission = null;

        public List<Route> allRoutes = new List<Route>();
        public List<MissionBase> allMissions = new List<MissionBase>();

        public MissionBase getMissionByName(string missionName)
        {
            foreach(MissionBase mission in allMissions)
            {
                if (mission.missionName == missionName) return mission;
            }

            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwarmController.Models.Plan
{
    class PlanController
    {
        private static PlanController planController = null;

        public static PlanController getPlanController()
        {
            if (planController == null)
            {
                planController = new PlanController();
                allRoutes = new List<Route>();
            }

            return planController;
        }


        public static MissionItem selectedMissionItem = null;
        public static Route selectedRoute = null;

        public static List<Route> allRoutes;
    }
}

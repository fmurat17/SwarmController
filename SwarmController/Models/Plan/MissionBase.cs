using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwarmController.Models.Plan
{
    public class MissionBase
    {
        public int missionID;
        public int numberOfDronesInMission;
        public List<int> assignedDrones;
    }
}

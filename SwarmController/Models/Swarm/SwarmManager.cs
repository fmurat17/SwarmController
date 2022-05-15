using SwarmController.Models.Plan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwarmController.Models.Swarm
{
    public class SwarmManager
    {
        private static SwarmManager swarmManager = null;
        public static SwarmManager getSwarmManager()
        {
            if (swarmManager == null)
            {
                swarmManager = new SwarmManager();
            }

            return swarmManager;
        }

        public MissionBase currentMission = null;
        public int totalNumberOfDrones = 3;

        // dict<port, missionID> -1 = available, others = assigned to a mission
        public Dictionary<int, int> allDrones = new Dictionary<int, int>();

        public void createDrones()
        {
            for(int i = 0; i < totalNumberOfDrones; i++)
            {
                int port = 5760 + i * 10;
                allDrones.Add(port, -1);
            }
        }

        public void startMission()
        {
            assignDrones();
            uploadMissionToDrones();
            flyDrones();
        }

        public void assignDrones()
        {
            int numberOfDronesInMission = currentMission.numberOfDronesInMission;
            int missionID = currentMission.missionID;

            for (int i = 0, p = 0; i < numberOfDronesInMission;)
            {
                int port = 5760 + (p * 10);
                p++;
                if (allDrones[port] == -1)
                {
                    allDrones[port] = missionID;
                    currentMission.assignedDrones.Add(port);
                    i++;
                }
            }
        }

        public void uploadMissionToDrones()
        {
            (currentMission as MissionSurvelliance).uploadMissionToDrones();
        }

        public void flyDrones()
        {

        }

    }
}
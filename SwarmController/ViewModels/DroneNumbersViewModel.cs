using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwarmController.ViewModels
{
    public class DroneNumbersViewModel : ViewModelBase
    {
        private int _availableNumberOfDrones;
        public int availableNumberOfDrones
        {
            get
            {
                return _availableNumberOfDrones;
            }
            set
            {
                _availableNumberOfDrones = value;
                OnPropertyChanged(nameof(availableNumberOfDrones));
            }
        }

        private int _inMissionNumberOfDrones;


        public int inMissionNumberOfDrones
        {
            get
            {
                return _inMissionNumberOfDrones;
            }
            set
            {
                _inMissionNumberOfDrones = value;
                OnPropertyChanged(nameof(inMissionNumberOfDrones));
            }
        }
        public DroneNumbersViewModel(int availableNumberOfDrones, int inMissionNumberOfDrones)
        {
            this.availableNumberOfDrones = availableNumberOfDrones;
            this.inMissionNumberOfDrones = inMissionNumberOfDrones;
        }

    }
}

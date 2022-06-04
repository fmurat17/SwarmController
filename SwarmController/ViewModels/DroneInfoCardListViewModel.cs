using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwarmController.ViewModels
{
    public class DroneInfoCardListViewModel : ViewModelBase
    {
        private ObservableCollection<DroneInfoCardViewModel> _droneInfoList = new ObservableCollection<DroneInfoCardViewModel>();
        public ObservableCollection<DroneInfoCardViewModel> droneInfoList
        {
            get
            {
                return _droneInfoList;
            }
            set
            {
                _droneInfoList = value;
                OnPropertyChanged(nameof(droneInfoList));
            }
        }
    }
}

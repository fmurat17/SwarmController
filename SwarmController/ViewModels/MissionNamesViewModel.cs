using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwarmController.ViewModels
{
    public class MissionNamesViewModel : ViewModelBase
    {
        private ObservableCollection<string> _missionNames = new ObservableCollection<string>();
        public ObservableCollection<string> missionNames
        {
            get
            {
                return _missionNames;
            }
            set
            {
                _missionNames = value;
                OnPropertyChanged(nameof(missionNames));
            }
        }

    }
}

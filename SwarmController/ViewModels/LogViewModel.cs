using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwarmController.ViewModels
{
    public class LogViewModel : ViewModelBase
    {
        private ObservableCollection<string> _logList = new ObservableCollection<string>();
        public ObservableCollection<string> logList
        {
            get
            {
                return _logList;
            }
            set
            {
                _logList = value;
                OnPropertyChanged(nameof(logList));
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace GPS_Project.ViewModels {
    public class BaseViewModel : INotifyPropertyChanged {
        public BaseViewModel() {

        }

        #region Properties
        bool _isBusy;
        public bool IsBusy {
            get { return _isBusy; }
            set {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        #endregion


        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}

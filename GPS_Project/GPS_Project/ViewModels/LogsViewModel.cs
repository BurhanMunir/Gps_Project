using GPS_Project.Helper;
using GPS_Project.Models;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GPS_Project.ViewModels {
    public class LogsViewModel : BaseViewModel {
        #region Properties
        public INavigation Navigation { get; set; }

        public ObservableCollection<GpsModel> LogsCollection { get; set; }
        #endregion
        #region Commands
        public ICommand GpsPageCommand { get; set; }
        public ICommand LogClearCommand { get; set; }
        public ICommand ExportJsonCommand { get; set; }
        #endregion

        #region Constructor
        public LogsViewModel(INavigation navigation) {
            Navigation = navigation;

            LogsCollection = new ObservableCollection<GpsModel>();

            GetLogs();

            GpsPageCommand = new Command(async () => {
                await Navigation.PopAsync();
            });
            ExportJsonCommand = new Command(async () => {
                var db = await Utils.Init();
                var logs = await db.Table<GpsModel>().OrderByDescending(x => x.TimeStamp).ToListAsync();
                if (logs != null) {
                    var jsonLogs = JsonConvert.SerializeObject(logs);
                    await Share.RequestAsync(jsonLogs);
                }
                
            });
            LogClearCommand = new Command(async () => {
                var db = await Utils.Init();
                var isDeleted = await db.DeleteAllAsync<GpsModel>();
                if (isDeleted > 0) {
                    LogsCollection.Clear();

                }
            });
            //Get Notify when new log is registered into the storage
            MessagingCenter.Subscribe<object, GpsModel>(this, "LogRegistered", (sender, args) => {
                var log = args as GpsModel;
                if (log != null) {
                    LogsCollection.Insert(0, log);
                }
            });
        }
        #endregion

        #region Methods
        //Get Registered Logs from local storage
        private async void GetLogs() {
            var db = await Utils.Init();
            var logs = await db.Table<GpsModel>().OrderByDescending(x => x.TimeStamp).ToListAsync();

            foreach (var log in logs) {
                LogsCollection.Add(log);
            }

        }
        #endregion
    }
}

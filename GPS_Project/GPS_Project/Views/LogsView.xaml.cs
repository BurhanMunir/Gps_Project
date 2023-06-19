using GPS_Project.Models;
using GPS_Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GPS_Project.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogsView : ContentPage
    {
        public LogsView()
        {
            InitializeComponent();
            this.BindingContext =new LogsViewModel(Navigation);

            //Scrolls the log's collectionview to Top when new log is registered
            MessagingCenter.Subscribe<object,GpsModel>(this, "LogRegistered", (sender, args) =>
            {
                if((GpsModel)args!=null)
                {
                    logList.ScrollTo(0);
                }
            });
        }
    }
}
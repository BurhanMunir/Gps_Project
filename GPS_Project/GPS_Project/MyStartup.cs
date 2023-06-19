using Microsoft.Extensions.DependencyInjection;
using Shiny.Locations;
using Shiny;
using System;
using System.Collections.Generic;
using System.Text;

namespace GPS_Project
{
   public class MyStartup : Shiny.ShinyStartup {
        public override void ConfigureServices(IServiceCollection services, IPlatform platform) {
            services.UseNotifications();
          
            
            
            //services.RegisterCommonServices();
            //services.RegisterJob(new JobInfo {
            //    Identifier=nameof(NotificationJob),
            //    TypeName=typeof(NotificationJob)
            //});
        }
    }
}

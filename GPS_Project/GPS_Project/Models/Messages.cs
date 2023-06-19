using System;
using System.Collections.Generic;
using System.Text;

namespace GPS_Project.Models {
    public class StartServiceMessage {
    }

    public class StopServiceMessage {
    }

    public class LocationMessage {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class LocationErrorMessage {
    }
}

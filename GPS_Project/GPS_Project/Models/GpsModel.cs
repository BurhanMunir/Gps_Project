using System;
using System.Collections.Generic;
using System.Text;

namespace GPS_Project.Models {
    public class GpsModel {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Heading { get; set; }
        public string HeadingAccuracy { get; set; }
        public string PositionAccuracy { get; set; }
        public string Speed { get; set; }
        public string TimeStamp { get; set; } = DateTime.Now.ToString();
        public string Accuracy { get; set; }

    }
}

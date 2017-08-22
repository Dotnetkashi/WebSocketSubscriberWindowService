using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Webscoket.Models
{
    public class LocationData
    {
        public string mac { get; set; }

        public string sequence { get; set; }

        public string sn { get; set; }

        public string bn { get; set; }

        public string fn { get; set; }

        public int x { get; set; }

        public int y { get; set; }

        public int z { get; set; }

        public double last_seen_ts { get; set; }
        public DateTime LastSeenDatetime { get; set; }
        public DateTime PostDateTime { get; set; }
        public string action { get; set; }

        public string fix_result { get; set; }

        public string[] an { get; set; }

        public string AreaName { get; set; }
    }
}

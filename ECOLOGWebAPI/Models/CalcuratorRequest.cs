using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECOLOGWebAPI.Models
{
    /*
     *  ECOLOG計算リクエストのリクエストボディデシリアライズクラス
     */
    public class CalcuratorRequest
    {
        public int Driver { get; set; }
        public int Car { get; set; }
        public int Sensor { get; set; }
        public string Direction { get; set; }
        IList<GPSTuple> GpsTuples { get; set; }
        public DateTime PostDate { get; set; }
    }

    public class GPSTuple
    {
        public DateTime GpsTime { get; set; }
        public DateTime PhoneTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public double Speed { get; set; }
    }
}

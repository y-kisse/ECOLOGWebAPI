using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

namespace ECOLOGWebAPI.Models
{
    /*
     * 計算結果レスポンスシリアライズ用クラス
     */
    public class CalculatorResponse
    {
        public int Driver { get; set; }
        public int Car { get; set; }
        public int Sensor { get; set; }
        public IList<ECOLOGTuple> EcologTuples { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("driver: {Driver}");
            sb.AppendLine("car: {Car}");
            sb.AppendLine("sensor: {Sensor}");
            foreach(ECOLOGTuple tuple in EcologTuples)
            {
                sb.AppendLine(tuple.ToString());
            }
            return sb.ToString();
        }
    }

    /*
     * 時間正規化した結果のシリアラライズクラス
     */ 
    public class ECOLOGTuple
    {
        public DateTime Jst { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
        public double TerrainAltitude { get; set; }
        public double AirResistanceLoss { get; set; }
        public double RollingLoss { get; set; }
        public double ConvertLoss { get; set; }
        public double RegeneLoss { get; set; }
        public double LostEnergy { get; set; }
        public double Efficiency { get; set; }
        public string Link { get; set; }
        public int SemanticLink { get; set; }
    }
}

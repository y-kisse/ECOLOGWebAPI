using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SensorLogInserterRe.Daos;

namespace ECOLOGWebAPI.Calculator
{
    // GPSデータ点から近傍の特定のリンクを取得するメソッドを記載
    public class LinkDetector
    {

        // GPSデータ点と、driver_idから最近傍リンクとセマンティックリンクを求める
        // driver_idで読み込むセマンティックリンクを限定する
        public static Tuple<string, int> detectLink(int driver_id, double lat, double lng)
        {
            string link_id = "RB000000000x";
            int semantic_link_id = -1;

            return new Tuple<string, int>(link_id, semantic_link_id);
        }
    }
}

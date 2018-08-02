using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using SensorLogInserterRe.Daos;

namespace ECOLOGWebAPI.Calculator
{
    // GPSデータ点から近傍の特定のリンクを取得するメソッドを記載
    public class LinkDetector
    {

        // GPSデータ点と、driver_idから最近傍リンクとセマンティックリンクを求める
        // driver_idで読み込むセマンティックリンクを限定する
        public static Tuple<string, int> detectLink(int driver_id, string direction, double lat, double lng)
        {
            string link_id = "RB000000000x";
            int semantic_link_id = -1;

            #region Driver毎に、特定のセマンティックリンクidのリストの作成

            List<int> semantic_link_id_list = new List<int>();
            
            switch (driver_id)
            {
                // 運転者やルート追加ごとにこれらの更新が必要になる。
                case 1:
                    if (direction == "outward")
                        for (int id = 187; id <= 201; id++)
                            semantic_link_id_list.Add(id);
                    else if (direction == "homeward")
                        for (int id = 202; id <= 218; id++)
                            semantic_link_id_list.Add(id);
                    break;
            }

            int[] semantic_link_id_array = semantic_link_id_list.ToArray();
            #endregion

            // 運転者ごとのリンクが得られる
            // TODO 何度も同じテーブルを得ることになるので、上手くキャッシュする仕組みが必要。  
            DataTable linkTable = LinkDao.GetLinkTableforMM(semantic_link_id_array);

            return new Tuple<string, int>(link_id, semantic_link_id);
        }
    }
}

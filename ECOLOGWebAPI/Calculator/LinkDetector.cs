﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using SensorLogInserterRe.Daos;
using SensorLogInserterRe.Models;

namespace ECOLOGWebAPI.Calculator
{
    // GPSデータ点から近傍の特定のリンクを取得するメソッドを記載
    public class LinkDetector
    {
        /*
         * シングルトンクラスとして作成
         * 1人しか使わず、その人のセマンティックリンクを何度も参照することになるため作成
         * DBへのクエリコスト軽減を意図。  
         * ドライバーID
         * 運転方向
         * その時のリンク集合をインスタンス変数として持つことにする。
         */ 

        private static LinkDetector _instance;
        public int DriverID { get; set; }
        public string Direction { get; set; }
        private DataTable _linkTable;

        private LinkDetector()
        {
            // for singleton
        }

        public static LinkDetector GetInstance(int driverID, string direction)
        {
            if (_instance == null)
            {
                // インスタンス作成処理 (インスタンスがない) 
                return CreateInstance(driverID, direction);
            }

            if ( !(_instance.DriverID == driverID && _instance.Direction == direction) )
            {
                // インスタンス作成処理 (既にあるインスタンスのドライバーIDと運転方向が引数と一致しない)
                return CreateInstance(driverID, direction);
            }

            // 既に有効なインスタンスが存在する場合にはそれを返す。  
            return _instance;
        }

        private static LinkDetector CreateInstance(int driverID, string direction)
        {
            // リンクテーブルを作成
            #region Driver毎に、特定のセマンティックリンクidのリストの作成

            List<int> semanticLinkIdList = new List<int>();

            switch (driverID)
            {
                // 運転者やルート追加ごとにこれらの更新が必要になる。
                // TODO: ここをハードコードにしない
                case 1:
                    if (direction == "outward")
                        for (int id = 187; id <= 201; id++)
                            semanticLinkIdList.Add(id);
                    else if (direction == "homeward")
                        for (int id = 202; id <= 218; id++)
                            semanticLinkIdList.Add(id);
                    break;
            }

            int[] semanticLinkIdArray = semanticLinkIdList.ToArray();
            #endregion

            return new LinkDetector
            {
                DriverID = driverID,
                Direction = direction,
                _linkTable = LinkDao.GetLinkTableforMM(semanticLinkIdArray)
            };
        }

        private static DataTable getLinkTable(int [] semanticLinkIdArray)
        {
            string semanticLinkStatement = "(";
            foreach (int semanticLinkId in semanticLinkIdArray)
            {
                semanticLinkStatement += semanticLinkId + ",";
            }

            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.AppendLine("SELECT");
            queryBuilder.AppendLine("    l1.LINK_ID AS LINK_ID,");
            queryBuilder.AppendLine("    l1.NUM,");
            queryBuilder.AppendLine("    ");

            return new DataTable();
        }

        public Tuple<string, int> detect(double lat, double lng)
        {
            string linkId = "RB000000000x";
            int semanticLinkId = -1;
            double distance = 255;

            // インスタンス変数のリンク集合から近傍リンクの絞り込み
            string query = "START_LAT > " + (lat - 0.05);
            query += " AND START_LAT < " + (lat + 0.05);
            query += " AND START_LONG > " + (lng - 0.05);
            query += " AND START_LOMG < " + (lng + 0.05);
            DataRow[] dataRows = _linkTable.Select(query);

            // 現在値をベクトル表現
            TwoDimensionalVector currentPoint = new TwoDimensionalVector(lat, lng);
            // 全探索
            foreach (DataRow row in dataRows)
            {
                // リンクの始端終端をそれぞれベクトル表現
                TwoDimensionalVector linkStartEdge = new TwoDimensionalVector((double)row["START_LAT"], (double)row["START_LONG"]);
                TwoDimensionalVector linkEndEdge = new TwoDimensionalVector((double)row["END_LAT"], (double)row["END_LONG"]);

                // ベクトル演算により現在値と着目しているリンクの最近傍点を得る
                TwoDimensionalVector matchedPoint = TwoDimensionalVector.nearest(linkStartEdge, linkEndEdge, currentPoint);

                // マッチ点と現在地の距離
                double tempDist = TwoDimensionalVector.distance(currentPoint, matchedPoint);

                if (tempDist < distance)
                {
                    distance = tempDist;
                    linkId = (string)row["LINK_ID"];
                    semanticLinkId = (int)row["SEMANTIC_LINK_ID"];
                }
            }

            return new Tuple<string, int>(linkId, semanticLinkId);
        }

        // GPSデータ点と、driver_idから最近傍リンクとセマンティックリンクを求める
        // driver_idで読み込むセマンティックリンクを限定する
        public static Tuple<string, int> detectLink(int driverId, string direction, double lat, double lng)
        {
            string linkId = "RB000000000x";
            int semanticLinkId = -1;

            #region Driver毎に、特定のセマンティックリンクidのリストの作成

            List<int> semanticLinkIdList = new List<int>();
            
            switch (driverId)
            {
                // 運転者やルート追加ごとにこれらの更新が必要になる。
                case 1:
                    if (direction == "outward")
                        for (int id = 187; id <= 201; id++)
                            semanticLinkIdList.Add(id);
                    else if (direction == "homeward")
                        for (int id = 202; id <= 218; id++)
                            semanticLinkIdList.Add(id);
                    break;
            }

            int[] semanticLinkIdArray = semanticLinkIdList.ToArray();
            #endregion

            // 運転者ごとのリンクが得られる
            // TODO 何度も同じテーブルを得ることになるので、上手くキャッシュする仕組みが必要。  
            DataTable linkTable = LinkDao.GetLinkTableforMM(semanticLinkIdArray);

            return new Tuple<string, int>(linkId, semanticLinkId);
        }
    }
}

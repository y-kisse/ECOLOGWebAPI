using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECOLOGWebAPI.Models;

namespace ECOLOGWebAPI.Calculator
{
    /*
     * EcologCalculatorクラス
     * SensorLogInserterReに存在するソースコードを用いて、EcologTupleを生成するクラス
     * 基本的には2つのGPSデータからECOLOG計算を行い、その計算結果を返すようなものを想定する。
     */
    public class EcologCalculator
    {
        public static ECOLOGTuple calculateEcologTuple(GPSTuple previusGPS, GPSTuple currentGPSTuple)
        {
            // ECOLOGTupleの空のインスタンスを作成
            ECOLOGTuple retTuple = new ECOLOGTuple();
            
            // GPSTupleから現在地、GPS時刻、速度を取得。
            // 端末時刻は時刻同期がちゃんと取れてる必要があるので、GPS時刻を使うのが無難。
            retTuple.Jst = currentGPSTuple.GpsTime;
            retTuple.Latitude = currentGPSTuple.Latitude;
            retTuple.Longitude = currentGPSTuple.Longitude;
            retTuple.Speed = currentGPSTuple.Speed;

            // 標高はMapMatching or 10mメッシュ標高データから取得する。

            // 各種損失はSensorLogInserterReの計算メソッドを用いて算出する

            // Efficiency

            // link and semantic_link
            // 近傍リンクをSQLで指定、すべてのリンクに対して、GPS点とリンクの距離を求める
            // 最近傍リンクまでの距離が閾値を超えない場合には、それをGPS点が通過するリンクとする


            return retTuple;
        }
    }
}

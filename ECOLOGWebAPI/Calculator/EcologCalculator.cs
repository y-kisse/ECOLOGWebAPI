using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using ECOLOGWebAPI.Models;
using SensorLogInserterRe.Calculators;
using SensorLogInserterRe.Models;

namespace ECOLOGWebAPI.Calculator
{
    /*
     * EcologCalculatorクラス
     * SensorLogInserterReに存在するソースコードを用いて、EcologTupleを生成するクラス
     * 基本的には2つのGPSデータからECOLOG計算を行い、その計算結果を返すようなものを想定する。
     * 1レコード作成ごとにDataTable作成するのも大変そうなので、リクエスト時に近傍or指定の道路リンク集合を渡すことにした方が良さそう。  
     */
    public class EcologCalculator
    {
        
        private static readonly double WindSpeed = 0;
        private static readonly double Rho = 1.22;
        private static readonly double Myu = 0.015;

        public static ECOLOGTuple calculateEcologTuple(GPSTuple previusGPS, GPSTuple currentGPSTuple, DataTable roadLinks, Car car)
        {
            // ECOLOGTupleの空のインスタンスを作成
            ECOLOGTuple retTuple = new ECOLOGTuple();
            
            // GPSTupleから現在地、GPS時刻、速度を取得。
            // 端末時刻は時刻同期がちゃんと取れてる必要があるので、GPS時刻を使うのが無難な気がする。
            retTuple.Jst = currentGPSTuple.GpsTime;
            retTuple.Latitude = currentGPSTuple.Latitude;
            retTuple.Longitude = currentGPSTuple.Longitude;
            retTuple.Speed = currentGPSTuple.Speed;

            // 10mメッシュ標高データから取得する。
            // シングルトンクラスのGetInstance()はインスタンスそのものを返すのでnewいらない。  
            AltitudeCalculator altitudeCalculator = AltitudeCalculator.GetInstance();
            Tuple<int, double> meshIdAltitudeTuple = altitudeCalculator.CalcAltitude(currentGPSTuple.Latitude, currentGPSTuple.Longitude);
            retTuple.TerrainAltitude = meshIdAltitudeTuple.Item2;

            double previusAltitude = altitudeCalculator.CalcAltitude(previusGPS.Latitude, previusGPS.Longitude).Item2;
            double altitudeDiff = retTuple.TerrainAltitude - previusAltitude;

            // distance
            double distanceDiff = DistanceCalculator.CalcDistance(previusGPS.Latitude,
                                                                                      previusGPS.Longitude,
                                                                                      currentGPSTuple.Latitude,
                                                                                      currentGPSTuple.Longitude);



            // resistancePower
            // TODO: 条件式変更(speed > 1 && distanceDiff > 0)
            // TODO: 条件式同一のため、処理記述箇所を統合
            double airResistancePower = 0;
            if (currentGPSTuple.Speed > 1 && distanceDiff > 0)
                airResistancePower = AirResistanceCalculator.CalcPower(Rho,
                                                                                              car.CdValue,
                                                                                              car.FrontalProjectedArea,
                                                                                              (currentGPSTuple.Speed + WindSpeed) / 3.6,
                                                                                              currentGPSTuple.Speed / 3.6);

            double rollingResistancePower = 0;
            if (currentGPSTuple.Speed > 1 && distanceDiff > 0)
                rollingResistancePower = RollingResistanceCalculator.CalcPower(Myu,
                                                                                                         car.Weight,
                                                                                                         Math.Atan(altitudeDiff / distanceDiff), // TODO: 前のタプルとの標高差と距離が角度求めるには必要。  
                                                                                                         currentGPSTuple.Speed / 3.6);

            double climbingResistancePower = 0;
            if (currentGPSTuple.Speed > 1 && distanceDiff > 0)
                climbingResistancePower = ClimbingResistanceCalculator.CalcPower(car.Weight,
                                                                                                              Math.Atan(altitudeDiff / distanceDiff), // TODO: rollingResistancePowerと同様
                                                                                                              currentGPSTuple.Speed / 3.6);

            double accResitancePower = 0;
            if (currentGPSTuple.Speed > 1 && distanceDiff > 0)
                accResitancePower = AccResistanceCalculator.CalcPower(previusGPS.Speed / 3.6,
                                                                                              previusGPS.GpsTime,
                                                                                              currentGPSTuple.Speed / 3.6,
                                                                                              currentGPSTuple.GpsTime,
                                                                                              car.Weight);

            double drivingResistancePower = airResistancePower
                                                           + rollingResistancePower
                                                           + climbingResistancePower
                                                           + accResitancePower;


            // torque
            double torque = 0;
            if (drivingResistancePower > 0 && currentGPSTuple.Speed > 0)
                torque = drivingResistancePower * 1000 * 3600 / (currentGPSTuple.Speed / 3.6) * car.TireRadius / car.ReductionRatio;

            // Efficiency
            int efficiency = EfficiencyCalculator.GetInstance().GetEfficiency(car, currentGPSTuple.Speed / 3.6, torque);

            // 各種損失はSensorLogInserterReの計算メソッドを用いて算出する
            double convertLoss = ConvertLossCaluculator.CalcEnergy(drivingResistancePower,
                                                                                           car,
                                                                                           currentGPSTuple.Speed / 3.6,
                                                                                           efficiency);

            double regeneEnergy = RegeneEnergyCalculator.CalcEnergy(drivingResistancePower,
                                                                                                currentGPSTuple.Speed / 3.6,
                                                                                                car,
                                                                                                efficiency);

            double regeneLoss = RegeneLossCalculator.CalcEnergy(drivingResistancePower,
                                                                                         regeneEnergy,
                                                                                         car,
                                                                                         currentGPSTuple.Speed / 3.6,
                                                                                         efficiency);

            double lostEnergy = LostEnergyCalculator.CalcEnergy(convertLoss, 
                                                                                       regeneLoss, 
                                                                                       airResistancePower, 
                                                                                       rollingResistancePower);




            // link and semantic_link
            // 近傍リンクをSQLで指定、すべてのリンクに対して、GPS点とリンクの距離を求める
            // 最近傍リンクまでの距離が閾値を超えない場合には、それをGPS点が通過するリンクとする


            return retTuple;
        }
    }
}

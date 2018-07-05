# ECOLOGWebAPI
ECOLOGMobileApp用、ECOLOG計算WebAPI.

## 経緯
もともと、MobileApp上でECOLOG計算を行う予定であったが、それはサーバーサイドに任せればよいのではという意見が出たので、そちらで試してみることにした。  
連続する2つ以上の座標データなどから算出をECOLOG推定を行い、結果を返すようなものを考えている。  

## Submodule
Submoduleとして、SensorlogInserterを利用している.  
必要のないディレクトリはソリューションから無視するようにした。  
(本来はsubmoduleの中でも必要なディレクトリのみをこちらのリポジトリに持ってきたかったが、上手く行かなかったためにこのような手法をとった。)  
依存するNugetパッケージを1つずつ入れている。  


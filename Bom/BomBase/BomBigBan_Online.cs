using UnityEngine;
using Photon.Pun;
using Unity.Collections.LowLevel.Unsafe;
public class BomBigBan_Online : BomBigBan_Base
{
    override protected void AddComponentInstanceManager(){
        cInsManager = gameObject.AddComponent<InstanceManager_Online>();
        cInsManager.SetPothonView(cPhotonView.ViewID);
    }
    protected override bool IsExplosion(){
        if(null == cInsManager){
            return false;
        }
        // 以下コードを有効にして、管理者のみが爆風を表示する。
        // RPCで位置情報を同期して、ネットワーク全体でブロードキャストして、各キューから取り出してもらう。インスタンス生成は各自で生成のまま。
        // Bomクラスはオンライン用の派生クラスを爆風別に用意して、Player側でボム生成時に、AddComponentでオンラインの場合は、オンラインスクリプトをaddする
        if(cPhotonView.IsMine == false){
            return false;
        }
        Vector3 v3 = Library_Base.GetPos(transform.position);
        if (Library_Base.IsPositionOutOfBounds(v3)){
            return false;
        }
        return true;
    }

    protected override void Explosion()
    {
        if (!IsExplosion())
        {
            return;
        }
        Vector3 v3 = Library_Base.GetPos(transform.position);
		photonView.RPC(nameof(Explosion_RPC), RpcTarget.All, v3);


    }

	[PunRPC]
    public void Explosion_RPC(Vector3 basePosition)
    {
        if(null == cInsManager){
            return;
        }
        moveManager.StopMoving();
        transform.position = basePosition;

        // Reset processed coordinates
        processedCoordinates.Clear();

        // Explode in X and Z directions (positive and negative)

        for (int i = 0; i <= iExplosionNum; i++)
        {
            for (int j = 0; j <= iExplosionNum; j++)
            {
				XZ_Explosion(basePosition, i, j);
            }
        }
        for (int i = 0; i <= iExplosionNum; i++)
        {
            for (int j = 0; j <= iExplosionNum; j++)
            {
                XZ_Explosion(basePosition, i, -j);
            }
        }
        for (int i = 0; i <= iExplosionNum; i++)
        {
            for (int j = 0; j <= iExplosionNum; j++)
            {
				XZ_Explosion(basePosition, -i, j);
            }
        }
        for (int i = 0; i <= iExplosionNum; i++)
        {
            for (int j = 0; j <= iExplosionNum; j++)
            {
                XZ_Explosion(basePosition, -i, -j);
            }
        }

        // Destroy the bomb object after explosion
        cInsManager.DestroyInstance(this.gameObject);
    }

    [PunRPC]
    public void InstantiateInstancePool_RPC(Vector3 position){
        if(null == cInsManager){
            return;
        }
        cInsManager.InstantiateInstancePool_Base(position);
    }
    [PunRPC]
    public void DestroyInstancePool_RPC(){
        if(null == cInsManager){
            return;
        }
        cInsManager.DestroyInstancePool_Base();

    }

}

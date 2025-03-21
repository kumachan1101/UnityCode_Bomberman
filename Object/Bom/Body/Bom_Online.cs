using UnityEngine;
using Photon.Pun;

public class Bom_Online : Bom_Base
{
    private PhotonView cPhotonView;
    protected override void AddComponentInstanceManager(){
        cInsManager = gameObject.AddComponent<InstanceManager_Online>();
        cPhotonView = GetComponent<PhotonView>();
        cInsManager.SetPothonView(cPhotonView.ViewID);
    }
 

    protected override bool IsExplosion(){
        if(null == cInsManager){
            return false;
        }
        // 管理者のみが爆風を表示する。
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
        if (!IsExplosion()) return;

        Vector3 v3 = Library_Base.GetPos(transform.position);
        Explosion_RPC(v3);
    }

    virtual public void Explosion_RPC(Vector3 v3)
    {
        HandleExplosion(v3);
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

    [PunRPC]
    public void DestroyInstance_RPC(int viewID){
        PhotonView pv = PhotonView.Find(viewID);
        if (pv != null)
        {
            PhotonNetwork.Destroy(pv.gameObject);
        }        
    }

}

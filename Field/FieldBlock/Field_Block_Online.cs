using UnityEngine;
using Photon.Pun;

public class Field_Block_Online : Field_Block_Base {

    void Start()
    {
		objectPooler = GetComponent<ObjectPooler_Base>();
        cLibrary = GameObject.Find("Library").GetComponent<Library_Base>();
		CreateFixedBlock();
    }


	protected override void SetFieldRange(){
		GameManager.SetFieldRange(10,10);
	}
    protected override void ClearBrokenList_RPC(){
        photonView.RPC(nameof(ClearBrokenList), RpcTarget.All);
    }

    protected override void InsBrokenBlock_RPC(int x, int y, int z){
        photonView.RPC(nameof(InsBrokenBlock), RpcTarget.All, x, y, z);
    }


    protected override void InsObjMove_RPC(int x, int y, int z, Library_Base.Direction randomDirection){
		if(false == GetComponent<PhotonView>().IsMine){
			return;
		}
        photonView.RPC(nameof(InsObjMove), RpcTarget.All, x, y, z, randomDirection);
    }

    public override void Rainbow_RPC(string sMaterialType){
        Rainbow(sMaterialType);
        /*
        if (false == PhotonNetwork.IsMasterClient){
            return;
        }
        */
        //photonView.RPC(nameof(Rainbow), RpcTarget.All, sMaterialType);
        
    }
/*
    public override GameObject DequeueObect(string tag)
    {
        GameObject obj = null;
        while(true){
            obj = explosionManager.DequeueObject(tag);
            if(false == explosionManager.GetExplosionList(obj)){
                break;
            }
        }
        return obj;
    }

    public override void EnqueueObject(GameObject obj)
    {
		if(false == GetComponent<PhotonView>().IsMine){
			return;
		}
        Explosion_Base explosionBase = obj.GetComponent<Explosion_Base>();
        photonView.RPC(nameof(EnqueueObject_RPC), RpcTarget.All, explosionBase.GetID());
    }

    [PunRPC]
    public void EnqueueObject_RPC(int id){
        explosionManager.EnqueueObject(explosionManager.GetGameObject(id));
    }
*/
    public override GameObject DequeueObject(string tag)
    {
        return explosionManager.DequeueObject(tag);
    }

    public override void EnqueueObject(GameObject obj)
    {
		explosionManager.EnqueueObject(obj);
    }

    public override void UpdateGroundExplosion(GameObject gObj)
    {
        // エンキューすると位置情報が変わるので同期する位置情報は退避しておく
        Vector3 pos = gObj.transform.position;
        EnqueueObject(gObj);
        if (false == PhotonNetwork.IsMasterClient){
            return;
        }
        photonView.RPC(nameof(UpdateGroundExplosion_RPC), RpcTarget.All, gObj.name, pos);
    }

    [PunRPC]
    public void UpdateGroundExplosion_RPC(string matname, Vector3 v3)
    {
        explosionManager.UpdateGroundExplosion(matname, v3);
    }


}
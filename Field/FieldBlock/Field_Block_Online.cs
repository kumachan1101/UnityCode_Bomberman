using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class Field_Block_Online : Field_Block_Base {

    void Start()
    {
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
        //Rainbow(sMaterialType);
        if (false == PhotonNetwork.IsMasterClient){
            return;
        }
        NotifyExplosionData(sMaterialType);
        
    }
    public void NotifyExplosionData(string objName)
    {
        List<Vector3> positions = new List<Vector3>();

        // ExplosionList から座標を収集
        foreach (GameObject obj in explosionManager.GetExplosionList())
        {
            if (obj != null && objName != obj.name.Replace("(Clone)",""))
            {
                positions.Add(obj.transform.position);
            }
        }

        // RPCで座標リストと名称を送信
        photonView.RPC("UpdateExplosionObjects", RpcTarget.All, objName, positions.ToArray());
    }

    [PunRPC]
    public void UpdateExplosionObjects(string objName, Vector3[] positions)
    {
        // 新しいオブジェクトリストを構築
        List<GameObject> objectsToRemove = new List<GameObject>();
        List<GameObject> objectsToAdd = new List<GameObject>();

        foreach (Vector3 position in positions)
        {
            GameObject gobj_cur = Library_Base.GetGameObjectAtExactPositionWithName(position, "Explosion");
            if (gobj_cur == null)
            {
                Debug.Log("gobj_cur is null");
                GameObject gobj = DequeueObject(objName);
                Explosion_Base cExplosion = gobj.GetComponent<Explosion_Base>();
                cExplosion.SetPosition(position);
                objectsToAdd.Add(gobj);
            }
            else
            {
                gobj_cur.name = gobj_cur.name.Replace("(Clone)", "");
                if (gobj_cur.name != objName)
                {
                    objectsToRemove.Add(gobj_cur);
                    GameObject gobj = DequeueObject(objName);
                    Explosion_Base cExplosion = gobj.GetComponent<Explosion_Base>();
                    cExplosion.SetPosition(position);
                    objectsToAdd.Add(gobj);
                }
                else
                {
                    Debug.Log(gobj_cur.name + " == " + objName);
                }
            }
        }

        // ExplosionManager経由でリストを更新
        explosionManager.AddToExplosionList(objectsToAdd);
        explosionManager.RemoveFromExplosionList(objectsToRemove);

        // 削除対象オブジェクトをキューに戻す
        foreach (GameObject obj in objectsToRemove)
        {
            EnqueueObject(obj);
        }
    }

/*
    [PunRPC]
    public void UpdateExplosionObjects(string objName, Vector3[] positions)
    {
        //Debug.Log($"Received data for {objName}. Positions: {positions.Length}");

        // 新しいオブジェクトリストを構築
        List<GameObject> objectsToRemove = new List<GameObject>();
        List<GameObject> objectsToAdd = new List<GameObject>();

        foreach (Vector3 position in positions)
        {
            GameObject gobj_cur = Library_Base.GetGameObjectAtExactPositionWithName(position,"Explosion");
            if(null == gobj_cur){
                Debug.Log("gobj_cur is null");
                GameObject gobj = DequeueObject(objName);
                Explosion_Base cExplosion = gobj.GetComponent<Explosion_Base>();
                cExplosion.SetPosition(position);
                objectsToAdd.Add(gobj);
            }
            else{
                gobj_cur.name = gobj_cur.name.Replace("(Clone)","");
                if(gobj_cur.name != objName){
                    objectsToRemove.Add(gobj_cur);
                    GameObject gobj = DequeueObject(objName);
                    Explosion_Base cExplosion = gobj.GetComponent<Explosion_Base>();
                    cExplosion.SetPosition(position);
                    objectsToAdd.Add(gobj);
                }
                else{
                    Debug.Log(gobj_cur.name + " == " + objName);
                }
            }
        }

        // ExplosionListを更新
        explosionManager.ExplosionList.AddRange(objectsToAdd);
        foreach (GameObject obj in objectsToRemove)
        {
            explosionManager.ExplosionList.Remove(obj);
            EnqueueObject(obj);
        }
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
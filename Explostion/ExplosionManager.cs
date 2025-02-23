
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
public enum PoolerType
{
    Local,
    Tower
}

public static class ObjectPoolerFactory
{
    public static ObjectPooler_Base Create(GameObject obj, PoolerType type)
    {
        switch (type)
        {
            case PoolerType.Local:
                return obj.AddComponent<ObjectPooler_Local>();
            case PoolerType.Tower:
                return obj.AddComponent<ObjectPooler_Tower>();
            default:
                throw new System.ArgumentException("Invalid PoolerType");
        }
    }
}


public class ExplosionManager : MonoBehaviourPunCallbacks
{
    private ExplosionTracker explosionTracker;
    private ExplosionPoolManager explosionPoolManager;

    public void Initialize(PoolerType type)
    {
        ObjectPooler_Base pooler = ObjectPoolerFactory.Create(this.gameObject, type);
        explosionPoolManager = new ExplosionPoolManager(pooler);
        explosionTracker = new ExplosionTracker();
    }
    public virtual void Rainbow_RPC(string sMaterialType) { }
    public virtual void UpdateGroundExplosion(GameObject gObj){}
    public void UpdateGroundExplosion(string objName, Vector3 position)
    {
        GameObject gtemp = DequeueObject(objName.Replace("(Clone)", ""));
        gtemp.transform.position = position;

        if (explosionTracker.HandleExplosionUpdate(objName, gtemp, out GameObject delobj))
        {
            EnqueueObject(gtemp); // 名前と位置が一致した場合のみエンキュー
            return;
        }

        if (delobj != null)
        {
            EnqueueObject(delobj);
        }
    }

    public void Rainbow(string objname)
    {
        var explosionsToRemove = new List<GameObject>();
        var explosionsToAdd = new List<GameObject>();

        foreach (var obj in GetExplosionList().Where(o => o != null && o.name != objname))
        {
            GameObject gobj = DequeueObject(objname);
            if (null != gobj)
            {
                explosionsToAdd.Add(gobj);
                gobj.GetComponent<Explosion_Base>().SetPosition(obj.transform.position);
            }

            explosionsToRemove.Add(obj);
        }

        UpdateExplosionList(explosionsToAdd, explosionsToRemove);

        foreach (var obj in explosionsToRemove)
        {
            EnqueueObject(obj);
        }
    }

    private void UpdateExplosionList(List<GameObject> toAdd, List<GameObject> toRemove)
    {
        AddToExplosionList(toAdd);
        RemoveFromExplosionList(toRemove);
    }

    public GameObject DequeueObject(string tag)
    {
        return explosionPoolManager.DequeueObject(tag);
    }

    public void EnqueueObject(GameObject obj)
    {
        //string tag = GetExplosionType(obj.name);
        explosionPoolManager.EnqueueObject(tag, obj);
    }

    public string GetExplosionType(string input)
    {
        return explosionPoolManager.GetExplosionType(input);
    }
/*
    public bool IsMatch(Vector3 targetPosition, Material targetMaterial)
    {
        return explosionTracker.IsMatch(targetPosition, targetMaterial);
    }
*/
    public IEnumerable<GameObject> GetExplosionList()
    {
        return explosionTracker.GetExplosionList();
    }
    
    public void AddToExplosionList(List<GameObject> objectsToAdd)
    {
        explosionTracker.AddToExplosionList(objectsToAdd);
    }

    public void RemoveFromExplosionList(List<GameObject> objectsToRemove)
    {
        explosionTracker.RemoveFromExplosionList(objectsToRemove);
    }
}

public class ExplosionManager_CpuMode : ExplosionManager
{
    public override void Rainbow_RPC(string sMaterialType){
        Rainbow(sMaterialType);
    }
    public override void UpdateGroundExplosion(GameObject gObj)
    {
        Vector3 pos = gObj.transform.position;
        EnqueueObject(gObj);
        UpdateGroundExplosion(gObj.name, pos);
    }

}

public class ExplosionManager_Online : ExplosionManager
{
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
        foreach (GameObject obj in GetExplosionList())
        {
            if (obj != null && objName != obj.name.Replace("(Clone)",""))
            {
                positions.Add(obj.transform.position);
            }
        }

        // RPCで座標リストと名称を送信
        photonView.RPC(nameof(UpdateExplosionObjects), RpcTarget.All, objName, positions.ToArray());
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
        AddToExplosionList(objectsToAdd);
        RemoveFromExplosionList(objectsToRemove);

        // 削除対象オブジェクトをキューに戻す
        foreach (GameObject obj in objectsToRemove)
        {
            EnqueueObject(obj);
        }
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
        Debug.Log("UpdateGroundExplosion");
        UpdateGroundExplosion(matname, v3);
    }   
}

public class ExplosionTracker
{
    static public List<GameObject> ExplosionList { get; private set; } = new List<GameObject>();

    public bool HandleExplosionUpdate(string objName, GameObject gtemp, out GameObject delobj)
    {
        delobj = null;

        foreach (GameObject obj in ExplosionList)
        {
            if (obj != null && obj.transform.position == gtemp.transform.position)
            {
                if (obj.name != objName)
                {
                    delobj = obj;
                    RemoveFromExplosionList(delobj);
                    AddToExplosionList(gtemp);
                    return false; // そのまま続行
                }
                else
                {
                    return true; // 名前と位置が一致 → エンキュー対象
                }
            }
        }

        AddToExplosionList(gtemp);
        return false;
    }

    public void AddToExplosionList(GameObject obj)
    {
        if (obj != null && !ExplosionList.Contains(obj))
        {
            ExplosionList.Add(obj);
        }
    }

    public void AddToExplosionList(List<GameObject> objectsToAdd)
    {
        foreach (var obj in objectsToAdd)
        {
            AddToExplosionList(obj);
        }
    }

    public void RemoveFromExplosionList(GameObject obj)
    {
        if (obj != null && ExplosionList.Contains(obj))
        {
            ExplosionList.Remove(obj);
        }
    }

    public void RemoveFromExplosionList(List<GameObject> objectsToRemove)
    {
        foreach (var obj in objectsToRemove)
        {
            RemoveFromExplosionList(obj);
        }
    }

    static public bool IsMatch(Vector3 targetPosition, Material targetMaterial)
    {
        foreach (GameObject obj in ExplosionList)
        {
            if (obj != null && obj.transform.position == targetPosition)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null && renderer.material.name.Replace(" (Instance)", "") == targetMaterial.name)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return true;
    }

    public IEnumerable<GameObject> GetExplosionList()
    {
        return ExplosionList;
    }


}

public class ExplosionPoolManager
{
    private ObjectPooler_Base objectPooler;

    public ExplosionPoolManager(ObjectPooler_Base pooler)
    {
        objectPooler = pooler;
        SetupPools();
    }

    public void SetupPools()
    {
        ClearPools();
        ConfigurePools();
        InitializePool();
    }


    private void ClearPools()
    {
        objectPooler.pools.Clear();
    }

    private void ConfigurePools()
    {
        AddPool(ExplosionTypes.Explosion1, 1000);
        AddPool(ExplosionTypes.Explosion2, 1000);
        AddPool(ExplosionTypes.Explosion3, 1000);
        AddPool(ExplosionTypes.Explosion4, 1000);
    }

    private void InitializePool()
    {
        objectPooler.InitializePool();
    }

    public void AddPool(string tag, int size)
    {
        GameObject prefab = Resources.Load<GameObject>(tag);
        ObjectPooler_Base.Pool newPool = new ObjectPooler_Base.Pool { tag = tag, prefab = prefab, size = size };
        objectPooler.pools.Add(newPool);
    }

    public GameObject DequeueObject(string tag)
    {
        return objectPooler.DequeueObject(GetExplosionType(tag));
    }

    public void EnqueueObject(string tag, GameObject obj)
    {
        objectPooler.EnqueueObject(GetExplosionType(obj.name), obj);
    }

    public string GetExplosionType(string input)
    {
        if (input.Contains(ExplosionTypes.Explosion1)) return ExplosionTypes.Explosion1;
        if (input.Contains(ExplosionTypes.Explosion2)) return ExplosionTypes.Explosion2;
        if (input.Contains(ExplosionTypes.Explosion3)) return ExplosionTypes.Explosion3;
        if (input.Contains(ExplosionTypes.Explosion4)) return ExplosionTypes.Explosion4;
        return null;
    }
}

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

    public bool IsMatch(Vector3 targetPosition, Material targetMaterial)
    {
        return explosionTracker.IsMatch(targetPosition, targetMaterial);
    }

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

public class ExplosionTracker
{
    public List<GameObject> ExplosionList { get; private set; } = new List<GameObject>();

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

    public bool IsMatch(Vector3 targetPosition, Material targetMaterial)
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
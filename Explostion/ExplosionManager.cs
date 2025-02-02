/*

using System.Collections.Generic;
using UnityEngine;
public class ExplosionManager : MonoBehaviour
{
    public List<GameObject> ExplosionList = new List<GameObject>();

    private ObjectPooler_Base objectPooler;
    public void Initialize(ObjectPooler_Base pooler)
    {
        objectPooler = pooler;
    }

    public void SetupStage()
    {
        objectPooler.pools.Clear();
        ConfigurePools();
        objectPooler.InitializePool(); // プールを再生成
    }
    virtual protected void ConfigurePools()
    {
        AddPool(ExplosionTypes.Explosion1, 1000);
        AddPool(ExplosionTypes.Explosion2, 1000);
        AddPool(ExplosionTypes.Explosion3, 1000);
        AddPool(ExplosionTypes.Explosion4, 1000);
    }

    private void AddPool(string tag, int size)
    {
        GameObject prefab = Resources.Load<GameObject>(tag);
        ObjectPooler_Base.Pool newPool = new ObjectPooler_Base.Pool { tag = tag, prefab = prefab, size = size };
        objectPooler.pools.Add(newPool);
    }

    public void UpdateGroundExplosion(string objName, Vector3 position)
    {
        GameObject delobj = null;
        GameObject gtemp = DequeueObject(objName.Replace("(Clone)",""));
        gtemp.transform.position = position;
        foreach (GameObject obj in ExplosionList)
        {
            if (obj != null && obj.transform.position == position){
                if(obj.name != objName)
                {
                    delobj = obj; // 後で削除
                    EnqueueObject(obj);      // オブジェクトをキューに戻す
                    break;
                }
                else
                {
                    EnqueueObject(gtemp); // マテリアルが一致する場合は再利用
                    return;
                }
            }
        }
        if(null != delobj){
            ExplosionList.Remove(delobj);
        }
        ExplosionList.Add(gtemp); // 新しいオブジェクトを追加
    }

    public string GetExplosionType(string input)
    {
        if (input.Contains(ExplosionTypes.Explosion1))
        {
            return ExplosionTypes.Explosion1;
        }
        else if (input.Contains(ExplosionTypes.Explosion2))
        {
            return ExplosionTypes.Explosion2;
        }
        else if (input.Contains(ExplosionTypes.Explosion3))
        {
            return ExplosionTypes.Explosion3;
        }
        else if (input.Contains(ExplosionTypes.Explosion4))
        {
            return ExplosionTypes.Explosion4;
        }
        else
        {
            return null;
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
					return true; // 位置とマテリアルが一致する場合
				}
				else{
					return false;
				}
			}
		}
		return true;
	}


    public GameObject DequeueObject(string tag)
    {
        return objectPooler.DequeueObject(tag);
    }

    public void EnqueueObject(GameObject obj)
    {
        string tag = GetExplosionType(obj.name);
        objectPooler.EnqueueObject(tag, obj);
    }


    // ExplosionListにオブジェクトを追加するメソッド
    public void AddToExplosionList(List<GameObject> objectsToAdd)
    {
        ExplosionList.AddRange(objectsToAdd);
    }

    // ExplosionListからオブジェクトを削除するメソッド
    public void RemoveFromExplosionList(List<GameObject> objectsToRemove)
    {
        foreach (GameObject obj in objectsToRemove)
        {
            ExplosionList.Remove(obj);
        }
    }

    // ExplosionListを取得するメソッド（必要なら条件付きで許可）
    public IEnumerable<GameObject> GetExplosionList()
    {
        return ExplosionList;
    }


}

*/

using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
    private ExplosionTracker explosionTracker;
    private ExplosionPoolManager explosionPoolManager;

    public void Initialize(ObjectPooler_Base pooler)
    {
        explosionPoolManager = new ExplosionPoolManager();
        explosionPoolManager.Initialize(pooler);
        explosionTracker = new ExplosionTracker();
    }

    public void SetupStage()
    {
        explosionPoolManager.ClearPools();
        ConfigurePools();
        explosionPoolManager.InitializePool();
    }

    protected virtual void ConfigurePools()
    {
        explosionPoolManager.AddPool(ExplosionTypes.Explosion1, 1000);
        explosionPoolManager.AddPool(ExplosionTypes.Explosion2, 1000);
        explosionPoolManager.AddPool(ExplosionTypes.Explosion3, 1000);
        explosionPoolManager.AddPool(ExplosionTypes.Explosion4, 1000);
    }

    public void UpdateGroundExplosion(string objName, Vector3 position)
    {
        GameObject delobj = null;
        GameObject gtemp = DequeueObject(objName.Replace("(Clone)", ""));
        gtemp.transform.position = position;
        
        foreach (GameObject obj in explosionTracker.ExplosionList)
        {
            if (obj != null && obj.transform.position == position)
            {
                if (obj.name != objName)
                {
                    delobj = obj;
                    EnqueueObject(obj);
                    break;
                }
                else
                {
                    EnqueueObject(gtemp);
                    return;
                }
            }
        }
        
        if (delobj != null)
        {
            explosionTracker.RemoveFromExplosionList(delobj);
        }
        explosionTracker.AddToExplosionList(gtemp);
    }

    public GameObject DequeueObject(string tag)
    {
        return explosionPoolManager.DequeueObject(tag);
    }

    public void EnqueueObject(GameObject obj)
    {
        string tag = GetExplosionType(obj.name);
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

    public void Initialize(ObjectPooler_Base pooler)
    {
        objectPooler = pooler;
    }

    public void ClearPools()
    {
        objectPooler.pools.Clear();
    }

    public void InitializePool()
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
        return objectPooler.DequeueObject(tag);
    }

    public void EnqueueObject(string tag, GameObject obj)
    {
        objectPooler.EnqueueObject(tag, obj);
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
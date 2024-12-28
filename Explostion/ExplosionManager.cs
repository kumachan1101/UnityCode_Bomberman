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

    public bool GetExplosionList(GameObject obj)
    {
        // ExplosionListに指定したobjが含まれているか確認
        foreach (GameObject listObj in ExplosionList)
        {
            if (listObj == obj)
            {
                return true; // 見つかった場合はtrueを返す
            }
        }
        return false; // 見つからなかった場合はfalseを返す
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

    public void AddExplosionObjects(IEnumerable<GameObject> objectsToAdd)
    {
        foreach (var obj in objectsToAdd)
        {
            if (obj != null && !ExplosionList.Contains(obj))
            {
                ExplosionList.Add(obj);
            }
        }
    }
    public void RemoveExplosionObjects(IEnumerable<GameObject> objectsToRemove)
    {
        foreach (var obj in objectsToRemove)
        {
            if (obj != null && ExplosionList.Contains(obj))
            {
                ExplosionList.Remove(obj);
            }
        }
    }

}

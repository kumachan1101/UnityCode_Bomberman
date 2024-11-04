using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Collections;

public class ExplosionManager : MonoBehaviour
{
    public List<GameObject> ExplosionList = new List<GameObject>();

    private ObjectPooler_Base objectPooler;
    private Library_Base cLibrary;
    private object lockObject = new object();

    public void Initialize(ObjectPooler_Base pooler, Library_Base library)
    {
        objectPooler = pooler;
        cLibrary = library;
    }


    public void AddPool(string tag, int size)
    {
        GameObject prefab = Resources.Load<GameObject>(tag);
        ObjectPooler_Base.Pool newPool = new ObjectPooler_Base.Pool { tag = tag, prefab = prefab, size = size };
        objectPooler.pools.Add(newPool);
    }


    public void UpdateGroundExplosion(GameObject gObj)
    {
        lock (lockObject)
        {
            Material cMaterial = gObj.GetComponent<Renderer>().material;
            GameObject objToRemove = null;

            foreach (GameObject obj in ExplosionList) // Assuming ExplosionList is public
            {
                if (obj != null && obj.transform.position == gObj.transform.position)
                {
                    Renderer renderer = obj.GetComponent<Renderer>();
                    if (renderer.material.name == cMaterial.name)
                    {
                        EnqueueObject(gObj); // Assuming Field_Block_Base is also a singleton
                        return;
                    }
                    else
                    {
                        objToRemove = obj;
                        break;
                    }
                }
            }

            if (objToRemove != null)
            {
                ExplosionList.Remove(objToRemove);
                EnqueueObject(objToRemove);
            }

            ExplosionList.Add(gObj);
        }
    }

    public void DelExplosion(Vector3 v3)
    {
        GameObject delobj = null;
        foreach (GameObject obj in ExplosionList)
        {
            if (obj != null && obj.transform.position == v3)
            {
                delobj = obj;
                break;
            }
        }
        if (delobj != null)
        {
            ExplosionList.Remove(delobj);
            Destroy(delobj);
        }
    }



    public void AddExplosion(GameObject g)
    {
        ExplosionList.Add(g);
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
}

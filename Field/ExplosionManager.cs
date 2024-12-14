using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
public class ExplosionManager : MonoBehaviour
{
    public List<GameObject> ExplosionList = new List<GameObject>();
    public List<GameObject> ExplosionList_temp = new List<GameObject>();

    public GameObject gtemp;

    public List<GameObject> ExplosionIDList = new List<GameObject>();

    private ObjectPooler_Base objectPooler;
    private Library_Base cLibrary;
    private object lockObject = new object();
    MaterialManager cMaterialManager;
    public void Initialize(ObjectPooler_Base pooler, Library_Base library)
    {
        objectPooler = pooler;
        cLibrary = library;
        cMaterialManager = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();        
    }

    public GameObject GetGameObject(int iID)
    {
        // リストをループしてIDをチェック
        foreach (GameObject explosion in ExplosionIDList)
        {
            if (explosion != null)
            {
                Explosion_Base explosionBase = explosion.GetComponent<Explosion_Base>();
                if (explosionBase != null && explosionBase.GetID() == iID)
                {
                    return explosion; // 一致するGameObjectを返す
                }
            }
        }
        Debug.Log("id:" + iID);
        return null; // 一致するIDが見つからなかった場合
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
                    string rendererMaterialName = renderer.material.name.Replace("(Instance)", "");
                    string newMaterialName = cMaterial.name.Replace("(Instance)", "");

                    if (rendererMaterialName == newMaterialName)
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

}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPooler_Base : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    [SerializeField] public List<Pool> pools;
    protected Queue<GameObject> objectPoolQueue;
    protected Dictionary<string, GameObject> prefabByTag;

	protected MaterialManager materialManager;


    protected virtual void Start()
    {
		materialManager = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
        //InitializePool();
    }
/*
    protected abstract GameObject CreateObject(GameObject prefab);

	public abstract void InitializePool();

    public abstract GameObject DequeueObject(string tag);
	public abstract void EnqueueObject(string tag, GameObject obj);

	public abstract void SetObjectActive_RPC(GameObject gObj, bool isActive);
*/
    protected GameObject CreateObject(GameObject prefab)
    {
        // ローカルでオブジェクトをインスタンス化
        return Instantiate(prefab, new Vector3(0, -2, 0), Quaternion.identity);
    }

    public void SetObjectActive_RPC(GameObject gObj, bool isActive)
    {
        gObj.SetActive(isActive);
    }

    public void InitializePool()
    {
        objectPoolQueue = new Queue<GameObject>();
        prefabByTag = new Dictionary<string, GameObject>();

        foreach (Pool pool in pools)
        {
            prefabByTag[pool.tag] = pool.prefab;

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = CreateObject(pool.prefab);
				SetObjectActive_RPC(obj, false);
                objectPoolQueue.Enqueue(obj);
            }
        }
    }
    public void EnqueueObject(string tag, GameObject obj)
    {
		//Debug.Log(tag);
        if (!prefabByTag.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag \"" + tag + "\" doesn't exist.");
            return;
        }

        if (obj != null && obj.activeInHierarchy)
        {
            obj.transform.position = new Vector3(0, -2, 0); // オブジェクトの位置を変更
            SetObjectActive_RPC(obj, false);
            objectPoolQueue.Enqueue(obj); // キューにエンキュー
            //Debug.Log("Enqueue: " + objectPoolQueue.Count);
        }
        else
        {
            Debug.LogWarning("Trying to enqueue a null or inactive object.");
        }
    }

    public GameObject DequeueObject(string tag)
    {
        foreach (var obj in objectPoolQueue)
        {
            if (!obj.activeInHierarchy && prefabByTag.ContainsKey(tag))
            {
                objectPoolQueue.Dequeue();
				SetObjectActive_RPC(obj, true);

                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material newMaterial = materialManager.GetMaterialOfTypeExplosion(tag); // タグに対応するマテリアルを取得する
                    renderer.material = newMaterial;
                }

                return obj;
            }
        }

        Debug.LogWarning("No available objects with tag \"" + tag + "\".");
        return null;
    }

    public Dictionary<string, int> PoolCounts
    {
        get
        {
            Dictionary<string, int> poolCounts = new Dictionary<string, int>();
            foreach (var obj in objectPoolQueue)
            {
                string tag = prefabByTag.FirstOrDefault(x => x.Value == obj).Key;
                if (!poolCounts.ContainsKey(tag))
                {
                    poolCounts[tag] = 0;
                }
                if (!obj.activeInHierarchy)
                {
                    poolCounts[tag]++;
                }
            }
            return poolCounts;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ObjectPooler_Base : MonoBehaviour
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

    protected abstract GameObject CreateObject(GameObject prefab);

	public abstract void InitializePool();

    public abstract GameObject DequeueObject(string tag);
	public abstract void EnqueueObject(string tag, GameObject obj);

	public abstract void SetObjectActive_RPC(GameObject gObj, bool isActive);

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

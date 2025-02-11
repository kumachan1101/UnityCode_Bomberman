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

    [SerializeField] public List<Pool> pools = new List<Pool>();
    protected Queue<GameObject> objectPoolQueue;
    protected Dictionary<string, GameObject> prefabByTag;
    protected Dictionary<string, Queue<GameObject>> objectPoolByTag;

    protected GameObject CreateObject(GameObject prefab)
    {
        // ローカルでオブジェクトをインスタンス化
        GameObject gobj = Instantiate(prefab, new Vector3(0, -2, 0), Quaternion.identity);
        AddExplostionComponent(gobj);
        return gobj;
    }
    virtual protected void AddExplostionComponent(GameObject instance){}

    public Dictionary<string, Queue<GameObject>> GetObjectPoolByTag()
    {
        return objectPoolByTag;
    }

    public void SetObjectActive_RPC(GameObject gObj, bool isActive)
    {
        gObj.SetActive(isActive);
    }

    public void InitializePool()
    {
        objectPoolByTag = new Dictionary<string, Queue<GameObject>>();
        prefabByTag = new Dictionary<string, GameObject>();

        foreach (Pool pool in pools)
        {
            prefabByTag[pool.tag] = pool.prefab;
            objectPoolByTag[pool.tag] = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = CreateObject(pool.prefab);
                //obj.GetComponent<Explosion_Base>().SetID(i);
                SetObjectActive_RPC(obj, false);
                objectPoolByTag[pool.tag].Enqueue(obj); // タグごとのキューに格納
            }
        }
    }

    // 指定されたタグと位置に一致するGameObjectを取得するメソッド
    public GameObject GetObjectByTagAndPosition(string tag, Vector3 position)
    {
        // タグが存在するか確認
        if (objectPoolByTag.ContainsKey(tag))
        {
            Queue<GameObject> poolQueue = objectPoolByTag[tag];

            // キューの中を検索
            foreach (GameObject obj in poolQueue)
            {
                if (obj.transform.position == position)
                {
                    return obj; // 一致するオブジェクトを返す
                }
            }
        }

        Debug.LogWarning($"No GameObject found with tag: {tag} at position: {position}");
        return null; // 見つからなかった場合はnullを返す
    }

    public void EnqueueObject(string tag, GameObject obj)
    {
        if (obj != null)
        {
            obj.transform.position = new Vector3(0, -2, 0); // オブジェクトの位置を変更
            SetObjectActive_RPC(obj, false);
            objectPoolByTag[tag].Enqueue(obj); // タグごとのキューにエンキュー
        }
        else
        {
            if (obj == null)
            {
                Debug.LogWarning("enqueue a null");
            }
            if (!obj.activeInHierarchy)
            {
                Debug.LogWarning("inactive object.");
            }
        }
        //Debug.Log(tag+":"+objectPoolByTag[tag].Count);
    }

    public GameObject DequeueObject(string tag)
    {
        if (objectPoolByTag.ContainsKey(tag) && objectPoolByTag[tag].Count > 0)
        {
            GameObject obj = objectPoolByTag[tag].Dequeue(); // タグごとのキューから取り出す

            SetObjectActive_RPC(obj, true); // オブジェクトをアクティブにする
            //Debug.Log(tag+":"+objectPoolByTag[tag].Count);
            return obj; // 条件に合うオブジェクトを返す
        }

        Debug.LogWarning("No available objects with tag \"" + tag + "\".");
        return null;
    }

}

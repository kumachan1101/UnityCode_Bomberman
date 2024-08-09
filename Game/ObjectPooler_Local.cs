using UnityEngine;
using System.Collections.Generic;
public class ObjectPooler_Local : ObjectPooler_Base
{

    protected override GameObject CreateObject(GameObject prefab)
    {
        // ローカルでオブジェクトをインスタンス化
        return Instantiate(prefab, new Vector3(0, 100, 0), Quaternion.identity);
    }

    public override void SetObjectActive_RPC(GameObject gObj, bool isActive)
    {
        gObj.SetActive(isActive);
    }

    public override void InitializePool()
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
    public override void EnqueueObject(string tag, GameObject obj)
    {
        if (!prefabByTag.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag \"" + tag + "\" doesn't exist.");
            return;
        }

        if (obj != null/* && obj.activeInHierarchy*/)
        {
            obj.transform.position = new Vector3(0, -2, 0); // オブジェクトの位置を変更
            SetObjectActive_RPC(obj, false);
            objectPoolQueue.Enqueue(obj); // キューにエンキュー
            Debug.Log("Enqueue: " + objectPoolQueue.Count);
        }
        else
        {
            Debug.LogWarning("Trying to enqueue a null or inactive object.");
        }
    }

    public override GameObject DequeueObject(string tag)
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
}

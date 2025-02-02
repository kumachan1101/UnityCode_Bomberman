using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
abstract public class InstanceManager_Base : MonoBehaviour
{
    protected GameObject prefab;
    protected string resource;
	protected Field_Block_Base cField;
    public GameObject gTemp;

    protected Queue<GameObject> instanceQueue = new Queue<GameObject>();
	void Start()
	{
		 cField = GameObject.Find("Field").GetComponent<Field_Block_Base>();
	}

    public GameObject InstantiateInstance(Vector3 position)
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab is not set.");
            return null;
        }
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        
        return instance;
    }

    virtual public void DestroyInstance(GameObject instance)
    {
        // オンライン生成した場合も、PhotonNetwork.Destroyではなく、以下Destroyで削除してしまっている。
        if(null != instance){
            var bomComponent = instance.GetComponent<Bom_Base>(); // BomComponentはカスタムコンポーネント
            bomComponent.bDel = true;
            Destroy(instance); // ローカルのみで削除
        }
    }

	public abstract void InstantiateInstancePool(Vector3 position);
    public abstract void DestroyInstancePool(GameObject instance);

    public void InstantiateInstancePool_Base(Vector3 position)
	{
		// Field_Block_Base 経由でデキューしてオブジェクトを取得
		GameObject explosion = cField.DequeueObject(prefab.name);

		if (explosion != null)
		{
			Explosion_Base cExplosion = explosion.GetComponent<Explosion_Base>();
			cExplosion.SetPosition(position);

			explosion.GetComponent<Explosion_Base>().ReqHide();
		}
		else
		{
			Debug.LogWarning("Explosion pool is empty or not found.");
		}
    }
    
    public virtual void SetPothonView(int viewid){}

    public void DestroyInstancePool_Base()
    {
        // キュー内のすべてのオブジェクトを処理
        while (instanceQueue.Count > 0)
        {
            GameObject gtemp = instanceQueue.Dequeue(); // キューから取り出し

            // キューに取り出したオブジェクトをエンキューする
            cField.EnqueueObject(gtemp);
        }
    }
	

    // プレハブをセットするメソッド
    public void SetPrefab(GameObject newPrefab)
    {
        prefab = newPrefab;
    }
}


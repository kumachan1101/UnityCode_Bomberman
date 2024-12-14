using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
abstract public class InstanceManager_Base : MonoBehaviour
{
    protected GameObject prefab;
    protected string resource;

    // 破棄予定のGameObjectを保持するリスト

    protected Queue<GameObject> instanceQueue = new Queue<GameObject>();

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
        /*
        PhotonView pv = instance.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine) {
            PhotonNetwork.Destroy(instance); // ネットワーク全体で削除
        } else {
            Destroy(instance); // ローカルのみで削除
        }
        */
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
    
    public GameObject gTemp;
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



	protected Field_Block_Base cField;
	
	void Start()
	{
		 cField = GameObject.Find("Field").GetComponent<Field_Block_Base>();
	}

    // 共通の位置設定メソッド
    protected void SetObjectPosition(GameObject instance, Vector3 position)
    {
        if (instance != null)
        {
            instance.transform.position = position;
        }
    }

    // プレハブをセットするメソッド
    public void SetPrefab(GameObject newPrefab)
    {
        prefab = newPrefab;
    }

    // リソース名をセットするメソッド
    public void SetResource(string newResource)
    {
        resource = newResource;
    }

    // 他の共通メソッドやロジックがあればここに追加
}


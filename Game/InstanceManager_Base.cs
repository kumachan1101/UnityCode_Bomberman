using UnityEngine;

public abstract class InstanceManager_Base : MonoBehaviour
{
    protected GameObject prefab;
    protected string resource;

    // 抽象メソッド：生成および破棄方法を定義
	public abstract GameObject InstantiateInstance(Vector3 position);
	public abstract void DestroyInstance(GameObject instance);


	public abstract GameObject InstantiateInstancePool(Vector3 position);

    public void DestroyInstancePool(GameObject instance)
    {
		if (instance == null)
		{
			Debug.LogWarning("Instance is null, cannot enqueue.");
			return;
		}
		cField.EnqueueObject(instance);
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


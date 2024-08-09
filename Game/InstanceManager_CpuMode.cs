using UnityEngine;

public class InstanceManager_CpuMode : InstanceManager_Base
{

    public override GameObject InstantiateInstancePool(Vector3 position)
	{
		// Field_Block_Base 経由でデキューしてオブジェクトを取得
		GameObject explosion = cField.DequeueObject(prefab.name);

		if (explosion != null)
		{
			// 取得したオブジェクトの位置を設定
			//explosion.transform.position = position;
			Explosion_Base cExplosion = explosion.GetComponent<Explosion_Base>();
			cExplosion.SetPosition_RPC(position);

			explosion.GetComponent<Explosion_Base>().ReqHide();
		}
		else
		{
			Debug.LogWarning("Explosion pool is empty or not found.");
		}
        return explosion;
    }


    public override GameObject InstantiateInstance(Vector3 position)
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab is not set.");
            return null;
        }
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        return instance;
    }

    public override void DestroyInstance(GameObject instance)
    {
        if (instance != null)
        {
            Destroy(instance);
        }
    }


}

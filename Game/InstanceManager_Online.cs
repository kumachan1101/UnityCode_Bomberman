using UnityEngine;
using Photon.Pun;

public class InstanceManager_Online : InstanceManager_Base
{

    public override GameObject InstantiateInstancePool(Vector3 position)
	{
		// Field_Block_Base 経由でデキューしてオブジェクトを取得
		GameObject explosion = cField.DequeueObject(prefab.name);

		if (explosion != null)
		{
			//Debug.Log("Calling SetPosition_RPC: " + position);
			Explosion_Base cExplosion = explosion.GetComponent<Explosion_Base>();
			cExplosion.SetPosition_RPC(position);
			//explosion.transform.position = position;

			cExplosion.ReqHide();
		}
		else
		{
			Debug.LogWarning("Explosion pool is empty or not found.");
		}
        return explosion;
    }

    public override GameObject InstantiateInstance(Vector3 position)
    {
        if (string.IsNullOrEmpty(prefab.name))
        {
            Debug.LogError("Resource is not set.");
            return null;
        }
		if(false == GetComponent<PhotonView>().IsMine){
			return null;
		}
        GameObject instance = PhotonNetwork.Instantiate(prefab.name, position, Quaternion.identity);
        return instance;
    }

	public override void DestroyInstance(GameObject instance)
	{
		if (instance != null)
		{
			PhotonView pv = instance.GetComponent<PhotonView>();
			if (pv != null && pv.IsMine)
			{
				PhotonNetwork.Destroy(pv.gameObject); // pvではなくpv.gameObjectを渡す
			}
		}
	}

}
using UnityEngine;
using Photon.Pun;

public class InstanceManager_Online : InstanceManager_Base
{
    public override GameObject InstantiateInstance(Vector3 position)
    {
        if (string.IsNullOrEmpty(resource))
        {
            Debug.LogError("Resource is not set.");
            return null;
        }
        GameObject instance = PhotonNetwork.Instantiate(resource, position, Quaternion.identity);
        return instance;
    }

    public override void DestroyInstance(GameObject instance)
    {
        if (instance != null)
        {
			if(false == instance.GetComponent<PhotonView>().IsMine){
				return;
			}

            PhotonView pv = instance.GetComponent<PhotonView>();
            if (pv != null)
            {
                PhotonNetwork.Destroy(pv);
            }
        }
    }
}
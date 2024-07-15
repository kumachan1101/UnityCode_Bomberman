using UnityEngine;
using Photon.Pun;

public class Explosion_Online : Explosion_Base
{
	protected override void DestroySync(GameObject g){
		PhotonView pv =g.GetComponent<PhotonView>();
        if (pv != null && pv.IsMine)
        {
            PhotonNetwork.Destroy(pv);
        }
        else
        {
            Debug.LogWarning("PhotonView not found or not owned by this player.");
        }
	}
	protected override bool IsSync(){
		return GetComponent<PhotonView>().IsMine;
	}

}

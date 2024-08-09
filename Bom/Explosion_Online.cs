using UnityEngine;
using Photon.Pun;

public class Explosion_Online : Explosion_Base
{
/*
	protected override void DestroySync(GameObject g) {
		PhotonView pv = g.GetComponent<PhotonView>();
		if (pv != null && pv.IsMine) {
			PhotonNetwork.Destroy(pv.gameObject); // pvではなくpv.gameObjectを渡す
		} else {
			Debug.LogWarning("PhotonView not found or not owned by this player.");
		}
	}
*/

	protected override bool IsSync(){
		return GetComponent<PhotonView>().IsMine;
	}


	public override void SetPosition_RPC(Vector3 position)
	{
		//Debug.Log("Calling SetPosition RPC with position: " + position);
		PhotonView photonView = GetComponent<PhotonView>();
		photonView.RPC("SetPosition", RpcTarget.All, position);
	}


}

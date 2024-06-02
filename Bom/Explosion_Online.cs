using UnityEngine;
using Photon.Pun;

public class Explosion_Online : Explosion_Base
{
	protected override void DestroySync(GameObject g){
		PhotonNetwork.Destroy(g.GetComponent<PhotonView>());
	}
	protected override bool IsSync(){
		return GetComponent<PhotonView>().IsMine;
	}

}

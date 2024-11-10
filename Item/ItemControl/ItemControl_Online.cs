using UnityEngine;
using Photon.Pun;
public class ItemControl_Online: ItemControl
{
    protected override void CreateItem_RPC(ABILITY eRand, Vector3 v3){
		if(false == GetComponent<PhotonView>().IsMine){
			return;
		}
        photonView.RPC(nameof(CreateItem), RpcTarget.All, eRand, v3);
    }
}

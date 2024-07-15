using System.Collections;
using System.Collections.Generic;
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


    [PunRPC]
    public void CreateItem(ABILITY eRand, Vector3 v3){
        GameObject gItem = Create(eRand);
        if(gItem != null){
            GameObject g = Instantiate(gItem);
            g.transform.position = v3;
        }
    }

}

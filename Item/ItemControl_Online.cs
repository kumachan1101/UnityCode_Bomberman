using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ItemControl_Online: ItemControl
{
    public override GameObject Create(ABILITY eItem){
        GameObject gItem = null;
        switch(eItem){
            case ABILITY.ABILITY_FIRE_UP:
                gItem = ItemFirePrefab;
                break;
            case ABILITY.ABILITY_BOM_UP:
                gItem= ItemBomPrefab;
                break;
            case ABILITY.ABILITY_BOM_EXPLODE:
                gItem= ItemBomExplodePrefab;
                break;
            case ABILITY.ABILITY_BOM_BIGBAN:
                gItem= ItemBomBigBanPrefab;
                break;
            case ABILITY.ABILITY_SPEED_UP:
                gItem= ItemSpeedPrefab;
                break;
            case ABILITY.ABILITY_BOM_KICK:
                gItem= ItemBomkickPrefab;
                break;
            case ABILITY.ABILITY_BOM_ATTACK:
                gItem= ItemBomAttackPrefab;
                break;
            case ABILITY.ABILITY_RAINBOW:
                gItem= ItemRainbowPrefab;
                break;
            case ABILITY.ABILITY_HEART:
                gItem= ItemHeartPrefab;
                break;
            case ABILITY.ABILITY_ADD_BLOCK:
                gItem= ItemAddBlockPrefab;
                break;
            case ABILITY.ABILITY_ADD_DUMMY:
                gItem= ItemAddDummyPrefab;
                break;

            default:
                gItem = null;
                break;
        }
        return gItem;
    }


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

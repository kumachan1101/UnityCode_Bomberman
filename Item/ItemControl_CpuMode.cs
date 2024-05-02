using UnityEngine;

public class ItemControl_CpuMode: ItemControl
{
    protected override void CreateItem_RPC(ABILITY eRand, Vector3 v3){
        CreateItem(eRand, v3);
    }

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
                int iRand = Random.Range(0, 5);
                if(0 == iRand){
                    gItem= ItemBomBigBanPrefab;
                }
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

}

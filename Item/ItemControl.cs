using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ItemControl: MonoBehaviourPunCallbacks
{
    public GameObject ItemFirePrefab;
    public GameObject ItemBomPrefab;
    public GameObject ItemBomExplodePrefab;
    public GameObject ItemBomBigBanPrefab;

    public GameObject ItemSpeedPrefab;

    public GameObject ItemBomkickPrefab;
    public GameObject ItemBomAttackPrefab;

    public GameObject ItemWallPrefab;
    public GameObject ItemRainbowPrefab;
    public GameObject ItemHeartPrefab;

    public GameObject ItemAddBlockPrefab;
    public GameObject ItemAddDummyPrefab;


    public List<GameObject> ItemList = new List<GameObject>();

    private bool bMaster = false;
    void Awake(){
        ItemFirePrefab = Resources.Load<GameObject>("item_fire");
        ItemBomPrefab = Resources.Load<GameObject>("item_bom");
        ItemBomExplodePrefab = Resources.Load<GameObject>("item_explode");
        ItemBomBigBanPrefab = Resources.Load<GameObject>("item_bigban");
        ItemSpeedPrefab = Resources.Load<GameObject>("item_speedup");
        ItemBomkickPrefab = Resources.Load<GameObject>("item_bomkick");
        ItemBomAttackPrefab = Resources.Load<GameObject>("item_bomattack");
        ItemWallPrefab = Resources.Load<GameObject>("item_wall");
        ItemRainbowPrefab = Resources.Load<GameObject>("item_rainbow");
        ItemHeartPrefab = Resources.Load<GameObject>("item_heart");
        ItemAddBlockPrefab = Resources.Load<GameObject>("item_addblock");
        ItemAddDummyPrefab = Resources.Load<GameObject>("item_adddummy");        
    }

    public void SetMaster(){
        bMaster = true;
    }

    public enum ABILITY {
        ABILITY_NOTHING,//能力なし
        /* 良い能力 */
        ABILITY_FIRE_UP,
        ABILITY_BOM_UP,

        ABILITY_BOM_EXPLODE,
        ABILITY_BOM_BIGBAN,
        ABILITY_SPEED_UP,
        ABILITY_BOM_KICK,
        ABILITY_BOM_ATTACK,
        ABILITY_WALL,
        ABILITY_RAINBOW,
        ABILITY_HEART,
        ABILITY_ADD_BLOCK,
        ABILITY_ADD_DUMMY,

        ABILITY_MAX,
        ABILITY_RANGE = 40
    }

    public virtual GameObject Create(ABILITY eItem){
        return null;
    }

    protected virtual void CreateItem_RPC(ABILITY eRand, Vector3 v3){
    }

    public int GetItemListNum(){
        int iLen = 0;
        foreach (GameObject gItem in ItemList) {
            if(null != gItem){
                iLen++;
            }
        }
        return iLen;

    }

    public bool IsItem(Vector3 v3){
        foreach (GameObject gItem in ItemList) {
            if(null != gItem){
                if(gItem.transform.position == v3){
                    return true;
                }
            }
        }
        return false;
    }

    public void CreateRandItem(Vector3 v3){
        if(false == bMaster){
            return;
        }
        int iRand = Random.Range((int)ABILITY.ABILITY_FIRE_UP, (int)ABILITY.ABILITY_RANGE);
        GameObject gItem = Create((ABILITY)iRand);
        if(gItem != null){
            CreateItem_RPC((ABILITY)iRand, v3);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.IO.LowLevel.Unsafe;
abstract public class ItemControl: MonoBehaviourPunCallbacks
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
    [System.Serializable]
    public class CreateItem
    {
        public string itemName;
        public GameObject itemPrefab;
    }
    protected List<CreateItem> itemList = new List<CreateItem>();
    // アイテムを生成する確率 (例えば、5分の1)
    public float itemSpawnProbability = 0.2f;  // 20%の確率

    void Awake(){
        CreateItem_AddList();
        /*
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
        */
    }

    abstract protected void CreateItem_AddList();
/*
    public enum ABILITY {
        ABILITY_NOTHING,//能力なし
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
        //ABILITY_RANGE = 40
        ABILITY_RANGE = ABILITY_MAX
    }
*/
    abstract public void CreateItem_RPC(Vector3 v3);

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

    // アイテムをランダムに生成する関数
    
    [PunRPC]
    public void CreateRandomItem(Vector3 position, int randomIndex)
    {
        CreateItem selectedItem = itemList[randomIndex];
        GameObject itemInstance = Instantiate(selectedItem.itemPrefab, position, Quaternion.identity);
    }

    abstract protected bool IsCreateItem();

}


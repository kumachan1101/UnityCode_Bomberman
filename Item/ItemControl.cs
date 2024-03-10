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

    public GameObject ItemWallPrefab;

    public List<GameObject> ItemList = new List<GameObject>();

    private bool bMaster = false;
    void Awake(){
        
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
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
        ABILITY_WALL,
        ABILITY_MAX,
        ABILITY_RANGE = 10
    }

    public GameObject Create(ABILITY eItem){
        GameObject gItem;
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
            /*
            case ABILITY.ABILITY_WALL:
                gItem= ItemWallPrefab;
                break;
            */
            default:
                gItem = null;
                break;
        }
        return gItem;
    }

/*
    private bool IsVaild(ABILITY eItem){

    }
*/
    private void OnTriggerEnter(Collider col) {
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
        int iRand = Random.Range((int)ABILITY.ABILITY_FIRE_UP, (int)ABILITY.ABILITY_MAX);
        GameObject gItem = Create((ABILITY)iRand);
        if(gItem != null){
            CreateItem_RPC((ABILITY)iRand, v3);
        }
    }

    protected virtual void CreateItem_RPC(ABILITY eRand, Vector3 v3){
        photonView.RPC(nameof(CreateItem), RpcTarget.All, eRand, v3);
    }


    [PunRPC]
    public void CreateItem(ABILITY eRand, Vector3 v3){
        GameObject gItem = Create(eRand);
        GameObject g = Instantiate(gItem);
        g.transform.position = v3;
    }

}

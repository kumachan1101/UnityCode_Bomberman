using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using PlayerBomName;
using BomKind;
using BomPosName;
using BomName;

public class BomControl : MonoBehaviourPunCallbacks
{
    public GameObject BomPrefab;
    public GameObject BomExplodePrefab;
    public GameObject BomBigBanPrefab;
    protected GameObject tempBom;
    protected GameObject gItemControl;
    protected ItemControl cItemControl;
    protected GameObject gField;
    protected Field cField;
    private SoundManager soundManager;

    protected Library cLibrary;

    public virtual void Awake(){
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        cLibrary = GameObject.Find("Library").GetComponent<Library>();
    }
    
    void Start()
    {
        init();
    }


    protected void init(){
        BomPrefab =Resources.Load<GameObject>("Bom");
        BomExplodePrefab = Resources.Load<GameObject>("BomExplode");
        BomBigBanPrefab = Resources.Load<GameObject>("BomBigBan");
    }
    
 
    public void DropBom(ref PlayerBomName.PlayerBom cPlayerBom, Vector3 v3, int iViewID,  Vector3 direction){
        if(null != cItemControl){
            if(cItemControl.IsItem(v3)){
                return;
            }
        }
        if(null != cLibrary){
            if(cLibrary.CheckPositionAndName(v3, "Explosion(Clone)")){
                return;
            }
        }
        
        BomKind.BOM_KIND eBomKind = cPlayerBom.GetBomKind();
        int iExplosionNum = cPlayerBom.GetExplosionNum();
        bool bBomKick = cPlayerBom.GetBomKick();
        string sMaterialType = cPlayerBom.GetMaterialType();
        bool bBomAttack = cPlayerBom.GetBomAttack();;
        MakeBom_RPC(v3, eBomKind,iViewID, iExplosionNum,  bBomKick, sMaterialType, bBomAttack, direction);
        cPlayerBom.AddBom(tempBom);

        soundManager.PlaySoundEffect("DROPBOMB");

    }

    protected virtual void MakeBom_RPC(Vector3 v3, BomKind.BOM_KIND eBomKind, int iViewID, int iExplosionNum, bool bBomKick, string sMaterialType, bool bBomAttack, Vector3 direction){
    }

    public void CancelInvokeAndCallExplosion(){
        if(null != tempBom){
            tempBom.GetComponent<Bom>().CancelInvokeAndCallExplosion();
        }
        
    }

}

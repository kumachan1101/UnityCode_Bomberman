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

    //private int iBomNum;
    //private int iExplosionNum;
    private BomKind.BOM_KIND eBomKind;

    // Start is called before the first frame update
    protected GameObject gItemControl;
    protected ItemControl cItemControl;

    protected GameObject gField;
    protected Field cField;

    private SoundManager soundManager;


/*
    public void SetBomSetting(string  MaterialType){
        sMaterialType = MaterialType;
        //MaterialManager cMaterialMng = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
        //Material newMaterial = cMaterialMng.GetMaterialOfType(MaterialType);
        //BomPrefab.GetComponent<Renderer>().material = newMaterial;
        //BomPrefab.GetComponent<Bom>().ExplosionPrefab.GetComponent<Renderer>().material = newMaterial;
    }
*/
    public virtual void Awake(){
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }
    
    void Start()
    {
        init();

      

        //iBomNum = 3;
        //iExplosionNum = 3;
    }


    protected void init(){
        BomPrefab =Resources.Load<GameObject>("Bom");
        BomExplodePrefab = Resources.Load<GameObject>("BomExplode");
        BomBigBanPrefab = Resources.Load<GameObject>("BomBigBan");
    }
    
    // Update is called once per frame
    void Update()
    {
        if(null == gField){
            gField = GameObject.Find("Field");
            if(null != gField){
                cField = gField.GetComponent<Field>();
            }
        }
        if(null == gItemControl){
            gItemControl = GameObject.Find("ItemControl(Clone)");
            if(null != gItemControl){
                cItemControl = gItemControl.GetComponent<ItemControl>();
            }
        }
    }

    public void DropBom(ref PlayerBomName.PlayerBom cPlayerBom, Vector3 v3, int iViewID,  Vector3 direction){
        //Debug.Log("DropBom");
        /*
        if(iBomNum <= GetBomNum()){
            return;
        }
        if(IsBom(v3)){
            return;
        }
        */
        if(null != cItemControl){
            if(cItemControl.IsItem(v3)){
                return;
            }
        }

        if(null != cField){
            if(cField.CheckPositionAndName(v3, "Explosion(Clone)")){
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
        photonView.RPC(nameof(MakeBom), RpcTarget.All, v3, eBomKind,iViewID, iExplosionNum,  bBomKick, sMaterialType, bBomAttack, direction);
    }

    [PunRPC]
    public void MakeBom(Vector3 v3, BomKind.BOM_KIND eBomKind, int iViewID, int iExplosionNum, bool bBomKick, string sMaterialType, bool bBomAttack, Vector3 direction){
        GameObject g;
        if(eBomKind == BomKind.BOM_KIND.BOM_KIND_BIGBAN){
            g = Instantiate(BomBigBanPrefab);    
        }
        else if(eBomKind == BomKind.BOM_KIND.BOM_KIND_EXPLODE){
            g = Instantiate(BomExplodePrefab);
        }
        else{
            g = Instantiate(BomPrefab);
        }

        MaterialManager cMaterialMng = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
        Material newMaterial = cMaterialMng.GetMaterialOfType(sMaterialType);
        g.GetComponent<Renderer>().material = newMaterial;
        g.GetComponent<Bom>().SetMaterialType(newMaterial);
        //g.GetComponent<Bom>().ExplosionPrefab.GetComponent<Renderer>().material = newMaterial;

        g.transform.position = v3;
        Bom cBom = g.GetComponent<Bom>();
        cBom.iExplosionNum = iExplosionNum;
        cBom.SetViewID(iViewID);
        if(bBomKick){
            cBom.AbailableBomKick();
        }
        if(bBomAttack){
            cBom.AbailableBomAttack(direction);
        }

        tempBom = g;
    }

    public void CancelInvokeAndCallExplosion(){
        if(null != tempBom){
            tempBom.GetComponent<Bom>().CancelInvokeAndCallExplosion();
        }
        
    }

}

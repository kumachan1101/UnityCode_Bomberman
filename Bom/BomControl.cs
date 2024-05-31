using UnityEngine;
using Photon.Pun;

public class BomControl : MonoBehaviourPunCallbacks
{
    public GameObject BomPrefab;
    protected GameObject tempBom;
    protected GameObject gItemControl;
    protected ItemControl cItemControl;
    protected GameObject gField;
    protected Field_Base cField;
    private SoundManager soundManager;

    protected Library cLibrary;

    public virtual void Awake(){
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        cLibrary = GameObject.Find("Library").GetComponent<Library>();
		ReadBomResource();
    }

	protected void ReadBomResource(){
		BomPrefab = Resources.Load<GameObject>("Bom");
/*
		DeleteComponent_Bom(BomPrefab);
		AddComponent_Bom(BomPrefab);
		DeleteComponent_Bom(BomExplodePrefab);
		AddComponent_BomExplode(BomExplodePrefab);
		DeleteComponent_Bom(BomBigBanPrefab);
		AddComponent_BomBigBan(BomBigBanPrefab);
*/
	}

	protected void DeleteComponent_Bom(GameObject gBom){
		Bom_Base cBom = gBom.AddComponent<Bom_Base>();
		Destroy(cBom);
	}

    protected virtual Bom_Base AddComponent_Bom(GameObject gBom){
        return null;
    }
    protected virtual Bom_Base AddComponent_BomExplode(GameObject gBom){
        return null;
    }
    protected virtual Bom_Base AddComponent_BomBigBan(GameObject gBom){
        return null;
    }

    void Update()
    {
        if(null == gField){
            gField = GameObject.Find("Field");
            if(null != gField){
                cField = gField.GetComponent<Field_Base>();
            }
        }
        if(null == gItemControl){
            gItemControl = GameObject.Find("ItemControl");
            if(null != gItemControl){
                cItemControl = gItemControl.GetComponent<ItemControl>();
            }
        }
    }
   
 
    public void DropBom(ref PlayerBomName.PlayerBom cPlayerBom, Vector3 v3, int iViewID,  Vector3 direction){
        if(null != cItemControl){
            if(cItemControl.IsItem(v3)){
                return;
            }
        }
        if(null != cLibrary){
            if(cLibrary.CheckPositionAndName(v3, "Explosion")){
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
            tempBom.GetComponent<Bom_Base>().CancelInvokeAndCallExplosion();
        }
        
    }

}

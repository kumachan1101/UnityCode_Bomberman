using UnityEngine;
using Photon.Pun;

public class BomControl : MonoBehaviourPunCallbacks
{
    public GameObject BomPrefab;
    protected GameObject tempBom;
    protected ItemControl cItemControl;
    private SoundManager soundManager;


    protected Bom_Base AddComponent_BomExplode(GameObject gBom){
        Bom_Base cBom = gBom.AddComponent<BomExplode_Base>();
        return cBom;
    }
    protected Bom_Base AddComponent_BomBigBan(GameObject gBom){
        Bom_Base cBom = gBom.AddComponent<BomBigBan_Base>();
        return cBom;
    }
    protected Bom_Base AddComponent_Bom(GameObject gBom){
        Bom_Base cBom = gBom.AddComponent<Bom_Base>();
        return cBom;
    }


    public virtual void Awake(){
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
		cItemControl = GameObject.Find("ItemControl").GetComponent<ItemControl>();

		ReadBomResource();
		CustomTypes.Register();
    }

	protected void ReadBomResource(){
		BomPrefab = Resources.Load<GameObject>("Bom");
	}

	protected void DeleteComponent_Bom(GameObject gBom){
		Bom_Base cBom = gBom.AddComponent<Bom_Base>();
		Destroy(cBom);
	}

    void Update()
    {
    }
   
 
    public void DropBom(ref PlayerBom cPlayerBom, Vector3 v3, Vector3 direction){
        if(null == cItemControl){
			return;
		}
		if(cItemControl.IsItem(v3)){
			return;
		}
		if(Library_Base.CheckPositionAndName(v3, "Explosion")){
			return;
		}
        
        BOM_KIND eBomKind = cPlayerBom.GetBomKind();
        int iExplosionNum = cPlayerBom.GetExplosionNum();
        bool bBomKick = cPlayerBom.CanKick();
        string sMaterialType = cPlayerBom.GetMaterialType();
        bool bBomAttack = cPlayerBom.CanAttack();;

		BomParameters bomParams = new BomParameters
		{
			position = v3,
			bomKind = eBomKind,
			viewID = 0,
			explosionNum = iExplosionNum,
			bomKick = bBomKick,
			materialType = sMaterialType,
			bomAttack = bBomAttack,
			direction = direction
		};

		MakeBom_RPC(bomParams);
        cPlayerBom.AddBom(tempBom);

        soundManager.PlaySoundEffect("DROPBOMB");

    }

	protected virtual void MakeBom_RPC(BomParameters bomParams){}

    public void CancelInvokeAndCallExplosion(){
        if(null != tempBom){
            tempBom.GetComponent<Bom_Base>().CancelInvokeAndCallExplosion();
        }
        
    }

}

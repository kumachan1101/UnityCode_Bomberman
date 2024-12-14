using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
public class BomControl : MonoBehaviourPunCallbacks
{
    public GameObject BomPrefab;
    protected GameObject tempBom;
    protected ItemControl cItemControl;
    private SoundManager soundManager;
    public List<GameObject> instanceList = new List<GameObject>();


    protected virtual Bom_Base AddComponent_BomExplode(GameObject gBom){
        return null;
    }
    protected virtual Bom_Base AddComponent_BomBigBan(GameObject gBom){
        return null;
    }
    protected virtual Bom_Base AddComponent_Bom(GameObject gBom){
        return null;
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
   
 
    public GameObject DropBom(BomParameters bomParams){
        if(null == cItemControl){
			return null;
		}
		if(cItemControl.IsItem(bomParams.position)){
			return null;
		}
		if(Library_Base.CheckPositionAndName(bomParams.position, "Explosion")){
			return null;
		}
		MakeBom_RPC(bomParams);
        soundManager.PlaySoundEffect("DROPBOMB");
        return tempBom;

    }

	protected virtual void MakeBom_RPC(BomParameters bomParams){}

    public void CancelInvokeAndCallExplosion(){
        if(null != tempBom){
            tempBom.GetComponent<Bom_Base>().CancelInvokeAndCallExplosion();
        }
        
    }

}

using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using System;

public interface IBomFactory
{
    Bom_Base CreateBom(GameObject gBom);
    Bom_Base CreateBomExplode(GameObject gBom);
    Bom_Base CreateBomBigBan(GameObject gBom);
}

public class BomControl : MonoBehaviourPunCallbacks
{
    public GameObject BomPrefab;
    protected GameObject tempBom;
    private SoundManager soundManager;
    protected IBomFactory bomFactory;

    protected void SetFactory(IBomFactory factory)
    {
        this.bomFactory = factory;
    }

    protected void AddBomComponents(GameObject gBom, BOM_KIND bomKind)
    {
        var bomActions = new Dictionary<BOM_KIND, Func<GameObject, Bom_Base>>
        {
            { BOM_KIND.BOM_KIND_BIGBAN, bomFactory.CreateBomBigBan },
            { BOM_KIND.BOM_KIND_EXPLODE, bomFactory.CreateBomExplode }
        };

        if (bomActions.TryGetValue(bomKind, out var func))
        {
            func(gBom);
        }
        else
        {
            bomFactory.CreateBom(gBom);
        }
    }


    protected virtual void InitFactory(){}
    public virtual void Awake(){
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
		ReadBomResource();
		CustomTypes.Register();
        InitFactory();
    }

	protected void ReadBomResource(){
		BomPrefab = Resources.Load<GameObject>("Bom");
	}

    public GameObject DropBom(BomParameters bomParams){
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

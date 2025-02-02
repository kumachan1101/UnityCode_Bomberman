using UnityEngine;
using Photon.Pun;

public class PowerGageIF_Tower : PowerGageIF_CpuMode
{

    protected override PowerGage CreatePowerGage(GameObject sliderObject)
    {
        return sliderObject.AddComponent<PowerGage_Tower>();
    }
	[PunRPC]
	public override void SyncSetDamage(int iDamage){
        if(cPowerGage == null){
			Debug.Log("cPowerGage is null");
			StartCoroutine(RetrySyncSetDamage(iDamage));
            return;
        }
		cPowerGage.SetDamage(iDamage);
		if(cPowerGage.IsDead()){
			GetComponent<Tower>().DestroySync();
			DestroySync();
		}
		//Debug.Log($"Received RPC 'SyncSetDamage' at ViewID: {photonView.ViewID} with Damage: {iDamage}");
		
	}


}



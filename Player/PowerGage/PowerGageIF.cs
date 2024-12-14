using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class PowerGageIF : MonoBehaviourPunCallbacks
{
	protected PowerGage cPowerGage;
	protected int iCanvasInsID;
	public void SetPowerGage(PowerGage cObj){
		cPowerGage = cObj;
	}

	public void SetCanvasInsID(int iID){
		//Debug.Log(iID);
		iCanvasInsID = iID;
	}

    public void SetDamage(int iDamage){
		//Debug.Log(cPowerGage);
        if(cPowerGage == null){
            return;
        }
        cPowerGage.SetDamage(iDamage);
		if(cPowerGage.IsDead()){
			GetComponent<Player_Base>().DestroySync();
		}
	}

	public void HeartUp(int iHeart){
        if(cPowerGage == null){
            return;
        }
		cPowerGage.HeartUp(iHeart);
	}

}



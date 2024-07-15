using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class PowerGage : MonoBehaviourPunCallbacks
{
	public Slider cSlider;

	public void SetDamage(int iDamage){
		SetDamage_RPC(iDamage);
	}

	protected virtual void SetDamage_RPC(int iDamage){
		if(false == GetComponent<PhotonView>().IsMine){
			return;
		}
		photonView.RPC(nameof(SyncSetDamage),RpcTarget.All, iDamage);
	}

	[PunRPC]
	public void SyncSetDamage(int iDamage){
		cSlider.value -= iDamage;
	}

	public void HeartUp(int iHeart){
		HeartUp_RPC(iHeart);
	}

	protected virtual void HeartUp_RPC(int iHeart){
		if(false == GetComponent<PhotonView>().IsMine){
			return;
		}

		photonView.RPC(nameof(SyncHeartUp),RpcTarget.All, iHeart);
	}

	[PunRPC]
	public void SyncHeartUp(int iHeart){
		cSlider.value += iHeart;
	}

	public void init(Color cColor, Vector3 v3){
		Debug.Log("init");
		cSlider.GetComponent<RectTransform>().position = v3;
		GameObject gFill = cSlider.transform.Find("Fill Area/Fill").gameObject;
		gFill.GetComponent<Image>().color = cColor;
	}

	public bool IsDead(){
		if(cSlider.value <= 0){
			return true;
		}
		return false;
	}
}



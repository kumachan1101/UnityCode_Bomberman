using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections;

public class PowerGageIF_Online : PowerGageIF
{
	void Start(){
		StartCoroutine(RetrySetupPowerGage());
	}

	private IEnumerator RetrySetupPowerGage(){
		int retries = 5; // 再試行回数
		float delay = 0.5f; // 再試行間隔
		while (retries > 0){
			SetupPowerGage();
			if (cPowerGage != null){
				yield break; // 成功したら終了
			}
			retries--;
			yield return new WaitForSeconds(delay);
		}
		Debug.LogError("Failed to setup cPowerGage after retries.");
	}

	void SetupPowerGage(){
		if(cPowerGage == null){
			PhotonView viewPlayer = PhotonView.Find(iCanvasInsID);
			if (viewPlayer == null)
			{
				Debug.LogError("ViewFind Error : "+iCanvasInsID);
				return;
			}

			gCanvas = viewPlayer.gameObject;
			if (gCanvas == null)
			{
				Debug.LogError("GameObject Error");
				return;
			}

			GameObject sliderObject = gCanvas.transform.Find("Slider").gameObject;
			cPowerGage = CreatePowerGage(sliderObject);
			//cPowerGage = sliderObject.AddComponent<PowerGage_Online>();
		}
	}

	protected override void SetDamage_RPC(int iDamage){
		if(false == GetComponent<PhotonView>().IsMine){
			return;
		}
		//Debug.Log($"Sending RPC 'SyncSetDamage' to ViewID: {photonView.ViewID} with Damage: {iDamage}");
		photonView.RPC("SyncSetDamage",RpcTarget.All, iDamage);
	}
	protected override void HeartUp_RPC(int iHeart){
		if(false == GetComponent<PhotonView>().IsMine){
			return;
		}
		if(false == cPowerGage.IsHeartUp()){
			return;
		}
		photonView.RPC("SyncHeartUp",RpcTarget.All, iHeart);
	}


}



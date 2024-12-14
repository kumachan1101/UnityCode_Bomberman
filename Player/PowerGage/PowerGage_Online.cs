using UnityEngine;
using Photon.Pun;
public class PowerGage_Online : PowerGage
{
	protected override void SetDamage_RPC(int iDamage){
		if(false == GetComponent<PhotonView>().IsMine){
			return;
		}
		if(false == IsPhotonView())	{
			return;
		}
		photonView.RPC(nameof(SyncSetDamage),RpcTarget.All, iDamage);
	}
	protected override void HeartUp_RPC(int iHeart){
		if(false == GetComponent<PhotonView>().IsMine){
			return;
		}

		if(cSlider.value == cSlider.maxValue){
			return;
		}
		if(false == IsPhotonView())	{
			return;
		}
		photonView.RPC(nameof(SyncHeartUp),RpcTarget.All, iHeart);
	}

	public bool IsPhotonView(){
        PhotonView pv = PhotonView.Find(photonView.ViewID);
        if (pv.gameObject != null)
        {
			return true;
        }        
		return false;
	}

	// 高負荷かどうかを判定する関数
	bool IsHighLoad()
	{
		int pingThreshold = 50; // 遅延200ms以上で高負荷と判断
		int currentPing = PhotonNetwork.GetPing();
		Debug.Log(currentPing);

		// 遅延が閾値を超えている場合、高負荷と判定
		return currentPing > pingThreshold;
	}

}


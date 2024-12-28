using Photon.Pun;
using UnityEngine;
public class ButtonClickScript_Online : ButtonClickScript
{
    // 他のクライアントがシーン遷移をリクエストする
    [PunRPC]
    public void LoadGameScene_RPC()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                PhotonNetwork.LoadLevel("GameTitle");
            }
            else
            {
                Debug.LogError("Cannot load level. Client is not connected or not in a room.");
            }
        }
    }

    // シーン遷移リクエストを送信する
    override public void LoadGameScene()
    {
        //Debug.Log("Sending scene change request to MasterClient...");
        PhotonView photonView = GetComponent<PhotonView>();
        photonView.RPC(nameof(LoadGameScene_RPC), RpcTarget.MasterClient);
    }
}
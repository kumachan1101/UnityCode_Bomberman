using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using PowerGageName;
public class Online : MonoBehaviourPunCallbacks
{
    private int playerCount;
    private Field_Base cField;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }


    public override void OnJoinedRoom()
    {
        playerCount = PhotonNetwork.PlayerList.Length; //ルームにいる人数を確認
        if (playerCount == 1)
        {
			Library.Instance.SetMaster();
        }
        GameObject gField = GameObject.Find("Field");
        cField = gField.GetComponent<Field_Base>();
        cField.CreateBrokenBlock();

        SpawnPlayerObjects(playerCount);
    }

    // 引数playerCountで受け取り、キャンバスとプレイヤーオブジェクトを生成する関数
    private void SpawnPlayerObjects(int playerCount)
    {
        string canvasName = "CanvasOnline";
        Vector3 v3PwrGage = new Vector3(0, 0, 0);

        GameObject gCanvas = PhotonNetwork.Instantiate(canvasName + playerCount, v3PwrGage, Quaternion.identity);
        Vector3 v3PlayerPos = cField.GetPlayerPosition(1, playerCount-1);
        GameObject gPlayer = PhotonNetwork.Instantiate("PlayerOnline" + playerCount, v3PlayerPos, Quaternion.identity);
        cField.SetName("PlayerOnline" + playerCount + "(Clone)");
    }


}
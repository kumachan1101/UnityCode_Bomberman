using Photon.Pun;
using UnityEngine;
public class Online : MonoBehaviourPunCallbacks
{
    private int playercnt;
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
		/*
        playercnt = PhotonNetwork.PlayerList.Length; //ルームにいる人数を確認
		GameObject gField = GameObject.Find("Field");
		Field_Player_Base cField = gField.GetComponent<Field_Player_Base>();
        cField.SpawnPlayerObjects(playercnt);
		*/
    }


}
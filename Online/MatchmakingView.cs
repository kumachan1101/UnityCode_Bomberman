using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MatchmakingView : MonoBehaviourPunCallbacks
{
    private RoomList roomList = new RoomList();
    private List<RoomButton> roomButtonList = new List<RoomButton>();
    private CanvasGroup canvasGroup;

    private void Start() {
        
        canvasGroup = GetComponent<CanvasGroup>();
        // ロビーに参加するまでは、全てのルーム参加ボタンを押せないようにする
        canvasGroup.interactable = false;

        // 全てのルーム参加ボタンを初期化する
        int roomId = 1;
        foreach (Transform child in transform) {
            if (child.TryGetComponent<RoomButton>(out var roomButton)) {
                roomButton.Init(this, roomId++);
                roomButton.SetPlayerCount(0);
                roomButtonList.Add(roomButton);
                
            }
        }
    }

    public override void OnJoinedLobby() {
        //Debug.Log("MatchmakingView:OnJoinedLobby");
        // ロビーに参加したら、ルーム参加ボタンを押せるようにする
        canvasGroup.interactable = true;
    }

    public override void OnRoomListUpdate(List<RoomInfo> changedRoomList) {
        //Debug.Log("OnRoomListUpdate");
        roomList.Update(changedRoomList);

        // 全てのルーム参加ボタンの表示を更新する
        foreach (var roomButton in roomButtonList) {
            if (roomList.TryGetRoomInfo(roomButton.RoomName, out var roomInfo)) {
                //Debug.Log(roomInfo.PlayerCount);
                roomButton.SetPlayerCount(roomInfo.PlayerCount);
            } else {
                roomButton.SetPlayerCount(0);
            }
        }
    }


    public void OnJoiningRoom() {
        // ルーム参加処理中は、全てのルーム参加ボタンを押せないようにする
        canvasGroup.interactable = false;
    }


    protected bool IsGameStart(){
        int playerCount = PhotonNetwork.PlayerList.Length; //ルームにいる人数を確認
        if(playerCount >= 2){
            return true;
        }
        return false;
    }


	private int myJoinOrder = -1; // 自分の入室順番を記録する変数
    public override void OnJoinedRoom()
    {
        myJoinOrder = GetMyJoinOrder();
        UpdateRoom();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        UpdateRoom();
    }

    private int GetMyJoinOrder()
    {
        int order = 0;
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if (player == PhotonNetwork.LocalPlayer)
            {
                break;
            }
            order++;
        }
        return order + 1; // 1-indexedとして返す
    }


	private void UpdateRoom() {
		// ルームへの参加が成功したら、UIを非表示にする
		int playerCount = PhotonNetwork.PlayerList.Length; // ルームにいる人数を確認
		string roomName = PhotonNetwork.CurrentRoom.Name;
		foreach (var roomButton in roomButtonList) {
			if (roomButton.RoomName == roomName) {
				roomButton.SetPlayerCount(playerCount);
				if (roomButton.GetIsMax(playerCount)) {
					GameObject gField = GameObject.Find("Field");
					Field_Block_Base cField = gField.GetComponent<Field_Block_Base>();
					cField.CreateField();
					cField.SetupStage();

					Field_Player_Base cFieldPlayer = gField.GetComponent<Field_Player_Base>();
					cFieldPlayer.SpawnPlayerObjects(myJoinOrder);
					cFieldPlayer.SetPlayerCnt(playerCount);

					gameObject.SetActive(false);
				}
			}
		}
	}

    public override void OnJoinRoomFailed(short returnCode, string message) {
        // ルームへの参加が失敗したら、再びルーム参加ボタンを押せるようにする
        canvasGroup.interactable = true;
    }
}
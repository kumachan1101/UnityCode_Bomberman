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

    //自分がルームに入室していない場合、ロビーが更新された
    public override void OnRoomListUpdate(List<RoomInfo> changedRoomList) {
        //Debug.Log("OnRoomListUpdate");
        roomList.Update(changedRoomList);
        foreach (var roomButton in roomButtonList) {
            if (roomList.TryGetRoomInfo(roomButton.RoomName, out var roomInfo)) {
                roomButton.SetPlayerCount(roomInfo.PlayerCount); // プレイヤー数を最新化
            } else {
                roomButton.SetPlayerCount(0); // 該当ルームが削除された場合
            }
        }
    }
    // 自分がルームに入室した
    public override void OnJoinedRoom()
    {
        UpdateRoom();
    }

    public void OnJoiningRoom() {
        // ルーム参加処理中は、全てのルーム参加ボタンを押せないようにする
        canvasGroup.interactable = false;
    }

    public override void OnJoinRoomFailed(short returnCode, string message) {
        // ルームへの参加が失敗したら、再びルーム参加ボタンを押せるようにする
        canvasGroup.interactable = true;
    }

    private void HandleRoomButtonUpdates(bool checkMaxPlayers)
    {
        int playerCount = PhotonNetwork.PlayerList.Length; // ルームにいる人数を確認
        string roomName = PhotonNetwork.CurrentRoom.Name;
        GameObject gField = GameObject.Find("Field");

        foreach (var roomButton in roomButtonList)
        {
            if (roomButton.RoomName == roomName)
            {
                // プレイヤー数の更新
                UpdatePlayerCount(roomButton, playerCount, gField);
                
                // 最大プレイヤー数のチェック処理が必要な場合のみ呼び出し
                if (checkMaxPlayers)
                {
                    CheckAndHandleMaxPlayers(playerCount, gField, roomButton);
                }
            }
        }
    }

    //自分が入室しているルームに、別のプレイヤーが入室した
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        HandleRoomButtonUpdates(true);
    }

    // 自分がルームから退出した
    public override void OnLeftRoom()
    {
        HandleRoomButtonUpdates(false);
    }

    // 他のプレイヤーがルームから退出した
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        HandleRoomButtonUpdates(false);
    }

    private void UpdateRoom() {
        int playerCount = PhotonNetwork.PlayerList.Length; // ルームにいる人数を確認
        string roomName = PhotonNetwork.CurrentRoom.Name;
        GameObject gField = GameObject.Find("Field");

        foreach (var roomButton in roomButtonList) {
            if (roomButton.RoomName == roomName) {
                HandleRoomButtonUpdate(roomButton, playerCount, gField);
            }
            else {
                roomButton.gameObject.SetActive(false);
            }
        }
    }

    // ルームボタンの更新処理を行う
    private void HandleRoomButtonUpdate(RoomButton roomButton, int playerCount, GameObject gField) {
        UpdatePlayerCount(roomButton, playerCount, gField);
        HandlePlayerObjects(playerCount, gField);
        CheckAndHandleMaxPlayers(playerCount, gField, roomButton);
    }

    // プレイヤー数を更新
    private void UpdatePlayerCount(RoomButton roomButton, int playerCount, GameObject gField) {
        roomButton.SetPlayerCount(playerCount);
        Field_Player_Base cFieldPlayer = gField.GetComponent<Field_Player_Base>();
        cFieldPlayer.SetPlayerCnt(playerCount);
    }

    // プレイヤーオブジェクトの処理
    private void HandlePlayerObjects(int playerCount, GameObject gField) {
        Field_Player_Base cFieldPlayer = gField.GetComponent<Field_Player_Base>();
        cFieldPlayer.SpawnPlayerObjects(playerCount);
    }

    // 最大プレイヤー数に達したかどうかを確認して、処理を行う
    private void CheckAndHandleMaxPlayers(int playerCount, GameObject gField, RoomButton roomButton) {
        if (roomButton.GetIsMax(playerCount)) {
            HandleMaxPlayers(gField);
        }
    }

    // 最大プレイヤー数に達した際の処理
    private void HandleMaxPlayers(GameObject gField) {
        Field_Block_Base cField = gField.GetComponent<Field_Block_Base>();
        GameObject gGameEndCanvasLocal = GameObject.Find("GameEndCanvas_Local(Clone)");
        Destroy(gGameEndCanvasLocal);

        if (PhotonNetwork.IsMasterClient) {
            cField.CreateField();
            GameObject gGameEndCanvas = PhotonNetwork.Instantiate("GameEndCanvas_Online", Vector3.zero, Quaternion.identity);
        }

        cField.SetupStage();
        gameObject.SetActive(false);
    }


}
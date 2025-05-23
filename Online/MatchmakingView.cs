using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class MatchmakingView : MonoBehaviourPunCallbacks
{
    //private List<RoomButton> roomButtonList = new List<RoomButton>();
    protected RoomListUI cRoomListUI;
    protected GameObject gField;

    private LobbyManager cLobbyManager;

	void Awake() {
		cRoomListUI = this.gameObject.AddComponent<RoomListUI>();
        //cLobbyManager = this.gameObject.AddComponent<LobbyManager>();
        gField = GameObject.Find("Field");
	}

	void Start() {
		gField.GetComponent<PlayerPositionManager>().SetPlayerPositions();
	}

	// 自分がルームに入室した
	public override void OnJoinedRoom()
    {
        UpdateRoom();
    }

    private void HandleRoomButtonUpdates(/*bool checkMaxPlayers*/)
    {
        int playerCount = PhotonNetwork.PlayerList.Length; // ルームにいる人数を確認
        string roomName = PhotonNetwork.CurrentRoom.Name;
        //GameObject gField = GameObject.Find("Field");
        //List<RoomButton> roomButtonList = cRoomListUI.GetRoomButtonList();
        RoomButton roomButton = cRoomListUI.GetMatchRoomButton(roomName);
        if(null != roomButton){
            UpdatePlayerCount(roomButton, playerCount);
            CheckAndHandleMaxPlayers(roomButton, playerCount);
        }
        /*
        foreach (var roomButton in roomButtonList)
        {
            if (roomButton.RoomName == roomName)
            {
                // プレイヤー数の更新
                UpdatePlayerCount(roomButton, playerCount);
                CheckAndHandleMaxPlayers(roomButton, playerCount);
                // 最大プレイヤー数のチェック処理が必要な場合のみ呼び出し
                if (checkMaxPlayers)
                {
                    CheckAndHandleMaxPlayers(roomButton, playerCount);
                }
            }
        }
        */
    }

    //自分が入室しているルームに、別のプレイヤーが入室した
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        HandleRoomButtonUpdates(/*true*/);
    }

    // 自分がルームから退出した
    public override void OnLeftRoom()
    {
        HandleRoomButtonUpdates(/*false*/);
    }

    // 他のプレイヤーがルームから退出した
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        HandleRoomButtonUpdates(/*false*/);
    }

    private void UpdateRoom() {
        int playerCount = PhotonNetwork.PlayerList.Length; // ルームにいる人数を確認
        string roomName = PhotonNetwork.CurrentRoom.Name;
        //gField.GetComponent<PlayerPositionManager>().SetPlayerPositions();
        //List<RoomButton> roomButtonList = cRoomListUI.GetRoomButtonList();
        RoomButton roomButton = cRoomListUI.GetMatchRoomButton(roomName);
        if(null != roomButton){
            HandleRoomButtonUpdate(roomButton, playerCount);
        }
        cRoomListUI.HideOtherRoomButtons(roomName);
        /*
        foreach (var roomButton in roomButtonList) {
            if (roomButton.RoomName == roomName) {
                HandleRoomButtonUpdate(roomButton, playerCount);
            }
            else {
                roomButton.gameObject.SetActive(false);
            }
        }
        */
    }

    // ルームボタンの更新処理を行う
    private void HandleRoomButtonUpdate(RoomButton roomButton, int playerCount) {
        UpdatePlayerCount(roomButton, playerCount);
        HandlePlayerObjects(playerCount);
        CheckAndHandleMaxPlayers(roomButton, playerCount);
    }
    protected virtual void HandleTowerObjects(int playerCount) {
    }

    // プレイヤー数を更新
    private void UpdatePlayerCount(RoomButton roomButton, int playerCount) {
        roomButton.SetPlayerCount(playerCount);
        PlayerCountManager cFieldPlayer = gField.GetComponent<PlayerCountManager>();
        cFieldPlayer.SetPlayerCount(playerCount);
    }

    // プレイヤーオブジェクトの処理
    private void HandlePlayerObjects(int playerCount) {
        PlayerSpawnManager cFieldPlayer = gField.GetComponent<PlayerSpawnManager>();
        cFieldPlayer.SpawnPlayer(playerCount);
    }

    // 最大プレイヤー数に達したかどうかを確認して、処理を行う
    protected void CheckAndHandleMaxPlayers(RoomButton roomButton, int playerCount) {
        if (roomButton.GetIsMax(playerCount)) {
            HandleMaxPlayers();
            HandleTowerObjects(playerCount);
        }
    }

    // 最大プレイヤー数に達した際の処理
    private void HandleMaxPlayers() {
        BlockCreateManager cField = gField.GetComponent<BlockCreateManager>();
        GameObject gGameEndCanvasLocal = GameObject.Find("GameEndCanvas_Local(Clone)");
        Destroy(gGameEndCanvasLocal);

        if (PhotonNetwork.IsMasterClient) {
            cField.CreateBrokenBlock();
            GameObject gGameEndCanvas = PhotonNetwork.Instantiate("GameEndCanvas_Online", Vector3.zero, Quaternion.identity);
        }

        //cField.SetupStage();
        cField.CompleteBlockCreate();
        gameObject.SetActive(false);
    }


}


public class RoomListUI : MonoBehaviourPunCallbacks
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
                roomButton.Init(roomId++,GetRoomName());
                roomButton.SetPlayerCount(0);
                roomButtonList.Add(roomButton);
            }
        }
    }

    protected virtual string GetRoomName(){
        return "Normal";
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
        canvasGroup.interactable = false;
    }
/*
    public List<RoomButton> GetRoomButtonList(){
        return roomButtonList;
    }

    public void OnJoiningRoom() {
        // ルーム参加処理中は、全てのルーム参加ボタンを押せないようにする
        canvasGroup.interactable = false;
    }
*/
    public override void OnJoinRoomFailed(short returnCode, string message) {
        // ルームへの参加が失敗したら、再びルーム参加ボタンを押せるようにする
        canvasGroup.interactable = true;
    }

    public RoomButton GetMatchRoomButton(string roomName){
        foreach (var roomButton in roomButtonList)
        {
            if (roomButton.RoomName == roomName)
            {
                return roomButton;
            }
        }
        return null;
    }


    public void HideOtherRoomButtons(string targetRoomName){
        foreach (var roomButton in roomButtonList){
            if (roomButton.RoomName != targetRoomName){
                roomButton.gameObject.SetActive(false);
            }
        }
    }
}
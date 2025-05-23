using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{
    [SerializeField]
    private int MaxPlayers = 0;

    [SerializeField]
    private TextMeshProUGUI label = default;

    private Button button;


    public string RoomName { get; private set; }

    private string lobbyName = "Default"; // ← 追加：ロビー名

    public void Init(int roomId, string lobbyName)
    {
        this.lobbyName = lobbyName;
        MaxPlayers = roomId + 1;

        RoomName = $"{lobbyName}_Room{MaxPlayers}P"; // ← ロビーごとにルーム名が変わる

        button = GetComponent<Button>();
        button.interactable = false;
        button.onClick.AddListener(OnButtonClick);
    }
/*
    public void Init(int roomId) {
        MaxPlayers = roomId+1;
        RoomName = $"Room{MaxPlayers}P";

        button = GetComponent<Button>();
        button.interactable = false;
        button.onClick.AddListener(OnButtonClick);

    }
*/
    private void OnButtonClick() {
        // ルーム参加処理中は、全ての参加ボタンを押せないようにする
        //matchmakingView.OnJoiningRoom();

        // ボタンに対応したルーム名のルームに参加する（ルームが存在しなければ作成してから参加する）
        var roomOptions = new RoomOptions();

        roomOptions.CleanupCacheOnLeave = true; // プレイヤー退出時にキャッシュを削除
        roomOptions.PlayerTtl = 0; // プレイヤー情報を即座に削除
        roomOptions.EmptyRoomTtl = 0; // ルームが空なら即座に削除
        PhotonNetwork.AutomaticallySyncScene = true;

        roomOptions.MaxPlayers = MaxPlayers;
        PhotonNetwork.JoinOrCreateRoom(RoomName, roomOptions, TypedLobby.Default);
    }

    public void SetPlayerCount(int playerCount) {
        label.text = $"{RoomName}\n{playerCount} / {MaxPlayers}";

        // ルームが満員でない時のみ、ルーム参加ボタンを押せるようにする
        button.interactable = (playerCount < MaxPlayers);
    }

    public bool GetIsMax(int playerCount){
        if(playerCount >= MaxPlayers){
            return true;
        }
        return false;
    }
}
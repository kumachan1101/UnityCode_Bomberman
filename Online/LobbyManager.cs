/*
using Photon.Realtime;
using ExitGames.Client.Photon;  // 追加
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks, IOnEventCallback  // IOnEventCallback を実装
{
    // ロビー状態を管理する辞書（ロビー番号 -> ロビーの状態）
    private Dictionary<int, bool> lobbyStates = new Dictionary<int, bool>();

    // ゲーム開始時にロビーの状態を取得
    public void Awake()
    {
        // ゲーム開始時にロビー状態を取得
        GetLobbyState(1);  // ロビー1の状態を取得
        GetLobbyState(2);  // ロビー2の状態を取得
    }

    // 引数で指定されたロビーの状態を取得
    public void GetLobbyState(int lobbyNumber)
    {
        if (lobbyStates.ContainsKey(lobbyNumber))
        {
            bool isConnected = lobbyStates[lobbyNumber];
            Debug.Log($"ロビー{lobbyNumber}の状態: {(isConnected ? "接続中" : "接続されていません")}");
        }
        else
        {
            Debug.Log($"ロビー{lobbyNumber}の情報はまだありません。");
        }
    }

    // ロビーの接続状態を設定
    public void SetLobbyState(int lobbyNumber, bool isConnected)
    {
        // ロビー状態を更新
        if (lobbyStates.ContainsKey(lobbyNumber))
        {
            lobbyStates[lobbyNumber] = isConnected;
        }
        else
        {
            lobbyStates.Add(lobbyNumber, isConnected);
        }

        // 更新後、他のプレイヤーに通知
        SendLobbyState(lobbyNumber, isConnected);
    }

    // ロビー接続状態を通知するためのイベント送信メソッド
    public void SendLobbyState(int lobbyNumber, bool isConnected)
    {
        object[] data = new object[] { lobbyNumber, isConnected };
        PhotonNetwork.RaiseEvent(100, data, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }

    // IOnEventCallback インターフェースの実装
    public void OnEvent(EventData photonEvent)
    {
        // イベントコードが100の場合、ロビー状態の更新
        if (photonEvent.Code == 100)
        {
            object[] data = (object[])photonEvent.CustomData;
            int lobbyNumber = (int)data[0];
            bool isConnected = (bool)data[1];

            // 状態更新
            SetLobbyState(lobbyNumber, isConnected);
        }
    }

    // ロビー参加時にイベントリスナーを追加
    public override void OnJoinedLobby()
    {
        PhotonNetwork.AddCallbackTarget(this);  // イベントリスナーを追加
        //Debug.Log("ロビーに参加しました");
    }

    // ロビー退出時にイベントリスナーを削除
    public override void OnLeftLobby()
    {
        PhotonNetwork.RemoveCallbackTarget(this);  // イベントリスナーを削除
        //Debug.Log("ロビーを退出しました");
    }
}

*/

using Photon.Realtime;
using ExitGames.Client.Photon;  // 追加
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks, IOnEventCallback  // IOnEventCallback を実装
{
    // ロビー状態を管理する辞書（ロビー番号 -> ロビーの状態）
    private Dictionary<int, bool> lobbyStates = new Dictionary<int, bool>();

    // ゲーム開始時にロビーの状態を取得
    public void Awake()
    {
        // ロビーに参加する前にイベントを送信しない
    }

    // ロビー参加時にイベントリスナーを追加
    public override void OnJoinedLobby()
    {
        PhotonNetwork.AddCallbackTarget(this);  // イベントリスナーを追加
    }

    // ルーム参加後にイベントを送信
    public override void OnJoinedRoom()
    {
        /*
        // ルームに参加した後に状態を送信
        SetLobbyState(1, true);  // ロビー1は接続中
        SetLobbyState(2, true);  // ロビー2は接続中

        // イベントを送信
        SendLobbyState(1, true); 
        SendLobbyState(2, true);
        */
    }

    // ロビー退出時にイベントリスナーを削除
    public override void OnLeftLobby()
    {
        PhotonNetwork.RemoveCallbackTarget(this);  // イベントリスナーを削除
    }

    // ロビー状態の初期化処理
    private void InitializeLobbyState()
    {
        // ロビー番号1と2の初期状態を取得（例えば接続しているかどうか）
        SetLobbyState(1, PhotonNetwork.IsConnected);
        SetLobbyState(2, PhotonNetwork.IsConnected);
    }

    // 引数で指定されたロビーの状態を取得
    public void GetLobbyState(int lobbyNumber)
    {
        if (lobbyStates.ContainsKey(lobbyNumber))
        {
            bool isConnected = lobbyStates[lobbyNumber];
            Debug.Log($"ロビー{lobbyNumber}の状態: {(isConnected ? "接続中" : "接続されていません")}");
        }
        else
        {
            Debug.Log($"ロビー{lobbyNumber}の情報はまだありません。");
        }
    }

    // ロビーの接続状態を設定
    public void SetLobbyState(int lobbyNumber, bool isConnected)
    {
        // ロビー状態を更新
        if (lobbyStates.ContainsKey(lobbyNumber))
        {
            lobbyStates[lobbyNumber] = isConnected;
        }
        else
        {
            lobbyStates.Add(lobbyNumber, isConnected);
        }

        // 更新後、他のプレイヤーに通知
        SendLobbyState(lobbyNumber, isConnected);

        // ロビーが接続された場合の処理（例えば、ゲーム開始フラグの設定など）
        if (isConnected)
        {
            Debug.Log($"ロビー{lobbyNumber}に接続しました");
        }
        else
        {
            Debug.Log($"ロビー{lobbyNumber}から切断されました");
        }
    }

    // ロビー接続状態を通知するためのイベント送信メソッド
    public void SendLobbyState(int lobbyNumber, bool isConnected)
    {
        // イベントコードを50に変更
        object[] data = new object[] { lobbyNumber, isConnected };

        // ルームに参加後にイベントを送信
        PhotonNetwork.RaiseEvent(50, data, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }

    // IOnEventCallback インターフェースの実装
    public void OnEvent(EventData photonEvent)
    {
        // イベントコードが50の場合、ロビー状態の更新
        if (photonEvent.Code == 50)
        {
            object[] data = (object[])photonEvent.CustomData;
            int lobbyNumber = (int)data[0];
            bool isConnected = (bool)data[1];

            // 状態更新
            SetLobbyState(lobbyNumber, isConnected);
        }
    }
}

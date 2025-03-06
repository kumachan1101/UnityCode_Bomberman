using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;

public class MasterClientNotificationManager : MonoBehaviour, IOnEventCallback
{
    public const byte NotificationEventCode = 1;
    private int receivedCount = 0; // 受信済みプレイヤー数


    // 通知をマスタークライアントに送信
    public void SendNotificationToMasterClient(string message)
    {
        if (PhotonNetwork.MasterClient == null){
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect(); // 切断処理を開始
            }
            return;
        }
        int masterClientActorNumber = PhotonNetwork.MasterClient.ActorNumber;
        object[] content = new object[] { message };

        RaiseEventOptions options = new RaiseEventOptions
        {
            TargetActors = new int[] { masterClientActorNumber }
        };

        PhotonNetwork.RaiseEvent(NotificationEventCode, content, options, SendOptions.SendReliable);

        Debug.Log("マスタークライアントに通知を送信: " + message);
    }

    // イベントを受信
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == NotificationEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            string message = (string)data[0];

            Debug.Log("通知を受信: " + message);

            if (PhotonNetwork.IsMasterClient)
            {
                HandleMasterClientNotification(message);
            }
        }
    }

    private void HandleMasterClientNotification(string message)
    {
        Debug.Log("マスタークライアントの通知処理: " + message);
        receivedCount++; // カウンターをインクリメント
        Debug.Log($"通知を受信したプレイヤー数: {receivedCount}");

        // ルーム内のプレイヤー数を取得
        int playerCount = PhotonNetwork.PlayerList.Length;

        if (receivedCount == playerCount)
        {
            Debug.Log("全プレイヤーが通知を受信しました！");
            receivedCount = 0;
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect(); // 切断処理を開始
            }
        }
        else
        {
            Debug.Log($"受信待ちのプレイヤー数: {playerCount - receivedCount}");
        }

    }
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}

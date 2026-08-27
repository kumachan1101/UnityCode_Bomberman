using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ButtonClickScript_Online : ButtonClickScript, IOnEventCallback
{
    public const byte ReturnTitleEventCode = 199;
    private bool loadRequested;

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == ReturnTitleEventCode && PhotonNetwork.IsMasterClient)
            LoadTitleAsMaster();
    }

    override public void LoadGameScene()
    {
        if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
        {
            Debug.LogError("Cannot return to title: client is not connected to a room.");
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            LoadTitleAsMaster();
            return;
        }

        if (PhotonNetwork.MasterClient == null)
        {
            Debug.LogError("Cannot return to title: master client is unavailable.");
            return;
        }

        RaiseEventOptions options = new RaiseEventOptions
        {
            TargetActors = new[] { PhotonNetwork.MasterClient.ActorNumber }
        };
        if (!PhotonNetwork.RaiseEvent(
            ReturnTitleEventCode, null, options, SendOptions.SendReliable))
        {
            Debug.LogError("Failed to request returning to the title.");
        }
    }

    private void LoadTitleAsMaster()
    {
        if (loadRequested) return;
        if (!PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
        {
            Debug.LogError("Cannot load title: master client is not connected to a room.");
            return;
        }

        loadRequested = true;
        PhotonNetwork.LoadLevel("GameTitle");
    }
}

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            LoadTitleLocally();
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            LoadTitleAsMaster();
            return;
        }

        if (PhotonNetwork.MasterClient == null)
        {
            LoadTitleLocally();
            return;
        }

        RaiseEventOptions options = new RaiseEventOptions
        {
            TargetActors = new[] { PhotonNetwork.MasterClient.ActorNumber }
        };
        if (!PhotonNetwork.RaiseEvent(
            ReturnTitleEventCode, null, options, SendOptions.SendReliable))
        {
            LoadTitleLocally();
            return;
        }

        StartCoroutine(ReturnToTitleIfMasterDoesNotRespond());
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
        StartCoroutine(ReturnToTitleIfMasterDoesNotRespond());
        PhotonNetwork.LoadLevel("GameTitle");
    }

    private IEnumerator ReturnToTitleIfMasterDoesNotRespond()
    {
        yield return new WaitForSecondsRealtime(3f);
        if (SceneManager.GetActiveScene().name != "GameTitle")
            LoadTitleLocally();
    }

    private void LoadTitleLocally()
    {
        StopAllCoroutines();
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom(false);
        SceneManager.LoadScene("GameTitle");
    }
}

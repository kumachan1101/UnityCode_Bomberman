using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClickScript_Online : ButtonClickScript, IOnEventCallback
{
    public const byte ReturnTitleEventCode = 199;
    public const byte ReturnTitleCommitEventCode = 198;
    private bool loadRequested;
    private bool commitSent;

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
            BroadcastReturnToTitle();
        else if (photonEvent.Code == ReturnTitleCommitEventCode)
            BeginGracefulReturn();
    }

    override public void LoadGameScene()
    {
        PrepareGameManagerForTitle();
        if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
        {
            BeginGracefulReturn();
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            BroadcastReturnToTitle();
            return;
        }

        if (PhotonNetwork.MasterClient == null)
        {
            BeginGracefulReturn();
            return;
        }

        RaiseEventOptions options = new RaiseEventOptions
        {
            TargetActors = new[] { PhotonNetwork.MasterClient.ActorNumber }
        };
        if (!PhotonNetwork.RaiseEvent(
            ReturnTitleEventCode, null, options, SendOptions.SendReliable))
        {
            BeginGracefulReturn();
            return;
        }

        StartCoroutine(ReturnToTitleIfMasterDoesNotRespond());
    }

    private void BroadcastReturnToTitle()
    {
        if (commitSent) return;
        if (!PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
        {
            Debug.LogError("Cannot load title: master client is not connected to a room.");
            BeginGracefulReturn();
            return;
        }

        commitSent = true;
        PrepareGameManagerForTitle();
        RaiseEventOptions options = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.Others
        };
        if (!PhotonNetwork.RaiseEvent(ReturnTitleCommitEventCode, null,
            options, SendOptions.SendReliable))
            Debug.LogError("Return-to-title commit could not be sent to the room.");
        BeginGracefulReturn();
    }

    private IEnumerator ReturnToTitleIfMasterDoesNotRespond()
    {
        yield return new WaitForSecondsRealtime(3f);
        if (SceneManager.GetActiveScene().name != "GameTitle")
            BeginGracefulReturn();
    }

    private void BeginGracefulReturn()
    {
        if (loadRequested) return;
        loadRequested = true;
        PrepareGameManagerForTitle();
        // PUN's scene-loaded callback writes the current scene to the room when
        // automatic synchronization is enabled.  Loading the local title while
        // LeaveRoom is completing otherwise attempts SetProperties in Leaving.
        // RoomButton enables synchronization again before the next room join.
        PhotonNetwork.AutomaticallySyncScene = false;
        StartCoroutine(LeaveRoomThenLoadTitle());
    }

    private IEnumerator LeaveRoomThenLoadTitle()
    {
        if (PhotonNetwork.InRoom)
        {
            bool leaveStarted = PhotonNetwork.LeaveRoom(false);
            float timeoutAt = Time.realtimeSinceStartup + 5f;
            while (leaveStarted && PhotonNetwork.InRoom &&
                   Time.realtimeSinceStartup < timeoutAt)
                yield return null;

            if (PhotonNetwork.InRoom)
                PhotonNetwork.Disconnect();
        }

        SceneManager.LoadScene("GameTitle");
    }

    private static void PrepareGameManagerForTitle()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.PrepareReturnToTitle();
    }
}

using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;

public class OnlineLobby : MonoBehaviourPunCallbacks
{
    private Coroutine connectRoutine;
    private bool connectionWanted;

    protected virtual void Start()
    {
        connectionWanted = true;
        StartConnectRoutine();
    }

    public override void OnConnectedToMaster()
    {
        if (connectionWanted && !PhotonNetwork.InLobby)
            ReqJoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        // タイトルへ戻った直後は前の切断処理が残る場合がある。
        // このシーンがまだ有効なら、切断完了後に接続をやり直す。
        if (connectionWanted && isActiveAndEnabled)
            StartConnectRoutine();
    }

    public override void OnDisable()
    {
        connectionWanted = false;
        if (connectRoutine != null)
        {
            StopCoroutine(connectRoutine);
            connectRoutine = null;
        }
        base.OnDisable();
    }

    private void StartConnectRoutine()
    {
        if (!isActiveAndEnabled || connectRoutine != null) return;
        connectRoutine = StartCoroutine(ConnectWhenPreviousSessionIsClosed());
    }

    private IEnumerator ConnectWhenPreviousSessionIsClosed()
    {
        while (connectionWanted &&
               PhotonNetwork.NetworkClientState == ClientState.Disconnecting)
            yield return null;

        connectRoutine = null;
        if (!connectionWanted || !isActiveAndEnabled) yield break;

        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (!PhotonNetwork.InLobby) ReqJoinLobby();
            yield break;
        }

        if (!PhotonNetwork.IsConnected && !PhotonNetwork.ConnectUsingSettings())
            Debug.LogError("Photon connection could not be started.");
    }

    public virtual void ReqJoinLobby(){}

    protected void BaseJoinLobby(TypedLobby lobby)
    {
        PhotonNetwork.JoinLobby(lobby);
    }

    public void DestroyAllPhotonViews()
    {
        foreach (PhotonView view in FindObjectsOfType<PhotonView>())
        {
            if (view.IsMine)
            {
                PhotonNetwork.Destroy(view.gameObject);
            }
        }
    }
}

using Photon.Pun;
using UnityEngine;

public sealed class ItemInvincible : Item
{
    public override void Reflection(GameObject playerObject)
    {
        if (playerObject == null) return;
        PlayerInvincibility effect = playerObject.GetComponent<PlayerInvincibility>();
        if (effect == null) effect = playerObject.AddComponent<PlayerInvincibility>();

        PhotonView view = playerObject.GetComponent<PhotonView>();
        if (view != null && PhotonNetwork.InRoom)
        {
            if (!view.IsMine) return;
            view.RPC(nameof(PlayerInvincibility.ActivateSynced), RpcTarget.All,
                PlayerInvincibility.DefaultDurationSeconds);
            return;
        }

        effect.Activate(PlayerInvincibility.DefaultDurationSeconds);
    }
}

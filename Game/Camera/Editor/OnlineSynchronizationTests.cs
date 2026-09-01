using System.Linq;
using NUnit.Framework;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class OnlineSynchronizationTests
{
    [Test]
    public void PlayerPrefabSynchronizesPositionRotationAndAnimation()
    {
        GameObject playerPrefab = Resources.Load<GameObject>("Player");
        Assert.That(playerPrefab, Is.Not.Null);
        PhotonView view = playerPrefab.GetComponent<PhotonView>();
        Assert.That(view, Is.Not.Null);
        Assert.That(view.ObservedComponents.OfType<PhotonTransformView>().Any(), Is.True,
            "Online movement needs PhotonTransformView in the observed component list.");
        Assert.That(view.ObservedComponents.OfType<PhotonAnimatorView>().Any(), Is.True,
            "Remote walking state needs PhotonAnimatorView in the observed component list.");

        PhotonTransformView transformView =
            view.ObservedComponents.OfType<PhotonTransformView>().Single();
        Assert.That(transformView.m_SynchronizePosition, Is.True);
        Assert.That(transformView.m_SynchronizeRotation, Is.True);
    }

    [Test]
    public void RoomCreationUsesTheCurrentlyObservedNamedLobby()
    {
        TypedLobby namedLobby = new TypedLobby("Lobby1", LobbyType.Default);
        Assert.That(RoomButton.ResolveTargetLobby(namedLobby), Is.SameAs(namedLobby));
        Assert.That(RoomButton.ResolveTargetLobby(null), Is.SameAs(TypedLobby.Default));
    }

    [Test]
    public void RemoteOnlinePlayerUsesKinematicPhysicsWhileReceivingTransforms()
    {
        GameObject player = new GameObject("Remote online player");
        try
        {
            player.AddComponent<Animator>();
            Rigidbody body = player.AddComponent<Rigidbody>();
            PhotonView view = player.AddComponent<PhotonView>();
            view.ControllerActorNr = PhotonNetwork.LocalPlayer.ActorNumber + 1;
            Assert.That(view.IsMine, Is.False);
            player.AddComponent<PlayerAction_Online>();

            PlayerMovement movement = player.AddComponent<PlayerMovement>();
            movement.Awake();

            Assert.That(body.isKinematic, Is.True,
                "Remote physics must not overwrite received Photon transforms.");
            Assert.That(body.collisionDetectionMode,
                Is.EqualTo(CollisionDetectionMode.ContinuousSpeculative));
            Assert.That(body.interpolation, Is.EqualTo(RigidbodyInterpolation.Interpolate));
        }
        finally
        {
            Object.DestroyImmediate(player);
        }
    }
}

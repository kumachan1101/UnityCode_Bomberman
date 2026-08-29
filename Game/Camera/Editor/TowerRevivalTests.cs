using System.Collections;
using System.Reflection;
using NUnit.Framework;
using Photon.Pun;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public sealed class TowerRevivalSpawnManagerProbe : PlayerSpawnManager_CpuMode_Tower
{
    public int SpawnCount { get; private set; }
    public int LastPlayerNo { get; private set; }

    public override void SpawnPlayer(int playerNo)
    {
        SpawnCount++;
        LastPlayerNo = playerNo;
    }
}

public sealed class TowerRevivalTests
{
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        if (!Application.isPlaying)
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        yield return new EnterPlayMode();
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        yield return new ExitPlayMode();
    }

    [UnityTest]
    public IEnumerator LocalTowerRevivesPlayerOneAndConsumesTowerPowerAfterDelayedSetup()
    {
        GameObject dispatcherObject = new GameObject("EventDispatcher");
        dispatcherObject.AddComponent<EventDispatcher>();
        GameObject managerObject = new GameObject("GameManager");
        GameManager gameManager = managerObject.AddComponent<GameManager>();
        SetPrivateField(gameManager, "bSetUp", true);

        GameObject field = new GameObject("Field");
        TowerRevivalSpawnManagerProbe spawnManager =
            field.AddComponent<TowerRevivalSpawnManagerProbe>();
        TowerSpawnManager towerManager = field.AddComponent<TowerSpawnManager>();
        towerManager.enabled = false;

        GameObject towerCanvas = Object.Instantiate(
            Resources.Load<GameObject>("CanvasPowerGageTower"));
        towerCanvas.name = "CanvasPowerGageTower1";
        towerCanvas.GetComponent<PowerGage_Slider>().SetPlayerNo(1);
        GameObject tower = new GameObject("Tower1");
        PowerGageIF_Tower towerPower = tower.AddComponent<PowerGageIF_Tower>();
        towerPower.SetCanvasInsID(towerCanvas.GetInstanceID());

        GameObject buttonCanvas = Object.Instantiate(
            Resources.Load<GameObject>("CanvasButtonTower"));
        TowerAddedRemoved_PlayerControl revival =
            buttonCanvas.GetComponentInChildren<TowerAddedRemoved_PlayerControl>();
        Button button = revival.GetComponent<Button>();

        yield return null;
        yield return null;
        revival.RefreshAvailability();

        Assert.That(field.GetComponent<PlayerNameManager>().GetPlayerName(), Is.Null.Or.Empty,
            "The controller must work even before PlayerNameManager receives Player1.");
        Assert.That(revival.ControlledPlayerNumber, Is.EqualTo(1));
        Assert.That(towerPower.IsPowerGageReady(), Is.True);
        Assert.That(towerPower.GetCurrentPower(), Is.EqualTo(3f));
        Assert.That(button.interactable, Is.True,
            "A missing local player with sufficient tower power must be revivable.");

        GameObject livingPlayer = new GameObject("Player1");
        livingPlayer.AddComponent<Player>();
        yield return null;
        revival.RefreshAvailability();
        Assert.That(button.interactable, Is.False,
            "The revival button must be disabled while Player1 is alive.");

        Object.Destroy(livingPlayer);
        yield return null;
        yield return null;
        revival.RefreshAvailability();
        Assert.That(button.interactable, Is.True);

        revival.PushButton();

        Assert.That(spawnManager.SpawnCount, Is.EqualTo(1));
        Assert.That(spawnManager.LastPlayerNo, Is.EqualTo(1));
        Assert.That(towerPower.GetCurrentPower(), Is.EqualTo(1f),
            "Reviving costs two points from the player's own tower.");
        Assert.That(button.interactable, Is.False,
            "One remaining tower point is insufficient for another revival.");

        Object.Destroy(buttonCanvas);
        Object.Destroy(tower);
        Object.Destroy(towerCanvas);
        Object.Destroy(field);
        Object.Destroy(managerObject);
        Object.Destroy(dispatcherObject);
        yield return null;
    }

    [Test]
    public void OnlineTowerSynchronizesRevivalPowerThroughPunRpc()
    {
        MethodInfo overrideMethod = typeof(TowerSpawnManager_Online).GetMethod(
            nameof(TowerSpawnManager.TrySpendRevivalPower));
        MethodInfo rpcMethod = typeof(TowerSpawnManager_Online).GetMethod(
            "SpendRevivalPowerForAll", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.That(overrideMethod, Is.Not.Null);
        Assert.That(overrideMethod.DeclaringType, Is.EqualTo(typeof(TowerSpawnManager_Online)));
        Assert.That(rpcMethod, Is.Not.Null);
        Assert.That(rpcMethod.GetCustomAttribute<PunRPC>(), Is.Not.Null);
    }

    private static void SetPrivateField(object target, string name, object value)
    {
        FieldInfo field = target.GetType().GetField(name,
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, name);
        field.SetValue(target, value);
    }
}

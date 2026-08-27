using System.Collections;
using System.Reflection;
using NUnit.Framework;
using Photon.Pun;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MainCameraControllerTests
{
    private MainCameraController controller;
    private Camera camera;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Unity replays setup up to the yield after entering Play Mode.
        if (!Application.isPlaying)
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        yield return new EnterPlayMode();
        CreateCamera();
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        yield return new ExitPlayMode();
    }

    [UnityTest]
    public IEnumerator FollowsMovementSmoothlyWithoutRotatingWithPlayer()
    {
        Player player = CreatePlayer<Player>(new Vector3(5f, 0.5f, 5f));
        yield return null;
        AssertCentered(player.transform.position + Vector3.up * 0.5f);
        Vector3 start = camera.transform.position;
        Quaternion rotation = camera.transform.rotation;

        player.transform.position += Vector3.right * 3f;
        player.transform.rotation = Quaternion.Euler(0f, 120f, 0f);
        StepFollow();
        Assert.That(camera.transform.position.x, Is.GreaterThan(start.x));
        Assert.That(camera.transform.position.x, Is.LessThan(start.x + 3f));
        for (int i = 0; i < 120; i++) StepFollow();

        Assert.That(Vector3.Distance(camera.transform.position, start + Vector3.right * 3f), Is.LessThan(0.01f));
        Assert.That(Quaternion.Angle(rotation, camera.transform.rotation), Is.LessThan(0.01f));
        AssertCentered(player.transform.position + Vector3.up * 0.5f);
    }

    [UnityTest]
    public IEnumerator TeleportAndHeightChangesKeepPlayerInView()
    {
        Player player = CreatePlayer<Player>(Vector3.zero);
        yield return null;
        player.transform.position = new Vector3(70f, 30f, -50f);
        StepFollow();
        AssertCentered(player.transform.position + Vector3.up * 0.5f);
        Assert.That(camera.transform.position.y, Is.GreaterThan(player.transform.position.y));
    }

    [UnityTest]
    public IEnumerator IgnoresCpuRemotePlayersAndOwnedDummiesAndRecoversOwnership()
    {
        Vector3 overview = camera.transform.position;
        CreatePlayer<Player_CpuMode>(new Vector3(40f, 0f, 20f));
        CreateOnlinePlayer<Player_Online_Dummy>(new Vector3(30f, 0f, 10f), true);
        Player_Online remote = CreateOnlinePlayer<Player_Online>(new Vector3(20f, 0f, 30f), false);
        yield return null;
        Assert.That(camera.transform.position, Is.EqualTo(overview));

        Player_Online local = CreateOnlinePlayer<Player_Online>(new Vector3(4f, 0.5f, 7f), true);
        yield return null;
        AssertCentered(local.transform.position + Vector3.up * 0.5f);
        Vector3 followingLocal = camera.transform.position;
        remote.transform.position += Vector3.right * 80f;
        StepFollow();
        Assert.That(camera.transform.position, Is.EqualTo(followingLocal));

        local.GetComponent<PhotonView>().ControllerActorNr = PhotonNetwork.LocalPlayer.ActorNumber + 1;
        local.transform.position += Vector3.right * 50f;
        StepFollow();
        Assert.That(camera.transform.position, Is.EqualTo(followingLocal));

        remote.GetComponent<PhotonView>().ControllerActorNr = PhotonNetwork.LocalPlayer.ActorNumber;
        yield return new WaitForSecondsRealtime(0.55f);
        StepFollow();
        AssertCentered(remote.transform.position + Vector3.up * 0.5f);
    }

    [UnityTest]
    public IEnumerator RetainsViewOnDeathAndAcquiresRespawnedPlayer()
    {
        Player player = CreatePlayer<Player>(new Vector3(2f, 0.5f, 2f));
        Player_CpuMode cpu = CreatePlayer<Player_CpuMode>(Vector3.one * 20f);
        yield return null;
        Vector3 lastPosition = camera.transform.position;
        Object.Destroy(player.gameObject);
        yield return null;
        cpu.transform.position += Vector3.right * 50f;
        StepFollow();
        Assert.That(camera.transform.position, Is.EqualTo(lastPosition));

        Player respawned = CreatePlayer<Player>(new Vector3(12f, 3f, 16f));
        yield return null;
        AssertCentered(respawned.transform.position + Vector3.up * 0.5f);
    }

    [UnityTest]
    public IEnumerator AcquiresExistingPlayerAndRejectsDuplicateCamera()
    {
        Object.Destroy(controller.gameObject);
        yield return null;
        Player player = CreatePlayer<Player>(new Vector3(8f, 0.5f, 3f));
        yield return null;
        CreateCamera();
        AssertCentered(player.transform.position + Vector3.up * 0.5f);

        Object.Instantiate(Resources.Load<GameObject>("Main Camera"));
        yield return null;
        Assert.That(Object.FindObjectsOfType<MainCameraController>().Length, Is.EqualTo(1));
        AssertCentered(player.transform.position + Vector3.up * 0.5f);
    }

    [UnityTest]
    public IEnumerator ReturningToTitleRestoresOverviewAndKeepsOneCamera()
    {
        Vector3 overview = camera.transform.position;
        Quaternion overviewRotation = camera.transform.rotation;
        float fieldOfView = camera.fieldOfView;
        CreatePlayer<Player>(new Vector3(40f, 0.5f, 30f));
        yield return null;
        Assert.That(camera.fieldOfView, Is.EqualTo(60f));

        yield return SceneManager.LoadSceneAsync("GameTitle", LoadSceneMode.Single);
        yield return null;
        Assert.That(Object.FindObjectsOfType<MainCameraController>().Length, Is.EqualTo(1));
        Assert.That(camera.transform.position, Is.EqualTo(overview));
        Assert.That(Quaternion.Angle(camera.transform.rotation, overviewRotation), Is.LessThan(0.01f));
        Assert.That(camera.fieldOfView, Is.EqualTo(fieldOfView));
    }

    [UnityTest]
    public IEnumerator HidesCameraControlsButKeepsMovementAndBombControls()
    {
        GameSceneScreenManager screen = new GameObject("Screen under test").AddComponent<GameSceneScreenManager>();
        typeof(BaseScreenManager).GetMethod("InitializeCanvas", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(screen, null);
        GameObject canvas = (GameObject)typeof(BaseScreenManager).GetField("currentCanvas", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(screen);
        yield return null;

        foreach (string name in new[] { "JoystickCamera", "up", "down" })
            Assert.That(canvas.transform.Find(name).gameObject.activeSelf, Is.False, name);
        Assert.That(canvas.transform.Find("JoystickPlayer").gameObject.activeSelf, Is.True);
        Assert.That(canvas.transform.Find("bom").gameObject.activeSelf, Is.True);
        Assert.That(canvas.GetComponent<CameraControlWithButtons>().enabled, Is.False);
        Assert.That(controller.GetComponent<CameraControlWithButtons>().enabled, Is.False);
        Assert.That(controller.GetComponent<JoystickCameraController>().enabled, Is.False);
    }

    [UnityTest]
    public IEnumerator SupportsPlayerTypesUsedByAllFourGameModes()
    {
        string[] modes = { "GameScene", "GameOnline", "GameTower", "GameTowerOnline" };
        string[] fieldPrefabs = { "FieldCpuMode", "FieldOnline", "FieldTower", "FieldTowerOnline" };
        System.Type[] spawnerTypes =
        {
            typeof(PlayerSpawnManager_CpuMode),
            typeof(PlayerSpawnManager_Online),
            typeof(PlayerSpawnManager_CpuMode_Tower),
            typeof(PlayerSpawnManager_Online_Tower)
        };
        bool[] online = { false, true, false, true };

        for (int i = 0; i < modes.Length; i++)
        {
            GameObject field = Resources.Load<GameObject>(fieldPrefabs[i]);
            Assert.That(field, Is.Not.Null, modes[i]);
            Assert.That(field.GetComponent(spawnerTypes[i]), Is.Not.Null,
                modes[i] + " player spawner");

            Player_Base player = online[i]
                ? CreateOnlinePlayer<Player_Online>(new Vector3(i * 8f, 0.5f, i * 5f), true)
                : CreatePlayer<Player>(new Vector3(i * 8f, 0.5f, i * 5f));
            yield return null;
            AssertCentered(player.transform.position + Vector3.up * 0.5f);
            Object.Destroy(player.gameObject);
            yield return null;
        }
    }

    private void CreateCamera()
    {
        GameObject root = Object.Instantiate(Resources.Load<GameObject>("Main Camera"));
        controller = root.GetComponent<MainCameraController>();
        camera = root.GetComponent<Camera>();
        camera.aspect = 1.6f;
    }

    private static T CreatePlayer<T>(Vector3 position) where T : Player_Base
    {
        GameObject root = new GameObject(typeof(T).Name);
        root.transform.position = position;
        return root.AddComponent<T>();
    }

    private static T CreateOnlinePlayer<T>(Vector3 position, bool local) where T : Player_Online
    {
        GameObject root = new GameObject(typeof(T).Name);
        root.transform.position = position;
        PhotonView view = root.AddComponent<PhotonView>();
        // Set the local fixture's controller without connecting to Photon servers.
        view.ControllerActorNr = PhotonNetwork.LocalPlayer.ActorNumber + (local ? 0 : 1);
        Assert.That(view.IsMine, Is.EqualTo(local));
        return root.AddComponent<T>();
    }

    private void StepFollow()
    {
        typeof(MainCameraController).GetMethod("UpdateFollow", BindingFlags.Instance | BindingFlags.NonPublic)
            .Invoke(controller, new object[] { 1f / 60f });
    }

    private void AssertCentered(Vector3 target)
    {
        Vector3 viewport = camera.WorldToViewportPoint(target);
        Assert.That(viewport.z, Is.GreaterThan(camera.nearClipPlane));
        Assert.That(viewport.x, Is.EqualTo(0.5f).Within(0.01f));
        Assert.That(viewport.y, Is.EqualTo(0.5f).Within(0.01f));
    }
}

using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public sealed class GameStatusAndAudioTests
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

    [Test]
    public void StageTitleContainsNumberTotalAndModeName()
    {
        Assert.That(GameStatusHudController.FormatStageTitle("ONLINE TOWER", 3, 5),
            Is.EqualTo("STAGE 3/5  •  ONLINE TOWER"));
        Assert.That(GameStatusHudController.FormatStageTitle("LOCAL BATTLE", 0, 0),
            Is.EqualTo("STAGE 1/1  •  LOCAL BATTLE"));
    }

    [UnityTest]
    public IEnumerator WinningAdvancesTheDisplayedStageOnlyWhenTheNextSceneStarts()
    {
        GameObject dispatcherObject = new GameObject("EventDispatcher");
        dispatcherObject.AddComponent<EventDispatcher>();
        GameObject managerObject = new GameObject("GameManager");
        GameManager manager = managerObject.AddComponent<GameManager>();
        yield return null;

        SetPrivateField(manager, "iStage", 1);
        SetPrivateField(manager, "maxStage", 5);
        manager.GameWin();
        manager.GameWin();

        Assert.That(manager.GetCurrentStage(), Is.EqualTo(1),
            "The win callback must not increment twice before the scene transition.");
        Assert.That(manager.GetStageCount(), Is.EqualTo(5));

        manager.CancelInvoke();
        Object.Destroy(managerObject);
        Object.Destroy(dispatcherObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ItemHudUsesIconsAndTracksTheLocalPlayersActualValues()
    {
        GameObject canvasObject = new GameObject("Canvas", typeof(RectTransform),
            typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        GameStatusHudController hud = canvasObject.AddComponent<GameStatusHudController>();
        hud.ConfigureMode("LOCAL BATTLE");

        GameObject playerObject = new GameObject("Player1", typeof(Animator),
            typeof(Rigidbody));
        playerObject.AddComponent<Player>();
        PlayerBom bomb = playerObject.AddComponent<PlayerBom>();
        PlayerMovement movement = playerObject.AddComponent<PlayerMovement>();
        yield return null;
        hud.RefreshNow();

        Assert.That(canvasObject.transform.Find("ItemStatus").childCount, Is.EqualTo(6));
        Assert.That(hud.GetCellValue("Fire"), Is.EqualTo("3"));
        Assert.That(hud.GetCellValue("Bomb"), Is.EqualTo("3"));
        Assert.That(hud.GetCellValue("Move"), Is.EqualTo("3"));
        Assert.That(hud.GetCellValue("BombSpeed"), Is.EqualTo("1"));
        Assert.That(hud.GetCellValue("Kick"), Is.EqualTo("--"));
        Assert.That(hud.GetCellValue("Special"), Is.EqualTo("NORMAL"));

        bomb.Request(ReqType.FireUp);
        bomb.Request(ReqType.BomUp);
        bomb.Request(ReqType.BomSpeedUp);
        bomb.Request(ReqType.BomKick);
        bomb.Request(ReqType.BomMulti);
        movement.SpeedUp();
        hud.RefreshNow();

        Assert.That(hud.GetCellValue("Fire"), Is.EqualTo("4"));
        Assert.That(hud.GetCellValue("Bomb"), Is.EqualTo("4"));
        Assert.That(hud.GetCellValue("Move"), Is.EqualTo("4"));
        Assert.That(hud.GetCellValue("BombSpeed"), Is.EqualTo("2"));
        Assert.That(hud.GetCellValue("Kick"), Is.EqualTo("ON"));
        Assert.That(hud.GetCellValue("Special"), Is.EqualTo("MULTI"));

        Object.Destroy(playerObject);
        Object.Destroy(canvasObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator SoundPrefabHasClipsAndActuallyRequestsPlayback()
    {
        GameObject soundObject = Object.Instantiate(Resources.Load<GameObject>("SoundManager"));
        SoundManager sound = soundObject.GetComponent<SoundManager>();
        yield return null;

        Assert.That(sound.IsAudioReady, Is.True);
        Assert.That(sound.GetComponent<AudioSource>(), Is.Not.Null);
        Assert.That(sound.sDropBomb.length, Is.GreaterThan(0f));
        Assert.That(sound.sGetItem.length, Is.GreaterThan(0f));
        Assert.That(sound.sExploison.length, Is.GreaterThan(0f));

        int before = sound.PlayedEffectCount;
        Assert.That(sound.PlaySoundEffect("DROPBOMB"), Is.True);
        Assert.That(sound.PlayedEffectCount, Is.EqualTo(before + 1));
        Assert.That(sound.PlaySoundEffect("EXPLOISON"), Is.True);
        Assert.That(sound.PlaySoundEffect("EXPLOISON"), Is.False,
            "Simultaneous explosion tiles should share one sound burst.");

        Object.Destroy(soundObject);
        yield return null;
    }

    private static void SetPrivateField(object target, string name, object value)
    {
        FieldInfo field = target.GetType().GetField(name,
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, name);
        field.SetValue(target, value);
    }
}

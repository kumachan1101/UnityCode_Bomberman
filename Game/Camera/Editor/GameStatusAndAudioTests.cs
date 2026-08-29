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
    public IEnumerator PlayerRemovalDuringWinningTransitionCannotResetNextStage()
    {
        GameObject dispatcherObject = new GameObject("EventDispatcher");
        dispatcherObject.AddComponent<EventDispatcher>();
        GameObject managerObject = new GameObject("GameManager");
        GameManager manager = managerObject.AddComponent<GameManager>();
        yield return null;

        SetPrivateField(manager, "iStage", 1);
        SetPrivateField(manager, "maxStage", 5);
        manager.GameWin();
        Assert.That(manager.IsStageTransitionPending(), Is.True);

        manager.NextStage();
        manager.GameOver();

        Assert.That(manager.GetCurrentStage(), Is.EqualTo(2),
            "Old-scene player destruction must not reset the stage during a win transition.");

        manager.CancelInvoke();
        Object.Destroy(managerObject);
        Object.Destroy(dispatcherObject);
        yield return null;
    }

    [Test]
    public void ThrowModeMakesItsSpeedExplicitAndOtherModesStayInactive()
    {
        Assert.That(GameStatusHudController.FormatDropModeStatus(
            BOM_ATTACK.BOM_ATTACK_THROW, 3), Is.EqualTo("THROW  SPD 3"));
        Assert.That(GameStatusHudController.FormatThrowSpeedStatus(
            BOM_ATTACK.BOM_ATTACK_THROW, 3), Is.EqualTo("SPEED 3"));
        Assert.That(GameStatusHudController.FormatThrowSpeedStatus(
            BOM_ATTACK.BOM_ATTACK_MULTI, 3), Is.EqualTo("INACTIVE"));
    }

    [UnityTest]
    public IEnumerator StageHudRefreshesWhenGameManagerAdvances()
    {
        GameObject dispatcherObject = new GameObject("EventDispatcher");
        dispatcherObject.AddComponent<EventDispatcher>();
        GameObject managerObject = new GameObject("GameManager");
        GameManager manager = managerObject.AddComponent<GameManager>();
        GameObject canvasObject = new GameObject("Canvas", typeof(RectTransform),
            typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        GameStatusHudController hud = canvasObject.AddComponent<GameStatusHudController>();
        hud.ConfigureMode("LOCAL BATTLE");
        yield return null;

        SetPrivateField(manager, "maxStage", 5);
        SetPrivateField(manager, "iStage", 1);
        hud.RefreshNow();
        Assert.That(hud.StageTextValue,
            Is.EqualTo("STAGE 1/5  •  LOCAL BATTLE"));

        SetPrivateField(manager, "iStage", 2);
        hud.RefreshNow();
        Assert.That(hud.StageTextValue,
            Is.EqualTo("STAGE 2/5  •  LOCAL BATTLE"));

        Object.Destroy(canvasObject);
        Object.Destroy(managerObject);
        Object.Destroy(dispatcherObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ConsecutiveBombsKeepTypeAndIndependentDropMode()
    {
        GameObject playerObject = new GameObject("Player1");
        PlayerBom bomb = playerObject.AddComponent<PlayerBom>();
        yield return null;

        bomb.Request(ReqType.ExplodeBom);
        bomb.Request(ReqType.BomAttack);
        bomb.Request(ReqType.BomSpeedUp);
        BomParameters first = bomb.CreateBomParameters(Vector3.zero, Vector3.right);
        BomParameters second = bomb.CreateBomParameters(Vector3.right, Vector3.right);

        Assert.That(first.bomKind, Is.EqualTo(BOM_KIND.BOM_KIND_EXPLODE));
        Assert.That(second.bomKind, Is.EqualTo(BOM_KIND.BOM_KIND_EXPLODE));
        Assert.That(first.bomAttack, Is.EqualTo(BOM_ATTACK.BOM_ATTACK_THROW));
        Assert.That(second.bomAttack, Is.EqualTo(BOM_ATTACK.BOM_ATTACK_THROW));
        Assert.That(first.iSpeed, Is.EqualTo(2));
        Assert.That(second.iSpeed, Is.EqualTo(2));

        bomb.Request(ReqType.BigBanBom);
        BomParameters replacement = bomb.CreateBomParameters(Vector3.zero, Vector3.forward);
        Assert.That(replacement.bomKind, Is.EqualTo(BOM_KIND.BOM_KIND_BIGBAN),
            "A new bomb type replaces the previous type within that group.");
        Assert.That(replacement.bomAttack, Is.EqualTo(BOM_ATTACK.BOM_ATTACK_THROW),
            "Changing bomb type must not clear the independent drop mode.");

        Object.Destroy(playerObject);
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

        Assert.That(canvasObject.transform.Find("ItemStatus/CoreGroup"), Is.Not.Null);
        Assert.That(canvasObject.transform.Find("ItemStatus/AbilityGroup"), Is.Not.Null);
        Assert.That(hud.GetCellValue("Fire"), Is.EqualTo("3"));
        Assert.That(hud.GetCellValue("Bomb"), Is.EqualTo("3"));
        Assert.That(hud.GetCellValue("Move"), Is.EqualTo("3"));
        Assert.That(hud.GetCellValue("Kick"), Is.EqualTo("OFF"));
        Assert.That(hud.GetCellValue("BombType"), Is.EqualTo("NORMAL"));
        Assert.That(hud.GetCellValue("DropMode"), Is.EqualTo("NORMAL"));
        Assert.That(hud.GetCellValue("ThrowSpeed"), Is.EqualTo("INACTIVE"));
        Assert.That(hud.GetCellActive("ThrowSpeed"), Is.False);

        bomb.Request(ReqType.FireUp);
        bomb.Request(ReqType.BomUp);
        bomb.Request(ReqType.BomSpeedUp);
        bomb.Request(ReqType.BomKick);
        bomb.Request(ReqType.ExplodeBom);
        bomb.Request(ReqType.BomAttack);
        movement.SpeedUp();
        hud.RefreshNow();

        Assert.That(hud.GetCellValue("Fire"), Is.EqualTo("4"));
        Assert.That(hud.GetCellValue("Bomb"), Is.EqualTo("4"));
        Assert.That(hud.GetCellValue("Move"), Is.EqualTo("4"));
        Assert.That(hud.GetCellValue("Kick"), Is.EqualTo("ON"));
        Assert.That(hud.GetCellValue("BombType"), Is.EqualTo("EXPLODE"));
        Assert.That(hud.GetCellValue("DropMode"), Is.EqualTo("THROW  SPD 2"));
        Assert.That(hud.GetCellValue("ThrowSpeed"), Is.EqualTo("SPEED 2"));
        Assert.That(hud.GetCellActive("ThrowSpeed"), Is.True);
        Assert.That(hud.GetCellIconName("BombType"), Does.EndWith("TypeExplode"));
        Assert.That(hud.GetCellIconName("DropMode"), Does.EndWith("ModeThrow"));

        // Bomb type and drop mode are independent. Each group is exclusive
        // internally, while the selected values from both groups coexist.
        bomb.Request(ReqType.BomMulti);
        hud.RefreshNow();
        Assert.That(hud.GetCellValue("BombType"), Is.EqualTo("EXPLODE"));
        Assert.That(hud.GetCellValue("DropMode"), Is.EqualTo("MULTI"));
        Assert.That(hud.GetCellValue("ThrowSpeed"), Is.EqualTo("INACTIVE"));
        Assert.That(hud.GetCellActive("ThrowSpeed"), Is.False);

        bomb.Request(ReqType.BigBanBom);
        hud.RefreshNow();
        Assert.That(hud.GetCellValue("BombType"), Is.EqualTo("BIG"));
        Assert.That(hud.GetCellValue("DropMode"), Is.EqualTo("MULTI"));
        Assert.That(hud.GetCellIconName("BombType"), Does.EndWith("TypeBig"));
        Assert.That(hud.GetCellIconName("DropMode"), Does.EndWith("ModeMulti"));

        Object.Destroy(playerObject);
        Object.Destroy(canvasObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator SoundPrefabHasClipsAndActuallyRequestsPlayback()
    {
        int previousPreference = PlayerPrefs.GetInt(
            SoundManager.SoundEnabledPreference, 1);
        PlayerPrefs.SetInt(SoundManager.SoundEnabledPreference, 1);
        GameObject soundObject = Object.Instantiate(Resources.Load<GameObject>("SoundManager"));
        SoundManager sound = soundObject.GetComponent<SoundManager>();
        GameObject canvasObject = new GameObject("Canvas", typeof(RectTransform),
            typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        GameStatusHudController hud = canvasObject.AddComponent<GameStatusHudController>();
        yield return null;
        hud.RefreshNow();

        Assert.That(sound.IsAudioReady, Is.True);
        Assert.That(sound.IsSoundEnabled, Is.True);
        Assert.That(hud.AudioButtonText, Is.EqualTo("SOUND ON"));
        Assert.That(sound.GetComponent<AudioSource>(), Is.Not.Null);
        Assert.That(sound.sDropBomb.length, Is.GreaterThan(0f));
        Assert.That(sound.sGetItem.length, Is.GreaterThan(0f));
        Assert.That(sound.sExploison.length, Is.GreaterThan(0f));

        int before = sound.PlayedEffectCount;
        Assert.That(sound.PlaySoundEffect("DROPBOMB"), Is.True);
        Assert.That(sound.PlayedEffectCount, Is.EqualTo(before + 1));

        Button soundButton = canvasObject.transform
            .Find("StageStatus/SoundToggle").GetComponent<Button>();
        soundButton.onClick.Invoke();
        Assert.That(sound.IsSoundEnabled, Is.False);
        Assert.That(sound.GetComponent<AudioSource>().mute, Is.True);
        Assert.That(hud.AudioButtonText, Is.EqualTo("SOUND OFF"));
        Assert.That(PlayerPrefs.GetInt(SoundManager.SoundEnabledPreference), Is.EqualTo(0));
        Assert.That(sound.PlaySoundEffect("GETITEM"), Is.False);
        Assert.That(sound.PlayedEffectCount, Is.EqualTo(before + 1));

        soundButton.onClick.Invoke();
        Assert.That(sound.IsSoundEnabled, Is.True);
        Assert.That(sound.GetComponent<AudioSource>().mute, Is.False);
        Assert.That(hud.AudioButtonText, Is.EqualTo("SOUND ON"));
        Assert.That(PlayerPrefs.GetInt(SoundManager.SoundEnabledPreference), Is.EqualTo(1));
        Assert.That(sound.PlaySoundEffect("EXPLOISON"), Is.True);
        Assert.That(sound.PlaySoundEffect("EXPLOISON"), Is.False,
            "Simultaneous explosion tiles should share one sound burst.");

        sound.SetSoundEnabled(previousPreference != 0);
        Object.Destroy(canvasObject);
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

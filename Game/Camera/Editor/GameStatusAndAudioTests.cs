using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public sealed class InvincibleItemControlProbe : ItemControl
{
    protected override IItemPathProvider CreateItemPathProvider()
    {
        return new ItemPathProvider_CpuMode();
    }

    public override void CreateItem_RPC(Vector3 position) { }
    protected override bool IsCreateItem() { return true; }

    public ItemInvincible SpawnInvincibilityItem(Vector3 position)
    {
        int index = itemList.FindIndex(item => item.itemName == "Item_Invincible");
        Assert.That(index, Is.GreaterThanOrEqualTo(0));
        CreateRandomItem(position, index);
        return Object.FindObjectOfType<ItemInvincible>();
    }

    public ItemMagnet SpawnMagnetItem(Vector3 position)
    {
        int index = itemList.FindIndex(item => item.itemName == "Item_Magnet");
        Assert.That(index, Is.GreaterThanOrEqualTo(0));
        CreateRandomItem(position, index);
        return Object.FindObjectOfType<ItemMagnet>();
    }
}

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

    [UnityTest]
    public IEnumerator ReturningToTitleResetsStageAndIgnoresSceneTeardownDeaths()
    {
        GameObject dispatcherObject = new GameObject("EventDispatcher");
        dispatcherObject.AddComponent<EventDispatcher>();
        GameObject managerObject = new GameObject("GameManager");
        GameManager manager = managerObject.AddComponent<GameManager>();
        yield return null;

        SetPrivateField(manager, "iStage", 3);
        manager.PrepareReturnToTitle();
        Assert.That(manager.GetCurrentStage(), Is.Zero);
        Assert.That(manager.IsStageTransitionPending(), Is.True);

        new PlayerAddedRemovedHandler(manager).OnRemoved(null);
        Assert.That(manager.GetCurrentStage(), Is.Zero,
            "Destroying players during title teardown must not schedule another game scene.");

        manager.CancelInvoke();
        Object.Destroy(managerObject);
        Object.Destroy(dispatcherObject);
        yield return null;
    }

    [Test]
    public void ThrowModeMakesItsSpeedExplicitAndOtherModesStayInactive()
    {
        Assert.That(GameStatusHudController.FormatDropModeStatus(
            BOM_ATTACK.BOM_ATTACK_THROW, 3), Is.EqualTo("THROW SPD 3 / 5"));
        Assert.That(GameStatusHudController.FormatThrowSpeedStatus(
            BOM_ATTACK.BOM_ATTACK_THROW, 3), Is.EqualTo("3 / 5"));
        Assert.That(GameStatusHudController.FormatThrowSpeedStatus(
            BOM_ATTACK.BOM_ATTACK_THROW, 5), Is.EqualTo("5 / 5 MAX"));
        Assert.That(GameStatusHudController.FormatThrowSpeedStatus(
            BOM_ATTACK.BOM_ATTACK_MULTI, 3), Is.EqualTo("INACTIVE"));
        Assert.That(GameStatusHudController.FormatCappedStatus(7, 7),
            Is.EqualTo("7 / 7 MAX"));
        Assert.That(GameStatusHudController.FormatUnlimitedStatus(6),
            Is.EqualTo("6 / NO MAX"));
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
        RectTransform invincibleCell = canvasObject.transform
            .Find("ItemStatus/Invincible") as RectTransform;
        Assert.That(invincibleCell, Is.Not.Null);
        Assert.That(invincibleCell.sizeDelta.x, Is.GreaterThan(100f));
        RectTransform magnetCell = canvasObject.transform
            .Find("ItemStatus/Magnet") as RectTransform;
        Assert.That(magnetCell, Is.Not.Null);
        Assert.That(magnetCell.sizeDelta.x, Is.GreaterThan(100f));
        Assert.That(hud.GetCellValue("Fire"), Is.EqualTo("3 / NO MAX"));
        Assert.That(hud.GetCellValue("Bomb"), Is.EqualTo("3 / NO MAX"));
        Assert.That(hud.GetCellValue("Move"), Is.EqualTo("3 / 7"));
        Assert.That(hud.GetCellValue("Kick"), Is.EqualTo("NOT OWNED"));
        Assert.That(hud.GetCellValue("Invincible"), Is.EqualTo("NONE"));
        Assert.That(hud.GetCellActive("Invincible"), Is.False);
        Assert.That(hud.GetCellValue("Magnet"), Is.EqualTo("NONE"));
        Assert.That(hud.GetCellActive("Magnet"), Is.False);
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

        Assert.That(hud.GetCellValue("Fire"), Is.EqualTo("4 / NO MAX"));
        Assert.That(hud.GetCellValue("Bomb"), Is.EqualTo("4 / NO MAX"));
        Assert.That(hud.GetCellValue("Move"), Is.EqualTo("4 / 7"));
        Assert.That(hud.GetCellValue("Kick"), Is.EqualTo("OWNED"));
        Assert.That(hud.GetCellMaxed("Kick"), Is.True);
        Assert.That(hud.GetCellValue("BombType"), Is.EqualTo("EXPLODE"));
        Assert.That(hud.GetCellValue("DropMode"), Is.EqualTo("THROW SPD 2 / 5"));
        Assert.That(hud.GetCellValue("ThrowSpeed"), Is.EqualTo("2 / 5"));
        Assert.That(hud.GetCellActive("ThrowSpeed"), Is.True);
        Assert.That(hud.GetCellIconName("BombType"), Does.EndWith("TypeExplode"));
        Assert.That(hud.GetCellIconName("DropMode"), Does.EndWith("ModeThrow"));

        for (int i = 0; i < 5; i++)
        {
            bomb.Request(ReqType.BomSpeedUp);
            movement.SpeedUp();
        }
        bomb.Request(ReqType.FireUp);
        bomb.Request(ReqType.FireUp);
        bomb.Request(ReqType.BomUp);
        bomb.Request(ReqType.BomUp);
        hud.RefreshNow();
        Assert.That(hud.GetCellValue("Fire"), Is.EqualTo("6 / NO MAX"));
        Assert.That(hud.GetCellValue("Bomb"), Is.EqualTo("6 / NO MAX"));
        Assert.That(hud.GetCellMaxed("Fire"), Is.False);
        Assert.That(hud.GetCellMaxed("Bomb"), Is.False);
        Assert.That(hud.GetCellValue("Move"), Is.EqualTo("7 / 7 MAX"));
        Assert.That(hud.GetCellMaxed("Move"), Is.True);
        Assert.That(hud.GetCellValue("DropMode"), Is.EqualTo("THROW SPD 5 / 5 MAX"));
        Assert.That(hud.GetCellValue("ThrowSpeed"), Is.EqualTo("5 / 5 MAX"));
        Assert.That(hud.GetCellMaxed("ThrowSpeed"), Is.True);

        PlayerInvincibility shield = playerObject.GetComponent<PlayerInvincibility>();
        shield.Activate(PlayerInvincibility.DefaultDurationSeconds);
        hud.RefreshNow();
        Assert.That(hud.GetCellValue("Invincible"), Is.EqualTo("6s"));
        Assert.That(hud.GetCellActive("Invincible"), Is.True);
        Assert.That(hud.GetCellIconName("Invincible"), Does.EndWith("Invincible"));

        PlayerItemMagnet magnet = playerObject.GetComponent<PlayerItemMagnet>();
        magnet.Activate(PlayerItemMagnet.DefaultDurationSeconds);
        hud.RefreshNow();
        Assert.That(hud.GetCellValue("Magnet"), Is.EqualTo("8s"));
        Assert.That(hud.GetCellActive("Magnet"), Is.True);
        Assert.That(hud.GetCellIconName("Magnet"), Does.EndWith("Magnet"));
        Assert.That(hud.GetCellActive("Invincible"), Is.True,
            "Timed shield and magnet effects must coexist.");

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
    public IEnumerator TimedInvincibilitySpawnsInEveryModeBlocksDamageAndRestoresVisibility()
    {
        Assert.That(new ItemPathProvider_CpuMode().GetItemPaths()["Item_Invincible"],
            Is.EqualTo("item_invincible"));
        Assert.That(new ItemPathProvider_Tower().GetItemPaths()["Item_Invincible"],
            Is.EqualTo("item_invincible"));
        Assert.That(new ItemPathProvider_Online().GetItemPaths()["Item_Invincible"],
            Is.EqualTo("item_invincible"));
        Assert.That(Resources.Load<GameObject>("item_invincible"), Is.Not.Null);

        GameObject field = new GameObject("Field");
        field.AddComponent<PlayerPowerManager_CpuMode>();
        GameObject canvasObject = Object.Instantiate(
            Resources.Load<GameObject>("CanvasPowerGage"));
        canvasObject.name = "CanvasPowerGage1";
        canvasObject.GetComponent<PowerGage_Slider>().SetPlayerNo(1);

        GameObject playerObject = new GameObject("Player1", typeof(Animator),
            typeof(Rigidbody));
        playerObject.AddComponent<Player>();
        PowerGageIF_CpuMode power = playerObject.AddComponent<PowerGageIF_CpuMode>();
        power.SetCanvasInsID(canvasObject.GetInstanceID());
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.transform.SetParent(playerObject.transform, false);
        Renderer playerPrefabRenderer = Resources.Load<GameObject>("Player")
            .GetComponentInChildren<Renderer>(true);
        Assert.That(playerPrefabRenderer, Is.Not.Null);
        visual.GetComponent<Renderer>().sharedMaterial =
            playerPrefabRenderer.sharedMaterial;

        GameObject controlObject = new GameObject("ItemControlProbe");
        InvincibleItemControlProbe control =
            controlObject.AddComponent<InvincibleItemControlProbe>();
        ItemInvincible item = control.SpawnInvincibilityItem(new Vector3(20f, 2f, 20f));
        Assert.That(item, Is.Not.Null);

        yield return null;
        yield return null;
        Assert.That(power.IsPowerGageReady(), Is.True);
        float initialPower = power.GetCurrentPower();
        Renderer visualRenderer = visual.GetComponent<Renderer>();
        Shader originalShader = visualRenderer.sharedMaterial.shader;
        Texture originalTexture = visualRenderer.sharedMaterial.GetTexture("_MainTex");
        Assert.That(visualRenderer.sharedMaterial.HasProperty("_Color"), Is.False,
            "The production player uses Unlit/Texture, which exposed the old visual bug.");

        item.Reflection(playerObject);
        PlayerInvincibility effect = playerObject.GetComponent<PlayerInvincibility>();
        Assert.That(effect.IsInvincible, Is.True);
        Assert.That(effect.RemainingSeconds,
            Is.GreaterThan(PlayerInvincibility.DefaultDurationSeconds - 0.5f));
        Assert.That(visualRenderer.material.HasProperty("_Color"), Is.True);
        Assert.That(visualRenderer.material.GetColor("_Color").a,
            Is.LessThan(0.7f));
        Assert.That(visualRenderer.material.GetTag("RenderType", false),
            Is.EqualTo("Transparent"));
        Assert.That(effect.CapturedRendererCount, Is.EqualTo(1));
        Assert.That(playerObject.transform.Find("GhostShieldAura"), Is.Not.Null,
            "The shield needs an unmistakable cyan visual around the player.");
        Transform shieldStatus = playerObject.transform.Find("GhostShieldStatus");
        Assert.That(shieldStatus, Is.Not.Null,
            "The active shield must show a readable status above the player.");
        TextMesh shieldStatusText = shieldStatus.GetComponent<TextMesh>();
        Assert.That(shieldStatusText.text, Does.StartWith("INVINCIBLE"));
        Assert.That(shieldStatus.gameObject.activeSelf, Is.True);

        power.SetDamage(1);
        power.SyncSetDamage(1);
        Assert.That(power.GetCurrentPower(), Is.EqualTo(initialPower),
            "Both local and synchronized damage must be ignored while protected.");

        effect.Deactivate();
        Assert.That(effect.IsInvincible, Is.False);
        Assert.That(shieldStatus.gameObject.activeSelf, Is.False);
        Assert.That(visualRenderer.sharedMaterial.shader, Is.EqualTo(originalShader));
        Assert.That(visualRenderer.sharedMaterial.GetTexture("_MainTex"),
            Is.EqualTo(originalTexture));
        power.SetDamage(1);
        Assert.That(power.GetCurrentPower(), Is.EqualTo(initialPower - 1f));

        effect.Activate(1.2f);
        bool sawVisibleFrame = false;
        bool sawHiddenFrame = false;
        for (int sample = 0; sample < 15; sample++)
        {
            yield return new WaitForSecondsRealtime(0.06f);
            sawVisibleFrame |= visualRenderer.enabled;
            sawHiddenFrame |= !visualRenderer.enabled;
        }
        Assert.That(sawVisibleFrame && sawHiddenFrame, Is.True,
            "The real player renderer must visibly blink before expiration.");
        yield return new WaitForSecondsRealtime(0.35f);
        Assert.That(effect.IsInvincible, Is.False,
            "The shield must expire instead of becoming permanent.");
        Assert.That(visualRenderer.enabled, Is.True);
        Assert.That(visualRenderer.sharedMaterial.shader, Is.EqualTo(originalShader));
        Assert.That(visualRenderer.sharedMaterial.GetTexture("_MainTex"),
            Is.EqualTo(originalTexture));

        Object.Destroy(item.gameObject);
        Object.Destroy(controlObject);
        Object.Destroy(playerObject);
        Object.Destroy(canvasObject);
        Object.Destroy(field);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TimedMagnetSpawnsInEveryModePullsVisibleItemsAndRespectsWalls()
    {
        Assert.That(new ItemPathProvider_CpuMode().GetItemPaths()["Item_Magnet"],
            Is.EqualTo("item_invincible"));
        Assert.That(new ItemPathProvider_Tower().GetItemPaths()["Item_Magnet"],
            Is.EqualTo("item_invincible"));
        Assert.That(new ItemPathProvider_Online().GetItemPaths()["Item_Magnet"],
            Is.EqualTo("item_invincible"));

        GameObject controlObject = new GameObject("ItemControlProbe");
        InvincibleItemControlProbe control =
            controlObject.AddComponent<InvincibleItemControlProbe>();
        ItemMagnet pickup = control.SpawnMagnetItem(new Vector3(20f, 2f, 20f));
        Assert.That(pickup, Is.Not.Null);
        Assert.That(pickup.transform.Find("MagnetRedArm"), Is.Not.Null);
        Assert.That(pickup.transform.Find("MagnetBlueArm"), Is.Not.Null);
        Assert.That(pickup.GetComponentsInChildren<Renderer>(true).Length,
            Is.GreaterThanOrEqualTo(6),
            "The ground pickup must show an unmistakable red/blue U magnet.");

        GameObject playerObject = new GameObject("Player1");
        PlayerItemMagnet magnet = playerObject.AddComponent<PlayerItemMagnet>();
        pickup.Reflection(playerObject);
        Assert.That(magnet.IsActive, Is.True);
        Assert.That(magnet.RemainingSeconds,
            Is.GreaterThan(PlayerItemMagnet.DefaultDurationSeconds - 0.5f));

        GameObject nearItemObject = new GameObject("NearItem", typeof(BoxCollider));
        nearItemObject.GetComponent<BoxCollider>().isTrigger = true;
        nearItemObject.AddComponent<ItemInvincible>();
        nearItemObject.transform.position = new Vector3(3f, 0f, 0f);

        GameObject blockedItemObject = new GameObject("BlockedItem", typeof(BoxCollider));
        blockedItemObject.GetComponent<BoxCollider>().isTrigger = true;
        blockedItemObject.AddComponent<ItemInvincible>();
        blockedItemObject.transform.position = new Vector3(0f, 0f, 3f);

        GameObject farItemObject = new GameObject("FarItem", typeof(BoxCollider));
        farItemObject.GetComponent<BoxCollider>().isTrigger = true;
        farItemObject.AddComponent<ItemInvincible>();
        farItemObject.transform.position = new Vector3(6f, 0f, 0f);

        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "SolidBlock";
        wall.transform.position = new Vector3(0f, 0.25f, 1.5f);
        wall.transform.localScale = new Vector3(1f, 1.5f, 0.5f);

        Physics.SyncTransforms();
        Vector3 nearStart = nearItemObject.transform.position;
        Vector3 blockedStart = blockedItemObject.transform.position;
        Vector3 farStart = farItemObject.transform.position;
        for (int frame = 0; frame < 10; frame++) yield return null;

        Assert.That(Vector3.Distance(nearItemObject.transform.position,
            playerObject.transform.position),
            Is.LessThan(Vector3.Distance(nearStart, playerObject.transform.position) - 0.25f));
        Assert.That(Vector3.Distance(blockedItemObject.transform.position, blockedStart),
            Is.LessThan(0.001f),
            "The magnet must not pull an item through a solid block.");
        Assert.That(Vector3.Distance(farItemObject.transform.position, farStart),
            Is.LessThan(0.001f),
            "Items outside the attraction radius must stay in place.");
        Assert.That(magnet.AttractedItemCountLastFrame, Is.GreaterThan(0));

        magnet.Activate(0.05f);
        magnet.Deactivate();
        magnet.Activate(0.05f);
        yield return new WaitForSecondsRealtime(0.08f);
        Assert.That(magnet.IsActive, Is.False,
            "The magnet must expire instead of becoming permanent.");

        Object.Destroy(wall);
        Object.Destroy(farItemObject);
        Object.Destroy(blockedItemObject);
        Object.Destroy(nearItemObject);
        Object.Destroy(playerObject);
        Object.Destroy(pickup.gameObject);
        Object.Destroy(controlObject);
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
        AudioSource source = sound.GetComponent<AudioSource>();
        Assert.That(source, Is.Not.Null);
        Assert.That(source.volume, Is.EqualTo(SoundManager.MasterOutputVolume).Within(0.001f));
        Assert.That(SoundManager.DropBombVolumeScale, Is.LessThanOrEqualTo(0.5f));
        Assert.That(SoundManager.GetItemVolumeScale, Is.LessThanOrEqualTo(0.4f));
        Assert.That(SoundManager.ExplosionVolumeScale, Is.LessThanOrEqualTo(0.55f));
        AssertComfortableClip(sound.sDropBomb, "bomb_place_soft", 0.12f, 0.22f);
        AssertComfortableClip(sound.sGetItem, "item_collect_soft", 0.25f, 0.40f);
        AssertComfortableClip(sound.sExploison, "explosion_soft", 0.45f, 0.65f);

        int before = sound.PlayedEffectCount;
        Assert.That(sound.PlaySoundEffect("DROPBOMB"), Is.True);
        Assert.That(sound.PlaySoundEffect("DROPBOMB"), Is.False,
            "Rapid bomb placement sounds should not stack into a sharp peak.");
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
        int beforeItem = sound.PlayedEffectCount;
        Assert.That(sound.PlaySoundEffect("GETITEM"), Is.True);
        Assert.That(sound.PlaySoundEffect("GETITEM"), Is.False,
            "Rapid item chimes should not stack into a piercing peak.");
        Assert.That(sound.PlayedEffectCount, Is.EqualTo(beforeItem + 1));
        Assert.That(sound.PlaySoundEffect("EXPLOISON"), Is.True);
        Assert.That(sound.PlaySoundEffect("EXPLOISON"), Is.False,
            "Simultaneous explosion tiles should share one sound burst.");

        sound.SetSoundEnabled(previousPreference != 0);
        Object.Destroy(canvasObject);
        Object.Destroy(soundObject);
        yield return null;
    }

    private static void AssertComfortableClip(AudioClip clip, string expectedName,
        float minimumLength, float maximumLength)
    {
        Assert.That(clip, Is.Not.Null);
        Assert.That(clip.name, Is.EqualTo(expectedName));
        Assert.That(clip.channels, Is.EqualTo(1));
        Assert.That(clip.length, Is.InRange(minimumLength, maximumLength));

        float[] samples = new float[clip.samples * clip.channels];
        Assert.That(clip.GetData(samples, 0), Is.True);

        float peak = 0f;
        double squareSum = 0d;
        double sampleSum = 0d;
        for (int i = 0; i < samples.Length; i++)
        {
            float absolute = Mathf.Abs(samples[i]);
            if (absolute > peak) peak = absolute;
            squareSum += samples[i] * samples[i];
            sampleSum += samples[i];
        }

        float rms = Mathf.Sqrt((float)(squareSum / samples.Length));
        float dcOffset = Mathf.Abs((float)(sampleSum / samples.Length));
        Assert.That(peak, Is.InRange(0.08f, 0.76f), "Clip must retain safe headroom.");
        Assert.That(rms, Is.InRange(0.01f, 0.30f), "Clip loudness is outside the safe test range.");
        Assert.That(dcOffset, Is.LessThan(0.02f), "Clip has an audible DC offset.");
        Assert.That(Mathf.Abs(samples[0]), Is.LessThan(0.01f));
        Assert.That(Mathf.Abs(samples[samples.Length - 1]), Is.LessThan(0.01f));
    }

    private static void SetPrivateField(object target, string name, object value)
    {
        FieldInfo field = target.GetType().GetField(name,
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, name);
        field.SetValue(target, value);
    }
}

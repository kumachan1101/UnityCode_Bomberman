using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public sealed class ReturnButtonProbe : ButtonClickScript
{
    public int ClickCount { get; private set; }

    public override void LoadGameScene()
    {
        ClickCount++;
    }
}

public class ResponsiveGameUiControllerTests
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
    public IEnumerator AllFourGameModesUseTheSharedResponsiveControls()
    {
        System.Type[] screenTypes =
        {
            typeof(GameSceneScreenManager),
            typeof(GameOnlineScreenManager),
            typeof(GameTowerSceneManager),
            typeof(GameTowerOnlineScreenManager)
        };

        foreach (System.Type screenType in screenTypes)
        {
            GameObject screenObject = new GameObject(screenType.Name);
            BaseScreenManager screen = (BaseScreenManager)screenObject.AddComponent(screenType);
            InvokeInitializeCanvas(screen);
            GameObject canvas = GetCurrentCanvas(screen);
            yield return null;

            Assert.That(canvas.GetComponent<ResponsiveGameUiController>(), Is.Not.Null,
                screenType.Name);
            AssertScaler(canvas.GetComponent<CanvasScaler>(), screenType.Name);

            RectTransform joystick = canvas.transform.Find("JoystickPlayer") as RectTransform;
            RectTransform bomb = canvas.transform.Find("bom") as RectTransform;
            Assert.That(joystick.anchorMin, Is.EqualTo(Vector2.zero), screenType.Name);
            Assert.That(joystick.sizeDelta,
                Is.EqualTo(ResponsiveGameUiController.MovementJoystickSize), screenType.Name);
            Assert.That(bomb.anchorMin, Is.EqualTo(Vector2.right), screenType.Name);
            Assert.That(bomb.sizeDelta,
                Is.EqualTo(ResponsiveGameUiController.BombButtonSize), screenType.Name);
            Assert.That(joystick.anchoredPosition.x, Is.GreaterThan(0f), screenType.Name);
            Assert.That(bomb.anchoredPosition.x, Is.LessThan(0f), screenType.Name);

            Object.Destroy(canvas);
            Object.Destroy(screenObject);
            yield return null;
        }
    }

    [UnityTest]
    public IEnumerator GaugesAndReturnButtonUseLargeSafeAnchoredLayout()
    {
        GameSceneScreenManager screen =
            new GameObject("Screen under test").AddComponent<GameSceneScreenManager>();
        InvokeInitializeCanvas(screen);
        GameObject mainCanvas = GetCurrentCanvas(screen);
        ResponsiveGameUiController responsive =
            mainCanvas.GetComponent<ResponsiveGameUiController>();

        GameObject returnCanvas = Object.Instantiate(
            Resources.Load<GameObject>("GameEndCanvas_Local"));
        GameObject playerGaugeObject = Object.Instantiate(
            Resources.Load<GameObject>("CanvasPowerGage"));
        GameObject towerGaugeObject = Object.Instantiate(
            Resources.Load<GameObject>("CanvasPowerGageTower"));
        playerGaugeObject.GetComponent<PowerGage_Slider>().SetPlayerCnt(1);
        towerGaugeObject.GetComponent<PowerGage_Slider>().SetPlayerCnt(1);
        yield return null;
        responsive.RefreshLayout();

        RectTransform returnButton = returnCanvas.transform.Find("ReturnTitle") as RectTransform;
        RectTransform playerGauge = playerGaugeObject.transform.Find("Slider") as RectTransform;
        RectTransform towerGauge = towerGaugeObject.transform.Find("Slider") as RectTransform;

        AssertScaler(returnCanvas.GetComponent<CanvasScaler>(), "ReturnTitle");
        AssertScaler(playerGaugeObject.GetComponent<CanvasScaler>(), "player gauge");
        AssertScaler(towerGaugeObject.GetComponent<CanvasScaler>(), "tower gauge");
        Assert.That(returnButton.anchorMin, Is.EqualTo(new Vector2(0.5f, 1f)));
        Assert.That(returnButton.sizeDelta,
            Is.EqualTo(ResponsiveGameUiController.ReturnButtonSize));
        Assert.That(returnButton.anchoredPosition.y, Is.LessThan(0f));
        Assert.That(playerGauge.anchorMin, Is.EqualTo(new Vector2(0f, 1f)));
        Assert.That(towerGauge.anchorMin, Is.EqualTo(Vector2.one));
        Assert.That(playerGauge.sizeDelta,
            Is.EqualTo(ResponsiveGameUiController.PowerGaugeSize));
        Assert.That(towerGauge.sizeDelta,
            Is.EqualTo(ResponsiveGameUiController.PowerGaugeSize));
        Assert.That(playerGauge.anchoredPosition.x, Is.GreaterThan(0f));
        Assert.That(towerGauge.anchoredPosition.x, Is.LessThan(0f));
    }

    [UnityTest]
    public IEnumerator CompactLayoutAvoidsOverlapAndLeavesUnrelatedCanvasesAlone()
    {
        GameSceneScreenManager screen =
            new GameObject("Screen under test").AddComponent<GameSceneScreenManager>();
        InvokeInitializeCanvas(screen);
        ResponsiveGameUiController responsive =
            GetCurrentCanvas(screen).GetComponent<ResponsiveGameUiController>();

        GameObject unrelated = new GameObject("Unrelated Canvas",
            typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
        CanvasScaler unrelatedScaler = unrelated.GetComponent<CanvasScaler>();
        unrelatedScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        unrelatedScaler.referenceResolution = new Vector2(333f, 444f);
        responsive.RefreshLayout();
        yield return null;

        Assert.That(unrelatedScaler.uiScaleMode,
            Is.EqualTo(CanvasScaler.ScaleMode.ConstantPixelSize));
        Assert.That(unrelatedScaler.referenceResolution,
            Is.EqualTo(new Vector2(333f, 444f)));

        float compactGaugeTop = InvokePrivateLayoutMargin("GetPowerGaugeTopMargin", 650f);
        float compactTowerBottom = InvokePrivateLayoutMargin("GetTowerActionBottomMargin", 650f);
        float wideGaugeTop = InvokePrivateLayoutMargin("GetPowerGaugeTopMargin", 1280f);
        float wideTowerBottom = InvokePrivateLayoutMargin("GetTowerActionBottomMargin", 1280f);
        float veryNarrowGaugeWidth = InvokePrivateLayoutMargin("GetPowerGaugeWidth", 500f);
        Assert.That(compactGaugeTop, Is.GreaterThan(ResponsiveGameUiController.ReturnButtonSize.y));
        Assert.That(compactTowerBottom,
            Is.GreaterThan(ResponsiveGameUiController.MovementJoystickSize.y));
        Assert.That(wideGaugeTop, Is.LessThan(ResponsiveGameUiController.ReturnButtonSize.y));
        Assert.That(wideTowerBottom,
            Is.LessThan(ResponsiveGameUiController.MovementJoystickSize.y));
        Assert.That(veryNarrowGaugeWidth,
            Is.LessThan(ResponsiveGameUiController.PowerGaugeSize.x));
        Assert.That(veryNarrowGaugeWidth * 2f + 64f, Is.LessThanOrEqualTo(500f));
    }

    [UnityTest]
    public IEnumerator OnlineModesUseTheNetworkAwareReturnButton()
    {
        BaseScreenManager[] onlineScreens =
        {
            new GameObject("GameOnline screen").AddComponent<GameOnlineScreenManager>(),
            new GameObject("GameTowerOnline screen").AddComponent<GameTowerOnlineScreenManager>()
        };

        foreach (BaseScreenManager screen in onlineScreens)
        {
            screen.InitializeScreen();
            yield return null;
            ButtonClickScript_Online onlineButton =
                Object.FindObjectOfType<ButtonClickScript_Online>();
            Assert.That(onlineButton, Is.Not.Null, screen.GetType().Name);
            Assert.That(onlineButton, Is.InstanceOf<Photon.Realtime.IOnEventCallback>(),
                screen.GetType().Name);
            Assert.That(Object.FindObjectOfType<ButtonClickScript_CpuMode>(), Is.Null,
                screen.GetType().Name);

            foreach (Canvas canvas in Object.FindObjectsOfType<Canvas>())
                Object.Destroy(canvas.gameObject);
            yield return null;
        }

        Assert.That(ButtonClickScript_Online.ReturnTitleEventCode,
            Is.Not.EqualTo(MasterClientNotificationManager.NotificationEventCode));
    }

    [Test]
    public void PortraitAndLandscapeSafeAreasKeepBottomControlsVisible()
    {
        AssertBottomControlsInsideSafeArea(
            new Vector2(653f, 1413f),
            new Vector2(390f, 844f),
            new Rect(0f, 34f, 390f, 776f));
        AssertBottomControlsInsideSafeArea(
            new Vector2(1412f, 652f),
            new Vector2(844f, 390f),
            new Rect(44f, 21f, 756f, 348f));
    }

    [UnityTest]
    public IEnumerator ReturnButtonInvokesItsRuntimeHandlerExactlyOnce()
    {
        GameObject root = new GameObject("Return canvas", typeof(RectTransform));
        GameObject child = new GameObject("ReturnTitle",
            typeof(RectTransform), typeof(Image), typeof(Button));
        child.transform.SetParent(root.transform, false);
        ReturnButtonProbe probe = root.AddComponent<ReturnButtonProbe>();
        yield return null;

        Button button = child.GetComponent<Button>();
        Assert.That(button.interactable, Is.True);
        button.onClick.Invoke();
        button.onClick.Invoke();
        Assert.That(probe.ClickCount, Is.EqualTo(1));
    }

    [Test]
    public void WebGLPostprocessorAddsRotationResizeAndSafeAreaSupport()
    {
        const string html = @"<!DOCTYPE html>
<html><head>
    <meta charset=""utf-8"">
</head><body><script>
      if (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent)) {

        var meta = document.createElement('meta');
        meta.name = 'viewport';
        meta.content = 'width=device-width, height=device-height, initial-scale=1.0, user-scalable=no, shrink-to-fit=yes';
        document.getElementsByTagName('head')[0].appendChild(meta);
        container.className = ""unity-mobile"";
        canvas.style.width = window.innerWidth + 'px';
        canvas.style.height = window.innerHeight + 'px';
        unityShowBanner('WebGL builds are not supported on mobile devices.');
      }
</script></body></html>";

        string result = ResponsiveWebGLPostprocessor.MakeIndexResponsive(html);
        string css = ResponsiveWebGLPostprocessor.MakeStyleResponsive("body { margin: 0; }");

        Assert.That(result, Does.Contain("viewport-fit=cover"));
        Assert.That(result, Does.Contain("orientationchange"));
        Assert.That(result, Does.Contain("window.visualViewport"));
        Assert.That(result, Does.Not.Contain("WebGL builds are not supported on mobile devices"));
        Assert.That(css, Does.Contain("safe-area-inset-left"));
        Assert.That(css, Does.Contain("touch-action: none"));
        Assert.That(ResponsiveWebGLPostprocessor.MakeIndexResponsive(result), Is.EqualTo(result));
        Assert.That(ResponsiveWebGLPostprocessor.MakeStyleResponsive(css), Is.EqualTo(css));
    }

    [Test]
    public void TitleButtonsUseOnePortraitColumnAndTwoLandscapeColumns()
    {
        Vector2 portraitCanvas = new Vector2(653f, 1413f);
        Vector4 portraitInsets = new Vector4(0f, 0f, 57f, 57f);
        Vector2 portraitSize = ResponsiveTitleUiController.CalculateButtonSize(
            portraitCanvas, portraitInsets, true);
        Vector2[] portraitPositions = new Vector2[4];
        for (int index = 0; index < portraitPositions.Length; index++)
            portraitPositions[index] = ResponsiveTitleUiController.CalculateButtonPosition(
                index, portraitCanvas, portraitInsets, portraitSize, true);

        foreach (Vector2 position in portraitPositions)
            Assert.That(position.x, Is.EqualTo(0f).Within(0.01f));
        Assert.That(portraitPositions[0].y, Is.GreaterThan(portraitPositions[1].y));
        Assert.That(portraitPositions[1].y, Is.GreaterThan(portraitPositions[2].y));
        Assert.That(portraitPositions[2].y, Is.GreaterThan(portraitPositions[3].y));
        foreach (Vector2 position in portraitPositions)
        {
            Assert.That(position.x - portraitSize.x * 0.5f,
                Is.GreaterThanOrEqualTo(-portraitCanvas.x * 0.5f + portraitInsets.x));
            Assert.That(position.x + portraitSize.x * 0.5f,
                Is.LessThanOrEqualTo(portraitCanvas.x * 0.5f - portraitInsets.y));
        }

        Vector2 landscapeCanvas = new Vector2(1412f, 652f);
        Vector4 landscapeInsets = new Vector4(74f, 74f, 35f, 35f);
        Vector2 landscapeSize = ResponsiveTitleUiController.CalculateButtonSize(
            landscapeCanvas, landscapeInsets, false);
        Vector2 local = ResponsiveTitleUiController.CalculateButtonPosition(
            0, landscapeCanvas, landscapeInsets, landscapeSize, false);
        Vector2 towerLocal = ResponsiveTitleUiController.CalculateButtonPosition(
            1, landscapeCanvas, landscapeInsets, landscapeSize, false);
        Vector2 online = ResponsiveTitleUiController.CalculateButtonPosition(
            2, landscapeCanvas, landscapeInsets, landscapeSize, false);
        Assert.That(local.x, Is.GreaterThan(towerLocal.x));
        Assert.That(local.y, Is.EqualTo(towerLocal.y));
        Assert.That(local.y, Is.GreaterThan(online.y));
    }

    private static void InvokeInitializeCanvas(BaseScreenManager screen)
    {
        typeof(BaseScreenManager).GetMethod("InitializeCanvas",
            BindingFlags.Instance | BindingFlags.NonPublic).Invoke(screen, null);
    }

    private static GameObject GetCurrentCanvas(BaseScreenManager screen)
    {
        return (GameObject)typeof(BaseScreenManager).GetField("currentCanvas",
            BindingFlags.Instance | BindingFlags.NonPublic).GetValue(screen);
    }

    private static void AssertScaler(CanvasScaler scaler, string context)
    {
        Assert.That(scaler.uiScaleMode,
            Is.EqualTo(CanvasScaler.ScaleMode.ScaleWithScreenSize), context);
        Assert.That(scaler.referenceResolution,
            Is.EqualTo(ResponsiveGameUiController.ReferenceResolution), context);
        Assert.That(scaler.screenMatchMode,
            Is.EqualTo(CanvasScaler.ScreenMatchMode.MatchWidthOrHeight), context);
        Assert.That(scaler.matchWidthOrHeight, Is.EqualTo(0.5f), context);
    }

    private static float InvokePrivateLayoutMargin(string methodName, float availableWidth)
    {
        return (float)typeof(ResponsiveGameUiController).GetMethod(methodName,
            BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { availableWidth });
    }

    private static void AssertBottomControlsInsideSafeArea(Vector2 canvasSize,
        Vector2 screenSize, Rect safeArea)
    {
        Vector4 insets = ResponsiveGameUiController.CalculateSafeInsets(
            canvasSize, screenSize, safeArea);
        Vector2 joystick = ResponsiveGameUiController.CalculateBottomCornerPosition(
            canvasSize, ResponsiveGameUiController.MovementJoystickSize, insets, false);
        Vector2 bombAnchored = ResponsiveGameUiController.CalculateBottomCornerPosition(
            canvasSize, ResponsiveGameUiController.BombButtonSize, insets, true);
        Vector2 bomb = new Vector2(canvasSize.x + bombAnchored.x, bombAnchored.y);

        AssertRectInsideSafeArea(joystick,
            ResponsiveGameUiController.MovementJoystickSize, canvasSize, insets, "joystick");
        AssertRectInsideSafeArea(bomb,
            ResponsiveGameUiController.BombButtonSize, canvasSize, insets, "bomb");
        Assert.That(joystick.x + ResponsiveGameUiController.MovementJoystickSize.x * 0.5f,
            Is.LessThan(bomb.x - ResponsiveGameUiController.BombButtonSize.x * 0.5f));
    }

    private static void AssertRectInsideSafeArea(Vector2 center, Vector2 size,
        Vector2 canvasSize, Vector4 insets, string context)
    {
        Assert.That(center.x - size.x * 0.5f, Is.GreaterThanOrEqualTo(insets.x), context);
        Assert.That(center.x + size.x * 0.5f,
            Is.LessThanOrEqualTo(canvasSize.x - insets.y), context);
        Assert.That(center.y - size.y * 0.5f, Is.GreaterThanOrEqualTo(insets.w), context);
        Assert.That(center.y + size.y * 0.5f,
            Is.LessThanOrEqualTo(canvasSize.y - insets.z), context);
    }
}

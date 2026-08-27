using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

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
}

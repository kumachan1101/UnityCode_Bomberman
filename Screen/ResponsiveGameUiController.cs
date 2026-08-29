using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class ResponsiveGameUiController : MonoBehaviour
{
    public static readonly Vector2 ReferenceResolution = new Vector2(1280f, 720f);
    public static readonly Vector2 BombButtonSize = new Vector2(176f, 176f);
    public static readonly Vector2 MovementJoystickSize = new Vector2(192f, 192f);
    public static readonly Vector2 ReturnButtonSize = new Vector2(240f, 76f);
    public static readonly Vector2 PowerGaugeSize = new Vector2(260f, 32f);

    private const float Margin = 28f;
    private const float BottomControlMargin = 44f;
    private const float GaugeGap = 8f;
    private const float CompactLayoutWidth = 900f;
    private const float RefreshInterval = 0.25f;

    private int lastScreenWidth = -1;
    private int lastScreenHeight = -1;
    private Rect lastSafeArea;
    private float nextRefreshTime;

    private void OnEnable()
    {
        RefreshLayout();
    }

    private void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight ||
            Screen.safeArea != lastSafeArea || Time.unscaledTime >= nextRefreshTime)
        {
            RefreshLayout();
        }
    }

    public void RefreshLayout()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            if (IsManagedCanvas(canvas)) ConfigureScaler(canvas);
        }

        Canvas.ForceUpdateCanvases();
        LayoutMainControls();

        foreach (Canvas canvas in canvases)
        {
            LayoutReturnButton(canvas);
            LayoutTowerActionButton(canvas);
        }

        foreach (PowerGage_Slider gauge in FindObjectsOfType<PowerGage_Slider>())
            LayoutPowerGauge(gauge);

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
        lastSafeArea = Screen.safeArea;
        nextRefreshTime = Time.unscaledTime + RefreshInterval;
    }

    public static void LayoutPowerGauge(PowerGage_Slider gauge)
    {
        if (gauge == null) return;
        Canvas canvas = gauge.GetComponent<Canvas>();
        RectTransform slider = gauge.transform.Find("Slider") as RectTransform;
        RectTransform canvasRect = canvas != null ? canvas.transform as RectTransform : null;
        if (canvas == null || slider == null || canvasRect == null) return;

        ConfigureScaler(canvas);
        Vector4 safe = GetSafeInsets(canvasRect);
        Vector2 canvasSize = GetCanvasSize(canvasRect);
        float availableWidth = canvasSize.x - safe.x - safe.y;
        float topMargin = GetPowerGaugeTopMargin(availableWidth);
        Vector2 gaugeSize = new Vector2(GetPowerGaugeWidth(availableWidth), PowerGaugeSize.y);
        int position = Mathf.Max(0, gauge.GetPlayerPosition() - 1);
        int rowCapacity = Mathf.Max(1, Mathf.FloorToInt(
            (canvasSize.y - safe.z - safe.w - topMargin - Margin) /
            (PowerGaugeSize.y + GaugeGap)));
        int row = position % rowCapacity;
        int column = position / rowCapacity;
        float vertical = safe.z + topMargin + gaugeSize.y * 0.5f +
            row * (gaugeSize.y + GaugeGap);
        bool towerGauge = gauge is PowerGage_Slider_Tower;

        slider.anchorMin = slider.anchorMax = towerGauge ? Vector2.one : new Vector2(0f, 1f);
        slider.pivot = new Vector2(0.5f, 0.5f);
        slider.sizeDelta = gaugeSize;
        float horizontal = Margin + gaugeSize.x * 0.5f +
            column * (gaugeSize.x + GaugeGap);
        slider.anchoredPosition = towerGauge
            ? new Vector2(-(safe.y + horizontal), -vertical)
            : new Vector2(safe.x + horizontal, -vertical);
    }

    private static void ConfigureScaler(Canvas canvas)
    {
        if (canvas == null || canvas.renderMode == RenderMode.WorldSpace) return;
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler == null) return;
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = ReferenceResolution;
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
    }

    private static bool IsManagedCanvas(Canvas canvas)
    {
        if (canvas == null || canvas.renderMode == RenderMode.WorldSpace) return false;
        Transform root = canvas.transform;
        return root.Find("JoystickPlayer") != null || root.Find("bom") != null ||
            root.Find("ReturnTitle") != null || root.Find("SpawnPlayer") != null ||
            canvas.GetComponent<PowerGage_Slider>() != null;
    }

    private void LayoutMainControls()
    {
        Canvas canvas = GetComponent<Canvas>();
        RectTransform canvasRect = transform as RectTransform;
        if (canvas == null || canvasRect == null) return;

        Vector4 safe = GetSafeInsets(canvasRect);
        Vector2 canvasSize = GetCanvasSize(canvasRect);
        LayoutBottomCorner(transform.Find("JoystickPlayer") as RectTransform,
            canvasSize, MovementJoystickSize, safe, false);
        LayoutBottomCorner(transform.Find("bom") as RectTransform,
            canvasSize, BombButtonSize, safe, true);

        Transform bomb = transform.Find("bom");
        if (bomb != null)
            ConfigureLabel(bomb.GetComponentInChildren<Text>(true), 24, 56);
    }

    private static void LayoutBottomCorner(RectTransform rect, Vector2 canvasSize,
        Vector2 size, Vector4 safeInsets, bool right)
    {
        if (rect == null) return;
        rect.anchorMin = rect.anchorMax = right ? Vector2.right : Vector2.zero;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = CalculateBottomCornerPosition(
            canvasSize, size, safeInsets, right);
    }

    // Safe-area clamping also protects controls when the WebGL canvas changes size
    // while a phone is rotating or while the browser toolbar is expanding.
    public static Vector2 CalculateBottomCornerPosition(Vector2 canvasSize,
        Vector2 size, Vector4 safeInsets, bool right)
    {
        float halfWidth = size.x * 0.5f;
        float halfHeight = size.y * 0.5f;
        float left = safeInsets.x + BottomControlMargin + halfWidth;
        float rightEdge = canvasSize.x - safeInsets.y - BottomControlMargin - halfWidth;
        float bottom = safeInsets.w + BottomControlMargin + halfHeight;
        float top = canvasSize.y - safeInsets.z - BottomControlMargin - halfHeight;

        float x = right ? Mathf.Max(left, rightEdge) : Mathf.Min(left, rightEdge);
        float y = Mathf.Min(bottom, top);
        x = Mathf.Clamp(x, halfWidth, Mathf.Max(halfWidth, canvasSize.x - halfWidth));
        y = Mathf.Clamp(y, halfHeight, Mathf.Max(halfHeight, canvasSize.y - halfHeight));
        return new Vector2(right ? x - canvasSize.x : x, y);
    }

    private static void LayoutReturnButton(Canvas canvas)
    {
        RectTransform canvasRect = canvas.transform as RectTransform;
        RectTransform button = canvas.transform.Find("ReturnTitle") as RectTransform;
        if (canvasRect == null || button == null) return;
        Vector4 safe = GetSafeInsets(canvasRect);
        button.anchorMin = button.anchorMax = new Vector2(0.5f, 1f);
        button.pivot = new Vector2(0.5f, 0.5f);
        button.sizeDelta = ReturnButtonSize;
        button.anchoredPosition = new Vector2(0f,
            -(safe.z + Margin + ReturnButtonSize.y * 0.5f));
        ConfigureLabel(button.GetComponentInChildren<Text>(true), 20, 38);
    }

    private static void LayoutTowerActionButton(Canvas canvas)
    {
        RectTransform canvasRect = canvas.transform as RectTransform;
        RectTransform button = canvas.transform.Find("SpawnPlayer") as RectTransform;
        if (canvasRect == null || button == null) return;
        Vector4 safe = GetSafeInsets(canvasRect);
        Vector2 canvasSize = GetCanvasSize(canvasRect);
        float availableWidth = canvasSize.x - safe.x - safe.y;
        Vector2 size = new Vector2(200f, 104f);
        button.anchorMin = button.anchorMax = new Vector2(0.5f, 0f);
        button.pivot = new Vector2(0.5f, 0.5f);
        button.sizeDelta = size;
        button.anchoredPosition = new Vector2(0f,
            safe.w + GetTowerActionBottomMargin(availableWidth) + size.y * 0.5f);
    }

    private static float GetPowerGaugeTopMargin(float availableWidth)
    {
        if (availableWidth >= CompactLayoutWidth) return Margin;
        return GameStatusHudController.StagePanelTopMargin + ReturnButtonSize.y +
            GameStatusHudController.PanelGap + GameStatusHudController.StagePanelHeight +
            GaugeGap;
    }

    private static float GetPowerGaugeWidth(float availableWidth)
    {
        float twoColumnWidth = (availableWidth - Margin * 2f - GaugeGap) * 0.5f;
        return Mathf.Clamp(twoColumnWidth, 120f, PowerGaugeSize.x);
    }

    private static float GetTowerActionBottomMargin(float availableWidth)
    {
        float statusClearance = GameStatusHudController.StatusPanelBottomMargin +
            GameStatusHudController.StatusPanelHeight + GameStatusHudController.PanelGap;
        if (availableWidth >= CompactLayoutWidth) return Mathf.Max(Margin, statusClearance);
        return Mathf.Max(statusClearance,
            Margin + Mathf.Max(MovementJoystickSize.y, BombButtonSize.y) + GaugeGap * 2f);
    }

    private static void ConfigureLabel(Text text, int minSize, int maxSize)
    {
        if (text == null) return;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = minSize;
        text.resizeTextMaxSize = maxSize;
    }

    // x: left, y: right, z: top, w: bottom (all in Canvas units).
    private static Vector4 GetSafeInsets(RectTransform canvasRect)
    {
        Vector2 size = GetCanvasSize(canvasRect);
        float screenWidth = Mathf.Max(1f, Screen.width);
        float screenHeight = Mathf.Max(1f, Screen.height);
        return CalculateSafeInsets(size, new Vector2(screenWidth, screenHeight),
            Screen.safeArea);
    }

    public static Vector4 CalculateSafeInsets(Vector2 canvasSize,
        Vector2 screenSize, Rect safeArea)
    {
        float screenWidth = Mathf.Max(1f, screenSize.x);
        float screenHeight = Mathf.Max(1f, screenSize.y);
        return new Vector4(
            Mathf.Max(0f, safeArea.xMin) / screenWidth * canvasSize.x,
            Mathf.Max(0f, screenWidth - safeArea.xMax) / screenWidth * canvasSize.x,
            Mathf.Max(0f, screenHeight - safeArea.yMax) / screenHeight * canvasSize.y,
            Mathf.Max(0f, safeArea.yMin) / screenHeight * canvasSize.y);
    }

    private static Vector2 GetCanvasSize(RectTransform canvasRect)
    {
        Vector2 size = canvasRect.rect.size;
        if (size.x <= 0f || size.y <= 0f) return ReferenceResolution;
        return size;
    }
}

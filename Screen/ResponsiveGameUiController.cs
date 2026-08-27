using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class ResponsiveGameUiController : MonoBehaviour
{
    public static readonly Vector2 ReferenceResolution = new Vector2(1280f, 720f);
    public static readonly Vector2 BombButtonSize = new Vector2(176f, 176f);
    public static readonly Vector2 MovementJoystickSize = new Vector2(192f, 192f);
    public static readonly Vector2 ReturnButtonSize = new Vector2(200f, 64f);
    public static readonly Vector2 PowerGaugeSize = new Vector2(260f, 32f);

    private const float Margin = 28f;
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
        LayoutBottomCorner(transform.Find("JoystickPlayer") as RectTransform,
            MovementJoystickSize, safe.x, safe.w, false);
        LayoutBottomCorner(transform.Find("bom") as RectTransform,
            BombButtonSize, safe.y, safe.w, true);

        Transform bomb = transform.Find("bom");
        if (bomb != null)
            ConfigureLabel(bomb.GetComponentInChildren<Text>(true), 24, 56);
    }

    private static void LayoutBottomCorner(RectTransform rect, Vector2 size,
        float sideInset, float bottomInset, bool right)
    {
        if (rect == null) return;
        rect.anchorMin = rect.anchorMax = right ? Vector2.right : Vector2.zero;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        float x = sideInset + Margin + size.x * 0.5f;
        rect.anchoredPosition = new Vector2(right ? -x : x,
            bottomInset + Margin + size.y * 0.5f);
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
        return Margin + ReturnButtonSize.y + GaugeGap * 2f;
    }

    private static float GetPowerGaugeWidth(float availableWidth)
    {
        float twoColumnWidth = (availableWidth - Margin * 2f - GaugeGap) * 0.5f;
        return Mathf.Clamp(twoColumnWidth, 120f, PowerGaugeSize.x);
    }

    private static float GetTowerActionBottomMargin(float availableWidth)
    {
        if (availableWidth >= CompactLayoutWidth) return Margin;
        return Margin + Mathf.Max(MovementJoystickSize.y, BombButtonSize.y) + GaugeGap * 2f;
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
        Rect safeArea = Screen.safeArea;
        return new Vector4(
            safeArea.xMin / screenWidth * size.x,
            (screenWidth - safeArea.xMax) / screenWidth * size.x,
            (screenHeight - safeArea.yMax) / screenHeight * size.y,
            safeArea.yMin / screenHeight * size.y);
    }

    private static Vector2 GetCanvasSize(RectTransform canvasRect)
    {
        Vector2 size = canvasRect.rect.size;
        if (size.x <= 0f || size.y <= 0f) return ReferenceResolution;
        return size;
    }
}

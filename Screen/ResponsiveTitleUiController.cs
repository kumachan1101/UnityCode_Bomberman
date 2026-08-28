using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class ResponsiveTitleUiController : MonoBehaviour
{
    public static readonly Vector2 PreferredButtonSize = new Vector2(460f, 120f);

    private const float Margin = 44f;
    private const float Gap = 40f;
    private const float RefreshInterval = 0.25f;
    private static readonly string[] ButtonNames =
    {
        "LocalPlay", "TowerLocalPlay", "OnlinePlay", "TowerOnlinePlay"
    };

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
        Canvas canvas = GetComponent<Canvas>();
        RectTransform canvasRect = transform as RectTransform;
        if (canvas == null || canvasRect == null) return;

        CanvasScaler scaler = GetComponent<CanvasScaler>();
        if (scaler != null)
        {
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = ResponsiveGameUiController.ReferenceResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
        }

        Canvas.ForceUpdateCanvases();
        Vector2 canvasSize = canvasRect.rect.size;
        if (canvasSize.x <= 0f || canvasSize.y <= 0f)
            canvasSize = ResponsiveGameUiController.ReferenceResolution;
        Vector4 safeInsets = ResponsiveGameUiController.CalculateSafeInsets(
            canvasSize, new Vector2(Screen.width, Screen.height), Screen.safeArea);
        bool portrait = canvasSize.y > canvasSize.x;
        Vector2 buttonSize = CalculateButtonSize(canvasSize, safeInsets, portrait);

        for (int index = 0; index < ButtonNames.Length; index++)
        {
            RectTransform button = transform.Find(ButtonNames[index]) as RectTransform;
            if (button == null) continue;
            button.anchorMin = button.anchorMax = new Vector2(0.5f, 0.5f);
            button.pivot = new Vector2(0.5f, 0.5f);
            button.sizeDelta = buttonSize;
            button.anchoredPosition = CalculateButtonPosition(
                index, canvasSize, safeInsets, buttonSize, portrait);

            Text text = button.GetComponentInChildren<Text>(true);
            if (text != null)
            {
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 20;
                text.resizeTextMaxSize = 42;
            }
        }

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
        lastSafeArea = Screen.safeArea;
        nextRefreshTime = Time.unscaledTime + RefreshInterval;
    }

    public static Vector2 CalculateButtonSize(Vector2 canvasSize,
        Vector4 safeInsets, bool portrait)
    {
        float availableWidth = Mathf.Max(1f, canvasSize.x - safeInsets.x - safeInsets.y);
        float availableHeight = Mathf.Max(1f, canvasSize.y - safeInsets.z - safeInsets.w);
        float maxWidth = portrait
            ? availableWidth - Margin * 2f
            : (availableWidth - Margin * 2f - Gap) * 0.5f;
        float maxHeight = portrait
            ? (availableHeight - Margin * 2f - Gap * 3f) * 0.25f
            : (availableHeight - Margin * 2f - Gap) * 0.5f;
        return new Vector2(
            Mathf.Clamp(PreferredButtonSize.x, 180f, Mathf.Max(180f, maxWidth)),
            Mathf.Clamp(PreferredButtonSize.y, 72f, Mathf.Max(72f, maxHeight)));
    }

    public static Vector2 CalculateButtonPosition(int index, Vector2 canvasSize,
        Vector4 safeInsets, Vector2 buttonSize, bool portrait)
    {
        float safeLeft = safeInsets.x;
        float safeRight = canvasSize.x - safeInsets.y;
        float safeBottom = safeInsets.w;
        float safeTop = canvasSize.y - safeInsets.z;
        Vector2 safeCenter = new Vector2(
            (safeLeft + safeRight - canvasSize.x) * 0.5f,
            (safeBottom + safeTop - canvasSize.y) * 0.5f);

        if (portrait)
        {
            float y = (1.5f - Mathf.Clamp(index, 0, 3)) * (buttonSize.y + Gap);
            return safeCenter + new Vector2(0f, y);
        }

        int clamped = Mathf.Clamp(index, 0, 3);
        int row = clamped / 2;
        int column = clamped % 2 == 0 ? 1 : 0;
        return safeCenter + new Vector2(
            (column - 0.5f) * (buttonSize.x + Gap),
            (0.5f - row) * (buttonSize.y + Gap));
    }
}

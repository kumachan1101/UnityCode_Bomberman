using System.Collections.Generic;
using System.Globalization;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class GameStatusHudController : MonoBehaviour
{
    private enum HudIcon
    {
        Fire,
        Bomb,
        Move,
        BombSpeed,
        Kick,
        Special
    }

    private sealed class StatusCell
    {
        public RawImage icon;
        public Text value;

        public void Set(string text, bool active)
        {
            value.text = text;
            value.color = active ? Color.white : new Color(0.65f, 0.68f, 0.72f, 1f);
            icon.color = active ? Color.white : new Color(1f, 1f, 1f, 0.28f);
        }
    }

    private const float RefreshInterval = 0.1f;
    private const float LayoutInterval = 0.25f;
    public const float StagePanelHeight = 46f;
    public const float StatusPanelHeight = 68f;
    public const float StagePanelTopMargin = 28f;
    public const float StatusPanelBottomMargin = 20f;
    public const float PanelGap = 8f;
    private const float CellGap = 5f;
    private const int CellCount = 6;

    private static readonly Dictionary<HudIcon, Texture2D> iconTextures =
        new Dictionary<HudIcon, Texture2D>();

    private readonly Dictionary<string, StatusCell> cells =
        new Dictionary<string, StatusCell>();

    private RectTransform stagePanel;
    private RectTransform statusPanel;
    private Text stageText;
    private Text audioText;
    private Player_Base localPlayer;
    private string modeName = "BATTLE";
    private int lastValidStage = 1;
    private float nextRefreshTime;
    private float nextLayoutTime;
    private int lastWidth = -1;
    private int lastHeight = -1;

    public string StageTextValue => stageText != null ? stageText.text : string.Empty;

    private void Awake()
    {
        BuildHud();
    }

    private void OnEnable()
    {
        Player_Base.onPlayerAdded.AddListener(OnPlayerAdded);
        Player_Base.onPlayerRemoved.AddListener(OnPlayerRemoved);
        FindLocalPlayer();
        RefreshNow();
    }

    private void OnDisable()
    {
        Player_Base.onPlayerAdded.RemoveListener(OnPlayerAdded);
        Player_Base.onPlayerRemoved.RemoveListener(OnPlayerRemoved);
        localPlayer = null;
    }

    private void Update()
    {
        if (Time.unscaledTime >= nextRefreshTime)
        {
            RefreshNow();
            nextRefreshTime = Time.unscaledTime + RefreshInterval;
        }

        if (Screen.width != lastWidth || Screen.height != lastHeight ||
            Time.unscaledTime >= nextLayoutTime)
        {
            LayoutHud();
        }
    }

    public void ConfigureMode(string displayName)
    {
        modeName = string.IsNullOrEmpty(displayName) ? "BATTLE" : displayName;
        RefreshStage();
    }

    public void RefreshNow()
    {
        if (localPlayer == null) FindLocalPlayer();
        RefreshStage();
        RefreshAudioStatus();
        RefreshItemStatus();
    }

    public string GetCellValue(string key)
    {
        StatusCell cell;
        return cells.TryGetValue(key, out cell) ? cell.value.text : string.Empty;
    }

    public static string FormatStageTitle(string mode, int stage, int stageCount)
    {
        return string.Format(CultureInfo.InvariantCulture, "STAGE {0}/{1}  •  {2}",
            Mathf.Max(1, stage), Mathf.Max(1, stageCount), mode);
    }

    private void BuildHud()
    {
        Font font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        stagePanel = CreatePanel("StageStatus", transform,
            new Color(0.035f, 0.055f, 0.085f, 0.9f));
        stageText = CreateText("StageName", stagePanel, font, 22,
            TextAnchor.MiddleLeft, FontStyle.Bold);
        RectTransform stageTextRect = stageText.rectTransform;
        stageTextRect.anchorMin = new Vector2(0f, 0f);
        stageTextRect.anchorMax = new Vector2(0.78f, 1f);
        stageTextRect.offsetMin = new Vector2(14f, 0f);
        stageTextRect.offsetMax = Vector2.zero;

        audioText = CreateText("AudioStatus", stagePanel, font, 15,
            TextAnchor.MiddleCenter, FontStyle.Bold);
        RectTransform audioRect = audioText.rectTransform;
        audioRect.anchorMin = new Vector2(0.78f, 0f);
        audioRect.anchorMax = Vector2.one;
        audioRect.offsetMin = Vector2.zero;
        audioRect.offsetMax = new Vector2(-8f, 0f);

        statusPanel = CreatePanel("ItemStatus", transform,
            new Color(0.025f, 0.035f, 0.055f, 0.82f));
        CreateStatusCell("Fire", "FIRE", HudIcon.Fire, font);
        CreateStatusCell("Bomb", "BOMB", HudIcon.Bomb, font);
        CreateStatusCell("Move", "MOVE", HudIcon.Move, font);
        CreateStatusCell("BombSpeed", "B.SPD", HudIcon.BombSpeed, font);
        CreateStatusCell("Kick", "KICK", HudIcon.Kick, font);
        CreateStatusCell("Special", "TYPE", HudIcon.Special, font);

        LayoutHud();
    }

    private void CreateStatusCell(string key, string caption, HudIcon iconType, Font font)
    {
        RectTransform cell = CreatePanel(key, statusPanel,
            new Color(0.1f, 0.13f, 0.18f, 0.92f));

        GameObject iconObject = new GameObject("Icon", typeof(RectTransform),
            typeof(CanvasRenderer), typeof(RawImage));
        iconObject.transform.SetParent(cell, false);
        RawImage icon = iconObject.GetComponent<RawImage>();
        icon.texture = GetIconTexture(iconType);
        icon.raycastTarget = false;
        RectTransform iconRect = icon.rectTransform;
        iconRect.anchorMin = iconRect.anchorMax = new Vector2(0.25f, 0.62f);
        iconRect.sizeDelta = new Vector2(36f, 36f);

        Text captionText = CreateText("Caption", cell, font, 10,
            TextAnchor.MiddleCenter, FontStyle.Bold);
        captionText.text = caption;
        captionText.color = new Color(0.72f, 0.78f, 0.86f, 1f);
        RectTransform captionRect = captionText.rectTransform;
        captionRect.anchorMin = new Vector2(0f, 0f);
        captionRect.anchorMax = new Vector2(0.5f, 0.28f);
        captionRect.offsetMin = Vector2.zero;
        captionRect.offsetMax = Vector2.zero;

        Text value = CreateText("Value", cell, font, key == "Special" ? 13 : 22,
            TextAnchor.MiddleCenter, FontStyle.Bold);
        RectTransform valueRect = value.rectTransform;
        valueRect.anchorMin = new Vector2(0.5f, 0f);
        valueRect.anchorMax = Vector2.one;
        valueRect.offsetMin = Vector2.zero;
        valueRect.offsetMax = Vector2.zero;

        cells[key] = new StatusCell { icon = icon, value = value };
    }

    private void LayoutHud()
    {
        RectTransform canvasRect = transform as RectTransform;
        if (canvasRect == null || stagePanel == null || statusPanel == null) return;

        Vector2 canvasSize = canvasRect.rect.size;
        if (canvasSize.x <= 0f || canvasSize.y <= 0f)
            canvasSize = ResponsiveGameUiController.ReferenceResolution;
        Vector4 safe = ResponsiveGameUiController.CalculateSafeInsets(canvasSize,
            new Vector2(Mathf.Max(1, Screen.width), Mathf.Max(1, Screen.height)),
            Screen.safeArea);
        float availableWidth = Mathf.Max(240f, canvasSize.x - safe.x - safe.y - 24f);

        stagePanel.anchorMin = stagePanel.anchorMax = new Vector2(0.5f, 1f);
        stagePanel.pivot = new Vector2(0.5f, 0.5f);
        stagePanel.sizeDelta = new Vector2(Mathf.Min(460f, availableWidth), StagePanelHeight);
        stagePanel.anchoredPosition = new Vector2((safe.x - safe.y) * 0.5f,
            -(safe.z + StagePanelTopMargin +
              ResponsiveGameUiController.ReturnButtonSize.y + PanelGap +
              StagePanelHeight * 0.5f));

        float statusWidth = Mathf.Min(600f, availableWidth);
        float statusHeight = StatusPanelHeight;
        statusPanel.anchorMin = statusPanel.anchorMax = new Vector2(0.5f, 0f);
        statusPanel.pivot = new Vector2(0.5f, 0.5f);
        statusPanel.sizeDelta = new Vector2(statusWidth, statusHeight);
        statusPanel.anchoredPosition = new Vector2((safe.x - safe.y) * 0.5f,
            safe.w + StatusPanelBottomMargin + statusHeight * 0.5f);

        float cellWidth = (statusWidth - CellGap * (CellCount + 1)) / CellCount;
        int index = 0;
        foreach (string key in new[] { "Fire", "Bomb", "Move", "BombSpeed", "Kick", "Special" })
        {
            RectTransform cell = cells[key].value.transform.parent as RectTransform;
            cell.anchorMin = cell.anchorMax = new Vector2(0f, 0.5f);
            cell.pivot = new Vector2(0f, 0.5f);
            cell.sizeDelta = new Vector2(cellWidth, statusHeight - CellGap * 2f);
            cell.anchoredPosition = new Vector2(CellGap + index * (cellWidth + CellGap), 0f);
            index++;
        }

        lastWidth = Screen.width;
        lastHeight = Screen.height;
        nextLayoutTime = Time.unscaledTime + LayoutInterval;
    }

    private void RefreshStage()
    {
        GameManager manager = FindObjectOfType<GameManager>();
        int stageCount = manager != null ? manager.GetStageCount() : 5;
        int stage = manager != null ? manager.GetCurrentStage() : lastValidStage;
        if (stage > 0) lastValidStage = stage;
        stageText.text = FormatStageTitle(modeName, lastValidStage, stageCount);
    }

    private void RefreshAudioStatus()
    {
        bool ready = SoundManager.Instance != null && SoundManager.Instance.IsAudioReady;
        audioText.text = ready ? "SFX ON" : "SFX --";
        audioText.color = ready
            ? new Color(0.35f, 1f, 0.55f, 1f)
            : new Color(0.65f, 0.68f, 0.72f, 1f);
    }

    private void RefreshItemStatus()
    {
        if (localPlayer == null)
        {
            foreach (StatusCell cell in cells.Values) cell.Set("--", false);
            return;
        }

        PlayerBom bomb = localPlayer.GetComponent<PlayerBom>();
        PlayerMovement movement = localPlayer.GetComponent<PlayerMovement>();
        if (bomb == null || movement == null)
        {
            foreach (StatusCell cell in cells.Values) cell.Set("--", false);
            return;
        }

        cells["Fire"].Set(bomb.Get<int>(GetKind.FireNum).ToString(), true);
        cells["Bomb"].Set(bomb.Get<int>(GetKind.BomNum).ToString(), true);
        cells["Move"].Set(movement.GetMoveSpeed().ToString("0.#",
            CultureInfo.InvariantCulture), true);
        cells["BombSpeed"].Set(bomb.Get<int>(GetKind.BomSpeed).ToString(), true);
        bool kick = bomb.Get<bool>(GetKind.BomKick);
        cells["Kick"].Set(kick ? "ON" : "--", kick);
        cells["Special"].Set(GetSpecialStatus(bomb), true);
    }

    public static string GetSpecialStatus(PlayerBom bomb)
    {
        if (bomb == null) return "--";
        BOM_KIND kind = bomb.Get<BOM_KIND>(GetKind.BomKind);
        if (kind == BOM_KIND.BOM_KIND_BIGBAN) return "BIG";
        if (kind == BOM_KIND.BOM_KIND_EXPLODE) return "BURST";
        BOM_ATTACK attack = bomb.Get<BOM_ATTACK>(GetKind.BomAttack);
        if (attack == BOM_ATTACK.BOM_ATTACK_MULTI) return "MULTI";
        if (attack == BOM_ATTACK.BOM_ATTACK_THROW) return "THROW";
        return "NORMAL";
    }

    private void FindLocalPlayer()
    {
        foreach (Player_Base player in FindObjectsOfType<Player_Base>())
        {
            OnPlayerAdded(player);
            if (localPlayer != null) return;
        }
    }

    private void OnPlayerAdded(Player_Base player)
    {
        if (localPlayer != null || !IsLocalPlayer(player)) return;
        localPlayer = player;
        RefreshItemStatus();
    }

    private void OnPlayerRemoved(Player_Base player)
    {
        if (localPlayer != player) return;
        localPlayer = null;
        RefreshItemStatus();
    }

    private static bool IsLocalPlayer(Player_Base player)
    {
        if (player == null || !player.isActiveAndEnabled) return false;
        if (player is Player) return true;
        if (!(player is Player_Online) || player is Player_Online_Dummy) return false;
        PhotonView view = player.GetComponent<PhotonView>();
        return view != null && view.IsMine;
    }

    private static RectTransform CreatePanel(string name, Transform parent, Color color)
    {
        GameObject panelObject = new GameObject(name, typeof(RectTransform),
            typeof(CanvasRenderer), typeof(Image));
        panelObject.transform.SetParent(parent, false);
        Image image = panelObject.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return panelObject.GetComponent<RectTransform>();
    }

    private static Text CreateText(string name, Transform parent, Font font,
        int fontSize, TextAnchor alignment, FontStyle style)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform),
            typeof(CanvasRenderer), typeof(Text));
        textObject.transform.SetParent(parent, false);
        Text text = textObject.GetComponent<Text>();
        text.font = font;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.fontStyle = style;
        text.color = Color.white;
        text.raycastTarget = false;
        return text;
    }

    private static Texture2D GetIconTexture(HudIcon type)
    {
        Texture2D texture;
        if (iconTextures.TryGetValue(type, out texture)) return texture;

        const int size = 48;
        texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        texture.name = "HUD " + type;
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        Color32[] pixels = new Color32[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
                pixels[y * size + x] = GetIconPixel(type, x, y);
        }
        texture.SetPixels32(pixels);
        texture.Apply(false, true);
        iconTextures[type] = texture;
        return texture;
    }

    private static Color32 GetIconPixel(HudIcon type, int x, int y)
    {
        Color32 clear = new Color32(0, 0, 0, 0);
        switch (type)
        {
            case HudIcon.Fire:
                if (y >= 6 && y <= 41 && Mathf.Abs(x - 24) <= (42 - y) * 0.55f)
                    return y < 23 ? new Color32(255, 196, 32, 255) : new Color32(255, 80, 28, 255);
                return clear;
            case HudIcon.Bomb:
                if ((x - 22) * (x - 22) + (y - 21) * (y - 21) <= 15 * 15)
                    return new Color32(25, 30, 38, 255);
                if (x >= 29 && x <= 38 && y >= 32 && y <= 37) return new Color32(245, 180, 45, 255);
                if (x >= 37 && y >= 36) return new Color32(255, 90, 35, 255);
                return clear;
            case HudIcon.Move:
                if (IsChevron(x, y, 8) || IsChevron(x, y, 21)) return new Color32(55, 220, 255, 255);
                return clear;
            case HudIcon.BombSpeed:
                if ((x - 17) * (x - 17) + (y - 22) * (y - 22) <= 11 * 11)
                    return new Color32(115, 65, 205, 255);
                if (IsChevron(x, y, 24)) return new Color32(255, 105, 235, 255);
                return clear;
            case HudIcon.Kick:
                if ((x >= 12 && x <= 26 && y >= 19 && y <= 41) ||
                    (x >= 12 && x <= 39 && y >= 9 && y <= 23) ||
                    (x >= 33 && x <= 42 && y >= 7 && y <= 15))
                    return new Color32(255, 205, 55, 255);
                return clear;
            case HudIcon.Special:
                int diamond = Mathf.Abs(x - 24) + Mathf.Abs(y - 24);
                if (diamond <= 18) return diamond <= 8
                    ? new Color32(255, 235, 110, 255)
                    : new Color32(190, 90, 255, 255);
                return clear;
            default:
                return clear;
        }
    }

    private static bool IsChevron(int x, int y, int offset)
    {
        int localX = x - offset;
        return localX >= 0 && localX <= 15 &&
            Mathf.Abs(Mathf.Abs(y - 24) - localX) <= 2;
    }
}

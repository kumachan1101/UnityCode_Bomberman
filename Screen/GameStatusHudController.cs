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
        FireRange,
        BombCount,
        MoveSpeed,
        Kick,
        Invincible,
        TypeNormal,
        TypeExplode,
        TypeBig,
        ModeNormal,
        ModeThrow,
        ModeMulti,
        BombSpeed,
    }

    private sealed class StatusCell
    {
        public RawImage icon;
        public Text value;
        public Image background;
        public bool isActive;
        public bool isMaxed;

        public void Set(string text, bool active, bool maxed = false)
        {
            isActive = active;
            isMaxed = maxed;
            value.text = text;
            value.color = maxed
                ? new Color(1f, 0.86f, 0.28f, 1f)
                : active ? Color.white : new Color(0.65f, 0.68f, 0.72f, 1f);
            icon.color = active ? Color.white : new Color(1f, 1f, 1f, 0.28f);
            background.color = maxed
                ? new Color(0.28f, 0.22f, 0.05f, 0.98f)
                : active
                ? new Color(0.1f, 0.15f, 0.21f, 0.96f)
                : new Color(0.065f, 0.075f, 0.09f, 0.78f);
        }

        public void SetIcon(HudIcon type)
        {
            icon.texture = GetIconTexture(type);
        }
    }

    private const float RefreshInterval = 0.1f;
    private const float LayoutInterval = 0.25f;
    public const float StagePanelHeight = 46f;
    public const float StatusPanelHeight = 156f;
    public const float StagePanelTopMargin = 28f;
    public const float StatusPanelBottomMargin = 20f;
    public const float PanelGap = 8f;
    private const float CellGap = 5f;
    private const int CoreCellCount = 5;
    private const int AbilityCellCount = 3;

    private static readonly Dictionary<HudIcon, Texture2D> iconTextures =
        new Dictionary<HudIcon, Texture2D>();

    private readonly Dictionary<string, StatusCell> cells =
        new Dictionary<string, StatusCell>();

    private RectTransform stagePanel;
    private RectTransform statusPanel;
    private Text stageText;
    private Text audioText;
    private Image audioButtonImage;
    private Player_Base localPlayer;
    private string modeName = "BATTLE";
    private int lastValidStage = 1;
    private float nextRefreshTime;
    private float nextLayoutTime;
    private int lastWidth = -1;
    private int lastHeight = -1;

    public string StageTextValue => stageText != null ? stageText.text : string.Empty;
    public string AudioButtonText => audioText != null ? audioText.text : string.Empty;

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

    public bool GetCellActive(string key)
    {
        StatusCell cell;
        return cells.TryGetValue(key, out cell) && cell.isActive;
    }

    public bool GetCellMaxed(string key)
    {
        StatusCell cell;
        return cells.TryGetValue(key, out cell) && cell.isMaxed;
    }

    public string GetCellIconName(string key)
    {
        StatusCell cell;
        return cells.TryGetValue(key, out cell) && cell.icon.texture != null
            ? cell.icon.texture.name : string.Empty;
    }

    public void ToggleSound()
    {
        SoundManager sound = SoundManager.Instance;
        if (sound == null || !sound.IsAudioReady) return;
        sound.SetSoundEnabled(!sound.IsSoundEnabled);
        RefreshAudioStatus();
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
        stageTextRect.anchorMax = new Vector2(0.74f, 1f);
        stageTextRect.offsetMin = new Vector2(14f, 0f);
        stageTextRect.offsetMax = Vector2.zero;
        stageText.resizeTextForBestFit = true;
        stageText.resizeTextMinSize = 13;
        stageText.resizeTextMaxSize = 22;

        GameObject audioButtonObject = new GameObject("SoundToggle",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        audioButtonObject.transform.SetParent(stagePanel, false);
        audioButtonImage = audioButtonObject.GetComponent<Image>();
        audioButtonImage.color = new Color(0.08f, 0.32f, 0.19f, 0.96f);
        Button audioButton = audioButtonObject.GetComponent<Button>();
        audioButton.targetGraphic = audioButtonImage;
        audioButton.navigation = new Navigation { mode = Navigation.Mode.None };
        audioButton.onClick.AddListener(ToggleSound);
        RectTransform audioButtonRect = audioButtonObject.GetComponent<RectTransform>();
        audioButtonRect.anchorMin = new Vector2(0.75f, 0.12f);
        audioButtonRect.anchorMax = new Vector2(0.985f, 0.88f);
        audioButtonRect.offsetMin = Vector2.zero;
        audioButtonRect.offsetMax = Vector2.zero;

        audioText = CreateText("Label", audioButtonRect, font, 14,
            TextAnchor.MiddleCenter, FontStyle.Bold);
        RectTransform audioRect = audioText.rectTransform;
        audioRect.anchorMin = Vector2.zero;
        audioRect.anchorMax = Vector2.one;
        audioRect.offsetMin = Vector2.zero;
        audioRect.offsetMax = Vector2.zero;
        audioText.resizeTextForBestFit = true;
        audioText.resizeTextMinSize = 9;
        audioText.resizeTextMaxSize = 14;

        statusPanel = CreatePanel("ItemStatus", transform,
            new Color(0.025f, 0.035f, 0.055f, 0.82f));
        CreateGroupLabel("CoreGroup", "COEXISTING  •  VALUE / LIMIT", font,
            new Color(0.35f, 0.86f, 1f, 1f));
        CreateStatusCell("Fire", "BLAST RANGE", HudIcon.FireRange, font);
        CreateStatusCell("Bomb", "BOMB COUNT", HudIcon.BombCount, font);
        CreateStatusCell("Move", "MOVE SPEED", HudIcon.MoveSpeed, font);
        CreateStatusCell("Kick", "KICK", HudIcon.Kick, font);
        CreateStatusCell("Invincible", "GHOST SHIELD", HudIcon.Invincible, font);

        CreateGroupLabel("AbilityGroup",
            "TYPE + DROP COEXIST / ONE PER GROUP", font,
            new Color(1f, 0.72f, 0.28f, 1f));
        CreateStatusCell("BombType", "BOMB TYPE (ONE)", HudIcon.TypeNormal, font);
        CreateStatusCell("DropMode", "DROP MODE (ONE)", HudIcon.ModeNormal, font);
        CreateStatusCell("ThrowSpeed", "THROW SPEED", HudIcon.BombSpeed, font);

        LayoutHud();
    }

    private void CreateStatusCell(string key, string caption, HudIcon iconType, Font font)
    {
        RectTransform cell = CreatePanel(key, statusPanel,
            new Color(0.1f, 0.13f, 0.18f, 0.92f));
        Image background = cell.GetComponent<Image>();

        GameObject iconObject = new GameObject("Icon", typeof(RectTransform),
            typeof(CanvasRenderer), typeof(RawImage));
        iconObject.transform.SetParent(cell, false);
        RawImage icon = iconObject.GetComponent<RawImage>();
        icon.texture = GetIconTexture(iconType);
        icon.raycastTarget = false;
        RectTransform iconRect = icon.rectTransform;
        iconRect.anchorMin = iconRect.anchorMax = new Vector2(0.18f, 0.5f);
        iconRect.sizeDelta = new Vector2(36f, 36f);

        Text captionText = CreateText("Caption", cell, font, 12,
            TextAnchor.UpperCenter, FontStyle.Bold);
        captionText.text = caption;
        captionText.color = new Color(0.72f, 0.78f, 0.86f, 1f);
        RectTransform captionRect = captionText.rectTransform;
        captionRect.anchorMin = new Vector2(0.38f, 0.48f);
        captionRect.anchorMax = new Vector2(1f, 0.94f);
        captionRect.offsetMin = Vector2.zero;
        captionRect.offsetMax = new Vector2(-3f, 0f);
        captionText.resizeTextForBestFit = true;
        captionText.resizeTextMinSize = 9;
        captionText.resizeTextMaxSize = 12;

        Text value = CreateText("Value", cell, font, 20,
            TextAnchor.MiddleCenter, FontStyle.Bold);
        RectTransform valueRect = value.rectTransform;
        valueRect.anchorMin = new Vector2(0.38f, 0.02f);
        valueRect.anchorMax = new Vector2(1f, 0.58f);
        valueRect.offsetMin = Vector2.zero;
        valueRect.offsetMax = new Vector2(-3f, 0f);
        value.resizeTextForBestFit = true;
        value.resizeTextMinSize = 12;
        value.resizeTextMaxSize = 20;

        cells[key] = new StatusCell { icon = icon, value = value, background = background };
    }

    private void CreateGroupLabel(string name, string label, Font font, Color color)
    {
        Text text = CreateText(name, statusPanel, font, 12,
            TextAnchor.MiddleLeft, FontStyle.Bold);
        text.text = label;
        text.color = color;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 9;
        text.resizeTextMaxSize = 12;
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

        float statusWidth = Mathf.Min(760f, availableWidth);
        float statusHeight = StatusPanelHeight;
        statusPanel.anchorMin = statusPanel.anchorMax = new Vector2(0.5f, 0f);
        statusPanel.pivot = new Vector2(0.5f, 0.5f);
        statusPanel.sizeDelta = new Vector2(statusWidth, statusHeight);
        statusPanel.anchoredPosition = new Vector2((safe.x - safe.y) * 0.5f,
            safe.w + StatusPanelBottomMargin + statusHeight * 0.5f);

        LayoutGroupLabel("CoreGroup", statusWidth, -3f);
        LayoutStatusRow(new[] { "Fire", "Bomb", "Move", "Kick", "Invincible" },
            CoreCellCount, statusWidth, -23f, 52f);
        LayoutGroupLabel("AbilityGroup", statusWidth, -78f);
        LayoutStatusRow(new[] { "BombType", "DropMode", "ThrowSpeed" },
            AbilityCellCount, statusWidth, -99f, 52f);

        lastWidth = Screen.width;
        lastHeight = Screen.height;
        nextLayoutTime = Time.unscaledTime + LayoutInterval;
    }

    private void LayoutGroupLabel(string name, float width, float top)
    {
        RectTransform label = statusPanel.Find(name) as RectTransform;
        if (label == null) return;
        label.anchorMin = label.anchorMax = new Vector2(0f, 1f);
        label.pivot = new Vector2(0f, 1f);
        label.sizeDelta = new Vector2(width - CellGap * 2f, 18f);
        label.anchoredPosition = new Vector2(CellGap + 2f, top);
    }

    private void LayoutStatusRow(string[] keys, int count, float width,
        float top, float height)
    {
        float cellWidth = (width - CellGap * (count + 1)) / count;
        for (int index = 0; index < keys.Length; index++)
        {
            RectTransform cell = cells[keys[index]].value.transform.parent as RectTransform;
            cell.anchorMin = cell.anchorMax = new Vector2(0f, 1f);
            cell.pivot = new Vector2(0f, 1f);
            cell.sizeDelta = new Vector2(cellWidth, height);
            cell.anchoredPosition = new Vector2(
                CellGap + index * (cellWidth + CellGap), top);
        }
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
        SoundManager sound = SoundManager.Instance;
        bool ready = sound != null && sound.IsAudioReady;
        bool enabled = ready && sound.IsSoundEnabled;
        audioText.text = !ready ? "SOUND --" : enabled ? "SOUND ON" : "SOUND OFF";
        audioText.color = enabled ? Color.white : new Color(0.78f, 0.8f, 0.83f, 1f);
        if (audioButtonImage != null)
        {
            audioButtonImage.color = enabled
                ? new Color(0.07f, 0.42f, 0.23f, 0.98f)
                : ready
                    ? new Color(0.34f, 0.11f, 0.12f, 0.98f)
                    : new Color(0.16f, 0.17f, 0.19f, 0.9f);
        }
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

        cells["Fire"].Set(FormatUnlimitedStatus(bomb.Get<int>(GetKind.FireNum)), true);
        cells["Bomb"].Set(FormatUnlimitedStatus(bomb.Get<int>(GetKind.BomNum)), true);
        float moveSpeed = movement.GetMoveSpeed();
        bool moveMaxed = moveSpeed >= PlayerMovement.MaximumMoveSpeed;
        cells["Move"].Set(FormatCappedStatus(moveSpeed,
            PlayerMovement.MaximumMoveSpeed), true, moveMaxed);
        bool kick = bomb.Get<bool>(GetKind.BomKick);
        cells["Kick"].Set(kick ? "OWNED" : "NOT OWNED", kick, kick);
        PlayerInvincibility invincibility = localPlayer.GetComponent<PlayerInvincibility>();
        bool shieldActive = invincibility != null && invincibility.IsInvincible;
        cells["Invincible"].Set(shieldActive
            ? Mathf.CeilToInt(invincibility.RemainingSeconds) + "s"
            : "NONE", shieldActive);

        BOM_KIND kind = bomb.Get<BOM_KIND>(GetKind.BomKind);
        cells["BombType"].Set(GetBombTypeStatus(kind), true);
        cells["BombType"].SetIcon(GetBombTypeIcon(kind));

        BOM_ATTACK attack = bomb.Get<BOM_ATTACK>(GetKind.BomAttack);
        int throwSpeed = bomb.Get<int>(GetKind.BomSpeed);
        cells["DropMode"].Set(FormatDropModeStatus(attack, throwSpeed), true);
        cells["DropMode"].SetIcon(GetDropModeIcon(attack));

        bool throwing = attack == BOM_ATTACK.BOM_ATTACK_THROW;
        bool throwSpeedMaxed = throwing &&
            throwSpeed >= BomConfigurationBomSpeedUp.MaximumValue;
        cells["ThrowSpeed"].Set(FormatThrowSpeedStatus(attack, throwSpeed),
            throwing, throwSpeedMaxed);
    }

    public static string GetBombTypeStatus(BOM_KIND kind)
    {
        if (kind == BOM_KIND.BOM_KIND_BIGBAN) return "BIG";
        if (kind == BOM_KIND.BOM_KIND_EXPLODE) return "EXPLODE";
        return "NORMAL";
    }

    public static string GetDropModeStatus(BOM_ATTACK attack)
    {
        if (attack == BOM_ATTACK.BOM_ATTACK_MULTI) return "MULTI";
        if (attack == BOM_ATTACK.BOM_ATTACK_THROW) return "THROW";
        return "NORMAL";
    }

    public static string FormatDropModeStatus(BOM_ATTACK attack, int throwSpeed)
    {
        if (attack == BOM_ATTACK.BOM_ATTACK_THROW)
        {
            return "THROW SPD " + FormatCappedStatus(Mathf.Max(1, throwSpeed),
                BomConfigurationBomSpeedUp.MaximumValue);
        }

        return GetDropModeStatus(attack);
    }

    public static string FormatThrowSpeedStatus(BOM_ATTACK attack, int throwSpeed)
    {
        if (attack != BOM_ATTACK.BOM_ATTACK_THROW) return "INACTIVE";
        return FormatCappedStatus(Mathf.Max(1, throwSpeed),
            BomConfigurationBomSpeedUp.MaximumValue);
    }

    public static string FormatCappedStatus(float current, float maximum)
    {
        string values = string.Format(CultureInfo.InvariantCulture, "{0:0.#} / {1:0.#}",
            current, maximum);
        return current >= maximum ? values + " MAX" : values;
    }

    public static string FormatUnlimitedStatus(int current)
    {
        return string.Format(CultureInfo.InvariantCulture, "{0} / NO MAX", current);
    }

    private static HudIcon GetBombTypeIcon(BOM_KIND kind)
    {
        if (kind == BOM_KIND.BOM_KIND_BIGBAN) return HudIcon.TypeBig;
        if (kind == BOM_KIND.BOM_KIND_EXPLODE) return HudIcon.TypeExplode;
        return HudIcon.TypeNormal;
    }

    private static HudIcon GetDropModeIcon(BOM_ATTACK attack)
    {
        if (attack == BOM_ATTACK.BOM_ATTACK_MULTI) return HudIcon.ModeMulti;
        if (attack == BOM_ATTACK.BOM_ATTACK_THROW) return HudIcon.ModeThrow;
        return HudIcon.ModeNormal;
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
            case HudIcon.FireRange:
                if (y >= 6 && y <= 41 && Mathf.Abs(x - 24) <= (42 - y) * 0.55f)
                    return y < 23 ? new Color32(255, 196, 32, 255) : new Color32(255, 80, 28, 255);
                return clear;
            case HudIcon.BombCount:
                if (InCircle(x, y, 20, 21, 14))
                    return new Color32(25, 30, 38, 255);
                if (InCircle(x, y, 35, 14, 8))
                    return new Color32(74, 86, 105, 255);
                if (x >= 27 && x <= 36 && y >= 32 && y <= 37)
                    return new Color32(245, 180, 45, 255);
                return clear;
            case HudIcon.MoveSpeed:
                if (IsChevron(x, y, 8) || IsChevron(x, y, 21)) return new Color32(55, 220, 255, 255);
                return clear;
            case HudIcon.Kick:
                if ((x >= 12 && x <= 26 && y >= 19 && y <= 41) ||
                    (x >= 12 && x <= 39 && y >= 9 && y <= 23) ||
                    (x >= 33 && x <= 42 && y >= 7 && y <= 15))
                    return new Color32(255, 205, 55, 255);
                return clear;
            case HudIcon.Invincible:
                int shieldDistance = Mathf.Abs(x - 24) + Mathf.Abs(y - 25);
                if ((shieldDistance >= 14 && shieldDistance <= 19 && y >= 10) ||
                    (y >= 6 && y <= 13 && Mathf.Abs(x - 24) <= 13))
                    return new Color32(90, 235, 255, 255);
                if (InCircle(x, y, 18, 25, 3) || InCircle(x, y, 30, 25, 3))
                    return new Color32(245, 255, 255, 255);
                if (y >= 17 && y <= 22 && Mathf.Abs(x - 24) <= 4)
                    return new Color32(245, 255, 255, 255);
                return clear;
            case HudIcon.TypeNormal:
                return GetBombPixel(x, y, 24, 21, 14,
                    new Color32(125, 138, 158, 255));
            case HudIcon.TypeExplode:
                int burst = Mathf.Abs(x - 24) + Mathf.Abs(y - 24);
                if (burst <= 10 ||
                    (Mathf.Abs(x - 24) <= 2 && Mathf.Abs(y - 24) <= 21) ||
                    (Mathf.Abs(y - 24) <= 2 && Mathf.Abs(x - 24) <= 21))
                    return burst <= 7
                        ? new Color32(255, 238, 95, 255)
                        : new Color32(255, 86, 30, 255);
                return clear;
            case HudIcon.TypeBig:
                return GetBombPixel(x, y, 23, 20, 18,
                    new Color32(255, 174, 38, 255));
            case HudIcon.ModeNormal:
                Color32 normalBomb = GetBombPixel(x, y, 24, 23, 11,
                    new Color32(150, 164, 184, 255));
                if (normalBomb.a > 0) return normalBomb;
                if (x >= 22 && x <= 26 && y >= 3 && y <= 12 ||
                    y >= 3 && y <= 7 && Mathf.Abs(x - 24) <= 8 - y)
                    return new Color32(90, 215, 255, 255);
                return clear;
            case HudIcon.ModeThrow:
                Color32 throwBomb = GetBombPixel(x, y, 14, 15, 8,
                    new Color32(180, 105, 255, 255));
                if (throwBomb.a > 0) return throwBomb;
                if (x >= 18 && x <= 40 && Mathf.Abs(y - x - 2) <= 2 ||
                    x >= 34 && x <= 43 && y >= 34 && Mathf.Abs((x + y) - 78) <= 2)
                    return new Color32(255, 118, 225, 255);
                return clear;
            case HudIcon.ModeMulti:
                if (InCircle(x, y, 13, 18, 7) || InCircle(x, y, 24, 27, 7) ||
                    InCircle(x, y, 35, 18, 7))
                    return new Color32(190, 105, 255, 255);
                return clear;
            case HudIcon.BombSpeed:
                if (InCircle(x, y, 16, 22, 10))
                    return new Color32(115, 65, 205, 255);
                if ((y == 13 || y == 21 || y == 29) && x >= 27 && x <= 43)
                    return new Color32(255, 105, 235, 255);
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

    private static bool InCircle(int x, int y, int centerX, int centerY, int radius)
    {
        int dx = x - centerX;
        int dy = y - centerY;
        return dx * dx + dy * dy <= radius * radius;
    }

    private static Color32 GetBombPixel(int x, int y, int centerX, int centerY,
        int radius, Color32 body)
    {
        if (InCircle(x, y, centerX, centerY, radius)) return body;
        if (x >= centerX + radius - 3 && x <= centerX + radius + 5 &&
            y >= centerY + radius - 1 && y <= centerY + radius + 4)
            return new Color32(245, 190, 55, 255);
        if (x >= centerX + radius + 4 && y >= centerY + radius + 3)
            return new Color32(255, 92, 38, 255);
        return new Color32(0, 0, 0, 0);
    }
}

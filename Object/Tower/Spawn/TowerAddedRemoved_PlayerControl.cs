using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerAddedRemoved_PlayerControl : Field_Event
{
    public const int RevivalPowerCost = 2;
    private const float RefreshInterval = 0.25f;

    private Button button;
    private Image buttonImage;
    private TMP_Text buttonLabel;
    private PlayerSpawnManager playerSpawnManager;
    private PlayerCountManager playerCountManager;
    private PlayerNameManager playerNameManager;
    private TowerSpawnManager towerSpawnManager;
    private GameManager gameManager;
    private bool listenersRegistered;
    private bool revivalInProgress;
    private int playerNo;
    private float nextRefreshTime;
    private readonly Color enabledColor = new Color(1f, 0.84f, 0f);
    private readonly Color pulseColor = new Color(1f, 0.46f, 0.05f);
    private readonly Color disabledColor = new Color(0.6627f, 0.6627f, 0.6627f);

    public int ControlledPlayerNumber { get { return playerNo; } }
    public bool IsRevivalAvailable { get { return CanRevivePlayer(); } }
    public string CurrentStatusText { get; private set; }

    protected override void Init()
    {
        GameObject field = GameObject.Find("Field");
        if (field != null)
        {
            playerSpawnManager = field.GetComponent<PlayerSpawnManager>();
            playerCountManager = field.GetComponent<PlayerCountManager>();
            playerNameManager = field.GetComponent<PlayerNameManager>();
            towerSpawnManager = field.GetComponent<TowerSpawnManager>();
        }

        GameObject managerObject = GameObject.Find("GameManager");
        gameManager = managerObject != null ? managerObject.GetComponent<GameManager>() : null;

        button = GetComponent<Button>();
        buttonImage = button != null ? button.GetComponent<Image>() : null;
        buttonLabel = GetComponentInChildren<TMP_Text>(true);
        if (button != null)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.white;
            colors.selectedColor = Color.white;
            colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
            colors.disabledColor = new Color(1f, 1f, 1f, 0.78f);
            colors.colorMultiplier = 1f;
            button.colors = colors;
            button.onClick.RemoveListener(PushButton);
            button.onClick.AddListener(PushButton);
        }

        if (buttonLabel != null)
        {
            buttonLabel.enableAutoSizing = true;
            buttonLabel.fontSizeMin = 13f;
            buttonLabel.fontSizeMax = 24f;
            buttonLabel.alignment = TextAlignmentOptions.Center;
            buttonLabel.raycastTarget = false;
        }

        TryResolvePlayerNumber(null);
        SetToneDown();
        UpdateStatusText(false);
    }

    private void Update()
    {
        if (button != null && button.interactable && buttonImage != null)
        {
            float pulse = (Mathf.Sin(Time.unscaledTime * 7f) + 1f) * 0.5f;
            buttonImage.color = Color.Lerp(enabledColor, pulseColor, pulse);
        }
        if (Time.unscaledTime < nextRefreshTime) return;
        RefreshAvailability();
        nextRefreshTime = Time.unscaledTime + RefreshInterval;
    }

    public void RefreshAvailability()
    {
        bool available = CanRevivePlayer();
        if (available) SetToneUp();
        else SetToneDown();
        UpdateStatusText(available);
    }

    public void SetToneUp()
    {
        if (button == null || buttonImage == null) return;
        button.interactable = true;
        buttonImage.color = enabledColor;
        if (buttonLabel != null) buttonLabel.color = new Color(0.12f, 0.08f, 0.02f, 1f);
    }

    public void SetToneDown()
    {
        if (button == null || buttonImage == null) return;
        button.interactable = false;
        buttonImage.color = disabledColor;
        if (buttonLabel != null) buttonLabel.color = new Color(0.18f, 0.18f, 0.18f, 1f);
    }

    private void UpdateStatusText(bool available)
    {
        string status;
        if (revivalInProgress)
        {
            status = "REVIVING PLAYER...";
        }
        else if (gameManager == null || !gameManager.GetSetUp())
        {
            status = "REVIVE PLAYER\nWAIT FOR START";
        }
        else if (!TryResolvePlayerNumber(null) || playerSpawnManager == null ||
                 towerSpawnManager == null)
        {
            status = "REVIVE PLAYER\nWAITING";
        }
        else if (IsControlledPlayerAlive())
        {
            status = "PLAYER ACTIVE\nREVIVE NOT NEEDED";
        }
        else
        {
            float currentPower = towerSpawnManager.GetCurrentRevivalPower(playerNo);
            if (available && currentPower >= 0f)
            {
                status = string.Format(CultureInfo.InvariantCulture,
                    "REVIVE PLAYER\nPOWER {0:0.#} -> {1:0.#}",
                    currentPower, currentPower - RevivalPowerCost);
            }
            else if (currentPower >= 0f)
            {
                status = string.Format(CultureInfo.InvariantCulture,
                    "REVIVE LOCKED\nPOWER {0:0.#} / NEED {1}",
                    currentPower, RevivalPowerCost + 1);
            }
            else
            {
                status = "REVIVE PLAYER\nPOWER WAITING";
            }
        }

        CurrentStatusText = status;
        if (buttonLabel != null) buttonLabel.text = status;
    }

    protected override void RegisterListeners()
    {
        if (listenersRegistered) return;
        Player_Base.onPlayerAdded.AddListener(Field_Player_Tower_OnAdded);
        Player_Base.onPlayerRemoved.AddListener(Field_Player_Tower_OnRemoved);
        listenersRegistered = true;
    }

    protected override void UnregisterListeners()
    {
        if (!listenersRegistered) return;
        Player_Base.onPlayerAdded.RemoveListener(Field_Player_Tower_OnAdded);
        Player_Base.onPlayerRemoved.RemoveListener(Field_Player_Tower_OnRemoved);
        listenersRegistered = false;
    }

    private bool TryResolvePlayerNumber(Player_Base playerHint)
    {
        if (playerNo > 0) return true;

        if (playerNameManager != null &&
            TryGetPlayerNumber(playerNameManager.GetPlayerName(), out playerNo))
        {
            return true;
        }

        if (IsLocallyControlledPlayer(playerHint) &&
            TryGetPlayerNumber(playerHint.gameObject.name, out playerNo))
        {
            return true;
        }

        foreach (Player_Base player in FindObjectsOfType<Player_Base>())
        {
            if (IsLocallyControlledPlayer(player) &&
                TryGetPlayerNumber(player.gameObject.name, out playerNo))
            {
                return true;
            }
        }

        // ローカルタワーモードの操作対象は常にPlayer1。
        if (playerSpawnManager is PlayerSpawnManager_CpuMode_Tower)
        {
            playerNo = 1;
            return true;
        }

        return false;
    }

    private bool CanRevivePlayer()
    {
        if (revivalInProgress || button == null || playerSpawnManager == null ||
            towerSpawnManager == null || gameManager == null || !gameManager.GetSetUp() ||
            !TryResolvePlayerNumber(null) || IsControlledPlayerAlive())
        {
            return false;
        }

        return towerSpawnManager.CanSpendRevivalPower(playerNo, RevivalPowerCost);
    }

    private bool IsControlledPlayerAlive()
    {
        foreach (Player_Base player in FindObjectsOfType<Player_Base>())
        {
            int existingPlayerNo;
            if (TryGetPlayerNumber(player.gameObject.name, out existingPlayerNo) &&
                existingPlayerNo == playerNo)
            {
                return true;
            }
        }

        return false;
    }

    private bool JudgeMyPlayer(Player_Base player)
    {
        if (player == null || !TryResolvePlayerNumber(player)) return false;
        int eventPlayerNo;
        return TryGetPlayerNumber(player.gameObject.name, out eventPlayerNo) &&
               eventPlayerNo == playerNo;
    }

    private void Field_Player_Tower_OnAdded(Player_Base player)
    {
        if (!JudgeMyPlayer(player)) return;
        revivalInProgress = false;
        SetToneDown();
        UpdateStatusText(false);
    }

    private void Field_Player_Tower_OnRemoved(Player_Base player)
    {
        if (!JudgeMyPlayer(player)) return;
        revivalInProgress = false;
        // OnDestroy中は対象Playerが検索に残る場合があるため、次フレームにも再判定する。
        StartCoroutine(RefreshAfterPlayerChange());
    }

    public void PushButton()
    {
        if (!CanRevivePlayer())
        {
            RefreshAvailability();
            return;
        }

        revivalInProgress = true;
        SetToneDown();
        UpdateStatusText(false);
        if (!towerSpawnManager.TrySpendRevivalPower(playerNo, RevivalPowerCost))
        {
            revivalInProgress = false;
            RefreshAvailability();
            return;
        }

        playerCountManager?.AddPlayerCount();
        playerSpawnManager.SpawnPlayer(playerNo);
        StartCoroutine(RefreshAfterPlayerChange());
    }

    private IEnumerator RefreshAfterPlayerChange()
    {
        yield return null;
        revivalInProgress = false;
        RefreshAvailability();
    }

    private static bool TryGetPlayerNumber(string value, out int number)
    {
        number = 0;
        if (string.IsNullOrEmpty(value)) return false;
        Match match = Regex.Match(value, @"Player(\d+)");
        return match.Success && int.TryParse(match.Groups[1].Value, out number) && number > 0;
    }

    private static bool IsLocallyControlledPlayer(Player_Base player)
    {
        if (player == null || player is Player_Online_Dummy || player is Player_CpuMode)
            return false;
        if (player is Player) return true;
        if (!(player is Player_Online)) return false;
        PhotonView view = player.GetComponent<PhotonView>();
        return view != null && view.IsMine;
    }
}

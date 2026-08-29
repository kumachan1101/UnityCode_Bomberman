using System.Collections;
using System.Text.RegularExpressions;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class TowerAddedRemoved_PlayerControl : Field_Event
{
    public const int RevivalPowerCost = 2;
    private const float RefreshInterval = 0.25f;

    private Button button;
    private Image buttonImage;
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
    private readonly Color disabledColor = new Color(0.6627f, 0.6627f, 0.6627f);

    public int ControlledPlayerNumber { get { return playerNo; } }
    public bool IsRevivalAvailable { get { return CanRevivePlayer(); } }

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
        if (button != null)
        {
            button.onClick.RemoveListener(PushButton);
            button.onClick.AddListener(PushButton);
        }

        TryResolvePlayerNumber(null);
        SetToneDown();
    }

    private void Update()
    {
        if (Time.unscaledTime < nextRefreshTime) return;
        RefreshAvailability();
        nextRefreshTime = Time.unscaledTime + RefreshInterval;
    }

    public void RefreshAvailability()
    {
        if (CanRevivePlayer()) SetToneUp();
        else SetToneDown();
    }

    public void SetToneUp()
    {
        if (button == null || buttonImage == null) return;
        button.interactable = true;
        buttonImage.color = enabledColor;
    }

    public void SetToneDown()
    {
        if (button == null || buttonImage == null) return;
        button.interactable = false;
        buttonImage.color = disabledColor;
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

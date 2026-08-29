using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public const string SoundEnabledPreference = "Bomberman.SoundEnabled";
    public const float MasterOutputVolume = 0.55f;
    public const float DropBombVolumeScale = 0.42f;
    public const float GetItemVolumeScale = 0.34f;
    public const float ExplosionVolumeScale = 0.50f;
    public const float DropBombCooldownSeconds = 0.06f;
    public const float GetItemCooldownSeconds = 0.08f;
    public const float ExplosionCooldownSeconds = 0.10f;

    private const string DropBombResourcePath = "Audio/bomb_place_soft";
    private const string GetItemResourcePath = "Audio/item_collect_soft";
    private const string ExplosionResourcePath = "Audio/explosion_soft";

    public AudioClip sDropBomb;
    public AudioClip sGetItem;
    public AudioClip sExploison;

    private static SoundManager instance = null;
    private AudioSource audioSource;
    private float nextDropBombTime;
    private float nextGetItemTime;
    private float nextExplosionTime;

    public static SoundManager Instance => instance;
    public int PlayedEffectCount { get; private set; }
    public bool IsSoundEnabled { get; private set; }
    public bool IsAudioReady => audioSource != null &&
        sDropBomb != null && sGetItem != null && sExploison != null;

    private void Awake()
    {
        // シングルトンパターンで、既存のインスタンスがある場合は自分自身を破棄
        if (instance == null)
        {
            // このインスタンスを保存し、DontDestroyOnLoadで破棄されないようにする
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadComfortableSoundEffects();
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 0f;
            audioSource.volume = MasterOutputVolume;
            ApplySoundEnabled(PlayerPrefs.GetInt(SoundEnabledPreference, 1) != 0);
        }
        else
        {
            // 既に存在するインスタンスがある場合、このGameObjectを破棄
            Destroy(gameObject);
        }
    }

    // 効果音を再生するメソッド
    public bool PlaySoundEffect(string effectName)
    {
        AudioClip clipToPlay = null;
        float volumeScale = 1f;

        // 引数に応じて適切な効果音を選択
        switch (effectName)
        {
            case "DROPBOMB":
                clipToPlay = sDropBomb;
                volumeScale = DropBombVolumeScale;
                break;
            case "GETITEM":
                clipToPlay = sGetItem;
                volumeScale = GetItemVolumeScale;
                break;
            case "EXPLOISON":
                clipToPlay = sExploison;
                volumeScale = ExplosionVolumeScale;
                break;
            default:
                Debug.LogWarning("指定された効果音がありません: " + effectName);
                return false;
        }

        if (clipToPlay == null || audioSource == null || !IsSoundEnabled) return false;

        float now = Time.unscaledTime;
        switch (effectName)
        {
            case "DROPBOMB":
                if (now < nextDropBombTime) return false;
                nextDropBombTime = now + DropBombCooldownSeconds;
                break;
            case "GETITEM":
                if (now < nextGetItemTime) return false;
                nextGetItemTime = now + GetItemCooldownSeconds;
                break;
            case "EXPLOISON":
                // A single bomb creates several explosion tiles at once.
                // Treat that burst as one SFX so its volume cannot stack.
                if (now < nextExplosionTime) return false;
                nextExplosionTime = now + ExplosionCooldownSeconds;
                break;
        }

        audioSource.PlayOneShot(clipToPlay, volumeScale);
        PlayedEffectCount++;
        return true;
    }

    private void LoadComfortableSoundEffects()
    {
        AudioClip dropBomb = Resources.Load<AudioClip>(DropBombResourcePath);
        AudioClip getItem = Resources.Load<AudioClip>(GetItemResourcePath);
        AudioClip explosion = Resources.Load<AudioClip>(ExplosionResourcePath);

        if (dropBomb != null) sDropBomb = dropBomb;
        if (getItem != null) sGetItem = getItem;
        if (explosion != null) sExploison = explosion;
    }

    public void SetSoundEnabled(bool enabled)
    {
        ApplySoundEnabled(enabled);
        PlayerPrefs.SetInt(SoundEnabledPreference, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ApplySoundEnabled(bool enabled)
    {
        IsSoundEnabled = enabled;
        if (audioSource != null) audioSource.mute = !enabled;
    }
}

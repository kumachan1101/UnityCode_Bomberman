using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public AudioClip sDropBomb;
    public AudioClip sGetItem;
    public AudioClip sExploison;

    private static SoundManager instance = null;
    private AudioSource audioSource;
    private float nextExplosionTime;

    public static SoundManager Instance => instance;
    public int PlayedEffectCount { get; private set; }
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
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.spatialBlend = 0f;
            audioSource.volume = 0.75f;
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

        // 引数に応じて適切な効果音を選択
        switch (effectName)
        {
            case "DROPBOMB":
                clipToPlay = sDropBomb;
                break;
            case "GETITEM":
                clipToPlay = sGetItem;
                break;
            case "EXPLOISON":
                clipToPlay = sExploison;
                break;
            default:
                Debug.LogWarning("指定された効果音がありません: " + effectName);
                return false;
        }

        if (clipToPlay == null || audioSource == null) return false;

        // A single bomb creates several explosion tiles at once. Playing the
        // same clip for every tile clips badly, so treat that burst as one SFX.
        if (effectName == "EXPLOISON" && Time.unscaledTime < nextExplosionTime)
            return false;
        if (effectName == "EXPLOISON") nextExplosionTime = Time.unscaledTime + 0.08f;

        audioSource.PlayOneShot(clipToPlay);
        PlayedEffectCount++;
        return true;
    }
}

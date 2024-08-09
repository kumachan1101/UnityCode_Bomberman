using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip sDropBomb;
    public AudioClip sGetItem;
    public AudioClip sExploison;

    private static SoundManager instance = null;

    private void Awake()
    {
        // シングルトンパターンで、既存のインスタンスがある場合は自分自身を破棄
        if (instance == null)
        {
            // このインスタンスを保存し、DontDestroyOnLoadで破棄されないようにする
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 既に存在するインスタンスがある場合、このGameObjectを破棄
            Destroy(gameObject);
        }
    }

    // 効果音を再生するメソッド
    public void PlaySoundEffect(string effectName)
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
                return; // 適切な効果音が見つからない場合は処理を中断
        }

        // 効果音を再生
        if (clipToPlay != null)
        {
            //AudioSource.PlayClipAtPoint(clipToPlay, Camera.main.transform.position);
			
        }
    }
}

using UnityEngine;

public class ChangeTextureColor : MonoBehaviour
{
    public string texturePathFormat = "Assets/JunkoChan/Models/Materials/JunkochanTexture{0}.png";

    void Start()
    {
        int playerIndex = GetPlayerIndex(gameObject.name);
        
        if (playerIndex >= 0 && playerIndex < 4)
        {
            string texturePath = string.Format(texturePathFormat, playerIndex + 1);
            Texture2D texture = LoadTexture(texturePath);
            
            if (texture == null)
            {
                Debug.LogError("テクスチャの読み込みに失敗しました: " + texturePath);
                return;
            }

            // オブジェクトのマテリアルにテクスチャを適用
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.mainTexture = texture;
            }
        }
        else
        {
            Debug.LogWarning("プレイヤーのテクスチャが設定されていません。");
        }
    }

    // プレイヤー名からインデックスを取得するヘルパー関数
    int GetPlayerIndex(string playerName)
    {
        // Player1～4の名称からインデックスを抽出
        if (playerName.StartsWith("Player"))
        {
            string indexStr = playerName.Substring("Player".Length);
            int index;
            if (int.TryParse(indexStr, out index))
            {
                return index - 1; // 配列のインデックスに変換するために-1する
            }
        }
        return -1; // マッチしない場合は-1を返す
    }

    // テクスチャを読み込むヘルパー関数
    Texture2D LoadTexture(string path)
    {
        Texture2D texture = new Texture2D(2, 2);
        byte[] fileData = System.IO.File.ReadAllBytes(path);
        texture.LoadImage(fileData); // テクスチャを読み込み
        return texture;
    }
}

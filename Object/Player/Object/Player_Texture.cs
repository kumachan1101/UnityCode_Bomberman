using UnityEngine;

public class Player_Texture : MonoBehaviour
{
    void Start(){
        int iPlayerNo = GetPlayerIndex(gameObject.name);
        Texture cTexture = GetPlayerTexture(iPlayerNo);
        ChangePlayerTexture(cTexture);
    }

    int GetPlayerIndex(string playerName)
    {
        // Player1～4の名称からインデックスを抽出
        if (playerName.StartsWith("Player"))
        {
            string indexStr = playerName.Substring("Player".Length);
            int index;
            if (int.TryParse(indexStr, out index))
            {
                return index;
            }
        }
        return -1; // マッチしない場合は-1を返す
    }
     public Texture GetPlayerTexture(int playerNumber)
    {
        // Resources直下に配置したテクスチャのパス
        string texturePath = $"JunkochanTexture{playerNumber}";

        // テクスチャをロード
        Texture texture = Resources.Load<Texture>(texturePath);

        // テクスチャが存在しない場合はエラーログ
        if (texture == null)
        {
            Debug.LogError($"テクスチャが見つかりませんでした: {texturePath}");
        }

        return texture;
    }
    public void ChangePlayerTexture(Texture newTexture)
    {
        // プレイヤーの子オブジェクトから"Body"の名前を持つオブジェクトを探す
        Transform bodyTransform = transform.Find("Mesh/Body");
        if (bodyTransform == null)
        {
            Debug.LogError("Bodyオブジェクトが見つかりませんでした。");
            return;
        }

        // Rendererコンポーネントを取得
        Renderer bodyRenderer = bodyTransform.GetComponent<Renderer>();
        if (bodyRenderer == null)
        {
            Debug.LogError("Rendererコンポーネントが見つかりませんでした。");
            return;
        }

        // マテリアルのテクスチャを変更
        if (bodyRenderer.material.HasProperty("_MainTex")) // "_MainTex"は通常のテクスチャプロパティ
        {
            bodyRenderer.material.SetTexture("_MainTex", newTexture);
            //Debug.Log("テクスチャが変更されました。");
        }
        else
        {
            Debug.LogError("マテリアルに_MainTexプロパティがありません。");
        }
    }

}
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System.Collections;
public class Field_Player_Base : Field_Base {
    protected string PlayerName;

    private int CuurentPlayerNum; //プレイヤー自身を除く他プレイヤーの数
    public int m_playerCount; //やられたプレイヤー含む全プレイヤー数
    public virtual int GetIndex(){
        return 0;
    }
	protected virtual void DestroySync(GameObject g){}
    protected virtual void SetPower(Slider cSlider){}
    protected virtual void GetCPUPlayerInfo(ref string canvasName, ref string playerName){}

    public virtual string GetBomMaterial(Vector3 target, int index)
    {
		return "";
    }

    public virtual void AddDummyPlayer(int playercnt, Vector3 v3){
        string canvasName = GetCanvasName() + playercnt;
        GameObject gCanvas = LoadResource(canvasName);
        Vector3 v3PwrGage = new Vector3(0, 0, 0);
        gCanvas.transform.position = v3PwrGage;

        GameObject gPlayer = LoadResource(GetPlayerName() + playercnt);
        gPlayer.name = "PlayerDummy" + playercnt;
        gPlayer.tag = "Player_DummyMode";
        gPlayer.transform.position = v3;

        // gPlayerにアタッチされているPlayerスクリプトを取得
        Player_Base playerComponent = gPlayer.GetComponent<Player_Base>();
        if (playerComponent != null)
        {
            // Playerスクリプトを削除
			// このタイミングですぐ消えるわけではないため、直後のコンポーネントを取得してもPlayerスクリプトが取得できてしまう。
			// 取得されてしまったPlayerスクリプトにスライダーを設定してしまうと、Destroyが動作したタイミングでスライダーが消える。
			StartCoroutine(DestroyComponentAndWait(playerComponent, gCanvas, gPlayer));
            //Destroy(playerComponent);
        }

        //CPUモードに切り替え
		m_playerCount++;
        Player_Base cPlayer = AddComponent(gPlayer);
        cPlayer.MaterialType = "BomMaterial"+playercnt;
		//Debug.Log(gameObject.GetComponent<Field_Player_Base>());
        cPlayer.SetPlayerSetting(m_playerCount);
    }

    private IEnumerator DestroyComponentAndWait(Player_Base component, GameObject gCanvas, GameObject gPlayer)
    {
        if (component != null)
        {
            Destroy(component);
            yield return new WaitForEndOfFrame(); // 現在のフレームの終了まで待つ
            // 次のフレームの開始時に削除を確認
            if (component == null)
            {
		        SetupSlider_RPC(gCanvas, gPlayer,m_playerCount);
            }
        }
    }
    protected void GetPlayerInfo(ref string canvasName, ref string playerName){
		canvasName = "Canvas1";
		playerName = "Player1";
    }


	public virtual void SetupSlider_RPC(GameObject gCanvas, GameObject gPlayer,int iPlayerNo){}

    protected virtual string GetCanvasName(){
        return "";
    }

    protected virtual string GetPlayerName(){
        return "";
    }
    protected virtual Player_Base AddComponent(GameObject gPlayer){
        return null;
    }

	public virtual void SetPlayerPositions(){}
    public virtual void SpawnPlayerObjects(int iPlayerNo){}

    public void SetName(string namepara){
        PlayerName = namepara;
    }

    public string GetName(){
        return PlayerName;
    }

    public void SetPlayerNum(int num){
        CuurentPlayerNum = num;
    }
    public int GetPlayerNum(){
        return CuurentPlayerNum;
    }


    public int GetArrayLength(int arrayIndex)
    {
        // 変数名を構築
        string variableName = "v3PlayerPos" + arrayIndex;

        // フィールドを取得して配列の長さを取得
        FieldInfo field = GetType().GetField(variableName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            Vector3[] array = (Vector3[])field.GetValue(this);
            return array.Length;
        }
        else
        {
			Debug.Log("GetArrayLength Error");
            // 変数が見つからない場合のエラーハンドリング
            return 0; // または適切なデフォルト値を返す
        }
    }


	protected void SetupSliderCommon(GameObject gCanvas, GameObject gPlayer, int iPlayerNo)
	{
		Slider slider = gCanvas.GetComponentInChildren<Slider>(); // Canvasの子要素からSliderを取得します。
		RectTransform sliderRectTransform = slider.GetComponent<RectTransform>(); // SliderのRectTransformを取得します。
		Vector3 newPosition = sliderRectTransform.anchoredPosition; // ローカル座標
		newPosition.y = newPosition.y - (iPlayerNo - 1) * 20;
		sliderRectTransform.anchoredPosition = newPosition; // 新しい座標を設定します。
		
		Player_Base cPlayer = gPlayer.GetComponent<Player_Base>();
		//Debug.Log(cPlayer);
		cPlayer.SetSlider(gCanvas);
}

    protected GameObject LoadResource(string loadname){
        // Resourcesフォルダ内のPlayer1プレハブを読み込む
        GameObject playerPrefab = Resources.Load<GameObject>(loadname);
        return Instantiate(playerPrefab);

    }
    protected void SetupPlayer(GameObject gPlayer, int i, GameObject gCanvas)
    {
		m_playerCount = i;
        gPlayer.transform.position = GetPlayerPosition(GetIndex(),i-1);
        Player_Base cPlayer = gPlayer.GetComponent<Player_Base>();
		//cPlayer.MaterialType = "BomMaterial" +i;
        cPlayer.SetPlayerSetting(i); // カウンタiを使用する
        SetupSlider_RPC(gCanvas, gPlayer,m_playerCount);
    }
    public Vector3 GetPlayerPosition(int arrayIndex, int elementIndex)
    {
        // 変数名を構築
        string variableName = "v3PlayerPos" + arrayIndex;

        // フィールドを取得して値を取得
        FieldInfo field = GetType().GetField(variableName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {
            Vector3[] array = (Vector3[])field.GetValue(this);
            if (elementIndex >= 0 && elementIndex < array.Length)
            {
                return array[elementIndex];
            }
            else
            {
				Debug.Log("GetPlayerPosition Error");
                // インデックスが範囲外の場合はエラーハンドリングを行う
                return Vector3.zero; // または適切なデフォルト値を返す
            }
        }
        else
        {
			Debug.Log("GetPlayerPosition Error");
            // 変数が見つからない場合のエラーハンドリング
            return Vector3.zero; // または適切なデフォルト値を返す
        }
    }



}
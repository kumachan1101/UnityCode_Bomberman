using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System.Collections;
public class Field_Player_Base : Field_Base {
    protected string PlayerName;

    [SerializeField]private int m_playerCount; //やられたプレイヤー含む全プレイヤー数

	public void SetPlayerCnt(int iPlayerCnt){
		m_playerCount = iPlayerCnt;
	}
	public void AddPlayerCnt(){
		m_playerCount++;
	}
	public int GetPlayerCnt(){
		return m_playerCount;
	}

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
		bool bIsMine = PreAddDummyPlayer();
		if(false == bIsMine){
			return;
		}
		string canvasName = "";
		string playerName = "";
        GetPlayerNames(playercnt, ref canvasName, ref playerName);
		GameObject gCanvas = InstantiateCanvas(canvasName);
        GameObject gPlayer = InstantiatePlayer(playerName, v3);
		if(null == gCanvas || null == gPlayer){
			return;
		}

        // gPlayerにアタッチされているPlayerスクリプトを取得
        Player_Base playerComponent = gPlayer.GetComponent<Player_Base>();
		//オンラインで破棄しないといけない
		PlayerDestroy(gPlayer);
        //CPUモードに切り替え
        Player_Base cPlayer = AddComponent_RPC(gPlayer);

		// Playerスクリプトを削除
		// このタイミングですぐ消えるわけではないため、直後のコンポーネントを取得してもPlayerスクリプトが取得できてしまう。
		// 取得されてしまったPlayerスクリプトにスライダーを設定してしまうと、Destroyが動作したタイミングでスライダーが消える。
		StartCoroutine(DestroyComponentAndWait(playerComponent, gCanvas, gPlayer));
    }

	protected virtual bool PreAddDummyPlayer(){return false;}

	public virtual void ChangeName_RPC(GameObject gObject, string name) {}

	protected virtual void ChangeName(GameObject gPlayer){}

	protected virtual void PlayerDestroy(GameObject gPlayer){}

	
    protected IEnumerator DestroyComponentAndWait(Player_Base component, GameObject gCanvas, GameObject gPlayer)
    {
        if (component != null)
        {
            yield return new WaitForEndOfFrame(); // 現在のフレームの終了まで待つ
            // 次のフレームの開始時に削除を確認
            if (component == null)
            {
		        SetupSlider_RPC(gCanvas, gPlayer,GetPlayerCnt());
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
    protected virtual Player_Base AddComponent_RPC(GameObject gPlayer){
        return null;
    }

	public virtual void SetPlayerPositions(){}
    public virtual void SpawnPlayerObjects(int iPlayerNo){
		string canvasName = "";
		string playerName = "";
		GetPlayerNames(iPlayerNo, ref canvasName, ref playerName);
		SetName(playerName+"(Clone)");
		
		GameObject gCanvas = InstantiateCanvas(canvasName);
		GameObject gPlayer = InstantiatePlayer(playerName, GetPlayerPosition(GetIndex(),iPlayerNo-1));
		if(null == gCanvas || null == gPlayer){
			return;
		}
		SetupSlider_RPC(gCanvas, gPlayer, iPlayerNo);
	}

    protected virtual GameObject InstantiateCanvas(string canvasName){return null;}
    protected virtual GameObject InstantiatePlayer(string playerName, Vector3 position){return null;}
    protected virtual void GetPlayerNames(int iPlayerNo, ref string canvasName, ref string playerName){}
	//protected virtual void GetPlayerDummyNames(int iPlayerNo, ref string canvasName, ref string playerName){}

    public void SetName(string namepara){
        PlayerName = namepara;
    }

    public string GetName(){
        return PlayerName;
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
		
		SetPower(slider);

		Player_Base cPlayer = gPlayer.GetComponent<Player_Base>();
		//Debug.Log(cPlayer);
		cPlayer.SetSlider(gCanvas);
	}




    protected GameObject LoadResource(string loadname){
        // Resourcesフォルダ内のPlayer1プレハブを読み込む
        GameObject playerPrefab = Resources.Load<GameObject>(loadname);
        return Instantiate(playerPrefab);

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
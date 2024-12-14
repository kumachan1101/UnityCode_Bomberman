using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using Photon.Pun;

public class Field_Player_Base : MonoBehaviourPunCallbacks {

    [SerializeField]private int m_playerCount; //やられたプレイヤー含む全プレイヤー数
    protected string playername; 
    public virtual int GetIndex(){
        return 0;
    }
	protected virtual void DestroySync(GameObject g){}
    public virtual void SetPower(Slider cSlider){}
    public virtual int GetPower(){
        return 10;
    }
    protected virtual void GetCPUPlayerInfo(ref string canvasName, ref string playerName){}

    public virtual string GetBomMaterial(Vector3 target, int index)
    {
		return "";
    }


    public virtual void AddDummyPlayer(int iPlayerNo, Vector3 v3){}
    public virtual void SpawnPlayerObjects(int iPlayerNo){}


	protected virtual bool PreAddDummyPlayer(){return false;}

	//protected virtual void PlayerDestroy(GameObject gPlayer){}
    /*
    protected void PlayerDestroyComponent(GameObject gPlayer){
		Destroy(gPlayer.GetComponent<Player_Base>());
		Destroy(gPlayer.GetComponent<PlayerBom>());
		Destroy(gPlayer.GetComponent<Player_Collision>());
		Destroy(gPlayer.GetComponent<PlayerBomToBomControl>());
        Destroy(gPlayer.GetComponent<PlayerAction>());
        Destroy(gPlayer.GetComponent<PowerGageIF>());
        Destroy(gPlayer.GetComponent<PlayerMovement>());
        Destroy(gPlayer.GetComponent<PlayerAnimation>());
        Destroy(gPlayer.GetComponent<Player_Texture>());
    }

	public virtual void SetupSlider_RPC(GameObject gCanvas, GameObject gPlayer,int iPlayerNo){}
    */

    protected virtual string GetCanvasName(){
        return "";
    }

    public virtual string GetPlayerName(){
        return "";
    }

    protected void SetPlayerName(string name){
        playername = name;
    }

	public virtual void SetPlayerPositions(){}

    protected virtual void GetPlayerNames(int iPlayerNo, ref string canvasName, ref string playerName)
    {
        canvasName = "Canvas" + iPlayerNo;
        playerName = "Player" + iPlayerNo;
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


	public void SetPlayerCnt(int iPlayerCnt){
		m_playerCount = iPlayerCnt;
	}
	public void AddPlayerCnt(){
		m_playerCount++;
	}
	public int GetPlayerCnt(){
		return m_playerCount;
	}
    protected void GetPlayerInfo(ref string canvasName, ref string playerName){
		canvasName = "Canvas1";
		playerName = "Player1";
    }

}
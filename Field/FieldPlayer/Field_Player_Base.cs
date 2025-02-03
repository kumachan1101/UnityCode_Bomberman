using UnityEngine;
using System.Reflection;
using Photon.Pun;

public class Field_Player_Base : MonoBehaviourPunCallbacks {

    protected PlayerNameManager cPlayerNameManager;
    protected PlayerCountManager cPlayerCountManager;
    private void Awake()
    {
        cPlayerNameManager = gameObject.AddComponent<PlayerNameManager>();
        cPlayerCountManager = gameObject.AddComponent<PlayerCountManager>();
    }
    public virtual int GetIndex(){
        return 0;
    }

    public virtual void AddDummyPlayer(int iPlayerNo, Vector3 v3){}
    public virtual void SpawnPlayerObjects(int iPlayerNo){}

    protected virtual bool IsAddDummyPlayer(int iPlayerNo){
        int iPlayerCnt = Library_Base.CountObjectsWithName("Player"+iPlayerNo);
        //Debug.Log(iPlayerCnt);
        if(iPlayerCnt >= 5){
            return false;
        }
        bool bIsMine = PreAddDummyPlayer();
        if (!bIsMine)
        {
            return false;
        }
        return true;
    }

	protected virtual bool PreAddDummyPlayer(){return false;}

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

public class PlayerCountManager : MonoBehaviour
{
    private int playerCount;//やられたプレイヤー含む全プレイヤー数

    public void SetPlayerCount(int count) => playerCount = count;
    public void AddPlayerCount() => playerCount++;
    public int GetPlayerCount() => playerCount;
}

public class PlayerNameManager : MonoBehaviour
{
    private string playerName;

    public string GetPlayerName() => playerName;
    public void SetPlayerName(string name) => playerName = name;
}

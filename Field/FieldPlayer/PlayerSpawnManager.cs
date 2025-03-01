using UnityEngine;
using System.Reflection;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine.PlayerLoop;


public class PlayerSpawnManager : MonoBehaviourPunCallbacks
{

    protected PlayerNameManager cPlayerNameManager;
    protected PlayerCountManager cPlayerCountManager;

    protected PlayerPositionManager cPlayerPositionManager;

    private void Awake()
    {
        cPlayerNameManager = gameObject.AddComponent<PlayerNameManager>();
        cPlayerCountManager = gameObject.AddComponent<PlayerCountManager>();
        PlayerAddConponent();
    }

    public virtual void RequestPlayerSpawn(){

        cPlayerPositionManager.SetPlayerPositions();
        SpawnPlayer(1);
        cPlayerNameManager.SetPlayerName("Player1");
        int iPlayerCnt = cPlayerPositionManager.GetPlayerCount();
        
        for(int i = 2; i <= iPlayerCnt; i++) {
            cPlayerCountManager.SetPlayerCount(i);
            int iPlayerNo = Random.Range(2, 4);
            SpawnPlayer(iPlayerNo);    
        }
    }

    public virtual void SpawnDummyPlayer(int iPlayerNo, Vector3 v3){}
    public virtual void SpawnPlayer(int iPlayerNo){}

    protected virtual void PlayerAddConponent (){}

	protected virtual bool PreSpawnDummyPlayer(){return false;}

    /// <summary>
    /// CanvasPowerGageを上詰めで再配置する (SetPlayerCntを利用)
    /// </summary>
    public void RearrangeCanvases()
    {
        // "CanvasPowerGage" 名称を持つオブジェクトをすべて取得
        List<GameObject> canvasPowerGages = Library_Base.FindGameObjectsByPartialName("CanvasPowerGage(Clone)");
        if (canvasPowerGages.Count == 0)
        {
            Debug.LogWarning("CanvasPowerGage が見つかりませんでした。");
            return;
        }

        // ソート: Canvas をプレイヤー番号順に並べ替え
        canvasPowerGages.Sort((a, b) =>
        {
            int noA = a.GetComponent<PowerGage_Slider>().GetPlayerNo();
            int noB = b.GetComponent<PowerGage_Slider>().GetPlayerNo();
            return noA.CompareTo(noB);
        });

        // 再配置処理
        int newPlayerCnt = 1; // 新しいプレイヤーのカウント
        foreach (GameObject canvas in canvasPowerGages)
        {
            PowerGage_Slider powerGageSlider = canvas.GetComponent<PowerGage_Slider>();
            if (powerGageSlider != null)
            {
                //Debug.Log(newPlayerCnt);
                powerGageSlider.SetPlayerCnt(newPlayerCnt); // 新しい位置を設定
                newPlayerCnt++; // 次の位置のためにカウントを増やす
            }
        }
    }


}


public abstract class PlayerPowerManager: MonoBehaviour
{
    public abstract int GetPower();
    
}

public class PlayerPositionManager: MonoBehaviour
{
    protected Vector3[] playerPositions;
    public int GetPlayerCount()
    {
        return playerPositions.Length;
    }
    public virtual void SetPlayerPositions(){}

    public Vector3 GetPlayerPosition(int playerIndex)
    {
        if (playerIndex >= 0 && playerIndex < playerPositions.Length)
        {
            return playerPositions[playerIndex];
        }
        Debug.LogWarning($"Player {playerIndex} not found!");
        return Vector3.zero;
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

    public void GetPlayerNames(int iPlayerNo, ref string canvasName, ref string playerName)
    {
        canvasName = "Canvas" + iPlayerNo;
        playerName = "Player" + iPlayerNo;
    }

}

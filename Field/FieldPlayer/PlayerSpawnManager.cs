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

    public void RequestPlayerSpawn(){

        cPlayerPositionManager.SetPlayerPositions();
        SpawnPlayerObjects(1);    
        int iPlayerCnt = cPlayerPositionManager.GetPlayerCount();
        
        for(int i = 2; i <= iPlayerCnt; i++) {
            cPlayerCountManager.SetPlayerCount(i);
            int iPlayerNo = Random.Range(2, 4);
            SpawnPlayerObjects(iPlayerNo);    
        }
    }

    public virtual void AddDummyPlayer(int iPlayerNo, Vector3 v3){}
    public virtual void SpawnPlayerObjects(int iPlayerNo){}

    protected virtual void PlayerAddConponent (){}

	protected virtual bool PreAddDummyPlayer(){return false;}

    protected void GetPlayerNames(int iPlayerNo, ref string canvasName, ref string playerName)
    {
        canvasName = "Canvas" + iPlayerNo;
        playerName = "Player" + iPlayerNo;
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
}

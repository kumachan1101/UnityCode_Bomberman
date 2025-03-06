using UnityEngine;

public class PlayerPowerManager_Tower: PlayerPowerManager_CpuMode
{
    public override int GetPower(){
        return 1;
    }
}

public class PlayerSpawnManager_CpuMode_Tower : PlayerSpawnManager_CpuMode {

    protected override void PlayerAddConponent (){
        cPlayerPositionManager = gameObject.AddComponent<PlayerPositionManager_CpuMode>();
        gameObject.AddComponent<PlayerPowerManager_Tower>();
    
        
    }

    public override void RequestPlayerSpawn(){

        cPlayerPositionManager.SetPlayerPositions();
        SpawnPlayer(1);
        cPlayerNameManager.SetPlayerName("Player1");
        int iPlayerCnt = cPlayerPositionManager.GetPlayerCount();
        
        for(int i = 2; i <= iPlayerCnt; i++) {
            cPlayerCountManager.SetPlayerCount(i);
            int iPlayerNo = i % 3 + 2;
            SpawnPlayer(iPlayerNo);    
        }
    }


}

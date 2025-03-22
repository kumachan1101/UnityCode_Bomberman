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
    protected override int DeterminePlayerNumber(int index) {
        return index;
    }
}

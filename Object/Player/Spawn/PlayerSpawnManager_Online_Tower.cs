

public class PlayerSpawnManager_Online_Tower : PlayerSpawnManager_Online {

    protected override void PlayerAddConponent (){
        cPlayerPositionManager = gameObject.AddComponent<PlayerPositionManager_Online>();
        gameObject.AddComponent<PlayerPowerManager_Tower>();
    }

}

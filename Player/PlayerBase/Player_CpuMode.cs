public class Player_CpuMode : Player_Base
{
    public override void AddPlayerComponent(){
        this.gameObject.AddComponent<PlayerAction_CpuMode>();
    }

}

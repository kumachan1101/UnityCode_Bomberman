
public class Player_Online_Dummy : Player_Online
{
    public override void AddPlayerComponent(){
        this.gameObject.AddComponent<PlayerAction_CpuMode>();
    }
}

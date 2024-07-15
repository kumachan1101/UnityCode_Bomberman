
public class Player_Online_Dummy : Player_Online
{
    protected override void CreatePlayerAction(){
        cPlayerAction = new PlayerAction_CpuMode(ref rigidBody, ref myTransform);
    }
    public override void UpdateKey(){
        DropBom();
    }
}

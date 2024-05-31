using PlayerActionName;
public class Player_Online_Dummy : Player_Online
{
    protected override void CreatePlayerAction(){
        cPlayerAction = new PlayerAction_CpuMode(ref rigidBody, ref myTransform, ref animator, ref cField, iViewID);
    }
    public override void UpdateKey(){
        DropBom();
    }

}

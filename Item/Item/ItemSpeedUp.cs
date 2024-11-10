public class ItemSpeedUp : Item{
    public override void Reflection(string objname){
        PlayerAction cPlayerAction = Library_Base.GetcPlayerActionFromObject(objname);
        if(null != cPlayerAction){
            cPlayerAction.SpeedUp();
        }
    }
}

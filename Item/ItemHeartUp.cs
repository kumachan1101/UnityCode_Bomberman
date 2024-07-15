public class ItemHeartUp : Item{
    public override void Reflection(string objname){
        Player_Base cPlayer = Library_Base.GetcPlayerFromObject(objname);
        if(null != cPlayer){
            cPlayer.HeartUp(5);
        }
    }
}

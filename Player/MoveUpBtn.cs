

public class MoveUpBtn : LongPressButton {
    public override void PushButton()
    {
        Player_Base cPlayer = Library_Base.FindAndSetPlayer();
        if (cPlayer != null)
        {
            cPlayer.GetPlayerAction().MoveUp();
        }
    }
    
}
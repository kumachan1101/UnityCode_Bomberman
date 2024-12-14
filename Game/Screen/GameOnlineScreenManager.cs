using UnityEngine;
public class GameOnlineScreenManager : BaseScreenManager
{
    public override void InitializeScreen()
    {
        InitializeCanvas();
        GameObject gGameEndCanvas = Instantiate(Resources.Load("GameEndCanvas_Local") as GameObject);
    }
}

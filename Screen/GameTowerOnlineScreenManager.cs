using UnityEngine;
public class GameTowerOnlineScreenManager : GameTowerSceneManager
{
    public override void InitializeScreen()
    {
        InitializeCanvas();
        GameObject gGameEndCanvas = Instantiate(Resources.Load("GameEndCanvas_Local") as GameObject);
    }



}

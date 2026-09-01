using UnityEngine;
public class GameTowerOnlineScreenManager : GameTowerSceneManager
{
    public override void InitializeScreen()
    {
        InitializeCanvas();
        GameObject gGameEndCanvas = Instantiate(Resources.Load("GameEndCanvas_Online") as GameObject);
        gGameEndCanvas.name = "GameEndCanvas_Online_Local";
    }



}

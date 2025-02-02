using UnityEngine;

public class GameTowerSceneManager : BaseScreenManager
{
    public override void InitializeScreen()
    {
        InitializeCanvas();
        CreateStage();
        GameObject gGameEndCanvas = Instantiate(Resources.Load("GameEndCanvas_Local") as GameObject);
    }

    private void CreateStage()
    {
        string prefabName = "FieldTower";
        GameObject prefab = (GameObject)Resources.Load(prefabName);
        GameObject gField = Instantiate(prefab);
        gField.name = "Field";
    }
}

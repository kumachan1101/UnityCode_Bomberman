using UnityEngine;

public class GameSceneScreenManager : BaseScreenManager
{
    public override void InitializeScreen()
    {
        InitializeCanvas();
        CreateStage();
    }

    private void CreateStage()
    {
        string prefabName = "Field100";
        GameObject prefab = (GameObject)Resources.Load(prefabName);
        GameObject gField = Instantiate(prefab);
        gField.name = "Field";
    }
}

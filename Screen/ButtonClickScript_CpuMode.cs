using UnityEngine;
public class ButtonClickScript_CpuMode : ButtonClickScript
{
    override public void LoadGameScene()
    {
        GameManager cGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cGameManager.ReturnTitle();
    }


}
using UnityEngine;
public class ButtonClickScript_CpuMode : ButtonClickScript
{
    override public void LoadGameScene()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.ReturnTitle();
            return;
        }

        // Returning to the title must still work if the manager was destroyed or
        // renamed during a scene transition.
        base.LoadGameScene();
    }


}

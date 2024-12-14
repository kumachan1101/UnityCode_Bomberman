using UnityEngine.SceneManagement;
public class ButtonClickScript_CpuMode : ButtonClickScript
{
    override public void LoadGameScene()
    {
		  SceneManager.LoadScene("GameTitle");
    }


}
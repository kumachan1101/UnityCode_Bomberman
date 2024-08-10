using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartCanvas : MonoBehaviour
{
    public void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void LoadGameOnline()
    {
        SceneManager.LoadScene("GameOnline");
    }

}

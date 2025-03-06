using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartCanvas : MonoBehaviour
{
    public void LoadGameScene()
    {
        GameManager cGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cGameManager.SwitchGameScene();
    }
    public void LoadGameOnline()
    {
        GameManager cGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cGameManager.SwitchGameOnline();
    }
    public void LoadGameTower()
    {
        GameManager cGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cGameManager.SwitchTowerScene();
    }
    public void LoadGameTowerOnline()
    {
        GameManager cGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cGameManager.SwitchGameTowerOnline();
    }

}

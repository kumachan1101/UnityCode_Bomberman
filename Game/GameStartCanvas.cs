using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartCanvas : MonoBehaviour
{
    // Start is called before the first frame update

    void Awake()
    {
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGameScene()
    {
        
        SceneManager.LoadScene("GameScene");
    }
    public void LoadGameOnline()
    {
        
        SceneManager.LoadScene("GameOnline");
    }

}

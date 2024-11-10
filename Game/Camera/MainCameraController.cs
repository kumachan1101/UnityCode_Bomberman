using UnityEngine;
using UnityEngine.SceneManagement;

public class MainCameraController : MonoBehaviour
{
    private static MainCameraController instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }
}

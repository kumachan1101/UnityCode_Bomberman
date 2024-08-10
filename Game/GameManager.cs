using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    //private static GameManager instance; // GameManagerのシングルトンインスタンス

    [SerializeField] private static int iStage;

    public static int xmax;
    public static int zmax;
    private GameObject currentCanvas;

    private static GameManager instance = null;

    private void Awake()
    {
        // シングルトンパターンで、既存のインスタンスがある場合は自分自身を破棄
        if (instance == null)
        {
            // このインスタンスを保存し、DontDestroyOnLoadで破棄されないようにする
            instance = this;
            DontDestroyOnLoad(gameObject);
	        iStage = 1;
    	    SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            // 既に存在するインスタンスがある場合、このGameObjectを破棄
            Destroy(gameObject);
        }
    }


    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
		HandleCanvasForScene(scene.name);
        switch (scene.name)
        {
            case "GameScene":
            case "GameOnline":
                CreateStage(scene.name);
                break;
            case "GameTitle":
                iStage = 1;
                break;
        }
    }
	public void DestroyAllPhotonViews()
	{
		foreach (PhotonView view in FindObjectsOfType<PhotonView>())
		{
			if (view.IsMine)
			{
				PhotonNetwork.Destroy(view.gameObject);
			}
		}
	}

    private void CreateStage(string scenename)
    {
        //Debug.Log(prefabName);
        
        if("GameScene" == scenename){
            string prefabName = "Field" + iStage;
            GameObject gField = default;
            GameObject prefab = (GameObject)Resources.Load(prefabName);
            gField = Instantiate(prefab);
            gField.name = "Field";
		}
		else if("GameTitle" == scenename){
			DestroyAllPhotonViews();
		}

        GameObject gGameEndCanvas = Instantiate(Resources.Load("GameEndCanvas") as GameObject);
        //GameObject gCanvas = Instantiate(Resources.Load("Canvas") as GameObject);
    }

    private void HandleCanvasForScene(string sceneName)
    {
        if (ShouldShowCanvas(sceneName))
        {
            if (currentCanvas == null)
            {
                currentCanvas = Instantiate(Resources.Load("Canvas") as GameObject);
                //DontDestroyOnLoad(currentCanvas);
                GameObject mainCamera = GameObject.Find("Main Camera");

                JoystickCameraController joystickController = mainCamera.GetComponent<JoystickCameraController>();
                Transform joystickCameraTransform = currentCanvas.transform.Find("JoystickCamera");
                joystickController.joystick = joystickCameraTransform.GetComponent<Joystick>();

                CameraControlWithButtons CameraController = mainCamera.GetComponent<CameraControlWithButtons>();
                Transform upTransform = currentCanvas.transform.Find("up");
                CameraController.upButton = upTransform.GetComponent<Button>();
                Transform downTransform = currentCanvas.transform.Find("down");
                CameraController.downButton = downTransform.GetComponent<Button>();
            }
        }
        else
        {
            if (currentCanvas != null)
            {
                Destroy(currentCanvas);
                currentCanvas = null;
            }
        }
    }

    private bool ShouldShowCanvas(string sceneName)
    {
        // Canvasを表示するシーン名を指定します
        return sceneName == "GameScene" || sceneName == "GameOnline";
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public int GetStage(){
        return iStage;
    }

    static public int NextStage(){
        iStage += 1;
        return iStage;
    }

    public void GameWin(){
        int iStage = GameManager.NextStage();
        if(iStage < 5){
            Invoke("SwitchGameScene", 5f);
        }
        else{
            GameOver();
        }
        
    }

    public void GameOver(){
        Invoke("SwitchGameOver", 5f);
    }

    public void SwitchGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void SwitchGameOver()
    {
        SceneManager.LoadScene("GameTitle");
    }

	static public void SetFieldRange(int x, int z){
		xmax = x * 2;		
		zmax = z * 2;
	}

}


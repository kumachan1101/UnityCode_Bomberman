using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    private BaseScreenManager currentScreenManager;

    [SerializeField] private int iStage;

    public static int xmax;
    public static int zmax;

    [SerializeField] private int maxStage;


    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Resourceフォルダにあるステージ数を取得
        maxStage = GetMaxStage();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CancelInvoke("SwitchGameScene"); 
        CancelInvoke("SwitchGameOver");

        // すでに存在するScreenManagerを削除
        if (currentScreenManager != null)
        {
            Destroy(currentScreenManager);
        }

        switch (scene.name)
        {
            case "GameScene":
                currentScreenManager = gameObject.AddComponent<GameSceneScreenManager>();
                break;
            case "GameOnline":
                currentScreenManager = gameObject.AddComponent<GameOnlineScreenManager>();
                break;
            case "GameTitle":
                currentScreenManager = gameObject.AddComponent<GameTitleScreenManager>();
                break;
            default:
                Debug.Log("未定義のシーンです: " + scene.name);
                return;
        }

        currentScreenManager.InitializeScreen();
    }

    private int GetMaxStage()
    {
        return 100;
    }

    public void GameWin()
    {
        int stage = NextStage();
        if (stage <= maxStage)
        {
            Invoke("SwitchGameScene", 5f);
        }
        else
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        iStage = 1;
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

    public int NextStage()
    {
        iStage += 1;
        return iStage;
    }

    public static void SetFieldRange(int x, int z)
    {
        xmax = x * 2;
        zmax = z * 2;
    }
}


/*
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

    [SerializeField] private int iStage;

    public static int xmax;
    public static int zmax;
    private GameObject currentCanvas;

    private static GameManager instance = null;
    [SerializeField] private int maxStage;
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
        CancelInvoke("SwitchGameScene"); 
        CancelInvoke("SwitchGameOver");

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
            //string prefabName = "Field" + iStage;
            string prefabName = "Field100";
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
        // Resourceフォルダにあるステージ数を取得
        maxStage = GetMaxStage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public int GetStage(){
        return iStage;
    }

    public int NextStage(){
        iStage += 1;
        return iStage;
    }

    // ステージ数をResourceフォルダ内から取得
    private int GetMaxStage()
    {
/*
        int stageCount = 0;
        
        // "Field" で始まるプレハブを探し、ステージ数をカウントする
        while (Resources.Load<GameObject>("Field" + (stageCount + 1)) != null)
        {
            stageCount++;
        }

        return stageCount;
*/
/*
        return 100;
    }

    public void GameWin(){
        int iStage = NextStage();
        //if(iStage < 5){
        if (iStage <= maxStage){
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
*/
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    private BaseScreenManager currentScreenManager;

    [SerializeField] private int iStage;

    public static int xmax = 20;
    public static int zmax = 20;

    [SerializeField] private int maxStage;

    private bool bSetUp = false;

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
            InitEvent();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected void InitEvent (){
        EventDispatcher eventDispatcher = GameObject.Find("EventDispatcher").GetComponent<EventDispatcher>();
        eventDispatcher.RegisterListener<CompleteBlockCreateEvent>(OnCompleteBlockCreateEvent);
    }

    //private void OnCompleteBlockCreateEvent(CompleteBlockCreateEvent eEvent){
    private void OnCompleteBlockCreateEvent(IEvent eEvent){
        //Debug.Log("OnCompleteBlockCreateLog");
        bSetUp = true;  
    } 
    public bool GetSetUp()
    {
        return bSetUp;
    }


    void Start()
    {
        maxStage = GetMaxStage();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bSetUp = false;
        CancelInvoke("SwitchGameScene"); 
        CancelInvoke("SwitchTowerScene"); 
        CancelInvoke("SwitchGameOver");
        CancelInvoke("SwitchGameTowerOnline");

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
            case "GameTower":
                currentScreenManager = gameObject.AddComponent<GameTowerSceneManager>();
                break;
            case "GameTowerOnline":
                currentScreenManager = gameObject.AddComponent<GameTowerOnlineScreenManager>();
                break;
            default:
                Debug.Log("未定義のシーンです: " + scene.name);
                return;
        }

        currentScreenManager.InitializeScreen();
    }

    private int GetMaxStage()
    {
        return 5;
    }

    public void GameTowerWin()
    {
        int stage = NextStage();
        if (stage <= maxStage)
        {
            Invoke("SwitchTowerScene", 5f);
        }
        else
        {
            GameOver();
        }
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
    
    public void ReturnTitle()
    {
        iStage = 0;
        SwitchGameOver();
    }

    public void GameOver()
    {
        iStage = 0;
        Invoke("SwitchGameOver", 5f);
    }

    public bool IsGameOver()
    {
        return iStage == 0;
    }

    public void SwitchTowerScene()
    {
        iStage = NextStage();
        SceneManager.LoadScene("GameTower");
    }

    public void SwitchGameScene()
    {
        iStage = NextStage();
        SceneManager.LoadScene("GameScene");
    }
    
    public void SwitchGameOnline()
    {
        iStage = NextStage();
        SceneManager.LoadScene("GameOnline");
    }

    public void SwitchGameTowerOnline()
    {
        iStage = NextStage();
        SceneManager.LoadScene("GameTowerOnline");
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

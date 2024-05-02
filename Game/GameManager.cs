using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    private static GameManager instance; // GameManagerのシングルトンインスタンス

    [SerializeField] private static int iStage;

    public static int xmax = 11*2;
    public static int zmax = 11*2;

    public static GameManager Instance
    {
        get
        {
            // インスタンスがまだ作成されていない場合は、新しいGameManagerを作成します。
            if (instance == null)
            {
                GameObject gameObject = new GameObject("GameManager");
                instance = gameObject.AddComponent<GameManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        // 既にGameManagerのインスタンスが存在する場合、新しいインスタンスを破棄します。
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        iStage = 1;
        SceneManager.sceneLoaded += OnSceneLoaded;
        instance = this;
    }


    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 新しいシーンがロードされたときに実行する処理を記述する
        // シーンのオブジェクトのAwake関数が呼ばれた後、Start関数が呼ばれる前に実行される
        // つまり、Awakeにて、ステージに係る情報を参照しない事
        //Debug.Log("Scene loaded: " + scene.name);
        if("GameScene" == scene.name){
            CreateStage(scene.name);
        }
        else if("GameOnline" == scene.name){
            iStage = 0;
            CreateStage(scene.name);
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
/*
            GameObject gItemControl = Instantiate(Resources.Load("ItemControl") as GameObject);
            gItemControl.name = "ItemControl";
            gItemControl.AddComponent<ItemControl_CpuMode>();

            GameObject gBomControl = Instantiate(Resources.Load("BomControl") as GameObject);
            gBomControl.name = "BomControl";
            gBomControl.AddComponent<BomControl_CpuMode>();
*/
        }

        GameObject gGameEndCanvas = Instantiate(Resources.Load("GameEndCanvas") as GameObject);

        //GameObject gMaterialManager = Instantiate(Resources.Load("MaterialManager") as GameObject);
        //gMaterialManager.name = "MaterialManager";

        GameObject gCanvas = Instantiate(Resources.Load("Canvas") as GameObject);

        /*
        else if("GameOnline"  == scenename){
            gItemControl.AddComponent<ItemControl>();
            gBomControl.AddComponent<BomControl>();
        }
        */

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


}

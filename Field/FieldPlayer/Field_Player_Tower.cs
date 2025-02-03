using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
public class Field_Player_Tower :Field_Event
{
    private Button button; // 対象のボタンをInspectorで設定
    private Field_Player_Base cField;
    private PlayerCountManager cPlayerCountManager;
    private GameManager cGameManager;
    private static bool listenersRegistered = false;

    Color gold;
    Color gray;
    private Image buttonImage;

    protected override void Init() {
        GameObject gFeild = GameObject.Find("Field");
        cField = gFeild.GetComponent<Field_Player_Base>();
        cPlayerCountManager = gFeild.GetComponent<PlayerCountManager>();
        cGameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        button = GetComponent<Button>();
        if (button == null){
            return;
        }
        button.onClick.RemoveListener(PushButton);
        button.onClick.AddListener(() => PushButton());

        GameObject parentObject = transform.parent.gameObject;
        string parentName = parentObject.name;
        if("CanvasPowerGageTower1" != parentObject.name){
            button.gameObject.SetActive(false);
            return;
        }
        buttonImage = button.GetComponent<Image>();
        gold = new Color(1f, 0.84f, 0f);
        gray = new Color(0.6627f, 0.6627f, 0.6627f);
        SetToneDown();
    }

    public void SetToneUp()
    {
        if(button == null || buttonImage == null){
            return;
        }
        button.interactable = true;
        buttonImage.color = gold;
    }
    public void SetToneDown()
    {
        if(button == null || buttonImage == null){
            return;
        }
        button.interactable = false;
        buttonImage.color = gray;
    }
    
    protected override void RegisterListeners()
    {
        if(listenersRegistered){
            return;
        }
        listenersRegistered = true;
        Player_Base.onPlayerAdded.AddListener(Field_Player_Tower_OnAdded);
        Player_Base.onPlayerRemoved.AddListener(Field_Player_Tower_OnRemoved);
    }
    protected override void UnregisterListeners()
    {
        listenersRegistered = false;
        Player_Base.onPlayerAdded.RemoveListener(Field_Player_Tower_OnAdded);
        Player_Base.onPlayerRemoved.RemoveListener(Field_Player_Tower_OnRemoved);
    }


    private bool JudgeMyPlayer(object obj){
        if (obj.GetType() == typeof(Player))
        {
            return true;
        }
        return false;
    }

    protected void Field_Player_Tower_OnAdded(object obj)
    {
        //Debug.Log(obj);
        if(JudgeMyPlayer(obj)){
            SetToneDown();
        }
        else{

        }
    }

    protected void Field_Player_Tower_OnRemoved(object obj)
    {
        //Debug.Log(obj);
        if(JudgeMyPlayer(obj)){
            SetToneUp();
        }
        else{
            EnsurePlayerExists(obj);
        }
	}

    public void PushButton()
    {
        SetToneDown();
        GameObject gObj = GameObject.Find("Tower1");
        if(null != gObj){
            cPlayerCountManager.AddPlayerCount();
            cField.SpawnPlayerObjects(1);
            gObj.GetComponent<PowerGageIF>().SetDamage(2);
        }
    }

    private void EnsurePlayerExists(object obj)
    {
        if(cGameManager.IsGameOver()){
            Debug.Log("EnsurePlayerExists GameOver");
            return;
        }
        if (obj is Player_Base gPlayer)
        {
            for(int iPlayerNo = 2; iPlayerNo <= 4; iPlayerNo++) {
                if(gPlayer.name == "Player"+iPlayerNo && GameObject.Find("Tower"+iPlayerNo) != null){
                    StartCoroutine(CallAddDummyPlayerWithDelay(iPlayerNo));
                    //GameObject.Find("Tower"+iPlayerNo).GetComponent<PowerGageIF>().SetDamage(2);
                }
            }
        }
    }
    private IEnumerator CallAddDummyPlayerWithDelay(int iPlayerNo)
    {
        yield return new WaitForSeconds(2f); // Wait for 1 second
        cField.AddDummyPlayer(iPlayerNo, cField.GetPlayerPosition(cField.GetIndex(), iPlayerNo - 1));
        //GameObject.Find("Tower" + iPlayerNo).GetComponent<PowerGageIF>().SetDamage(2);
    }
    string GetCurrentSceneInfo()
    {
        Scene currentScene = SceneManager.GetActiveScene(); // 現在のシーンを取得
        string sceneName = currentScene.name;              // シーン名
        int sceneIndex = currentScene.buildIndex;          // ビルドインデックス

        Debug.Log($"現在のシーン名: {sceneName}, インデックス: {sceneIndex}");
        return sceneName;
    }
}

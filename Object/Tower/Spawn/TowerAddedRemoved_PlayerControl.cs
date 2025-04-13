using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;
public class TowerAddedRemoved_PlayerControl :Field_Event
{
    private Button button; // 対象のボタンをInspectorで設定
    private PlayerSpawnManager cField;
    private PlayerCountManager cPlayerCountManager;
    private PlayerNameManager cPlayerNameManager;
    private static bool listenersRegistered = false;

    Color gold;
    Color gray;
    private Image buttonImage;
    private int iPlayerNo;
    private string PlayerTowerName;

    protected override void Init() {
        GameObject gFeild = GameObject.Find("Field");
        cField = gFeild.GetComponent<PlayerSpawnManager>();
        //cPlayerPositionManager = gFeild.AddComponent<PlayerPositionManager_CpuMode>();
        cPlayerCountManager = gFeild.GetComponent<PlayerCountManager>();
        cPlayerNameManager = gFeild.GetComponent<PlayerNameManager>();
        string sPlayerNo = cPlayerNameManager.GetPlayerNoString();
        int.TryParse(sPlayerNo, out iPlayerNo);
        PlayerTowerName = MakeTowerName(sPlayerNo);
        
        button = GetComponent<Button>();
        if (button == null){
            return;
        }
        button.onClick.RemoveListener(PushButton);
        button.onClick.AddListener(() => PushButton());

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



    private string ExtractTrailingNumber(string input)
    {
        Match match = Regex.Match(input, @"\d+$");
        return match.Success ? match.Value : "";
    }
/*
    private string GetPlayerName(){
        return cPlayerNameManager.GetPlayerName();
    }
*/
    private string MakeTowerName(string number)
    {
        return "Tower" + number;
    }

    private bool JudgeMyPlayer(object obj)
    {
        if (obj.GetType() == typeof(Player) || obj.GetType() == typeof(Player_Online))
        {
            // objの名称から数字のみを抽出
            string objName = obj.ToString();
            string numberStr = Regex.Replace(objName, @"\D", ""); // 数字以外を除去

            if (int.TryParse(numberStr, out int extractedNumber))
            {
                return extractedNumber == iPlayerNo;
            }
        }
        return false;
    }

    protected void Field_Player_Tower_OnAdded(object obj)
    {
        //Debug.Log(obj);
        if(JudgeMyPlayer(obj)){
            SetToneDown();
        }
    }

    protected void Field_Player_Tower_OnRemoved(object obj)
    {
        //Debug.Log(obj);
        if(JudgeMyPlayer(obj)){
            SetToneUp();
        }
	}

    public void PushButton()
    {
        SetToneDown();
        
        GameObject gObj = GameObject.Find(PlayerTowerName);
        if(null != gObj){
            cPlayerCountManager.AddPlayerCount();
            cField.SpawnPlayer(iPlayerNo);
            gObj.GetComponent<PowerGageIF>().SetDamage(2);
        }
    }
}

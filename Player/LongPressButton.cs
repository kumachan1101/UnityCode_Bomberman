using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LongPressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    private bool isPressed = false;

    public void OnPointerDown(PointerEventData eventData) {
        isPressed = true;
        // ボタンが押されたときの処理をここに記述
    }

    public void OnPointerUp(PointerEventData eventData) {
        // ボタンが離されたときの処理をここに記述
        isPressed = false;
        Player_Base cPlayer = FindAndSetPlayer();
        if (cPlayer != null)
        {
            cPlayer.GetPlayerAction().MoveClear(cPlayer);
        }

    }

    void Update() {
        if (isPressed) {
            // ボタンが押されている間ずっと実行したい処理をここに記述
            PushButton();
        }
    }    
    public virtual void PushButton()
    {
    }
    
    protected Player_Base FindAndSetPlayer()
    {
        Field_Base cField = GameObject.Find("Field").GetComponent<Field_Base>();
        //Debug.Log(cField);
        string name = cField.GetName();
        if(null == name){
            return null;
        }

        GameObject gPlayer = FindPlayerObject(name);
		return gPlayer.GetComponent<Player_Base>();
    }

    private GameObject FindPlayerObject(string name)
    {
        return GameObject.Find(name);
    }

}
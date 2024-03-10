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
        Player cPlayer = FindAndSetPlayer();
        if (cPlayer != null)
        {
            cPlayer.GetPlayerAction().BtnMoveClear(cPlayer);
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
    
    protected Player FindAndSetPlayer()
    {
        Field cField = GameObject.Find("Field").GetComponent<Field>();
        string name = cField.GetName();
        GameObject gPlayer = FindPlayerObject(name);

        if (name.Contains("PlayerOnline"))
        {
            return gPlayer.GetComponent<Player_Online>();
        }
        if (name.Contains("Player"))
        {
            return gPlayer.GetComponent<Player>();
        }
        else
        {
            return null;    
        }

    }

    private GameObject FindPlayerObject(string name)
    {
        return GameObject.Find(name);
    }

}
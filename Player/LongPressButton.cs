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
        Player_Base cPlayer = Library_Base.FindAndSetPlayer();
        if (cPlayer != null)
        {
            cPlayer.GetPlayerAction().MoveClear();
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
    

}
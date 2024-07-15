using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraControlWithButtons : MonoBehaviour
{
    public Button upButton; // 上ボタンの参照
    public Button downButton; // 下ボタンの参照
    public float moveSpeed = 5f; // カメラの移動速度

    private bool isUpButtonPressed = false;
    private bool isDownButtonPressed = false;

    void Start()
    {
		GetInstance();
    }

	private void GetInstance(){
		if(null == upButton || null == downButton){
			return;
		}

        // 上ボタンのイベントトリガーを設定
        EventTrigger upButtonEventTrigger = upButton.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry upButtonPointerDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        upButtonPointerDownEntry.callback.AddListener((data) => { isUpButtonPressed = true; });
        upButtonEventTrigger.triggers.Add(upButtonPointerDownEntry);

        EventTrigger.Entry upButtonPointerUpEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        upButtonPointerUpEntry.callback.AddListener((data) => { isUpButtonPressed = false; });
        upButtonEventTrigger.triggers.Add(upButtonPointerUpEntry);

        // 下ボタンのイベントトリガーを設定
        EventTrigger downButtonEventTrigger = downButton.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry downButtonPointerDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        downButtonPointerDownEntry.callback.AddListener((data) => { isDownButtonPressed = true; });
        downButtonEventTrigger.triggers.Add(downButtonPointerDownEntry);

        EventTrigger.Entry downButtonPointerUpEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        downButtonPointerUpEntry.callback.AddListener((data) => { isDownButtonPressed = false; });
        downButtonEventTrigger.triggers.Add(downButtonPointerUpEntry);

	}

    void Update()
    {
		GetInstance();
        if (isUpButtonPressed)
        {
            MoveCameraUp();
        }

        if (isDownButtonPressed)
        {
            MoveCameraDown();
        }
    }

    void MoveCameraUp()
    {
        // Y座標を増加
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
    }

    void MoveCameraDown()
    {
        // Y座標を減少
        transform.position -= new Vector3(0, moveSpeed * Time.deltaTime, 0);
    }
}

using UnityEngine;
using UnityEngine.UI;
public class ButtonClickScript : MonoBehaviour
{
	void Awake(){
        Button button = transform.Find("ReturnTitle").GetComponent<Button>();

        if (button != null)
        {
            // 動的に関数を追加
            button.onClick.AddListener(() => LoadGameScene());
        }
        else
        {
            Debug.LogError("Button not found on GameEndCanvas");
        }
	}
    virtual public void LoadGameScene(){}
}
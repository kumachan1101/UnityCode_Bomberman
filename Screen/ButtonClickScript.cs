using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ButtonClickScript : MonoBehaviour
{
    private Button returnButton;
    private bool sceneLoadRequested;

    protected virtual void Awake()
    {
        Transform returnTransform = transform.Find("ReturnTitle");
        returnButton = returnTransform != null
            ? returnTransform.GetComponent<Button>()
            : GetComponentInChildren<Button>(true);

        if (returnButton == null)
        {
            Debug.LogError("ReturnTitle button was not found on " + name + ".", this);
            return;
        }

        // Prefabs used to contain a broken persistent callback with a null target.
        // The runtime listener below is the single authoritative callback.
        returnButton.onClick.AddListener(HandleReturnButtonClicked);
    }

    protected virtual void OnDestroy()
    {
        if (returnButton != null)
            returnButton.onClick.RemoveListener(HandleReturnButtonClicked);
    }

    private void HandleReturnButtonClicked()
    {
        if (sceneLoadRequested) return;
        sceneLoadRequested = true;
        LoadGameScene();
    }

    public virtual void LoadGameScene()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.PrepareReturnToTitle();
        SceneManager.LoadScene("GameTitle");
    }
}

using UnityEngine;
public abstract class BaseScreenManager:MonoBehaviour
{
    protected GameObject currentCanvas;

    public abstract void InitializeScreen();

    protected virtual void InitializeCanvas()
    {
        if (currentCanvas == null)
        {
            currentCanvas = Instantiate(Resources.Load("Canvas") as GameObject);

            // This legacy component is also attached to the Canvas prefab.
            CameraControlWithButtons cameraButtons = currentCanvas.GetComponent<CameraControlWithButtons>();
            if (cameraButtons != null) cameraButtons.enabled = false;
            HideCameraControl("JoystickCamera");
            HideCameraControl("up");
            HideCameraControl("down");

            ResponsiveGameUiController responsiveUi =
                currentCanvas.GetComponent<ResponsiveGameUiController>();
            if (responsiveUi == null)
                responsiveUi = currentCanvas.AddComponent<ResponsiveGameUiController>();
            responsiveUi.RefreshLayout();
        }
    }

    private void HideCameraControl(string controlName)
    {
        Transform control = currentCanvas.transform.Find(controlName);
        if (control != null) control.gameObject.SetActive(false);
    }
}

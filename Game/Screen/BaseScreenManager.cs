using UnityEngine;
using UnityEngine.UI;

public abstract class BaseScreenManager:MonoBehaviour
{
    protected GameObject currentCanvas;

    public abstract void InitializeScreen();

    protected virtual void InitializeCanvas()
    {
        if (currentCanvas == null)
        {
            currentCanvas = Instantiate(Resources.Load("Canvas") as GameObject);
            GameObject mainCamera = GameObject.Find("Main Camera");

            JoystickCameraController joystickController = mainCamera.GetComponent<JoystickCameraController>();
            Transform joystickCameraTransform = currentCanvas.transform.Find("JoystickCamera");
            joystickController.joystick = joystickCameraTransform.GetComponent<Joystick>();

            CameraControlWithButtons cameraController = mainCamera.GetComponent<CameraControlWithButtons>();
            Transform upTransform = currentCanvas.transform.Find("up");
            cameraController.upButton = upTransform.GetComponent<Button>();
            Transform downTransform = currentCanvas.transform.Find("down");
            cameraController.downButton = downTransform.GetComponent<Button>();
        }
        GameObject gGameEndCanvas = Instantiate(Resources.Load("GameEndCanvas") as GameObject);
    }
}

using UnityEngine;

public class JoystickController : MonoBehaviour
{
    public Joystick joystick;
	private Vector3 moveVector;
    private bool gestureControlEnabled;

    void FixedUpdate()
    {
        if (gestureControlEnabled)
        {
            if (!HasActiveTouch())
                moveVector = Vector3.zero;
            return;
        }

        if (joystick == null)
        {
            Debug.LogWarning("Joystick reference not set in JoystickController!");
            return;
        }

        float x = joystick.Horizontal;
        float y = joystick.Vertical;

        moveVector = new Vector3(x, 0, y);
    }

	public Vector3 GetMoveVector(){
		if (gestureControlEnabled && !HasActiveTouch())
			moveVector = Vector3.zero;
		return moveVector;
	}

    public void SetGestureControlEnabled(bool enabled)
    {
        gestureControlEnabled = enabled;
        moveVector = Vector3.zero;
    }

    public void SetGestureMoveVector(Vector2 direction)
    {
        if (!gestureControlEnabled) return;
        moveVector = new Vector3(direction.x, 0f, direction.y);
    }

    private static bool HasActiveTouch()
    {
        for (int index = 0; index < Input.touchCount; index++)
        {
            TouchPhase phase = Input.GetTouch(index).phase;
            if (phase != TouchPhase.Ended && phase != TouchPhase.Canceled)
                return true;
        }
        return false;
    }
}

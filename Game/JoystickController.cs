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
}

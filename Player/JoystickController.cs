using UnityEngine;

public class JoystickController : MonoBehaviour
{
    public Joystick joystick;
	private Vector3 moveVector;
    void FixedUpdate()
    {
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
}

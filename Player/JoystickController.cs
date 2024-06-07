using UnityEngine;

public class JoystickController : MonoBehaviour
{
    public Joystick joystick;

    void Update()
    {
        float x = joystick.Horizontal;
        float y = joystick.Vertical;

        Field_Base cField = GameObject.Find("Field").GetComponent<Field_Base>();
        string name = cField.GetName();
        GameObject gPlayer = GameObject.Find(name);
        Player_Base cPlayer = gPlayer.GetComponent<Player_Base>();
        PlayerAction cPlayerAction = cPlayer.GetPlayerAction();

        Vector3 moveVector = new Vector3(x, 0, y);
        cPlayerAction.MovePlayer(gPlayer, moveVector, 2f);
    }
}

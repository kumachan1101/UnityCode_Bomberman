using UnityEngine;

public class PlayerInput
{
    public bool pushBtnUp { get; set; }
    public bool pushBtnDown { get; set; }
    public bool pushBtnLeft { get; set; }
    public bool pushBtnRight { get; set; }
    public bool pushBtnEnter { get; set; }

    public void UpdateInput()
    {
        pushBtnUp = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
        pushBtnDown = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        pushBtnLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        pushBtnRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        pushBtnEnter = Input.GetKey(KeyCode.Space);
    }

    public void ClearInput()
    {
        pushBtnUp = false;
        pushBtnDown = false;
        pushBtnLeft = false;
        pushBtnRight = false;
        pushBtnEnter = false;
    }
}

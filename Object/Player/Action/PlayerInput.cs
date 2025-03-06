using UnityEngine;
using System.Collections.Generic;

public enum InputAction
{
    Up,
    Down,
    Left,
    Right,
    Enter
}

public class PlayerInput
{
    private Dictionary<InputAction, bool> inputStates;
    private Dictionary<InputAction, KeyCode[]> keyMappings;

    private JoystickController joystickController;

    private Vector3 joystickMove;
    public PlayerInput()
    {
        // 入力状態を初期化
        inputStates = new Dictionary<InputAction, bool>
        {
            { InputAction.Up, false },
            { InputAction.Down, false },
            { InputAction.Left, false },
            { InputAction.Right, false },
            { InputAction.Enter, false }
        };

        // キーボードキーのマッピング
        keyMappings = new Dictionary<InputAction, KeyCode[]>
        {
            { InputAction.Up, new[] { KeyCode.UpArrow, KeyCode.W } },
            { InputAction.Down, new[] { KeyCode.DownArrow, KeyCode.S } },
            { InputAction.Left, new[] { KeyCode.LeftArrow, KeyCode.A } },
            { InputAction.Right, new[] { KeyCode.RightArrow, KeyCode.D } },
            { InputAction.Enter, new[] { KeyCode.Return } }
        };

        // ジョイスティックコントローラーの取得
        GameObject joystickPlayer = GameObject.Find("JoystickPlayer");
        if (joystickPlayer != null)
        {
            joystickController = joystickPlayer.GetComponent<JoystickController>();
        }
    }

    public bool IsInputActive(InputAction action)
    {
        return inputStates.TryGetValue(action, out bool isActive) && isActive;
    }

    public void UpdateInput()
    {
        // キーボードの入力を更新
        foreach (var mapping in keyMappings)
        {
            inputStates[mapping.Key] = false; // 初期化
            foreach (var key in mapping.Value)
            {
                if (mapping.Key == InputAction.Enter)
                {
                    // Enterの場合のみ GetKeyDown を使用
                    if (Input.GetKeyDown(key))
                    {
                        inputStates[mapping.Key] = true;
                        break;
                    }
                }
                else
                {
                    // その他のアクションは GetKey を使用
                    if (Input.GetKey(key))
                    {
                        inputStates[mapping.Key] = true;
                        break;
                    }
                }
            }
        }

        // ジョイスティックの入力を反映（必要に応じて上書き）
        if (joystickController != null)
        {
            joystickMove = joystickController.GetMoveVector();
            if (joystickMove.z > 0) inputStates[InputAction.Up] = true;
            if (joystickMove.z < 0) inputStates[InputAction.Down] = true;
            if (joystickMove.x < 0) inputStates[InputAction.Left] = true;
            if (joystickMove.x > 0) inputStates[InputAction.Right] = true;
        }
    }

    public void ClearInput()
    {
        // Dictionary の全てのキーを列挙して値を false に設定
        var keys = new List<InputAction>(inputStates.Keys); // キーをリストにコピー
        foreach (var key in keys)
        {
            inputStates[key] = false;
        }
    }



    public Vector3 GetJoystickVector(){
        return joystickMove;
    }
}

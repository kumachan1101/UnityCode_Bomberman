using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.Collections.LowLevel.Unsafe;

[System.Serializable]
public class PlayerAction : MonoBehaviour
{
    [SerializeField] protected PlayerMovement playerMovement;
    private PlayerInput playerInput;

    private PlayerBomToBomControl cPlayerBomToBomControl;

    // 入力と方向の紐付けを行うDictionaryを用意
    private Dictionary<InputAction, Vector3> movementBindings;
    private Dictionary<InputAction, Action> actionBindings;

    public void Start()
    {
        InitCommon();
        InitDiff();
    }

    private void InitCommon()
    {
        InitializeBindings();
        playerMovement = this.gameObject.AddComponent<PlayerMovement>();
        playerInput = new PlayerInput();
        cPlayerBomToBomControl = this.gameObject.AddComponent<PlayerBomToBomControl>();
    }

    protected virtual void InitDiff() { }

    protected virtual void InitializeBindings()
    {
        movementBindings = new Dictionary<InputAction, Vector3>
        {
            { InputAction.Up, Vector3.forward },
            { InputAction.Down, Vector3.back },
            { InputAction.Left, Vector3.left },
            { InputAction.Right, Vector3.right }
        };

        actionBindings = new Dictionary<InputAction, Action>
        {
            { InputAction.Enter, DropBom }
        };
    }

    void Update()
    {
        UpdatePlayer();
    }

    protected virtual bool IsAvailable()
    {
        return true;
    }

    protected void UpdatePlayer()
    {
        if (!IsAvailable())
        {
            return;
        }

        playerInput.UpdateInput();
        UpdatePlayerMovement();
        UpdatePlayerActions();
    }

    protected virtual void UpdatePlayerMovement()
    {
        // ジョイスティックの移動ベクトルを取得し動作に反映
        Vector3 joystickMove = playerInput.GetJoystickVector();
        if (joystickMove != Vector3.zero)
        {
            PerformPlayerAction(joystickMove);
            return;
        }

        // キー入力の方向を処理
        foreach (var binding in movementBindings)
        {
            if (playerInput.IsInputActive(binding.Key))
            {
                PerformPlayerAction(binding.Value);
                return;
            }
        }

        // どの方向にも動かない場合の処理
        MoveClear();
    }

    protected virtual void UpdatePlayerActions()
    {
        foreach (var binding in actionBindings)
        {
            if (playerInput.IsInputActive(binding.Key))
            {
                binding.Value.Invoke();
            }
        }
    }

    public virtual void UpdateKey() { }

    public void DropBom()
    {
        Vector3 position = Library_Base.GetPos(transform.position);
        Vector3 direction = transform.forward;
        cPlayerBomToBomControl.RequestDropBom(position, direction);
    }

    public void MoveClear()
    {
        playerMovement.MoveClear();
    }

    public void PerformPlayerAction(Vector3 moveDirection)
    {
        MoveClear();
        playerMovement.Move(moveDirection);
    }

    public void SpeedUp()
    {
        if(null == playerMovement){
            return;
        }
        playerMovement.SpeedUp();
    }
}

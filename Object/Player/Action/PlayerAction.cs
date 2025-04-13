
using UnityEngine;
using System.Collections.Generic;
using System;
public interface IPlayerActionStrategy
{
    void UpdateStrategy(PlayerMovement playerMovement);
    void DropBom();
}

/*
public abstract class BasePlayerActionStrategy : IPlayerActionStrategy
{
    private PlayerBomToBomControl bomControl;
    protected PlayerInput playerInput;

    protected BasePlayerActionStrategy(PlayerBomToBomControl bomControl)
    {
        this.bomControl = bomControl;
        playerInput = new PlayerInput();
    }

    public abstract void UpdateStrategy(PlayerMovement playerMovement);
    
    public void DropBom()
    {
        //Vector3 position = Library_Base.GetPos(bomControl.transform.position);
        //Vector3 direction = bomControl.transform.forward;
        bomControl.RequestDropBom();
    }
}
*/


public abstract class BasePlayerActionStrategy : IPlayerActionStrategy
{
    private readonly Action requestDropBomAction;
    protected PlayerInput playerInput;

    protected BasePlayerActionStrategy(Action requestDropBomAction)
    {
        this.requestDropBomAction = requestDropBomAction;
        playerInput = new PlayerInput();
    }

    public abstract void UpdateStrategy(PlayerMovement playerMovement);
    
    public void DropBom()
    {
        requestDropBomAction?.Invoke();
    }
}



public class PlayerActionStrategy : BasePlayerActionStrategy
{
    private Dictionary<InputAction, Vector3> movementBindings;

/*
    public PlayerActionStrategy(PlayerBomToBomControl bomControl) : base(bomControl)
    {
        movementBindings = new Dictionary<InputAction, Vector3>
        {
            { InputAction.Up, Vector3.forward },
            { InputAction.Down, Vector3.back },
            { InputAction.Left, Vector3.left },
            { InputAction.Right, Vector3.right }
        };
    }
*/
    // Action を受け取るように変更
    public PlayerActionStrategy(Action requestDropBomAction) : base(requestDropBomAction)
    {
        movementBindings = new Dictionary<InputAction, Vector3>
        {
            { InputAction.Up, Vector3.forward },
            { InputAction.Down, Vector3.back },
            { InputAction.Left, Vector3.left },
            { InputAction.Right, Vector3.right }
        };
    }

    public override void UpdateStrategy(PlayerMovement playerMovement)
    {
        playerInput.UpdateInput();
        Vector3? moveDirection = GetMoveDirection(playerInput);
        if (moveDirection.HasValue)
        {
            playerMovement.Move(moveDirection.Value);
        }
        else
        {
            playerMovement.MoveClear();
        }

        if (playerInput.IsInputActive(InputAction.Enter))
        {
            DropBom();
        }
    }

    private Vector3? GetMoveDirection(PlayerInput playerInput)
    {
        Vector3 joystickMove = playerInput.GetJoystickVector();
        if (joystickMove != Vector3.zero)
        {
            return joystickMove;
        }

        foreach (var binding in movementBindings)
        {
            if (playerInput.IsInputActive(binding.Key))
            {
                return binding.Value;
            }
        }

        return null;
    }
}


public class PlayerAction : MonoBehaviour
{
    private PlayerMovement playerMovement;

    protected PlayerBomToBomControl cPlayerBomToBomControl;
    protected IPlayerActionStrategy playerStrategy;

    public void Start()
    {
        cPlayerBomToBomControl = gameObject.AddComponent<PlayerBomToBomControl>();
        playerMovement = gameObject.AddComponent<PlayerMovement>();

        CreatePlayerStrategy();
    }

    protected virtual void CreatePlayerStrategy()
    {
        //playerStrategy = new PlayerActionStrategy(cPlayerBomToBomControl);
        playerStrategy = new PlayerActionStrategy(cPlayerBomToBomControl.RequestDropBom);
    }

    protected virtual bool IsAvailable() => true;

    void Update()
    {
        if (!IsAvailable()) return;

        playerStrategy.UpdateStrategy(playerMovement);
    }

    public void DropBom()
    {
        playerStrategy.DropBom();
    }
}


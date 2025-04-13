
using UnityEngine;
using System.Collections.Generic;
using System;
public class CpuPlayerActionStrategy : BasePlayerActionStrategy
{
    private float timer;
    private float changeDirectionInterval = 3f;
    private Dictionary<int, Vector3> randomMovementBindings;
    private float bombCooldown = 3f;
    private float bombTimer = 0f;
    private int randomDirection;

/*
    public CpuPlayerActionStrategy(PlayerBomToBomControl bomControl) : base(bomControl)
    {
        randomMovementBindings = new Dictionary<int, Vector3>
        {
            { 0, Vector3.forward },
            { 1, Vector3.back },
            { 2, Vector3.left },
            { 3, Vector3.right }
        };
    }
*/
    public CpuPlayerActionStrategy(Action requestDropBomAction) : base(requestDropBomAction)
    {
        randomMovementBindings = new Dictionary<int, Vector3>
        {
            { 0, Vector3.forward },
            { 1, Vector3.back },
            { 2, Vector3.left },
            { 3, Vector3.right }
        };
    }

    public override void UpdateStrategy(PlayerMovement playerMovement)
    {
        if(ShouldChangeDirection()){
            randomDirection = GetNewRandomDirection();
        }

        Vector3? moveDirection = GetMoveDirection();
        if (moveDirection.HasValue)
        {
            playerMovement.Move(moveDirection.Value);
        }

        if (ShouldDropBomb())
        {
            DropBom();
            ResetBombTimer();
        }
    }
    private bool ShouldChangeDirection()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = changeDirectionInterval;
            return true;
        }
        return false;
    }

    private int GetNewRandomDirection()
    {
        return UnityEngine.Random.Range(0, randomMovementBindings.Count);
    }

    private Vector3? GetMoveDirection()
    {
        if (randomMovementBindings.TryGetValue(randomDirection, out var direction))
        {
            return direction;
        }
        return null;
    }

    private bool ShouldDropBomb()
    {
        bombTimer -= Time.deltaTime;
        return bombTimer <= 0f;
    }

    private void ResetBombTimer()
    {
        bombTimer = UnityEngine.Random.Range(1f, bombCooldown);
    }
}

public class PlayerAction_CpuMode : PlayerAction
{
    protected override void CreatePlayerStrategy()
    {
        playerStrategy = new CpuPlayerActionStrategy(cPlayerBomToBomControl.RequestDropBom);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionAttack : Explosion_Base
{
    private Vector3 moveDirection;
    private float moveSpeed;

    private int movePower;

    ExplosionAttack(Vector3 v3, Vector3 para_moveDirection, float para_moveSpeed, int para_movePower){
        transform.position = v3;
        moveDirection = para_moveDirection;
        moveSpeed = para_moveSpeed;
        movePower = para_movePower;
        bField = true;
    }

    void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime * 2;
    }
 
}

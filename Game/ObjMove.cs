using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjMove : MonoBehaviour
{
    public float moveSpeed = 2f; // 物体の移動速度
    [SerializeField] private Vector3 moveDirection = Vector3.right; // 初期の移動方向（右方向）

    public void SetMoveDirection(Vector3 v3){
        moveDirection = v3;
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.tag);
        
        // 衝突中の物体があるかを確認
        if (IsTagMatch("Player", other.tag)) 
        {
            //other.gameObject.GetComponent<Explosion>().StopInvoke();
        }
    }

    void OnTriggerStay(Collider other)
    {
        // 衝突中の物体があるかを確認
        if (IsTagMatch("Player", other.tag)) 
        {
            // 指定された方向に物体を移動
            MoveInDirection(other.gameObject, moveDirection);
        }
    }

    // タグが指定された文字列で始まるかどうかをチェックする関数
    bool IsTagMatch(string tagStart, string tag)
    {
        return tag.StartsWith(tagStart);
    }

    // 指定された方向に物体を移動する関数
    void MoveInDirection(GameObject obj, Vector3 direction)
    {
        Vector3 moveDelta = direction * moveSpeed * Time.deltaTime;
        obj.transform.position += moveDelta;
    }
}

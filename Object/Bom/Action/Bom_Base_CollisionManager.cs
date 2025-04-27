using UnityEngine;

public class Bom_Base_CollisionManager : MonoBehaviour
{
    public bool CheckForCollision()
    {

        bool bCollistion = false;
        // 移動方向にレイを飛ばして衝突を検知
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1f))
        {
            // 衝突したオブジェクトの名前によって処理を分岐する
            switch (hit.transform.name)
            {
                case "Broken(Clone)":
                case "FixedWall(Clone)":
                case "Wall(Clone)":
                case "Bom(Clone)":
                case "Bombigban(Clone)":
                case "BomExplode(Clone)":
                    bCollistion = true;
                    break;
                default:
                    bCollistion = false;
                    break;
            }
        }
        return bCollistion;
    }
/*
    void OnTriggerEnter(Collider other)
    {
        switch (other.transform.name)
        {
            case "Explosion1(Clone)":
            case "Explosion2(Clone)":
            case "Explosion3(Clone)":
            case "Explosion4(Clone)":
                //CancelInvokeAndCallExplosion();
                break;
            default:
                return;
        }
    }
*/

    void OnTriggerExit(Collider other)
    {
        //Debug.Log("すり抜けた！");
        GetComponent<SphereCollider>().isTrigger = false;
        
    }
}

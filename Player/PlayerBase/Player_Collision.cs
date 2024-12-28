using UnityEngine;
using Photon.Pun;
public class Player_Collision : MonoBehaviour
{

    private string MaterialType;
    private float lastDamageTime = 0f;
    private float damageCooldown = 1f; // ダメージを受ける間隔（秒）
    private MaterialManager materialManager;
    void Start()
    {
        InitializeMaterialType();
	}

	private void InitializeMaterialType()
	{
		materialManager = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
		MaterialType = materialManager.GetBomMaterialByPlayerName(this.gameObject.name);
	}


    public void OnTriggerEnter (Collider other)
    {
        if (other.CompareTag ("Explosion"))
        {
            string materialName = other.GetComponent<Renderer>().material.name.Replace(" (Instance)", "");
            if(MaterialType != materialName){
                if (Time.time - lastDamageTime < damageCooldown){
                    return;
                }
                int iDamage = other.GetComponent<Explosion_Base>().GetDamage();
                GetComponent<PowerGageIF>().SetDamage(iDamage);
                // 最後にダメージを受けた時間を更新
                lastDamageTime = Time.time;
            }
        }
    }

    private void OnCollisionEnter(Collision collision){
        switch (collision.transform.name){
            case "Bom(Clone)":
            case "Bombigban(Clone)":
            case "BomExplode(Clone)":
                //Debug.Log(collision.transform.name);
                // ここに処理を記述
                break;
            case "Ground(Clone)":
            default:
                return;
        }

        Vector3 collisionDirectionTemp = Vector3.zero;
        Vector3 collisionDirection = Vector3.zero;

        foreach (ContactPoint contact in collision.contacts)
        {
            collisionDirectionTemp += contact.point - transform.position;
        }

        collisionDirectionTemp /= collision.contacts.Length;

        //Debug.Log("X :" + collisionDirectionTemp.x + "Z :" + collisionDirectionTemp.z);
        float threshold = 0.3f; // 閾値
        // x軸方向の判定
        if (Mathf.Abs(collisionDirectionTemp.x) > threshold)
        {
            collisionDirection.x = collisionDirectionTemp.x;
        }
        else if (Mathf.Abs(collisionDirectionTemp.z) > threshold)
        {
            collisionDirection.z = collisionDirectionTemp.z;
        }
        // Bomオブジェクトに方向を伝える
        collision.transform.GetComponent<Bom_Base>().SetMoveDirection(collisionDirection * 1.5F);
    }


}
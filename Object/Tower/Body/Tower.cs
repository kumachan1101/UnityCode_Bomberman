using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class Tower : MonoBehaviour
{
    private string MaterialType;
    private float lastDamageTime = 0f;
    private float damageCooldown = 1f; // ダメージを受ける間隔（秒）

    public static UnityEvent<Tower> onAdded = new UnityEvent<Tower>();
    public static UnityEvent<Tower> onRemoved = new UnityEvent<Tower>();

    void Start()
    {
        onAdded.Invoke(this);  // 自分が追加されたことを通知
        UpdateMaterial();

    }
    void OnDestroy()
    {
        onRemoved.Invoke(this);  // 自分が削除されたことを通知
    }
	public virtual void DestroySync(){
		Destroy(this.gameObject);
	}
    public void SetMaterialType(string Type){
        MaterialType = Type;
    }

    private void OnTriggerEnter (Collider other)
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
    void UpdateMaterial()
    {
        // インスタンス生成直後にマテリアルを設定している事もあり、Awakeのタイミングではまだマテリアルが取得できない。
        // 初回描画のタイミングであれば取得可能であるため、ここでマテリアルを設定している
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            renderer = GetComponentInChildren<Renderer>();
        }
        if (renderer != null)
        {
            MaterialManager cMaterialMng = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
            Material newMaterial = cMaterialMng.GetMaterial(MaterialType);
            renderer.material = newMaterial;
        }
        else
        {
            Debug.LogWarning("Renderer component not found on the game object or its children.");
        }
    }
}

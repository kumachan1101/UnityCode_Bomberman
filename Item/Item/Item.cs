using UnityEngine;
public abstract class Item : MonoBehaviour {
    private SoundManager soundManager;

    void Awake(){
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }
    void Start(){
    }

    public abstract void Reflection(GameObject gObj);

    private void OnTriggerEnter(Collider col){
        if(col.transform.name.StartsWith("Player")){
            Reflection(col.gameObject);
            Destroy(this.gameObject);
            soundManager.PlaySoundEffect("GETITEM");
        }
    }



}

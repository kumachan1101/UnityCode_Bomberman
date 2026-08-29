using UnityEngine;
public abstract class Item : MonoBehaviour {
    private SoundManager soundManager;

    void Awake(){
        soundManager = SoundManager.Instance;
        if (soundManager == null)
        {
            GameObject soundObject = GameObject.Find("SoundManager");
            if (soundObject != null) soundManager = soundObject.GetComponent<SoundManager>();
        }
    }
    void Start(){
    }

    public abstract void Reflection(GameObject gObj);

    private void OnTriggerEnter(Collider col){
        if(col.transform.name.StartsWith("Player")){
            Reflection(col.gameObject);
            Destroy(this.gameObject);
            if (soundManager != null) soundManager.PlaySoundEffect("GETITEM");
        }
    }



}

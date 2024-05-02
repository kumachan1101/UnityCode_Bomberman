
public class ExplosionFixed : Explosion
{
    // Start is called before the first frame update
    void Start()
    {
        if(bField){
            //cField.AddExplosion(this.gameObject);
            return;
        }
        //Invoke(nameof(hide), 1f);
    }
}

public class Player : Player_Base
{
    public override void AddPlayerComponent(){
        this.gameObject.AddComponent<PlayerAction>();
    }

}

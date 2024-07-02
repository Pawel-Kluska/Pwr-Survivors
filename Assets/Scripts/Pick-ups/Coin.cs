
public class Coin : Pickup
{

    protected override void OnDestroy()
    {
        if (target)
            GameManager.instance.Coins++;
    }
}

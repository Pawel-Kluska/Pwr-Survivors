
public class HealthPotion : Pickup
{
    public int healthToRestore;

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (target)
            target.RestoreHealth(healthToRestore);
    }
}

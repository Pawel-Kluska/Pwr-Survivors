
public class ExperienceGem : Pickup
{
    public int experienceGranted;

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (target)
            target.IncreaseExperience(experienceGranted);
    }
}

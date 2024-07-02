using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "Character Data")]
public class CharacterData : ScriptableObject
{
    [SerializeField]
    Sprite icon;
    public Sprite Icon { get => icon; private set => icon = value; }

    [SerializeField]
    new string name;
    public string Name { get => name; private set => name = value; }

    [SerializeField]
    WeaponData startingWeapon;
    public WeaponData StartingWeapon { get => startingWeapon; private set => startingWeapon = value; }

    [System.Serializable]
    public struct Stats
    {
        public float maxHealth, recovery;
        [Range(-1, 10)] public float moveSpeed, might;
        [Range(-1, 5)] public float speed, duration;
        public float magnet;
        [Range(-1, 1)] public float cooldown;


        public static Stats operator +(Stats s1, Stats s2)
        {
            s1.maxHealth += s2.maxHealth;
            s1.recovery += s2.recovery;
            s1.moveSpeed += s2.moveSpeed;
            s1.might += s2.might;
            s1.speed += s2.speed;
            s1.magnet += s2.magnet;
            s1.duration += s2.duration;
            s1.cooldown += s2.cooldown;

            return s1;
        }
    }
    public Stats stats = new Stats
    {
        maxHealth = 500,
        recovery = 0,
        moveSpeed = 1,
        might = 1,
        speed = 1,
        duration = 1,
        magnet = 0.8f,
        cooldown = 1
    };
}

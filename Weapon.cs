public class Weapon
{
    public string Type { get; set; }
    public int MaxDamage { get; set; }
    public string AsciiArt { get; set; }

    public Weapon(string type, int maxDamage, string asciiArt)
    {
        Type = type;
        MaxDamage = maxDamage;
        AsciiArt = asciiArt;
    }

    public const int EnchantmentBonus = 4;

    /// <summary>
    /// The Magical Weapon the Merchant makes from this one: same kind and art, +4 maximum damage.
    /// The "magical ..." type is a canonical key; each language hand-translates it in
    /// displayWeaponMap so grammatical gender is right (ADR 0001).
    /// </summary>
    public Weapon Enchant() => new Weapon("magical " + Type, MaxDamage + EnchantmentBonus, AsciiArt);
}
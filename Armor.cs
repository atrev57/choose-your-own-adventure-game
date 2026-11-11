// Armor the player wears. Its Armor Bonus extends the Defense Roll beyond Agility
// into the Partial Block range (see GLOSSARY.md).
public class Armor
{
    public string Type { get; }
    public int ArmorBonus { get; }

    public Armor(string type, int armorBonus)
    {
        Type = type;
        ArmorBonus = armorBonus;
    }

    // The Merchant's Magical Armor. Type is a canonical key translated via displayArmorMap.
    public static Armor Magical() => new Armor("magical armor", 5);
}

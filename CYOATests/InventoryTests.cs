// CYOATests/InventoryTests.cs
// The Inventory (Wielding, Wearing, Carrying) as shown in the character stats and the combat stats screen.
using System.Text.RegularExpressions;

[TestClass]
[DoNotParallelize]
public class InventoryTests
{
    private static Messages EnglishMessages()
    {
        Messages messages = new Messages();
        messages.ReadDictionary();
        return messages;
    }

    private static Player Thief() => new Player
    {
        Name = "Aria",
        Race = "Human",
        Occupation = "Thief",
        Strength = 11,
        Agility = 9,
        HealthPoints = 14,
        MaximumHealthPoints = 14,
        Weapon = new Weapon("dagger", 6, "-)==>")
    };

    private static string CharacterStats(Player player, Messages messages)
    {
        TextWriter originalOut = Console.Out;
        using var writer = new StringWriter();
        try
        {
            Console.SetOut(writer);
            player.DisplayStats(messages);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
        return writer.ToString();
    }

    private static string CombatStats(Player player, Messages messages) =>
        new Combat(player, new Dragon(new Die()), messages).GetCombatStatsDisplay();

    private static string[] Lines(string text) =>
        text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

    private static int CountOf(string text, string fragment) =>
        Regex.Matches(text, Regex.Escape(fragment)).Count;

    /// <summary>Asserts the fragments appear as lines in this order.</summary>
    private static void AssertLinesInOrder(string text, params string[] expected)
    {
        string[] lines = Lines(text);
        int from = 0;
        foreach (string line in expected)
        {
            int at = Array.IndexOf(lines, line, from);
            Assert.IsTrue(at >= 0, $"Expected line '{line}' (in order) in:\n{text}");
            from = at + 1;
        }
    }

    [TestMethod]
    public void CharacterStats_ShowInventory_GroupedAsWieldingWearingCarrying()
    {
        string stats = CharacterStats(Thief(), EnglishMessages());

        AssertLinesInOrder(stats,
            "Health Points: 14",
            "----- Inventory -----",
            "Wielding:",
            "  Weapon: dagger",
            "  Weapon's Maximum Damage Points: 6",
            "Wearing:",
            "  nothing",
            "Carrying:",
            "  Gold: 15");
    }

    [TestMethod]
    public void CharacterStats_ShowGoldExactlyOnce_UnderCarrying()
    {
        string stats = CharacterStats(Thief(), EnglishMessages());

        Assert.AreEqual(1, CountOf(stats, "Gold: "), "Gold appears only under Carrying.");
        AssertLinesInOrder(stats, "Carrying:", "  Gold: 15");
    }

    [TestMethod]
    public void Wearing_ShowsMagicalArmorAndArmorBonus_WhenWorn()
    {
        Player player = Thief();
        player.Armor = Armor.Magical();

        string stats = CharacterStats(player, EnglishMessages());

        AssertLinesInOrder(stats,
            "Wearing:",
            "  Armor: Magical Armor",
            "  Armor Bonus: 5",
            "Carrying:");
        Assert.AreEqual(0, CountOf(stats, "  nothing"), "Nothing is empty once armor is worn.");
    }

    [TestMethod]
    public void Wielding_ShowsNothing_ForAPlayerWithNoWeapon()
    {
        Player player = Thief();
        player.Weapon = null;

        string stats = CharacterStats(player, EnglishMessages());

        AssertLinesInOrder(stats, "Wielding:", "  nothing", "Wearing:", "  nothing", "Carrying:");
    }

    [TestMethod]
    public void Wielding_ShowsAMagiciansLightningBoltSpell()
    {
        Player player = Thief();
        player.Occupation = "Magician";
        player.Weapon = new Weapon("lightning bolt spell", 12, "zap~~~~~~");

        string stats = CharacterStats(player, EnglishMessages());

        AssertLinesInOrder(stats,
            "Wielding:",
            "  Weapon: lightning bolt spell",
            "  Weapon's Maximum Damage Points: 12",
            "Wearing:");
    }

    [TestMethod]
    public void Wielding_ShowsTheEnchantedWeaponsHigherMaximumDamage()
    {
        Player player = Thief();
        player.Weapon = player.Weapon!.Enchant();

        string stats = CharacterStats(player, EnglishMessages());

        AssertLinesInOrder(stats,
            "Wielding:",
            "  Weapon: magical dagger",
            "  Weapon's Maximum Damage Points: 10",
            "Wearing:");
    }

    [TestMethod]
    public void Carrying_ShowsHealingPotionsAfterGold_OnlyWhenThereAreAny()
    {
        Messages messages = EnglishMessages();
        Player player = Thief();

        Assert.IsFalse(CharacterStats(player, messages).Contains("Healing Potions"),
            "No Healing Potion line when the count is 0.");

        player.HealingPotions = 2;
        AssertLinesInOrder(CharacterStats(player, messages), "Carrying:", "  Gold: 15", "  Healing Potions: 2");
    }

    [TestMethod]
    public void CombatStats_ShowTheSameInventory_IncludingGold_BelowCurrentOverMaximumHealth()
    {
        Messages messages = EnglishMessages();
        Player player = Thief();
        player.HealthPoints = 9;
        player.HealingPotions = 1;
        player.Armor = Armor.Magical();

        string combat = CombatStats(player, messages);

        AssertLinesInOrder(combat,
            "Health Points: 9 / 14",
            "----- Inventory -----",
            "Wielding:",
            "  Weapon: dagger",
            "  Weapon's Maximum Damage Points: 6",
            "Wearing:",
            "  Armor: Magical Armor",
            "  Armor Bonus: 5",
            "Carrying:",
            "  Gold: 15",
            "  Healing Potions: 1",
            "--- Dragon ---");
        CollectionAssert.IsSubsetOf(player.GetInventoryLines(messages), Lines(combat),
            "The combat stats show the same Inventory as the character stats.");
    }

    [TestMethod]
    [DataRow("Spanish", "----- Inventario -----", "Empunando:", "Vistiendo:", "Llevando:", "  nada")]
    [DataRow("French", "----- Inventaire -----", "En main:", "Porte:", "Transporte:", "  rien")]
    [DataRow("Italian", "----- Inventario -----", "Impugna:", "Indossa:", "Trasporta:", "  niente")]
    public void CharacterStats_ShowTheInventoryInEveryLanguage(
        string language, string header, string wielding, string wearing, string carrying, string nothing)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();

        string stats = CharacterStats(Thief(), messages);

        AssertLinesInOrder(stats, header, wielding, wearing, nothing, carrying);
        Assert.IsFalse(Regex.IsMatch(stats, @"\[[a-z_0-9]+\]"), "Output contains a missing-key marker.");
    }
}

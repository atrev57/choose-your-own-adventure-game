// CYOATests/MerchantTests.cs
// Playthroughs of the South path Merchant through Game.StartGame with scripted console input.
// Every playthrough ends with exit, so the random character-creation rolls never affect assertions.
using System.Text.RegularExpressions;

[TestClass]
[DoNotParallelize]
public class MerchantTests
{
    // English -> create character -> Human Thief "Aria" -> roll x3 -> view stats -> start adventure.
    private const string CreateThief = "1\n1\nhuman\nAria\nthief\nroll\nroll\nroll\n1\n2\n";

    private static string PlayGame(string input)
    {
        TextReader originalIn = Console.In;
        TextWriter originalOut = Console.Out;
        try
        {
            using var writer = new StringWriter();
            Console.SetIn(new StringReader(input));
            Console.SetOut(writer);
            new Game().StartGame();
            return writer.ToString();
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    private static int CountOf(string text, string fragment) =>
        Regex.Matches(text, Regex.Escape(fragment)).Count;

    [TestMethod]
    [Timeout(5000)]
    public void NewCharacter_StartsWith15Gold_ShownInStats()
    {
        string output = PlayGame(CreateThief + "e\n");

        StringAssert.Contains(output, "Gold: 15");
    }

    [TestMethod]
    [Timeout(5000)]
    public void SouthPath_Leave_KeepsGoldAndReturnsToFork()
    {
        string output = PlayGame(CreateThief + "s\n3\ne\n");

        StringAssert.Contains(output, "You meet a Merchant");
        StringAssert.Contains(output, "You have 15 Gold.");
        StringAssert.Contains(output, "1. Magical Armor (10 Gold)");
        StringAssert.Contains(output, "2. Magical Weapon (10 Gold)");
        StringAssert.Contains(output, "3. Leave");
        StringAssert.Contains(output, "You leave the Merchant with 15 Gold");
        Assert.IsFalse(output.Contains("return home"), "South path must no longer send the player home.");
        Assert.AreEqual(2, CountOf(output, "You are walking in the mountains."), "Player should be back at the path fork.");
    }

    [TestMethod]
    [Timeout(5000)]
    public void SouthPath_BuyWeapon_Thief_EnchantsDaggerTo10AndLeaves5Gold()
    {
        // Buy the weapon, then leave (the menu stays open after a purchase; ADR 0002).
        string output = PlayGame(CreateThief + "s\n2\n3\ne\n");

        StringAssert.Contains(output, "You bought a magical dagger! Its maximum damage is now 10. You have 5 Gold left.");
        StringAssert.Contains(output, "You leave the Merchant with 5 Gold");
        Assert.AreEqual(2, CountOf(output, "You are walking in the mountains."), "Player should be back at the path fork.");
    }

    [TestMethod]
    [Timeout(5000)]
    public void SouthPath_BuyArmor_SetsArmorBonus5AndLeaves5Gold()
    {
        string output = PlayGame(CreateThief + "s\n1\n3\ne\n");

        StringAssert.Contains(output, "You bought Magical Armor! Its Armor Bonus is 5. You have 5 Gold left.");
        StringAssert.Contains(output, "You leave the Merchant with 5 Gold");
        Assert.AreEqual(2, CountOf(output, "You are walking in the mountains."), "Player should be back at the path fork.");
    }

    [TestMethod]
    [Timeout(5000)]
    public void SouthPath_SecondPurchase_IsUnaffordable()
    {
        // Buy the weapon, try the armor and the weapon again, then leave.
        string output = PlayGame(CreateThief + "s\n2\n1\n2\n3\ne\n");

        Assert.AreEqual(2, CountOf(output, "You don't have enough Gold for that."));
        Assert.IsFalse(output.Contains("You bought Magical Armor"), "15 Gold must not buy both items (ADR 0002).");
        Assert.IsFalse(output.Contains("maximum damage is now 14"), "The weapon must not be enchanted twice.");
        StringAssert.Contains(output, "You leave the Merchant with 5 Gold");
    }

    [TestMethod]
    [Timeout(5000)]
    public void SouthPath_InvalidMerchantInput_ShowsErrorAndReprompts()
    {
        string output = PlayGame(CreateThief + "s\n9\nweapon\n3\ne\n");

        Assert.AreEqual(2, CountOf(output, "Invalid choice. Please try again."));
        Assert.AreEqual(3, CountOf(output, "You have 15 Gold."), "The Merchant menu should be shown again after each invalid choice.");
        StringAssert.Contains(output, "You leave the Merchant with 15 Gold");
    }

    [TestMethod]
    [Timeout(5000)]
    public void SouthPath_SecondTrip_FindsEmptyStallAndReturnsToFork()
    {
        string output = PlayGame(CreateThief + "s\n3\ns\ne\n");

        Assert.AreEqual(1, CountOf(output, "You meet a Merchant"), "The Merchant is met only once per character.");
        StringAssert.Contains(output, "The Merchant's stall is empty.");
        Assert.AreEqual(3, CountOf(output, "You are walking in the mountains."), "Player should be back at the path fork.");
    }

    [TestMethod]
    [Timeout(5000)]
    public void SouthPath_AfterMerchant_PlayerCanGoNorth()
    {
        // Buy, leave, go North, then retreat at the encounter prompt (before any combat rolls).
        string output = PlayGame(CreateThief + "s\n2\n3\nn\nr\n");

        StringAssert.Contains(output, "You take the North path.");
        StringAssert.Contains(output, "You quickly retreat.");
    }

    // --- Magical Armor display ---

    [TestMethod]
    public void CombatStats_ShowArmorNameAndBonus()
    {
        Messages messages = new Messages();
        messages.ReadDictionary();
        Player player = new Player { Name = "Aria", Armor = Armor.Magical() };

        string stats = new Combat(player, new Dragon(new Die()), messages).GetCombatStatsDisplay();

        StringAssert.Contains(stats, "Armor: Magical Armor");
        StringAssert.Contains(stats, "Armor Bonus: 5");
    }

    [TestMethod]
    public void PlayerStats_ShowTranslatedArmorNameAndBonus()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Italian");
        messages.ReadDictionary();
        Player player = new Player { Name = "Giulia", Armor = Armor.Magical() };

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

        StringAssert.Contains(writer.ToString(), "Armatura: armatura magica");
        StringAssert.Contains(writer.ToString(), "Bonus Armatura: 5");
    }

    // --- Magical Weapon display ---

    [TestMethod]
    public void CombatStats_ShowMagicalWeaponNameAndDamage()
    {
        Messages messages = new Messages();
        messages.ReadDictionary();
        Player player = new Player { Name = "Aria", Weapon = new Weapon("dagger", 6, "-)==>").Enchant() };

        string stats = new Combat(player, new Dragon(new Die()), messages).GetCombatStatsDisplay();

        StringAssert.Contains(stats, "Weapon: magical dagger");
        StringAssert.Contains(stats, "Weapon's Maximum Damage Points: 10");
    }

    [TestMethod]
    [DataRow("English")]
    [DataRow("Spanish")]
    [DataRow("French")]
    [DataRow("Italian")]
    public void EveryMagicalWeaponType_HasADisplayNameInEveryLanguage(string language)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();

        foreach (string baseType in new[] { "long sword", "lightning bolt spell", "dagger", "long bow" })
        {
            string magicalType = new Weapon(baseType, 1, "").Enchant().Type;
            string display = messages.TranslateWeaponForDisplay(magicalType);

            StringAssert.Contains(display, "magi", $"{language} has no magical display name for '{magicalType}'.");
            if (language != "English")
                Assert.AreNotEqual(magicalType, display, $"{language} shows the untranslated key '{magicalType}'.");
        }
    }

    [TestMethod]
    [DataRow("Spanish", "dagger", "daga magica")]
    [DataRow("Spanish", "long bow", "arco largo magico")]
    [DataRow("Italian", "long sword", "spada lunga magica")]
    [DataRow("Italian", "long bow", "arco lungo magico")]
    public void MagicalWeaponNames_UseGrammaticalGender(string language, string baseType, string expected)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();

        string magicalType = new Weapon(baseType, 1, "").Enchant().Type;

        Assert.AreEqual(expected, messages.TranslateWeaponForDisplay(magicalType));
    }
}

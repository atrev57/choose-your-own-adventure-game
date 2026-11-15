// CYOATests/InventoryAtForkTests.cs
// Viewing the Inventory at the path fork through Game.StartGame with scripted console input.
// Every playthrough ends with exit, so the random character-creation rolls never affect assertions.
using System.Text.RegularExpressions;

[TestClass]
[DoNotParallelize]
public class InventoryAtForkTests
{
    // English -> create character -> Human Thief "Aria" -> roll x3 -> start adventure (no stats view).
    private const string CreateThief = "1\n1\nhuman\nAria\nthief\nroll\nroll\nroll\n2\n";

    private static readonly string NL = Environment.NewLine;

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
    public void ViewInventory_BeforeSouth_ShowsStartingInventory_ThenRepromptsForAPath()
    {
        string output = PlayGame(CreateThief + "i\ne\n");

        StringAssert.Contains(output,
            "----- Inventory -----" + NL +
            "Wielding:" + NL +
            "  Weapon: dagger" + NL +
            "  Weapon's Maximum Damage Points: 6" + NL +
            "Wearing:" + NL +
            "  nothing" + NL +
            "Carrying:" + NL +
            "  Gold: 15" + NL);
        Assert.AreEqual(0, CountOf(output, "Healing Potions:"), "No Healing Potion has been found yet.");
        Assert.AreEqual(2, CountOf(output, "You are walking in the mountains."), "The path is asked for again.");
        StringAssert.Contains(output, "Adventure ends. Bye.");
    }

    [TestMethod]
    [Timeout(5000)]
    public void ViewInventory_AfterBuyingMagicalWeaponAndHealingPotion_ShowsTheUpdatedInventory()
    {
        // South -> buy the Magical Weapon (2) and a Healing Potion (3) -> leave (4) -> view -> exit.
        string output = PlayGame(CreateThief + "s\n2\n3\n4\ni\ne\n");

        StringAssert.Contains(output,
            "Wielding:" + NL +
            "  Weapon: magical dagger" + NL +
            "  Weapon's Maximum Damage Points: 10" + NL +
            "Wearing:" + NL +
            "  nothing" + NL +
            "Carrying:" + NL +
            "  Gold: 0" + NL +
            "  Healing Potions: 2" + NL);
    }

    [TestMethod]
    [Timeout(5000)]
    public void ViewInventory_AfterBuyingMagicalArmor_ShowsItUnderWearingWithTheRemainingGold()
    {
        string output = PlayGame(CreateThief + "s\n1\n4\ni\ne\n");

        StringAssert.Contains(output,
            "Wearing:" + NL +
            "  Armor: Magical Armor" + NL +
            "  Armor Bonus: 5" + NL +
            "Carrying:" + NL +
            "  Gold: 5" + NL +
            "  Healing Potions: 1" + NL);
    }

    [TestMethod]
    [Timeout(5000)]
    public void ViewInventory_FullWordInMixedCase_WorksLikeI()
    {
        string output = PlayGame(CreateThief + "  InVeNtOrY  \ne\n");

        StringAssert.Contains(output, "Carrying:" + NL + "  Gold: 15" + NL);
        Assert.AreEqual(0, CountOf(output, "Invalid choice"));
    }

    [TestMethod]
    [Timeout(5000)]
    public void ViewingRepeatedly_NeverTriggersTheSouthVisitOrTheEmptyStall()
    {
        string output = PlayGame(CreateThief + "i\ni\ns\n4\ni\ni\ne\n");

        Assert.AreEqual(4, CountOf(output, "----- Inventory -----"));
        Assert.AreEqual(1, CountOf(output, "You meet a Merchant"));
        Assert.AreEqual(0, CountOf(output, "The Merchant's stall is empty."));
        Assert.AreEqual(2, CountOf(output, "  Healing Potions: 1" + NL),
            "Both views after the South trip show the one found Healing Potion.");
        Assert.AreEqual(0, CountOf(output, "Healing Potions: 2"), "Viewing finds no more Healing Potions.");
        Assert.AreEqual(6, CountOf(output, "You are walking in the mountains."));
    }

    [TestMethod]
    [Timeout(5000)]
    public void InvalidInputAtTheFork_ShowsTheInvalidPathMessage_ListingEveryChoiceIncludingI()
    {
        string output = PlayGame(CreateThief + "west\ne\n");

        StringAssert.Contains(output, "Invalid choice. Please enter 'n', 's', 'e', or 'i'.");
        Assert.AreEqual(0, CountOf(output, "----- Inventory -----"));
    }

    [TestMethod]
    [Timeout(5000)]
    public void PathPrompt_OffersIForInventory()
    {
        string output = PlayGame(CreateThief + "e\n");

        StringAssert.Contains(output, "Enter 'i' for inventory.");
    }
}

// CYOATests/HealingPotionTests.cs
// The found Healing Potion, played through Game.StartGame with scripted console input.
// Stats after the South trip are read from the combat stats screen, which is printed before
// any combat roll; the playthrough then retreats, so random rolls never affect assertions.
using System.Text.RegularExpressions;

[TestClass]
[DoNotParallelize]
public class HealingPotionTests
{
    // English -> create character -> Human Thief "Aria" -> roll x3 -> view stats -> start adventure.
    private const string CreateThief = "1\n1\nhuman\nAria\nthief\nroll\nroll\nroll\n1\n2\n";

    // Go North, attack, then retreat at the first combat prompt.
    private const string FightThenRetreat = "n\na\nr\n";

    private const string FoundHealingPotion = "you find a Healing Potion";

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
    public void FirstSouthTrip_FindsHealingPotionBeforeMerchant_AndStatsShowOne()
    {
        string output = PlayGame(CreateThief + "s\n3\n" + FightThenRetreat);

        int narrative = output.IndexOf("You take the South path.");
        int found = output.IndexOf(FoundHealingPotion);
        int merchant = output.IndexOf("You meet a Merchant");

        Assert.IsTrue(narrative >= 0 && found > narrative, "The Healing Potion is found after the South narrative.");
        Assert.IsTrue(merchant > found, "The Healing Potion is found before the Merchant intro.");
        StringAssert.Contains(output, "Healing Potions: 1");
    }

    [TestMethod]
    [Timeout(5000)]
    public void SecondSouthTrip_ShowsEmptyStall_AndFindsNoHealingPotion()
    {
        string output = PlayGame(CreateThief + "s\n3\ns\n" + FightThenRetreat);

        StringAssert.Contains(output, "The Merchant's stall is empty.");
        Assert.AreEqual(1, CountOf(output, FoundHealingPotion), "The Healing Potion is found only once per character.");
        StringAssert.Contains(output, "Healing Potions: 1");
        Assert.IsFalse(output.Contains("Healing Potions: 2"));
    }

    [TestMethod]
    [Timeout(5000)]
    public void NeverWentSouth_NewCharacterHasNoHealingPotionLine()
    {
        // Character stats are viewed during creation; combat stats come after going straight North.
        string output = PlayGame(CreateThief + FightThenRetreat);

        StringAssert.Contains(output, "----- Character Summary -----");
        StringAssert.Contains(output, "===== Combat Stats =====");
        Assert.IsFalse(output.Contains("Healing Potions"), "No Healing Potion line when the count is 0.");
    }

    [TestMethod]
    [DataRow("English", "Healing Potion")]
    [DataRow("Spanish", "pocion curativa")]
    [DataRow("French", "potion de soin")]
    [DataRow("Italian", "pozione curativa")]
    public void HealingPotionDisplayName_ExistsInEveryLanguage(string language, string expected)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();

        Assert.AreEqual(expected, messages.GetMessage("healing_potion_name"));
    }

    [TestMethod]
    [Timeout(5000)]
    public void Italian_SouthFindsHealingPotion_LeaveMerchant_StatsShowCount()
    {
        // Italian -> Mezzuomo Ladro "Giulia" -> start adventure -> South -> leave (3)
        // -> North -> attack -> retreat at the first combat prompt.
        string output = PlayGame("4\n1\nmezzuomo\nGiulia\nladro\nroll\nroll\nroll\n2\ns\n3\nn\na\nr\n");

        int found = output.IndexOf("Lungo la strada trovi una pozione curativa");
        Assert.IsTrue(found >= 0, "The Italian found Healing Potion message is shown.");
        Assert.IsTrue(output.IndexOf("Incontri un Mercante") > found, "Found before the Merchant intro.");
        StringAssert.Contains(output, "Lasci il Mercante con 15 monete d'oro");
        StringAssert.Contains(output, "Pozioni Curative: 1");
        // Combat stats print before any roll, so current health still equals the maximum.
        Assert.IsTrue(Regex.IsMatch(output, @"Punti Vita: (-?\d+) / \1\b"), "Combat stats show current / maximum in Italian.");
        Assert.IsFalse(Regex.IsMatch(output, @"\[[a-z_0-9]+\]"), "Output contains a missing-key marker like [some_key].");
    }
}

// CYOATests/DrinkHealingPotionTests.cs
// Drinking a Healing Potion in the dragon fight, driven through Combat.StartCombat with
// scripted Dice and scripted console input.
//
// Player's Die: attack roll (d20), then on a hit the dragon's Defense Roll (d20) and damage.
// The hero's Strength is 10, so a player attack roll of 20 always misses.
// Dragon's Die: Strength, Agility, Health at construction, then per attack: attack roll (d20)
// and on a hit the player's Defense Roll (d20) and damage (d12). The dragon's Strength is 18,
// so a dragon attack roll of 20 always misses. A ScriptedDie throws when it runs out of rolls,
// so an unexpected extra round fails the test.
using System.Text.RegularExpressions;

[TestClass]
[DoNotParallelize]
public class DrinkHealingPotionTests
{
    private const int Miss = 20;
    private const int DragonStrength = 18;
    private const int DragonAgility = 5;
    private const int PlayerAgility = 5;

    private const string PlayerAttacks = "you withdraw your";
    private const string DragonAttacks = "Smolderfang attacks";

    private static Messages LoadMessages(string language = "English")
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    private static Player Hero(int health, int maximumHealth, int healingPotions, params int[] rolls) =>
        new Player(new ScriptedDie(rolls))
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 10,
            Agility = PlayerAgility,
            HealthPoints = health,
            MaximumHealthPoints = maximumHealth,
            HealingPotions = healingPotions,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };

    private static Dragon ScriptedDragon(int health, params int[] rollsAfterConstruction) =>
        new Dragon(new ScriptedDie([DragonStrength, DragonAgility, health, .. rollsAfterConstruction]));

    private static (bool won, string output) Fight(Player player, Dragon dragon, string input, Messages? messages = null)
    {
        TextReader originalIn = Console.In;
        TextWriter originalOut = Console.Out;
        try
        {
            using var writer = new StringWriter();
            Console.SetIn(new StringReader(input));
            Console.SetOut(writer);
            bool won = new Combat(player, dragon, messages ?? LoadMessages()).StartCombat();
            return (won, writer.ToString());
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
    public void Drink_At15Of17_Restores2_AndHealthBecomes17()
    {
        // Round 1: both miss. Heal round: the dragon misses. Then retreat.
        Player player = Hero(15, 17, 1, Miss);
        Dragon dragon = ScriptedDragon(30, Miss, Miss);

        (_, string output) = Fight(player, dragon, "h\nr\n");

        StringAssert.Contains(output, "You drink a Healing Potion and restore 2 Health Points (17 / 17).");
        Assert.AreEqual(17, player.HealthPoints);
        Assert.AreEqual(0, player.HealingPotions);
    }

    [TestMethod]
    [Timeout(5000)]
    [DataRow(17, 9)]
    [DataRow(16, 8)]
    public void Drink_RestoresHalfOfMaximumHealth_RoundedUp(int maximumHealth, int expectedRestored)
    {
        Player player = Hero(1, maximumHealth, 1, Miss);
        Dragon dragon = ScriptedDragon(30, Miss, Miss);

        (_, string output) = Fight(player, dragon, "h\nr\n");

        StringAssert.Contains(output, $"restore {expectedRestored} Health Points ({1 + expectedRestored} / {maximumHealth})");
        Assert.AreEqual(1 + expectedRestored, player.HealthPoints);
    }

    [TestMethod]
    public void Drink_WithMaximumHealth1_Restores1()
    {
        // A living player with Maximum Health Points 1 is always at Maximum Health Points in combat,
        // so this case is checked on the player directly.
        Player player = Hero(0, 1, 1);

        HealingPotionDrink drink = player.DrinkHealingPotion();

        Assert.AreEqual(HealingPotionDrinkOutcome.Drank, drink.Outcome);
        Assert.AreEqual(1, drink.Restored);
        Assert.AreEqual(1, player.HealthPoints);
    }

    [TestMethod]
    [Timeout(5000)]
    public void Heal_AtMaximumHealthPoints_IsRefused_KeepsPotion_AndNoRoundIsPlayed()
    {
        // Round 1: both miss, leaving the player at Maximum Health Points. Then refused heal, then retreat.
        Player player = Hero(17, 17, 1, Miss);
        Dragon dragon = ScriptedDragon(30, Miss);

        (_, string output) = Fight(player, dragon, "h\nr\n");

        StringAssert.Contains(output, "Your Health Points are already at their maximum.");
        Assert.AreEqual(1, player.HealingPotions);
        Assert.AreEqual(1, CountOf(output, DragonAttacks), "The dragon attacks only in the opening round.");
        Assert.AreEqual(2, CountOf(output, "'h' to drink a Healing Potion"), "The refused heal re-prompts.");
    }

    [TestMethod]
    [Timeout(5000)]
    public void Heal_WithNoHealingPotions_IsRefused_AndNoRoundIsPlayed()
    {
        Player player = Hero(10, 17, 0, Miss);
        Dragon dragon = ScriptedDragon(30, Miss);

        (_, string output) = Fight(player, dragon, "h\nr\n");

        StringAssert.Contains(output, "You have no Healing Potions left.");
        Assert.AreEqual(10, player.HealthPoints);
        Assert.AreEqual(1, CountOf(output, DragonAttacks), "The dragon attacks only in the opening round.");
        Assert.AreEqual(2, CountOf(output, "'h' to drink a Healing Potion"), "The refused heal re-prompts.");
    }

    [TestMethod]
    [Timeout(5000)]
    public void HealRound_PlayerDoesNotAttack_DragonDoes_AndOnePotionIsUsed()
    {
        // Round 1: both miss. Heal round: 10 + 9 = 17, then the dragon hits for 3. Then retreat.
        Player player = Hero(10, 17, 2, Miss);
        Dragon dragon = ScriptedDragon(30, Miss, 1, PlayerAgility + 1, 3);

        (_, string output) = Fight(player, dragon, "h\nr\n");

        Assert.AreEqual(1, CountOf(output, PlayerAttacks), "The player attacks only in the opening round.");
        Assert.AreEqual(2, CountOf(output, DragonAttacks), "The dragon attacks in the heal round too.");
        Assert.AreEqual(1, player.HealingPotions);
        Assert.AreEqual(14, player.HealthPoints);

        int drank = output.IndexOf("You drink a Healing Potion");
        int dragonDamage = output.IndexOf("Smolderfang deals 3 damage.");
        Assert.IsTrue(drank >= 0 && dragonDamage > drank, "The dragon attacks after the player drinks.");
    }

    [TestMethod]
    [Timeout(5000)]
    public void HealRound_DragonAttackAfterDrinking_CanStillDefeatPlayer()
    {
        // Round 1: both miss. Heal round: 1 + 9 = 10, then the dragon hits for 12.
        Player player = Hero(1, 17, 1, Miss);
        Dragon dragon = ScriptedDragon(30, Miss, 1, PlayerAgility + 1, 12);

        (bool won, string output) = Fight(player, dragon, "h\n");

        Assert.IsFalse(won);
        Assert.AreEqual(0, player.HealthPoints);
        StringAssert.Contains(output, "TestHero has been defeated.");
        Assert.IsFalse(output.Contains("You hastily retreat"));
    }

    [TestMethod]
    [Timeout(5000)]
    public void CombatPrompt_ShowsRemainingHealingPotions()
    {
        // Round 1: both miss. Heal round: the dragon misses. Then retreat.
        Player player = Hero(10, 17, 2, Miss);
        Dragon dragon = ScriptedDragon(30, Miss, Miss);

        (_, string output) = Fight(player, dragon, "h\nr\n");

        StringAssert.Contains(output, "Enter 'a' to attack, 'h' to drink a Healing Potion (2 left) or 'r' to retreat.");
        StringAssert.Contains(output, "Enter 'a' to attack, 'h' to drink a Healing Potion (1 left) or 'r' to retreat.");
    }

    [TestMethod]
    [Timeout(5000)]
    [DataRow("a")]
    [DataRow("attack")]
    [DataRow("x")] // invalid input still plays an attack round, as before
    public void AttackOnlyFight_PlaysTheSameSequenceAsBefore(string input)
    {
        // Round 1: both miss. Round 2: the player hits for 10 and the 10-health dragon falls.
        Player player = Hero(17, 17, 0, Miss, 10, DragonAgility + 1, 10);
        Dragon dragon = ScriptedDragon(10, Miss);

        (bool won, string output) = Fight(player, dragon, input + "\n");

        Assert.IsTrue(won);
        Assert.AreEqual(2, CountOf(output, PlayerAttacks));
        Assert.AreEqual(1, CountOf(output, DragonAttacks));

        int firstPlayerAttack = output.IndexOf(PlayerAttacks);
        int dragonAttack = output.IndexOf(DragonAttacks);
        int prompt = output.IndexOf("Enter 'a' to attack");
        int secondPlayerAttack = output.IndexOf(PlayerAttacks, prompt);
        int defeated = output.IndexOf("you have defeated the dragon");
        Assert.IsTrue(firstPlayerAttack < dragonAttack && dragonAttack < prompt
                      && prompt < secondPlayerAttack && secondPlayerAttack < defeated,
            "Player attacks, dragon attacks, prompt, player attacks, dragon defeated.");
    }

    [TestMethod]
    [Timeout(5000)]
    [DataRow("English", "(1 left)")]
    [DataRow("Spanish", "(te quedan 1)")]
    [DataRow("French", "(il vous en reste 1)")]
    [DataRow("Italian", "(te ne restano 1)")]
    public void DrinkRefusalsAndPrompt_AreLocalized_InEveryLanguage(string language, string promptCount)
    {
        Messages messages = LoadMessages(language);

        // Drink at 10/17, the dragon misses, then heal again with none left, then retreat.
        (_, string drinkThenNone) = Fight(Hero(10, 17, 1, Miss), ScriptedDragon(30, Miss, Miss), "h\nh\nr\n", messages);
        // At Maximum Health Points: refused, then retreat.
        (_, string atMaximum) = Fight(Hero(17, 17, 1, Miss), ScriptedDragon(30, Miss), "h\nr\n", messages);

        StringAssert.Contains(drinkThenNone, promptCount);
        StringAssert.Contains(drinkThenNone, "(17 / 17)");
        StringAssert.Contains(drinkThenNone, messages.GetMessage("healing_potion_none"));
        StringAssert.Contains(atMaximum, messages.GetMessage("healing_potion_at_maximum"));
        foreach (string output in new[] { drinkThenNone, atMaximum })
            Assert.IsFalse(Regex.IsMatch(output, @"\[[a-z_0-9]+\]"), "Output contains a missing-key marker like [some_key].");
    }
}

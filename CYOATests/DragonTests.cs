using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
[TestClass]
public class DragonTests
{

    private Messages GetMessages(string language = "English")
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    [TestMethod]
    public void NewDragon_InitializesRequiredProperties()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);

        Assert.AreEqual("Smolderfang", dragon.Name);
        Assert.IsTrue(dragon.Strength >= 1 && dragon.Strength <= 20);
        Assert.IsTrue(dragon.Agility >= 1 && dragon.Agility <= 20);
        Assert.IsTrue(dragon.HealthPoints >= 1 && dragon.HealthPoints <= 20);

        Assert.IsNotNull(dragon.Weapon);
        Assert.AreEqual("claws", dragon.Weapon.Type);
        Assert.AreEqual(12, dragon.Weapon.MaxDamage);
        Assert.IsFalse(string.IsNullOrWhiteSpace(dragon.Weapon.AsciiArt));
    }

    [TestMethod]
    public void GetRandomDefenseTaunt_ReturnsExpectedTaunt()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);
        Messages messages = GetMessages();

        string taunt = dragon.GetRandomDefenseTaunt(messages);

        string[] validTaunts =
        {
        messages.GetMessage("dragon_defense_taunt_1"),
        messages.GetMessage("dragon_defense_taunt_2"),
        messages.GetMessage("dragon_defense_taunt_3"),
        messages.GetMessage("dragon_defense_taunt_4")
    };

        CollectionAssert.Contains(validTaunts, taunt);
    }

    [TestMethod]
    public void GetRandomDamageReply_ReturnsExpectedReply()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);
        Messages messages = GetMessages();

        string reply = dragon.GetRandomDamageReply(messages);

        string[] validReplies =
        {
        messages.GetMessage("dragon_damage_reply_1"),
        messages.GetMessage("dragon_damage_reply_2"),
        messages.GetMessage("dragon_damage_reply_3"),
        messages.GetMessage("dragon_damage_reply_4")
    };

        CollectionAssert.Contains(validReplies, reply);
    }

    [TestMethod]
    public void Attack_ReturnsNonEmptyResultString()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);

        Player player = new Player
        {
            Name = "TestHero",
            Agility = 10,
            HealthPoints = 30
        };

        string result = dragon.Attack(player, GetMessages());

        Assert.IsFalse(string.IsNullOrWhiteSpace(result));
    }

    [TestMethod]
    public void Attack_DoesNotIncreasePlayerHealth()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);

        Player player = new Player
        {
            Name = "TestHero",
            Agility = 10,
            HealthPoints = 30
        };

        int startingHealth = player.HealthPoints;

        string result = dragon.Attack(player, GetMessages());

        Assert.IsTrue(player.HealthPoints <= startingHealth);
    }

    [TestMethod]
    public void Attack_KeepsPlayerHealthNonNegative()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);

        Player player = new Player
        {
            Name = "TestHero",
            Agility = 0,
            HealthPoints = 1
        };

        string result = dragon.Attack(player, GetMessages());

        Assert.IsTrue(player.HealthPoints >= 0);
    }

    // Dragon construction consumes three rolls: Strength, Agility, HealthPoints.
    // Attack then rolls: attack (d20), Defense Roll (d20), damage (d12).
    private const int DragonStrength = 10;
    private const int DragonAgility = 5;
    private const int DragonHealth = 15;
    private const int HeroAgility = 10;
    private const int StartingHealth = 30;
    private const int DamageRoll = 7;

    private static Dragon ScriptedDragon(params int[] attackRolls)
    {
        int[] constructionRolls = { DragonStrength, DragonAgility, DragonHealth };
        return new Dragon(new ScriptedDie([.. constructionRolls, .. attackRolls]));
    }

    private static Player Hero() => new Player
    {
        Name = "TestHero",
        Agility = HeroAgility,
        HealthPoints = StartingHealth
    };

    [TestMethod]
    public void Attack_Miss_DoesNoDamage()
    {
        Dragon dragon = ScriptedDragon(DragonStrength + 1);
        Player player = Hero();
        Messages messages = GetMessages();

        string result = dragon.Attack(player, messages);

        Assert.AreEqual(StartingHealth, player.HealthPoints);
        StringAssert.Contains(result, string.Format(messages.GetMessage("dragon_missed_player"), dragon.Name, player.Name));
    }

    [TestMethod]
    public void Attack_DefenseRollEqualToAgility_IsFullBlock()
    {
        Dragon dragon = ScriptedDragon(DragonStrength, HeroAgility);
        Player player = Hero();
        Messages messages = GetMessages();

        string result = dragon.Attack(player, messages);

        Assert.AreEqual(StartingHealth, player.HealthPoints);
        StringAssert.Contains(result, string.Format(messages.GetMessage("player_defended_dragon_attack"), player.Name));
    }

    [TestMethod]
    public void Attack_DefenseRollOneAboveAgility_DoesFullDamage()
    {
        Dragon dragon = ScriptedDragon(DragonStrength, HeroAgility + 1, DamageRoll);
        Player player = Hero();
        Messages messages = GetMessages();

        string result = dragon.Attack(player, messages);

        Assert.AreEqual(StartingHealth - DamageRoll, player.HealthPoints);
        StringAssert.Contains(result, string.Format(messages.GetMessage("dragon_damage_dealt"), dragon.Name, DamageRoll));
    }

    // --- Magical Armor: Partial Block band (Armor Bonus 5) ---

    private static Player ArmoredHero()
    {
        Player player = Hero();
        player.Armor = Armor.Magical();
        return player;
    }

    [TestMethod]
    public void Attack_Armored_DefenseRollOneAboveAgility_IsPartialBlockForHalfDamage()
    {
        Dragon dragon = ScriptedDragon(DragonStrength, HeroAgility + 1, 7);
        Player player = ArmoredHero();
        Messages messages = GetMessages();

        string result = dragon.Attack(player, messages);

        Assert.AreEqual(StartingHealth - 3, player.HealthPoints, "7 damage halved and rounded down is 3.");
        StringAssert.Contains(result, "TestHero's armor turns the blow: a Partial Block!");
        StringAssert.Contains(result, string.Format(messages.GetMessage("dragon_damage_dealt"), dragon.Name, 3));
    }

    [TestMethod]
    [DataRow(0, 8, 0, false, DisplayName = "Roll = Agility: Full Block, no damage")]
    [DataRow(5, 8, 4, true, DisplayName = "Roll = Agility + 5: still a Partial Block")]
    [DataRow(1, 8, 4, true, DisplayName = "Even damage halves exactly")]
    [DataRow(1, 1, 0, true, DisplayName = "Damage 1 on a Partial Block rounds down to 0")]
    [DataRow(6, 8, 8, false, DisplayName = "Roll = Agility + 6: full damage")]
    public void Attack_Armored_DefenseRollBands(int rollAboveAgility, int damageRoll, int expectedDamage, bool expectPartialBlock)
    {
        // A Full Block consumes no damage roll; the extra scripted roll is simply unused.
        Dragon dragon = ScriptedDragon(DragonStrength, HeroAgility + rollAboveAgility, damageRoll);
        Player player = ArmoredHero();

        string result = dragon.Attack(player, GetMessages());

        Assert.AreEqual(StartingHealth - expectedDamage, player.HealthPoints);
        Assert.AreEqual(expectPartialBlock, result.Contains("Partial Block"));
    }
}

[TestClass]
[DoNotParallelize]
public class GamePbi18Tests
{
    private TextReader? _originalIn;
    private TextWriter? _originalOut;

    [TestInitialize]
    public void Setup()
    {
        _originalIn = Console.In;
        _originalOut = Console.Out;
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (_originalIn != null)
            Console.SetIn(_originalIn);

        if (_originalOut != null)
            Console.SetOut(_originalOut);
    }

    [TestMethod]
    public void DragonDisplayStats_UsesLocalizedLabels()
    {
        Die die = new Die();
        Dragon dragon = new Dragon(die);

        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();

        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        dragon.DisplayStats(messages);

        string output = writer.ToString();

        StringAssert.Contains(output, messages.GetMessage("dragon_stats_header"));
        StringAssert.Contains(output, messages.GetMessage("stats_name"));
        StringAssert.Contains(output, messages.GetMessage("stats_strength"));
        StringAssert.Contains(output, messages.GetMessage("stats_agility"));
        StringAssert.Contains(output, messages.GetMessage("stats_health"));
        StringAssert.Contains(output, messages.GetMessage("stats_weapon"));
        StringAssert.Contains(output, messages.GetMessage("stats_damage"));
    }

    [TestMethod]
    public void HandleDragonEncounter_AttackPath_DisplaysEncounterFlowText()
    {
        Game game = new Game();
        Die die = new Die();
        Dragon dragon = new Dragon(die);

        Player player = new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 20,
            Agility = 20,
            HealthPoints = 25,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };

        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();

        // "a" enters combat, then "r" retreats from Combat immediately
        Console.SetIn(new StringReader("a\nr\n"));

        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        MethodInfo? method = typeof(Game).GetMethod(
            "HandleDragonEncounter",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        Assert.IsNotNull(method, "HandleDragonEncounter method was not found.");

        method!.Invoke(game, new object[] { player, dragon, messages });

        string output = writer.ToString();

        StringAssert.Contains(output, messages.GetMessage("north_path_narrative"));
        StringAssert.Contains(output, string.Format(messages.GetMessage("dragon_intro"), dragon.Name));
        StringAssert.Contains(output, messages.GetMessage("dragon_stats_intro"));
        StringAssert.Contains(output, messages.GetMessage("dragon_encounter_prompt"));
    }

    [TestMethod]
    public void DragonEncounter_MessageKeys_AreLoaded()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();

        Assert.AreNotEqual("[north_path_narrative]", messages.GetMessage("north_path_narrative"));
        Assert.AreNotEqual("[dragon_intro]", messages.GetMessage("dragon_intro"));
        Assert.AreNotEqual("[dragon_stats_intro]", messages.GetMessage("dragon_stats_intro"));
        Assert.AreNotEqual("[dragon_encounter_prompt]", messages.GetMessage("dragon_encounter_prompt"));
        Assert.AreNotEqual("[dragon_stats_header]", messages.GetMessage("dragon_stats_header"));
    }
}

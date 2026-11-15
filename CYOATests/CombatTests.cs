[TestClass]
public class CombatTests
{
    private Messages CreateEnglishMessages()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        return messages;
    }

    private Player CreateTestPlayer()
    {
        return new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 15,
            Agility = 12,
            HealthPoints = 25,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
    }

    private Dragon CreateTestDragon()
    {
        Dragon dragon = new Dragon(new Die())
        {
            Name = "Smolderfang",
            Strength = 18,
            Agility = 10,
            HealthPoints = 30,
            Weapon = new Weapon("claws", 12, "<<< claws >>>")
        };

        return dragon;
    }

    [TestMethod]
    public void Combat_Constructor_InitializesPlayerAndDragon()
    {
        Player player = new Player();
        Dragon dragon = new Dragon(new Die());
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        Assert.AreSame(player, combat.player);
        Assert.AreSame(dragon, combat.dragon);
    }

    [TestMethod]
    public void Combat_PlayerRetreated_DefaultsToFalse()
    {
        Player player = new Player();
        Dragon dragon = new Dragon(new Die());
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        Assert.IsFalse(combat.PlayerRetreated);
    }

    [TestMethod]
    public void GetCombatStatsDisplay_ReturnsNonEmptyString()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        Assert.IsFalse(string.IsNullOrWhiteSpace(result));
    }

    [TestMethod]
    public void GetCombatStatsDisplay_IncludesCombatHeaders()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, messages.GetMessage("combat_stats_header"));
        StringAssert.Contains(result, messages.GetMessage("combat_player_header"));
        StringAssert.Contains(result, messages.GetMessage("combat_dragon_header"));
    }

    [TestMethod]
    public void GetCombatStatsDisplay_IncludesPlayerCombatStats()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, player.Name);
        StringAssert.Contains(result, player.Strength.ToString());
        StringAssert.Contains(result, player.Agility.ToString());
        StringAssert.Contains(result, player.HealthPoints.ToString());
        StringAssert.Contains(result, player.Weapon!.Type);
        StringAssert.Contains(result, player.Weapon.MaxDamage.ToString());
    }

    [TestMethod]
    public void GetCombatStatsDisplay_IncludesDragonCombatStats()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, dragon.Name);
        StringAssert.Contains(result, dragon.Strength.ToString());
        StringAssert.Contains(result, dragon.Agility.ToString());
        StringAssert.Contains(result, dragon.HealthPoints.ToString());
        StringAssert.Contains(result, dragon.Weapon.Type);
        StringAssert.Contains(result, dragon.Weapon.MaxDamage.ToString());
    }

    [TestMethod]
    [DoNotParallelize]
    public void GetCombatStatsDisplay_ShowsCurrentOverMaximumHealth_AfterDamage()
    {
        // Human: Strength d20=10 + d4=2, Agility d20=12, Health d20=15 + Strength modifier 2 = 17.
        Player player = new Player(new ScriptedDie(10, 2, 12, 15));
        Messages messages = CreateEnglishMessages();

        TextReader originalIn = Console.In;
        TextWriter originalOut = Console.Out;
        try
        {
            Console.SetIn(new StringReader("human\nHero\nfighter\nroll\nroll\nroll\n2\n"));
            Console.SetOut(new StringWriter());
            player.CreateCharacter(messages);
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }

        player.HealthPoints -= 2; // the dragon's damage

        string result = new Combat(player, CreateTestDragon(), messages).GetCombatStatsDisplay();

        StringAssert.Contains(result, "Health Points: 15 / 17");
    }

    [TestMethod]
    public void GetCombatStatsDisplay_UsesLocalizedStatLabels()
    {
        Player player = CreateTestPlayer();
        Dragon dragon = CreateTestDragon();
        Messages messages = CreateEnglishMessages();

        Combat combat = new Combat(player, dragon, messages);

        string result = combat.GetCombatStatsDisplay();

        StringAssert.Contains(result, messages.GetMessage("stats_name"));
        StringAssert.Contains(result, messages.GetMessage("stats_strength"));
        StringAssert.Contains(result, messages.GetMessage("stats_agility"));
        StringAssert.Contains(result, messages.GetMessage("stats_health"));
        StringAssert.Contains(result, messages.GetMessage("stats_weapon"));
        StringAssert.Contains(result, messages.GetMessage("stats_damage"));
    }
}

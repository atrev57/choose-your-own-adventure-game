[TestClass]
public class PlayerTests
{
    [TestMethod]
    public void NewPlayer_ShouldHaveDefaultValues()
    {
        Player player = new Player();

        Assert.AreEqual("", player.Name);
        Assert.AreEqual("", player.Race);
        Assert.AreEqual("", player.Occupation);

        Assert.AreEqual(0, player.Strength);
        Assert.AreEqual(0, player.Agility);
        Assert.AreEqual(0, player.HealthPoints);

        Assert.IsNull(player.Weapon);
    }

    // Player.Attack rolls on the player's Die: attack (d20), dragon Defense Roll (d20), damage.
    // The dragon's Die supplies its three construction rolls, then its taunt or damage reply pick.
    private const int HeroStrength = 15;
    private const int DragonStrength = 10;
    private const int DragonAgility = 5;
    private const int DragonHealth = 20;

    private static Messages EnglishMessages()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        return messages;
    }

    private static Player ScriptedHero(params int[] rolls) => new Player(new ScriptedDie(rolls))
    {
        Name = "TestHero",
        Race = "Human",
        Occupation = "Fighter",
        Strength = HeroStrength,
        Weapon = new Weapon("long sword", 12, "-)=====>")
    };

    private static Dragon ScriptedDragon(params int[] rollsAfterConstruction) =>
        new Dragon(new ScriptedDie([DragonStrength, DragonAgility, DragonHealth, .. rollsAfterConstruction]));

    [TestMethod]
    public void Attack_ScriptedMiss_PrintsMissMessage()
    {
        Player player = ScriptedHero(HeroStrength + 1);
        Dragon dragon = ScriptedDragon();
        Messages messages = EnglishMessages();

        string result = player.Attack(dragon, messages);

        StringAssert.Contains(result, "You missed Smolderfang.");
        Assert.AreEqual(DragonHealth, dragon.HealthPoints);
    }

    [TestMethod]
    public void Attack_ScriptedHitPastDefenseWithDamage7_LowersDragonHealthBy7()
    {
        Player player = ScriptedHero(HeroStrength, DragonAgility + 1, 7);
        Dragon dragon = ScriptedDragon(1);
        Messages messages = EnglishMessages();

        string result = player.Attack(dragon, messages);

        Assert.AreEqual(DragonHealth - 7, dragon.HealthPoints);
        StringAssert.Contains(result, "You deal 7 damage.");
        StringAssert.Contains(result, "Smolderfang's health points are now 13.");
    }

    [TestMethod]
    public void Attack_ScriptedHitWithDefenseRollEqualToDragonAgility_IsDefended()
    {
        Player player = ScriptedHero(HeroStrength, DragonAgility);
        Dragon dragon = ScriptedDragon(1);
        Messages messages = EnglishMessages();

        string result = player.Attack(dragon, messages);

        Assert.AreEqual(DragonHealth, dragon.HealthPoints);
        StringAssert.Contains(result, string.Format(messages.GetMessage("dragon_defended_player_attack"), dragon.Name));
    }
}

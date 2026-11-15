// CYOATests/AttackTests.cs
// Attack.Resolve driven directly with a ScriptedDie and small stand-in Combatants: no Messages,
// no console. One test uses a real Dragon to pin that the dragon's Health Points stop at 0 (issue 02).
// Roll order: hit roll (d20), then on a hit the Defense Roll (d20), then unless
// Full Blocked the damage roll. A ScriptedDie throws when it runs out of rolls, so a test that
// scripts only the rolls it expects proves no extra roll happened.

[TestClass]
public class AttackTests
{
    private sealed class StandInCombatant : ICombatant
    {
        public int Strength { get; init; } = 10;
        public int Agility { get; init; } = 5;
        public int ArmorBonus { get; init; }
        public int MaximumDamage { get; init; } = 12;
        public int HealthPoints { get; set; } = 20;
    }

    [TestMethod]
    public void Miss_RollsNoDefenseRoll_AndDealsNoDamage()
    {
        var attacker = new StandInCombatant { Strength = 10 };
        var defender = new StandInCombatant { HealthPoints = 20 };

        AttackOutcome outcome = Attack.Resolve(attacker, defender, new ScriptedDie(11));

        Assert.IsFalse(outcome.Hit);
        Assert.AreEqual(Block.None, outcome.Block);
        Assert.AreEqual(0, outcome.Damage);
        Assert.IsFalse(outcome.DefenderDefeated);
        Assert.AreEqual(20, defender.HealthPoints);
    }

    [TestMethod]
    public void HitRollExactlyAtStrength_WithNoBlock_DealsFullDamage()
    {
        var attacker = new StandInCombatant { Strength = 10, MaximumDamage = 12 };
        var defender = new StandInCombatant { Agility = 5, ArmorBonus = 0, HealthPoints = 20 };

        // Hit roll 10 (= Strength), Defense Roll 6 (just above Agility, no Armor Bonus), damage 7.
        AttackOutcome outcome = Attack.Resolve(attacker, defender, new ScriptedDie(10, 6, 7));

        Assert.IsTrue(outcome.Hit);
        Assert.AreEqual(Block.None, outcome.Block);
        Assert.AreEqual(7, outcome.Damage);
        Assert.IsFalse(outcome.DefenderDefeated);
        Assert.AreEqual(13, defender.HealthPoints);
    }

    [TestMethod]
    public void DefenseRollExactlyAtAgility_IsAFullBlock_WithNoDamageRoll()
    {
        var attacker = new StandInCombatant { Strength = 10 };
        var defender = new StandInCombatant { Agility = 5, ArmorBonus = 5, HealthPoints = 20 };

        // Hit roll 1, Defense Roll 5 (= Agility). No damage roll is scripted.
        AttackOutcome outcome = Attack.Resolve(attacker, defender, new ScriptedDie(1, 5));

        Assert.IsTrue(outcome.Hit);
        Assert.AreEqual(Block.Full, outcome.Block);
        Assert.AreEqual(0, outcome.Damage);
        Assert.IsFalse(outcome.DefenderDefeated);
        Assert.AreEqual(20, defender.HealthPoints);
    }

    [TestMethod]
    public void DefenseRollExactlyAtAgilityPlusArmorBonus_IsAPartialBlock_HalvingDamageRoundedDown()
    {
        var attacker = new StandInCombatant { Strength = 10, MaximumDamage = 12 };
        var defender = new StandInCombatant { Agility = 5, ArmorBonus = 5, HealthPoints = 20 };

        // Hit roll 1, Defense Roll 10 (= Agility + Armor Bonus), damage 7 -> 3.
        AttackOutcome outcome = Attack.Resolve(attacker, defender, new ScriptedDie(1, 10, 7));

        Assert.IsTrue(outcome.Hit);
        Assert.AreEqual(Block.Partial, outcome.Block);
        Assert.AreEqual(3, outcome.Damage);
        Assert.AreEqual(17, defender.HealthPoints);
    }

    [TestMethod]
    public void DamageBeyondHealthPoints_StopsAtZero()
    {
        var attacker = new StandInCombatant { Strength = 10, MaximumDamage = 12 };
        var defender = new StandInCombatant { Agility = 5, HealthPoints = 3 };

        // Hit roll 1, Defense Roll 20 (no block), damage 7 against 3 Health Points.
        AttackOutcome outcome = Attack.Resolve(attacker, defender, new ScriptedDie(1, 20, 7));

        Assert.AreEqual(7, outcome.Damage);
        Assert.IsTrue(outcome.DefenderDefeated);
        Assert.AreEqual(0, defender.HealthPoints);
    }

    [TestMethod]
    public void DamageExactlyEqualToHealthPoints_DefeatsTheDefender()
    {
        var attacker = new StandInCombatant { Strength = 10, MaximumDamage = 12 };
        var defender = new StandInCombatant { Agility = 5, HealthPoints = 7 };

        AttackOutcome outcome = Attack.Resolve(attacker, defender, new ScriptedDie(1, 20, 7));

        Assert.IsTrue(outcome.DefenderDefeated);
        Assert.AreEqual(0, defender.HealthPoints);
    }

    [TestMethod]
    public void DamageBeyondHealthPoints_StopsAtZero_WhenTheDragonIsTheDefender()
    {
        var attacker = new StandInCombatant { Strength = 10, MaximumDamage = 12 };
        // The dragon rolls Strength 10, Agility 5 and Health Points 3 when it is built.
        var dragon = new Dragon(new ScriptedDie(10, 5, 3));

        // Hit roll 1, Defense Roll 20 (no block), damage 7 against 3 Health Points.
        AttackOutcome outcome = Attack.Resolve(attacker, dragon, new ScriptedDie(1, 20, 7));

        Assert.IsTrue(outcome.DefenderDefeated);
        Assert.AreEqual(0, dragon.HealthPoints);
    }
}

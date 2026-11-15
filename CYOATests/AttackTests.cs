// CYOATests/AttackTests.cs
// Attack.Resolve driven directly with a ScriptedDie and small stand-in Combatants: no Messages,
// no console. Roll order: hit roll (d20), then on a hit the Defense Roll (d20), then unless
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
        public bool HealthPointsStopAtZero { get; init; } = true;
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
    public void DamageBeyondHealthPoints_StopsAtZero_WhenTheDefendersHealthPointsStopAtZero()
    {
        var attacker = new StandInCombatant { Strength = 10, MaximumDamage = 12 };
        var defender = new StandInCombatant { Agility = 5, HealthPoints = 3, HealthPointsStopAtZero = true };

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

    // Step one keeps the dragon's Health Points going below 0, as before; issue 02 changes this.
    [TestMethod]
    public void DamageBeyondHealthPoints_GoesBelowZero_WhenTheDefendersHealthPointsDoNotStopAtZero()
    {
        var attacker = new StandInCombatant { Strength = 10, MaximumDamage = 12 };
        var defender = new StandInCombatant { Agility = 5, HealthPoints = 3, HealthPointsStopAtZero = false };

        AttackOutcome outcome = Attack.Resolve(attacker, defender, new ScriptedDie(1, 20, 7));

        Assert.IsTrue(outcome.DefenderDefeated);
        Assert.AreEqual(-4, defender.HealthPoints);
    }
}

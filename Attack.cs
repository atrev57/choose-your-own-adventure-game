/// <summary>Either side of an Attack: the player or the dragon (see GLOSSARY.md).</summary>
public interface ICombatant
{
    int Strength { get; }
    int Agility { get; }

    // Extends a Full Block's range into the Partial Block range; 0 means no Partial Block.
    int ArmorBonus { get; }

    // The maximum damage of the weapon this Combatant attacks with.
    int MaximumDamage { get; }

    int HealthPoints { get; set; }

    // Temporary: only the player's Health Points stop at 0 today. Issue 02
    // (.scratch/attack-resolution/issues/02) makes the clamp apply to both and removes this.
    bool HealthPointsStopAtZero { get; }
}

public enum Block { None, Full, Partial }

/// <summary>What happened in one Attack. The defender's new Health Points are read from the defender.</summary>
public readonly record struct AttackOutcome(bool Hit, Block Block, int Damage, bool DefenderDefeated);

/// <summary>
/// Resolves every Attack, in either direction, with one set of rules (ADR 0003). It changes the
/// defender's Health Points itself and has no Messages or console output.
/// </summary>
public static class Attack
{
    public static AttackOutcome Resolve(ICombatant attacker, ICombatant defender, Die die)
    {
        int hitRoll = die.Roll(20);
        if (hitRoll > attacker.Strength)
            return new AttackOutcome(false, Block.None, 0, false);

        int defenseRoll = die.Roll(20);
        if (defenseRoll <= defender.Agility)
            return new AttackOutcome(true, Block.Full, 0, false);

        int damage = die.Roll(attacker.MaximumDamage);

        Block block = Block.None;
        if (defenseRoll <= defender.Agility + defender.ArmorBonus)
        {
            block = Block.Partial;
            damage /= 2;
        }

        defender.HealthPoints -= damage;
        if (defender.HealthPointsStopAtZero && defender.HealthPoints < 0)
            defender.HealthPoints = 0;

        return new AttackOutcome(true, block, damage, defender.HealthPoints <= 0);
    }
}

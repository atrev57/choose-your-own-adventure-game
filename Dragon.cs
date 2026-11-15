public class Dragon : ICombatant
{
    private readonly Die _die;

    public string Name { get; set; } = "";
    public int Strength { get; set; }
    public int Agility { get; set; }
    public int HealthPoints { get; set; }

    // Weapon is now a real shared class in the repo.
    public Weapon Weapon { get; set; }

    // Example use of taunts:
    // string taunt = dragon.GetRandomDefenseTaunt().Replace("{race}", player.Race);
    private readonly List<string> _defenseTauntKeys = new()
{
    "dragon_defense_taunt_1",
    "dragon_defense_taunt_2",
    "dragon_defense_taunt_3",
    "dragon_defense_taunt_4"
};

    private readonly List<string> _damageReplyKeys = new()
{
    "dragon_damage_reply_1",
    "dragon_damage_reply_2",
    "dragon_damage_reply_3",
    "dragon_damage_reply_4"
};

    public Dragon(Die die)
    {
        _die = die;

        Name = "Smolderfang";

        // Dragon stats are initialized using Die so the class aligns better
        // with the sequence/start-game diagrams and shared repo structure.
        Strength = _die.Roll(20);
        Agility = _die.Roll(20);
        HealthPoints = _die.Roll(20);

        // The dragon uses a placeholder weapon object for now.
        // This can be refined later if combat requirements expand.
        Weapon = new Weapon("claws", 12, "<<< claws >>>");
    }

    // The dragon wears no armor, so its Partial Block range is empty.
    public int ArmorBonus => 0;

    public int MaximumDamage => Weapon.MaxDamage;

    // Issue 02 makes the dragon's Health Points stop at 0 too.
    bool ICombatant.HealthPointsStopAtZero => false;

    public string Attack(Player player, Messages messages)
    {
        var output = new List<string>();

        output.Add(string.Format(
            messages.GetMessage("dragon_attack_intro"),
            Name,
            player.Name,
            messages.TranslateWeaponForDisplay(Weapon.Type)
        ));

        if (!string.IsNullOrWhiteSpace(Weapon.AsciiArt))
            output.Add(Weapon.AsciiArt);

        AttackOutcome outcome = global::Attack.Resolve(this, player, _die);

        if (!outcome.Hit)
        {
            output.Add(string.Format(messages.GetMessage("dragon_missed_player"), Name, player.Name));
            return string.Join(Environment.NewLine, output);
        }

        output.Add(string.Format(messages.GetMessage("dragon_hit_player"), Name, player.Name));

        if (outcome.Block == Block.Full)
        {
            output.Add(string.Format(messages.GetMessage("player_defended_dragon_attack"), player.Name));
            return string.Join(Environment.NewLine, output);
        }

        if (outcome.Block == Block.Partial)
            output.Add(string.Format(messages.GetMessage("player_partial_block"), player.Name));

        output.Add(string.Format(messages.GetMessage("dragon_damage_dealt"), Name, outcome.Damage));
        output.Add(string.Format(messages.GetMessage("player_health_now"), player.Name, player.HealthPoints));

        if (outcome.DefenderDefeated)
            output.Add(string.Format(messages.GetMessage("player_defeated_narrative"), player.Name));

        return string.Join(Environment.NewLine, output);
    }


    public string GetRandomDefenseTaunt(Messages messages)
    {
        int index = _die.Roll(_defenseTauntKeys.Count) - 1;
        return messages.GetMessage(_defenseTauntKeys[index]);
    }

    public string GetRandomDamageReply(Messages messages)
    {
        int index = _die.Roll(_damageReplyKeys.Count) - 1;
        return messages.GetMessage(_damageReplyKeys[index]);
    }

    public void DisplayStats(Messages messages)
    {
        Console.WriteLine(messages.GetMessage("dragon_stats_header"));
        Console.WriteLine(messages.GetMessage("stats_name") + Name);
        Console.WriteLine(messages.GetMessage("stats_strength") + Strength);
        Console.WriteLine(messages.GetMessage("stats_agility") + Agility);
        Console.WriteLine(messages.GetMessage("stats_health") + HealthPoints);
        Console.WriteLine(messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(Weapon.Type));
        Console.WriteLine(messages.GetMessage("stats_damage") + Weapon.MaxDamage);
    }
}
public enum HealingPotionDrinkOutcome { Drank, NoHealingPotions, AtMaximumHealthPoints }

/// <summary>The result of trying to drink a Healing Potion, with the Health Points actually restored.</summary>
public readonly record struct HealingPotionDrink(HealingPotionDrinkOutcome Outcome, int Restored);

public class Player : ICombatant
{
    private readonly Die _die;

    // Lines under an Inventory heading (Wielding, Wearing, Carrying) are indented by this much.
    private const string InventoryIndent = "  ";

    public Player() : this(new Die())
    {
    }

    // Like the dragon, the player can be given a Die so tests can script its rolls.
    public Player(Die die)
    {
        _die = die;
    }

    public string Name { get; set; } = "";
    public string Race { get; set; } = "";
    public string Occupation { get; set; } = "";

    public int Strength { get; set; }
    public int Agility { get; set; }
    public int HealthPoints { get; set; }

    // The Health Points rolled at character creation; damage never changes it.
    public int MaximumHealthPoints { get; set; }

    // Every character starts with the same Gold: enough for exactly one Merchant item (ADR 0002).
    public const int StartingGold = 15;
    public int Gold { get; set; } = StartingGold;

    // Lives on the player so every new character meets a fresh Merchant.
    public bool HasMetMerchant { get; set; }

    public int HealingPotions { get; set; }

    public void FindHealingPotion(Messages messages)
    {
        HealingPotions++;
        Console.WriteLine(string.Format(messages.GetMessage("healing_potion_found"),
            messages.GetMessage("healing_potion_name")));
    }

    /// <summary>
    /// Drinks a Healing Potion: restores half of Maximum Health Points, rounded up (at least 1),
    /// never going above Maximum Health Points. Refused, changing nothing, when the player has no
    /// Healing Potions or is already at Maximum Health Points.
    /// </summary>
    public HealingPotionDrink DrinkHealingPotion()
    {
        if (HealingPotions <= 0)
            return new HealingPotionDrink(HealingPotionDrinkOutcome.NoHealingPotions, 0);

        if (HealthPoints >= MaximumHealthPoints)
            return new HealingPotionDrink(HealingPotionDrinkOutcome.AtMaximumHealthPoints, 0);

        int healAmount = Math.Max(1, (MaximumHealthPoints + 1) / 2);
        int restored = Math.Min(healAmount, MaximumHealthPoints - HealthPoints);

        HealingPotions--;
        HealthPoints += restored;
        return new HealingPotionDrink(HealingPotionDrinkOutcome.Drank, restored);
    }

    public Weapon? Weapon { get; set; }
    public Armor? Armor { get; set; }

    // From the Magical Armor the player is Wearing; 0 without it.
    public int ArmorBonus => Armor?.ArmorBonus ?? 0;

    // From the weapon the player is Wielding.
    public int MaximumDamage => Weapon?.MaxDamage ?? 0;

    public void CreateCharacter(Messages messages)
    {
        Race = PromptForRace(messages);
        Name = PromptForName(messages);
        Occupation = PromptForOccupation(messages);

        AssignWeaponByOccupation();

        int strengthModifier = RollStrength(messages);
        RollAgility(messages);
        RollHealthPoints(messages, strengthModifier);

        PromptForNextAction(messages);
    }

    private string PromptForRace(Messages messages)
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("race_prompt"));
            string? input = Console.ReadLine()?.Trim();

            if (messages.IsValidRace(input))
                return messages.NormalizeRace(input!);

            Console.WriteLine(messages.GetMessage("race_invalid"));
        }
    }

    private string PromptForName(Messages messages)
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("name_prompt"));
            string? input = Console.ReadLine()?.Trim();

            if (!string.IsNullOrWhiteSpace(input))
                return input;

            Console.WriteLine(messages.GetMessage("name_invalid"));
        }
    }

    private string PromptForOccupation(Messages messages)
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("occupation_prompt"));
            string? input = Console.ReadLine()?.Trim();

            if (messages.IsValidOccupation(input))
                return messages.NormalizeOccupation(input!);

            Console.WriteLine(messages.GetMessage("occupation_invalid"));
        }
    }

    private void AssignWeaponByOccupation()
    {
        switch (Occupation.ToLower())
        {
            case "fighter":
                Weapon = new Weapon("long sword", 12, "-)=====>");
                break;
            case "magician":
                Weapon = new Weapon("lightning bolt spell", 12, "zap~~~~~~");
                break;
            case "thief":
                Weapon = new Weapon("dagger", 6, "-)==>");
                break;
            case "archer":
                Weapon = new Weapon("long bow", 8, "}    -->");
                break;
        }
    }

    private int RollStrength(Messages messages)
    {
        PromptRoll(messages.GetMessage("roll_strength"), messages);
        Strength = _die.Roll(20);

        int modifier = _die.Roll(4);

        if (Race == "Halfling")
        {
            Strength -= modifier;
            return -modifier;
        }

        Strength += modifier;
        return modifier;
    }

    private void RollAgility(Messages messages)
    {
        PromptRoll(messages.GetMessage("roll_agility"), messages);
        Agility = _die.Roll(20);

        if (Race == "Halfling" || Race == "Elf")
            Agility += _die.Roll(4);
    }

    private void RollHealthPoints(Messages messages, int strengthModifier)
    {
        PromptRoll(messages.GetMessage("roll_health"), messages);
        HealthPoints = _die.Roll(20) + strengthModifier;
        MaximumHealthPoints = HealthPoints;
    }

    private void PromptForNextAction(Messages messages)
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("next_action"));
            string? input = Console.ReadLine()?.Trim();

            if (input == "1")
                DisplayStats(messages);
            else if (input == "2")
                return;
            else
                Console.WriteLine(messages.GetMessage("invalid"));
        }
    }

    public void DisplayStats(Messages messages)
    {
        Console.WriteLine(messages.GetMessage("stats_header"));
        Console.WriteLine(messages.GetMessage("stats_name") + Name);
        Console.WriteLine(messages.GetMessage("stats_race") + messages.TranslateRaceForDisplay(Race));
        Console.WriteLine(messages.GetMessage("stats_occupation") + messages.TranslateOccupationForDisplay(Occupation));
        Console.WriteLine(messages.GetMessage("stats_strength") + Strength);
        Console.WriteLine(messages.GetMessage("stats_agility") + Agility);
        Console.WriteLine(messages.GetMessage("stats_health") + HealthPoints);

        foreach (string line in GetInventoryLines(messages))
            Console.WriteLine(line);
    }

    /// <summary>
    /// The Inventory shared by the character stats and the combat stats screen: a header, then
    /// Wielding, Wearing and Carrying, always in that order and always present.
    /// </summary>
    public List<string> GetInventoryLines(Messages messages)
    {
        string nothing = InventoryIndent + messages.GetMessage("stats_inventory_nothing");
        var lines = new List<string> { messages.GetMessage("stats_inventory_header") };

        lines.Add(messages.GetMessage("stats_wielding"));
        if (Weapon != null)
        {
            lines.Add(InventoryIndent + messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(Weapon.Type));
            lines.Add(InventoryIndent + messages.GetMessage("stats_damage") + Weapon.MaxDamage);
        }
        else
        {
            lines.Add(nothing);
        }

        lines.Add(messages.GetMessage("stats_wearing"));
        if (Armor != null)
        {
            lines.Add(InventoryIndent + messages.GetMessage("stats_armor") + messages.TranslateArmorForDisplay(Armor.Type));
            lines.Add(InventoryIndent + messages.GetMessage("stats_armor_bonus") + Armor.ArmorBonus);
        }
        else
        {
            lines.Add(nothing);
        }

        lines.Add(messages.GetMessage("stats_carrying"));
        lines.Add(InventoryIndent + messages.GetMessage("stats_gold") + Gold);
        if (HealingPotions > 0)
            lines.Add(InventoryIndent + messages.GetMessage("stats_healing_potions") + HealingPotions);

        return lines;
    }

    public string Attack(Dragon dragon, Messages messages)
    {
        // Turned away before any Die is rolled, so Attack.Resolve can assume a weapon.
        if (Weapon == null)
            return "You have no weapon to attack with.";

        var output = new List<string>();

        output.Add(string.Format(
            messages.GetMessage("player_attack_intro"),
            messages.TranslateOccupationForDisplay(Occupation),
            messages.TranslateWeaponForDisplay(Weapon.Type)
        ));

        // If this fails to compile, change AsciiArt to whatever exists in Weapon.cs
        output.Add(Weapon.AsciiArt);

        AttackOutcome outcome = global::Attack.Resolve(this, dragon, _die);

        if (!outcome.Hit)
        {
            output.Add(string.Format(messages.GetMessage("player_missed_dragon"), dragon.Name));
            return string.Join(Environment.NewLine, output);
        }

        output.Add(string.Format(messages.GetMessage("player_hit_dragon"), dragon.Name));

        if (outcome.Block == Block.Full)
        {
            output.Add(string.Format(messages.GetMessage("dragon_defended_player_attack"), dragon.Name));
            output.Add(string.Format(
                dragon.GetRandomDefenseTaunt(messages),
                messages.TranslateRaceForDisplay(Race)
            ));
            return string.Join(Environment.NewLine, output);
        }

        // The dragon's Armor Bonus is 0, so it never makes a Partial Block.
        output.Add(string.Format(messages.GetMessage("player_damage_dealt"), outcome.Damage));
        output.Add(string.Format(messages.GetMessage("dragon_health_now"), dragon.Name, dragon.HealthPoints));

        if (outcome.DefenderDefeated)
            output.Add(messages.GetMessage("dragon_defeated_narrative"));
        else
            output.Add(dragon.GetRandomDamageReply(messages));

        return string.Join(Environment.NewLine, output);
    }

    private void PromptRoll(string prompt, Messages messages)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            string? input = Console.ReadLine()?.Trim().ToLower();

            if (input == "roll")
                return;

            Console.WriteLine(messages.GetMessage("roll_invalid"));
        }
    }
}
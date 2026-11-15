public class Combat
{
    public Player player { get; set; }
    public Dragon dragon { get; set; }
    public Messages messages { get; set; }

    public bool PlayerRetreated { get; private set; }

    public Combat(Player player, Dragon dragon, Messages messages)
    {
        this.player = player;
        this.dragon = dragon;
        this.messages = messages;
        PlayerRetreated = false;
    }

    public bool StartCombat()
    {
        Console.WriteLine(GetCombatStatsDisplay());

        // The first round always opens with the player's attack; after that the prompt decides.
        bool playerAttacks = true;

        while (true)
        {
            if (playerAttacks)
            {
                PlayerAttacksDragonSequence();

                if (dragon.HealthPoints <= 0)
                    return true;
            }

            DragonAttacksPlayerSequence();

            if (player.HealthPoints <= 0)
                return false;

            PlayerAction action = PromptForAction();

            if (action == PlayerAction.Retreat)
            {
                PlayerRetreated = true;
                Console.WriteLine(messages.GetMessage("retreat_combat"));
                return false;
            }

            playerAttacks = action == PlayerAction.Attack;
        }
    }

    private enum PlayerAction { Attack, Heal, Retreat }

    /// <summary>
    /// Asks what the player does next round. Drinking happens here, so a heal round has
    /// no player attack.
    /// </summary>
    private PlayerAction PromptForAction()
    {
        while (true)
        {
            Console.WriteLine(string.Format(messages.GetMessage("attack_prompt"), player.HealingPotions));
            string? input = Console.ReadLine()?.Trim().ToLower();

            if (input == "r" || input == "retreat")
                return PlayerAction.Retreat;

            if (input == "a" || input == "attack")
                return PlayerAction.Attack;

            if (input == "h" || input == "heal")
            {
                HealingPotionDrink drink = player.DrinkHealingPotion();

                switch (drink.Outcome)
                {
                    case HealingPotionDrinkOutcome.NoHealingPotions:
                        Console.WriteLine(messages.GetMessage("healing_potion_none"));
                        continue;
                    case HealingPotionDrinkOutcome.AtMaximumHealthPoints:
                        Console.WriteLine(messages.GetMessage("healing_potion_at_maximum"));
                        continue;
                }

                Console.WriteLine(string.Format(messages.GetMessage("healing_potion_drank"),
                    messages.GetMessage("healing_potion_name"), drink.Restored,
                    player.HealthPoints, player.MaximumHealthPoints));
                return PlayerAction.Heal;
            }

            // Unchanged from before the Healing Potion: invalid input still plays an attack round.
            Console.WriteLine(messages.GetMessage("invalid"));
            return PlayerAction.Attack;
        }
    }

    public string GetCombatStatsDisplay()
    {
        var output = new List<string>();

        output.Add(messages.GetMessage("combat_stats_header"));
        output.Add(messages.GetMessage("combat_player_header"));
        output.Add(messages.GetMessage("stats_name") + player.Name);
        output.Add(messages.GetMessage("stats_strength") + player.Strength);
        output.Add(messages.GetMessage("stats_agility") + player.Agility);
        output.Add(string.Format(messages.GetMessage("stats_health_current_maximum"),
            player.HealthPoints, player.MaximumHealthPoints));

        output.AddRange(player.GetEquipmentStatLines(messages));

        output.Add("");
        output.Add(messages.GetMessage("combat_dragon_header"));
        output.Add(messages.GetMessage("stats_name") + dragon.Name);
        output.Add(messages.GetMessage("stats_strength") + dragon.Strength);
        output.Add(messages.GetMessage("stats_agility") + dragon.Agility);
        output.Add(messages.GetMessage("stats_health") + dragon.HealthPoints);
        output.Add(messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(dragon.Weapon.Type));
        output.Add(messages.GetMessage("stats_damage") + dragon.Weapon.MaxDamage);

        return string.Join(Environment.NewLine, output);
    }

    public void PlayerAttacksDragonSequence()
    {
        Console.WriteLine(player.Attack(dragon, messages));
    }

    public void DragonAttacksPlayerSequence()
    {
        Console.WriteLine(dragon.Attack(player, messages));
    }
}
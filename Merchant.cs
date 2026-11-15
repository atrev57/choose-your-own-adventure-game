// The Merchant met once per character on the South path (see GLOSSARY.md).
// The either/or choice comes from prices and starting Gold (ADR 0002), not a special rule.
public class Merchant
{
    public const int MagicalItemPrice = 10;
    public const int HealingPotionPrice = 5;
    public const int HealingPotionStock = 2;

    // Each character meets the Merchant once, so a new Merchant always has a full stock.
    // The found Healing Potion is not part of it.
    private int _healingPotionsInStock = HealingPotionStock;

    public void Visit(Player player, Messages messages)
    {
        player.HasMetMerchant = true;
        Console.WriteLine(messages.GetMessage("merchant_intro"));

        while (true)
        {
            Console.WriteLine(string.Format(messages.GetMessage("merchant_gold"), player.Gold));
            string healingPotionOffer = _healingPotionsInStock > 0
                ? string.Format(messages.GetMessage("merchant_healing_potion_offer"), HealingPotionPrice)
                : messages.GetMessage("merchant_healing_potion_sold_out_offer");
            Console.WriteLine(string.Format(messages.GetMessage("merchant_menu"), MagicalItemPrice, healingPotionOffer));
            Console.Write(messages.GetMessage("enter_choice"));
            string? input = Console.ReadLine()?.Trim();

            if (input == "4")
            {
                Console.WriteLine(string.Format(messages.GetMessage("merchant_leave"), player.Gold));
                return;
            }

            if (input != "1" && input != "2" && input != "3")
            {
                Console.WriteLine(messages.GetMessage("invalid"));
                continue;
            }

            if (input == "3" && _healingPotionsInStock <= 0)
            {
                Console.WriteLine(messages.GetMessage("merchant_sold_out"));
                continue;
            }

            int price = input == "3" ? HealingPotionPrice : MagicalItemPrice;
            if (player.Gold < price)
            {
                Console.WriteLine(messages.GetMessage("merchant_cannot_afford"));
                continue;
            }

            if (input == "1")
                BuyMagicalArmor(player, messages);
            else if (input == "2")
                BuyMagicalWeapon(player, messages);
            else
                BuyHealingPotion(player, messages);
        }
    }

    private void BuyHealingPotion(Player player, Messages messages)
    {
        player.Gold -= HealingPotionPrice;
        player.HealingPotions++;
        _healingPotionsInStock--;

        Console.WriteLine(string.Format(messages.GetMessage("merchant_bought_healing_potion"),
            messages.GetMessage("healing_potion_name"), player.HealingPotions, player.Gold));
    }

    private void BuyMagicalArmor(Player player, Messages messages)
    {
        player.Gold -= MagicalItemPrice;
        player.Armor = Armor.Magical();

        Console.WriteLine(string.Format(messages.GetMessage("merchant_bought_armor"),
            messages.TranslateArmorForDisplay(player.Armor.Type), player.Armor.ArmorBonus, player.Gold));
    }

    private void BuyMagicalWeapon(Player player, Messages messages)
    {
        if (player.Weapon == null)
            return;

        player.Gold -= MagicalItemPrice;
        player.Weapon = player.Weapon.Enchant();

        Console.WriteLine(string.Format(messages.GetMessage("merchant_bought_weapon"),
            messages.TranslateWeaponForDisplay(player.Weapon.Type), player.Weapon.MaxDamage, player.Gold));
    }
}

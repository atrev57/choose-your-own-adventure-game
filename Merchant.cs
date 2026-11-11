// The Merchant met once per character on the South path (see GLOSSARY.md).
// The either/or choice comes from prices and starting Gold (ADR 0002), not a special rule.
public class Merchant
{
    public const int ItemPrice = 10;

    public void Visit(Player player, Messages messages)
    {
        player.HasMetMerchant = true;
        Console.WriteLine(messages.GetMessage("merchant_intro"));

        while (true)
        {
            Console.WriteLine(string.Format(messages.GetMessage("merchant_gold"), player.Gold));
            Console.WriteLine(string.Format(messages.GetMessage("merchant_menu"), ItemPrice));
            Console.Write(messages.GetMessage("enter_choice"));
            string? input = Console.ReadLine()?.Trim();

            if (input == "3")
            {
                Console.WriteLine(string.Format(messages.GetMessage("merchant_leave"), player.Gold));
                return;
            }

            if (input != "1" && input != "2")
            {
                Console.WriteLine(messages.GetMessage("invalid"));
                continue;
            }

            if (player.Gold < ItemPrice)
            {
                Console.WriteLine(messages.GetMessage("merchant_cannot_afford"));
                continue;
            }

            if (input == "1")
                BuyMagicalArmor(player, messages);
            else
                BuyMagicalWeapon(player, messages);
        }
    }

    private void BuyMagicalArmor(Player player, Messages messages)
    {
        player.Gold -= ItemPrice;
        player.Armor = Armor.Magical();

        Console.WriteLine(string.Format(messages.GetMessage("merchant_bought_armor"),
            messages.TranslateArmorForDisplay(player.Armor.Type), player.Armor.ArmorBonus, player.Gold));
    }

    private void BuyMagicalWeapon(Player player, Messages messages)
    {
        if (player.Weapon == null)
            return;

        player.Gold -= ItemPrice;
        player.Weapon = player.Weapon.Enchant();

        Console.WriteLine(string.Format(messages.GetMessage("merchant_bought_weapon"),
            messages.TranslateWeaponForDisplay(player.Weapon.Type), player.Weapon.MaxDamage, player.Gold));
    }
}

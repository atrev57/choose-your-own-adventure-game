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

            if (input == "1")
            {
                if (player.Gold < ItemPrice)
                    Console.WriteLine(messages.GetMessage("merchant_cannot_afford"));
                else
                    BuyMagicalWeapon(player, messages);
            }
            else if (input == "2")
            {
                Console.WriteLine(string.Format(messages.GetMessage("merchant_leave"), player.Gold));
                return;
            }
            else
            {
                Console.WriteLine(messages.GetMessage("invalid"));
            }
        }
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

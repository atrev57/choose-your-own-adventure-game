# 04: The Merchant sells Healing Potions

**What to build:** The Merchant sells **Healing Potions** for 5 Gold each, with a stock of 2 per character. After 2 purchases they are **Sold Out**. The menu becomes 1 Magical Armor (10 Gold), 2 Magical Weapon (10 Gold), 3 Healing Potion (5 Gold, or shown as Sold Out), 4 Leave. The "cannot afford" check uses each item's own price. The found Healing Potion does not count against the stock. A new character always meets a Merchant with a full stock.

**Blocked by:** 03 (drinking a Healing Potion in combat)

**Status:** ready-for-agent

Spec: `../spec.md`. Pricing: ADR 0002 and its 2026-11-15 amendment; re-check it before changing any price or the stock.

- [x] The Merchant menu shows the four options in order, in all four languages; Leave is 4. Existing Merchant tests are updated for the new numbering.
- [x] Buying a Healing Potion deducts 5 Gold and prints the new Healing Potion count and remaining Gold (15 → 10 Gold, count 1 → 2 after the found one).
- [x] After buying 2, the menu shows option 3 as Sold Out; choosing it prints the Sold Out message, re-prompts, and leaves Gold and count unchanged.
- [x] A Magical Item plus a Healing Potion leaves 0 Gold and both items.
- [x] Two Healing Potions, then a Magical Item attempt, prints "cannot afford" with 5 Gold kept.
- [x] Buying two Magical Items is still impossible.
- [x] Bought Healing Potions can be drunk in combat like the found one (the count shown in combat includes them).
- [x] New messages (menu, purchase result, Sold Out) exist in all four languages without accents; key-parity passes.
- [x] A full Italian playthrough through the game's start method: South → buy 2 Healing Potions → Sold Out on the third try → leave → exit.

## Comments

- The stock is a private field on `Merchant`. `Game` creates a new `Merchant` for each character's single visit, so every new character gets a full stock of 2. The found Healing Potion never touches it.
- Choosing a Sold Out Healing Potion shows the Sold Out message even with 0 Gold; Sold Out is checked before price.
- The menu's option 3 comes from two new keys, `merchant_healing_potion_offer` and `merchant_healing_potion_sold_out_offer`, which fill `{1}` in `merchant_menu`.
- `merchant_bought_healing_potion` always uses the plural ("You now have N Healing Potions"). That's safe only because the found Healing Potion always comes before the Merchant, so the count after a purchase is at least 2.
- `Merchant.ItemPrice` was renamed to `MagicalItemPrice`.

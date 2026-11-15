# 02: View the Inventory at the path fork

**What to build:** At the path fork, the player can type `i` or `inventory` (case-insensitive, trimmed) to see their Inventory, then gets asked for a path again. Viewing changes nothing, doesn't count as a South visit, and can be repeated as often as the player likes, so a player coming back from the Merchant can check what they bought and how much Gold they have left before going North. The path prompt mentions the new choice in every language. Input stays language-neutral, like `n`, `s` and `e`.

See `.scratch/inventory/spec.md` (Implementation Decisions "Path fork", Testing Decisions seam 1).

**Blocked by:** 01 (Show the Inventory grouped as Wielding, Wearing and Carrying)

**Status:** ready-for-agent

- [ ] `i` at the fork before going South shows Wielding (the starting weapon), Wearing nothing, and Carrying 15 Gold with no Healing Potion line, then re-prompts for a path
- [ ] After the first South trip with purchases, `i` shows the updated Inventory: the Magical Weapon's damage or Magical Armor with its Armor Bonus, the remaining Gold, and the Healing Potion count
- [ ] `inventory` (full word, mixed case) works the same as `i`
- [ ] Viewing the Inventory doesn't trigger the South visit, the found Healing Potion or the empty stall, and can be done repeatedly
- [ ] The path-choice validator accepts `i` and `inventory`
- [ ] Invalid input at the fork still shows the existing invalid-path message
- [ ] The path prompt offers `i` for inventory in English, Spanish, French and Italian, without accents (ADR 0001); the key-parity test passes
- [ ] A full Italian playthrough that views the Inventory passes
- [ ] Players who never use the inventory choice see the fork, the Merchant and the dragon fight behave as before; the full test suite passes

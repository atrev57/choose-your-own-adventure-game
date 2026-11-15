# 01: Show the Inventory grouped as Wielding, Wearing and Carrying

**What to build:** Wherever the player's possessions are shown, they appear as one **Inventory** block: an Inventory header, then **Wielding**, **Wearing** and **Carrying** sections, always in that order and always present. Wielding shows the weapon and its maximum damage (a Magician's lightning bolt spell and a Magical Weapon count). Wearing shows Magical Armor and its Armor Bonus, or a "nothing" line. Carrying shows Gold, then Healing Potions only when the count is above 0.

The character stats and the combat stats screen build this block from one shared place, which replaces today's shared weapon, armor and Healing Potion stat lines. Gold is no longer printed separately in the character stats; it appears only under Carrying, so the combat stats now show Gold too. Stats that aren't possessions (Name, Race, Occupation, Strength, Agility, Health Points, and current / maximum Health Points in combat) stay above the Inventory, unchanged. The dragon's section is unchanged.

See `.scratch/inventory/spec.md` (Implementation Decisions, Testing Decisions seam 2 and 3) and the Inventory section of `GLOSSARY.md`.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

- [x] Character stats show, in order: the Inventory header, Wielding, Wearing and Carrying, with all three headings present even when a section is empty
- [x] Wielding shows the weapon (translated) and its maximum damage using the existing labels, including for a Magician and after the Merchant enchants the weapon
- [x] Wearing shows Magical Armor and its Armor Bonus when worn, and a "nothing" line otherwise
- [x] A player with no weapon (only possible in tests) sees the same "nothing" line under Wielding
- [x] Carrying shows Gold, then the Healing Potions line only when the count is above 0
- [x] Gold appears exactly once in the character stats
- [x] The combat stats show the same Inventory block, including Gold, with current / maximum Health Points still above it
- [x] The Inventory header, the three section headings and the "nothing" line exist in English, Spanish, French and Italian, without accents (ADR 0001); the key-parity test passes
- [x] Existing combat stats, Merchant and Healing Potion tests that assert on these lines are updated, and the full test suite passes
- [x] The Inventory glossary terms in `GLOSSARY.md` are committed with this ticket

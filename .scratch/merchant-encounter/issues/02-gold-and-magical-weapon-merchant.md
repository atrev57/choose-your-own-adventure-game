# 02: Gold and a Merchant who sells the Magical Weapon

**What to build:** A playable Merchant on the South path. Every character starts with 15 Gold. On their first South trip they meet the Merchant, who shows their Gold and offers a Magical Weapon for 10 Gold or the chance to leave. Buying enchants the player's own weapon (same kind, same art, +4 maximum damage, magical name). Afterwards the player returns to the path fork; later South trips find the stall empty. Fully localized in all four languages.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

Spec: `../spec.md`. Pricing: ADR 0002. Localization conventions: ADR 0001.

- [x] A new character has 15 Gold, shown in the character stats at creation.
- [x] The South path narrative no longer says the player returns home (all 4 languages).
- [x] First South trip per character shows the Merchant with the player's Gold and numbered options: Magical Weapon (10 Gold), Leave.
- [x] Buying the Magical Weapon replaces the weapon with its magical counterpart (same art, max damage +4, own type string), deducts 10 Gold, and prints what was bought, the new max damage and the remaining Gold.
- [x] Leaving keeps all 15 Gold and changes nothing.
- [x] Invalid Merchant input shows an error and re-prompts.
- [x] After the Merchant, the player is back at the path fork and can go North.
- [x] A second South trip by the same character shows an empty-stall message and returns to the fork. Merchant state lives on the player, so a new character meets a fresh Merchant.
- [x] All four magical weapon names have hand-translated, gender-correct display entries in every language; a test checks every magical weapon type translates in every language.
- [x] The magical weapon's name and damage appear on the combat stats screen (tested by building a combat directly and checking its stats display).
- [x] Playthrough tests through the game's start method (scripted console input, ending with exit) cover: buy weapon (Thief: damage 6 → 10, 5 Gold left), leave, invalid input then valid, second South trip.
- [x] Key-parity test passes.

## Comments

- Implemented. Per ADR 0002, the Merchant menu stays open after a purchase, so a second purchase is refused with "not enough Gold" (no "one purchase" rule). The player picks Leave to return to the fork. Invalid input reuses the existing `invalid` message.

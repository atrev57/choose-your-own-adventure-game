# 03: Drinking a Healing Potion in combat

**What to build:**
- In the dragon fight, the prompt after each exchange offers `a`/`attack`, `h`/`heal` (showing how many Healing Potions are left) and `r`/`retreat`. The first round still opens with an automatic attack.
- Drinking restores half of Maximum Health Points, rounded up, with a minimum of 1. It never goes above Maximum Health Points, and the message reports the amount actually restored.
- Drinking takes the place of the player's attack that round, and the dragon still attacks.
- Healing with no Healing Potions, or at full health, is refused with a message and re-prompts. No round is played and nothing is used up.
- Attack-only fights play out exactly as before.

**Blocked by:** 01 (player dice that tests can control), 02 (Maximum Health Points and the found Healing Potion)

**Status:** ready-for-agent

Spec: `../spec.md`. Localization: ADR 0001.

- [x] Combat (scripted Die + console input): heal amount for Maximum Health Points 17 → 9, 16 → 8, and 1 → 1.
- [x] At 15/17, drinking reports 2 restored and health becomes 17.
- [x] At full health, `h` is refused, the count is unchanged, and the dragon does not attack.
- [x] With no Healing Potions, `h` is refused, and the dragon does not attack.
- [x] In a heal round the player does not attack but the dragon does, and the count goes down by one.
- [x] The dragon's attack after drinking can still defeat the player.
- [x] The combat prompt shows the remaining count.
- [x] An attack-only fight produces the same sequence as before; existing combat and game tests still pass.
- [x] The drink message, both refusal messages and the updated combat prompt exist in all four languages without accents; key-parity passes.

## Comments

- The 1 -> 1 heal amount is tested on `Player.DrinkHealingPotion()` directly, not through combat: a living player with Maximum Health Points 1 is always at their maximum, so combat refuses the heal.
- The refusal for a player already at Maximum Health Points uses the key `healing_potion_at_maximum` and avoids the phrase "full health", which GLOSSARY.md lists under Avoid.
- A player who never went South still sees the heal option in the combat prompt with "(0 left)", and `h` is refused. This follows this ticket's "offers ... h/heal after each exchange". Spec user story 32 ("exactly as before, with no heal available") could be read as hiding the option instead, so it needs a decision.
- Invalid input at the combat prompt still plays an attack round, as before.

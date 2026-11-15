# 01: One Attack resolution module for both Combatants

**What to build:** A refactor with no gameplay change. Move the hit roll, Defense Roll, Full and Partial Block, and damage rules out of the player's and the dragon's Attack methods into `Attack.Resolve(attacker, defender, die)`. It returns an `AttackOutcome` and works for both directions through a shared Combatant interface. The two Attack methods keep their names and output, and only describe the outcome in Messages.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

Spec: `../spec.md`. Decision: `docs/adr/0003-one-attack-resolution-for-both-combatants.md`.

- [x] Player and Dragon implement `ICombatant` (Strength, Agility, Armor Bonus, maximum damage, Health Points).
- [x] `Attack.Resolve` decides hit, block and damage, lowers the defender's Health Points, and returns `Hit`, `Block`, `Damage` and `DefenderDefeated`.
- [x] The order of Die rolls is unchanged (hit, Defense Roll, damage, then narration roll), and the attacker's Die still rolls the defender's Defense Roll.
- [x] The dragon's Health Points are lowered exactly as today, still without stopping at 0 (that change is issue 02).
- [x] An unarmed player is still turned away before any Die is rolled.
- [x] `RollAttack`, `AttackHits` and `RollDamage` are removed from Player and Dragon.
- [x] New `CYOATests/AttackTests.cs` covers a miss, a hit with no block, a Full Block, a Partial Block (halved, rounded down), no Armor Bonus, the player's Health Points stopping at 0, and `DefenderDefeated`.
- [x] All existing tests pass without edits.
- [x] The `GLOSSARY.md` updates from the design session (Attack, Combatant, Defense Roll and Full Block for either defender) and ADR 0003 are committed with this issue.

## Comments

- To keep the dragon's Health Points going below 0 in this step without a "who is attacking?" branch (ADR 0003), `ICombatant` has one extra, temporary member: `HealthPointsStopAtZero` (true for Player, false for Dragon, implemented explicitly so it isn't public on either). Issue 02 should clamp unconditionally in `Attack.Resolve` and delete it; `AttackTests` pins the unclamped case, and that test is the one to flip.
- Review noted, and deliberately left alone: `Player.Attack` has no Partial Block narration, because the dragon's Armor Bonus is 0 and there's no message key for a dragon Partial Block. Giving the dragon armor would need one.

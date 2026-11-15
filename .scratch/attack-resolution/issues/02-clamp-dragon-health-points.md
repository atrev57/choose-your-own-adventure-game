# 02: The dragon's Health Points stop at 0

**What to build:** The one visible change. When the player's Attack does more damage than the dragon has Health Points left, the dragon's Health Points become 0, not a negative number, matching how the player's already work. The rule lives in `Attack.Resolve`, so it is the same for both Combatants.

**Blocked by:** 01

**Status:** ready-for-agent

Spec: `../spec.md`.

- [x] A test in `CYOATests/AttackTests.cs`: the dragon at 3 Health Points takes 7 damage and ends at 0 with `DefenderDefeated` true. It fails before the change.
- [x] The combat output after a finishing blow shows the dragon at 0 Health Points, in every language.
- [x] Victory is still detected exactly as before.
- [x] All other tests pass. Any existing test that asserted a negative dragon Health Points value is updated, with a note in the commit message.

## Comments

- `Attack.Resolve` now clamps at 0 for every defender, and the temporary `ICombatant.HealthPointsStopAtZero` is deleted. The step-one test that pinned -4 was replaced by one using a real `Dragon` (fails before: actual -4). The narration reads `dragon.HealthPoints` through `dragon_health_now`, which has the `{1}` placeholder in en/es/fr/it, so every language shows 0. `Combat.cs` still detects victory with `HealthPoints <= 0`. `AttackOutcome.Damage` still reports the rolled damage (e.g. 7), as the player-side clamp always did.

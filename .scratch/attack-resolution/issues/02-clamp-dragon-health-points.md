# 02: The dragon's Health Points stop at 0

**What to build:** The one visible change. When the player's Attack does more damage than the dragon has Health Points left, the dragon's Health Points become 0, not a negative number, matching how the player's already work. The rule lives in `Attack.Resolve`, so it is the same for both Combatants.

**Blocked by:** 01

**Status:** ready-for-agent

Spec: `../spec.md`.

- [ ] A test in `CYOATests/AttackTests.cs`: the dragon at 3 Health Points takes 7 damage and ends at 0 with `DefenderDefeated` true. It fails before the change.
- [ ] The combat output after a finishing blow shows the dragon at 0 Health Points, in every language.
- [ ] Victory is still detected exactly as before.
- [ ] All other tests pass. Any existing test that asserted a negative dragon Health Points value is updated, with a note in the commit message.

## Comments

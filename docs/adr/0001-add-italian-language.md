# ADR 0001: Add Italian as a Supported Language

- **Status:** Accepted
- **Date:** 2026-11-09
- **Author:** Alyssa Trevino
- **Related files:** `language_data.json`, `Game.cs`, `CYOATests/ItalianLanguageTests.cs`

## Context

The Choose Your Own Adventure game supports English, Spanish, and French. All player-facing
text lives in `language_data.json`, grouped by language. Each language provides six sections:

| Section | Purpose |
|---|---|
| `dictionary` | Every message the game prints, looked up by key (e.g. `"welcome"`) |
| `raceMap` / `occupationMap` | Turns what the player types (e.g. `"elfo"`) into the internal value (`"Elf"`) |
| `displayRaceMap` / `displayOccupationMap` / `displayWeaponMap` | Turns internal values back into the player's language for display |

`Messages.cs` loads one language section at startup, based on the player's choice in the
language menu in `Game.StartGame()`.

We wanted to add a fourth language to reach more players. Adding it also shows that the
localization design can grow without changes to the game logic.

<!-- TODO: add the real reason for choosing Italian, e.g. assignment requirement, team member speaks it, target audience. -->

## Decision

Add **Italian** as the fourth language, using the existing localization structure:

1. **`language_data.json`:** add an `"Italian"` section containing all six maps and every
   dictionary key that English has.
2. **`Game.cs`:** add `4. Italiano` to the language menu and map input `"4"` to `"Italian"`.
   Any other input still defaults to English.
3. **No changes** to `Messages.cs`, `Player.cs`, `Dragon.cs`, or `Combat.cs`.

### Conventions followed

- **No accented characters**, to match the existing Spanish and French text (e.g. "Adios",
  "montanas"). Accented vowels are written with a trailing apostrophe, a common Italian
  fallback: `è` → `e'`, `più` → `piu'`, `agilità` → `agilita'`.
- **Command letters stay the same in every language.** Players type `n`/`s`/`e`, `a`/`r`, and
  `roll`, because `Game.cs`, `Combat.cs`, and `Player.cs` check for those exact strings.
- **Race and job names** follow standard Italian fantasy terms:
  Umano, Elfo, Nano, Mezzuomo (Halfling); Guerriero, Mago, Ladro, Arciere.
- **Translations follow the English text**, which is the most complete version. (For example,
  the French victory and defeat messages are shorter than the English ones.)

## Alternatives Considered

| Option | Why not chosen |
|---|---|
| **Use proper Italian accents (UTF-8)** | Would be more correct Italian, but inconsistent with the other languages. Some Windows consoles show accented characters wrongly unless `Console.OutputEncoding` is set to UTF-8. Worth revisiting for all languages together. |
| **Translate the command letters** (e.g. `a` for *attacca*, `r` for *ritirati*, `t` for *tira*) | Would require language-specific input handling in `Game.cs`, `Combat.cs`, and `Player.cs`. Spanish and French already use English letters, so this would be a separate, cross-language change. |
| **Allow English race/job names in every language** | Would change validation behavior for all languages, not just Italian. Out of scope. |
| **Generate the language menu from the JSON file** | Would let new languages be added with no code changes, but needs a display name for each language and a new method on `Messages`. A good follow-up, not required for one language. |

## Consequences

### Positive

- Players can choose Italian from the language menu.
- No game logic changed, so there is no risk to combat, character creation, or path behavior.
- The existing test `LanguageFile_EveryLanguageHasSameKeysAsEnglish` covers Italian
  automatically, so a future English key that isn't translated into Italian fails the build.
- New tests in `CYOATests/ItalianLanguageTests.cs` (29 tests) check loading, the
  `{0}`/`{1}`/`{2}` placeholders, player input, display names, and a full Italian playthrough
  via `Game.StartGame()`.

### Negative / Trade-offs

- **Grammatical gender.** Italian articles and adjectives change with gender. Weapon names
  differ in gender (*la spada* vs. *il pugnale*), so the attack message was reworded to avoid a
  gendered article: `"Come {0}, sfoderi l'arma ({1}) e attacchi il drago."` Messages about the
  player (e.g. *"Sei stato sconfitto"*) use the masculine form, since the game does not track
  player gender. Spanish and French have the same limitation.
- **No accents** makes the text slightly less natural for native readers.
- **Language list in two places.** Adding a language still means editing both
  `language_data.json` and the menu in `Game.cs`.
- **Some English text remains:** the dragon's weapon art `<<< claws >>>` is hardcoded in
  `Dragon.cs`, and the language menu prompt is multilingual by design.
- **Translation review.** The Italian text has not been reviewed by a native speaker.

## Follow-ups

- [ ] Have a native or fluent Italian speaker review the translations.
- [ ] Consider switching all languages to UTF-8 with proper accents (set `Console.OutputEncoding`).
- [ ] Consider generating the language menu from `language_data.json`.
- [ ] Move the dragon's weapon art into the language file, or make it language-neutral.

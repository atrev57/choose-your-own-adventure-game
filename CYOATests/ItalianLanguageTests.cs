// CYOATests/ItalianLanguageTests.cs
using System.Text.Json;
using System.Text.RegularExpressions;

[TestClass]
public class ItalianLanguageTests
{
    private static Messages GetItalian()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("Italian");
        messages.ReadDictionary();
        return messages;
    }

    private static JsonElement LoadLanguageSection(JsonDocument doc, string language) =>
        doc.RootElement.GetProperty(language).GetProperty("dictionary");

    private static JsonDocument LoadLanguageFile() =>
        JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "language_data.json")));

    // --- Loading ---

    [TestMethod]
    public void ReadDictionary_Italian_LoadsWithoutFallingBackToEnglish()
    {
        Messages messages = GetItalian();
        Assert.AreEqual("Italian", messages.CurrentLanguage);
    }

    [TestMethod]
    public void ReadDictionary_Italian_LoadsWelcomeAndGoodbye()
    {
        Messages messages = GetItalian();
        Assert.AreEqual("Benvenuto nel Gioco Scegli la Tua Avventura", messages.GetMessage("welcome"));
        Assert.AreEqual("L'avventura finisce. Ciao.", messages.GetMessage("goodbye"));
    }

    [TestMethod]
    public void ReadDictionary_Italian_HasEveryEnglishKey()
    {
        using JsonDocument doc = LoadLanguageFile();
        Messages messages = GetItalian();

        foreach (JsonProperty key in LoadLanguageSection(doc, "English").EnumerateObject())
        {
            Assert.AreNotEqual($"[{key.Name}]", messages.GetMessage(key.Name),
                $"Key '{key.Name}' is missing from the Italian dictionary.");
        }
    }

    [TestMethod]
    public void ReadDictionary_Italian_PlaceholdersMatchEnglish()
    {
        // If English says "{0} attacks {1} with {2}!", Italian must also use {0}, {1} and {2},
        // otherwise string.Format would crash or drop a name during combat.
        using JsonDocument doc = LoadLanguageFile();
        JsonElement english = LoadLanguageSection(doc, "English");
        JsonElement italian = LoadLanguageSection(doc, "Italian");
        var placeholder = new Regex(@"\{\d+\}");

        foreach (JsonProperty entry in english.EnumerateObject())
        {
            var expected = placeholder.Matches(entry.Value.GetString()!).Select(m => m.Value).ToHashSet();
            var actual = placeholder.Matches(italian.GetProperty(entry.Name).GetString()!).Select(m => m.Value).ToHashSet();

            Assert.IsTrue(expected.SetEquals(actual),
                $"'{entry.Name}': English uses [{string.Join(",", expected)}] but Italian uses [{string.Join(",", actual)}].");
        }
    }

    // --- Player input (what the player types) ---

    [TestMethod]
    [DataRow("umano", "Human")]
    [DataRow("ELFO", "Elf")]
    [DataRow("Nano", "Dwarf")]
    [DataRow("  mezzuomo  ", "Halfling")]
    public void Italian_RaceInput_IsValidAndNormalized(string input, string expected)
    {
        Messages messages = GetItalian();
        Assert.IsTrue(messages.IsValidRace(input));
        Assert.AreEqual(expected, messages.NormalizeRace(input));
    }

    [TestMethod]
    [DataRow("guerriero", "Fighter")]
    [DataRow("MAGO", "Magician")]
    [DataRow("Ladro", "Thief")]
    [DataRow("arciere", "Archer")]
    public void Italian_OccupationInput_IsValidAndNormalized(string input, string expected)
    {
        Messages messages = GetItalian();
        Assert.IsTrue(messages.IsValidOccupation(input));
        Assert.AreEqual(expected, messages.NormalizeOccupation(input));
    }

    [TestMethod]
    public void Italian_RejectsEnglishRaceAndOccupationNames()
    {
        Messages messages = GetItalian();
        Assert.IsFalse(messages.IsValidRace("halfling"));
        Assert.IsFalse(messages.IsValidOccupation("fighter"));
    }

    // --- Display text ---

    [TestMethod]
    [DataRow("Human", "Umano")]
    [DataRow("Elf", "Elfo")]
    [DataRow("Dwarf", "Nano")]
    [DataRow("Halfling", "Mezzuomo")]
    public void Italian_TranslatesRaceForDisplay(string race, string expected)
    {
        Assert.AreEqual(expected, GetItalian().TranslateRaceForDisplay(race));
    }

    [TestMethod]
    [DataRow("Fighter", "Guerriero")]
    [DataRow("Magician", "Mago")]
    [DataRow("Thief", "Ladro")]
    [DataRow("Archer", "Arciere")]
    public void Italian_TranslatesOccupationForDisplay(string occupation, string expected)
    {
        Assert.AreEqual(expected, GetItalian().TranslateOccupationForDisplay(occupation));
    }

    [TestMethod]
    [DataRow("long sword", "spada lunga")]
    [DataRow("lightning bolt spell", "incantesimo del fulmine")]
    [DataRow("dagger", "pugnale")]
    [DataRow("long bow", "arco lungo")]
    [DataRow("claws", "artigli")]
    public void Italian_TranslatesWeaponForDisplay(string weapon, string expected)
    {
        Assert.AreEqual(expected, GetItalian().TranslateWeaponForDisplay(weapon));
    }

    // --- Game.cs: choosing option 4 in the real game ---
    // These redirect Console, so they must not run in parallel with other tests.

    private static string PlayGame(string input)
    {
        TextReader originalIn = Console.In;
        TextWriter originalOut = Console.Out;
        try
        {
            using var writer = new StringWriter();
            Console.SetIn(new StringReader(input));
            Console.SetOut(writer);
            new Game().StartGame();
            return writer.ToString();
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    [TestMethod]
    [DoNotParallelize]
    [Timeout(5000)]  // fail fast if the game gets stuck waiting for input
    public void StartGame_LanguageMenu_ListsItaliano()
    {
        string output = PlayGame("4\n2\n");
        StringAssert.Contains(output, "4. Italiano");
    }

    [TestMethod]
    [DoNotParallelize]
    [Timeout(5000)]  // fail fast if the game gets stuck waiting for input
    public void StartGame_Option4_RunsGameInItalian()
    {
        // Choose Italian, then exit from the main menu.
        string output = PlayGame("4\n2\n");

        StringAssert.Contains(output, "Benvenuto nel Gioco Scegli la Tua Avventura");
        StringAssert.Contains(output, "Scegli; 1. Crea Personaggio o 2. Esci.");
        StringAssert.Contains(output, "L'avventura finisce. Ciao.");
    }

    [TestMethod]
    [DoNotParallelize]
    [Timeout(5000)]  // fail fast if the game gets stuck waiting for input
    public void StartGame_Option4_CreatesCharacterInItalian()
    {
        // Italian -> create character -> Mezzuomo "Giulia" the Ladro -> roll x3
        // -> view stats -> start adventure -> exit at the path prompt.
        string output = PlayGame("4\n1\nmezzuomo\nGiulia\nladro\nroll\nroll\nroll\n1\n2\ne\n");

        StringAssert.Contains(output, "----- Riepilogo del Personaggio -----");
        StringAssert.Contains(output, "Nome: Giulia");
        StringAssert.Contains(output, "Razza: Mezzuomo");
        StringAssert.Contains(output, "Professione: Ladro");
        StringAssert.Contains(output, "Arma: pugnale");
        StringAssert.Contains(output, "Stai camminando tra le montagne.");
        Assert.IsFalse(output.Contains("[") && Regex.IsMatch(output, @"\[[a-z_0-9]+\]"),
            "Output contains a missing-key marker like [some_key].");
    }

    [TestMethod]
    [DoNotParallelize]
    [Timeout(5000)]  // fail fast if the game gets stuck waiting for input
    public void StartGame_Italian_SouthBuyArmorThenBackToFork()
    {
        // Italian -> Mezzuomo Ladro "Giulia" -> start adventure -> South -> buy armor (1) -> leave (3) -> exit.
        string output = PlayGame("4\n1\nmezzuomo\nGiulia\nladro\nroll\nroll\nroll\n2\ns\n1\n3\ne\n");

        StringAssert.Contains(output, "Incontri un Mercante");
        StringAssert.Contains(output, "Hai 15 monete d'oro.");
        StringAssert.Contains(output, "1. Armatura Magica (10 monete d'oro)");
        StringAssert.Contains(output, "Hai comprato: armatura magica! Il suo Bonus Armatura e' 5. Ti restano 5 monete d'oro.");
        StringAssert.Contains(output, "Lasci il Mercante con 5 monete d'oro");
        Assert.AreEqual(2, Regex.Matches(output, "Stai camminando tra le montagne.").Count, "Player should be back at the path fork.");
        StringAssert.Contains(output, "L'avventura finisce. Ciao.");
        Assert.IsFalse(Regex.IsMatch(output, @"\[[a-z_0-9]+\]"), "Output contains a missing-key marker like [some_key].");
    }
}

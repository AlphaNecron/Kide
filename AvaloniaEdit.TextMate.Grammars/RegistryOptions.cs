using System;
using System.Collections.Generic;
using System.IO;
using AvaloniaEdit.TextMate.Resources;
using Newtonsoft.Json;
using TextMateSharp.Internal.Grammars.Reader;
using TextMateSharp.Internal.Themes.Reader;
using TextMateSharp.Internal.Types;
using TextMateSharp.Registry;
using TextMateSharp.Themes;

namespace AvaloniaEdit.TextMate.Grammars
{
    public class RegistryOptions : IRegistryOptions
    {
        private readonly Dictionary<string, GrammarDefinition> _availableGrammars =
            new Dictionary<string, GrammarDefinition>();

        private readonly ThemeName _defaultTheme;

        public RegistryOptions(ThemeName defaultTheme)
        {
            _defaultTheme = defaultTheme;
            InitializeAvailableGrammars();
        }

        public ICollection<string> GetInjections(string scopeName)
        {
            return null;
        }

        public IRawTheme GetTheme(string scopeName)
        {
            var themeStream = ResourceLoader.TryOpenThemeStream(scopeName.Replace("./", string.Empty));

            if (themeStream == null)
                return null;

            using (themeStream)
            using (var reader = new StreamReader(themeStream))
            {
                return ThemeReader.ReadThemeSync(reader);
            }
        }

        public IRawGrammar GetGrammar(string scopeName)
        {
            var grammarStream = ResourceLoader.TryOpenGrammarStream(GetGrammarFile(scopeName));

            if (grammarStream == null)
                return null;

            using (grammarStream)
            using (var reader = new StreamReader(grammarStream))
            {
                return GrammarReader.ReadGrammarSync(reader);
            }
        }

        public IRawTheme GetDefaultTheme()
        {
            return LoadTheme(_defaultTheme);
        }

        public List<Language> GetAvailableLanguages()
        {
            var result = new List<Language>();

            foreach (var definition in _availableGrammars.Values)
            foreach (var language in definition.Contributes.Languages)
            {
                if (language.Aliases == null || language.Aliases.Count == 0)
                    continue;

                result.Add(language);
            }

            return result;
        }

        public Language GetLanguageByExtension(string extension)
        {
            foreach (var definition in _availableGrammars.Values)
            foreach (var language in definition.Contributes.Languages)
            {
                if (language.Extensions == null)
                    continue;

                foreach (var languageExtension in language.Extensions)
                    if (extension.Equals(languageExtension,
                            StringComparison.OrdinalIgnoreCase))
                        return language;
            }

            return null;
        }

        public string GetScopeByExtension(string extension)
        {
            foreach (var definition in _availableGrammars.Values)
            foreach (var language in definition.Contributes.Languages)
            foreach (var languageExtension in language.Extensions)
                if (extension.Equals(languageExtension,
                        StringComparison.OrdinalIgnoreCase))
                    foreach (var grammar in definition.Contributes.Grammars)
                        return grammar.ScopeName;

            return null;
        }

        public string GetScopeByLanguageId(string languageId)
        {
            if (string.IsNullOrEmpty(languageId))
                return null;

            foreach (var definition in _availableGrammars.Values)
            foreach (var grammar in definition.Contributes.Grammars)
                if (languageId.Equals(grammar.Language))
                    return grammar.ScopeName;

            return null;
        }

        public IRawTheme LoadTheme(ThemeName name)
        {
            return GetTheme(GetThemeFile(name));
        }

        private void InitializeAvailableGrammars()
        {
            var serializer = new JsonSerializer();

            foreach (var grammar in GrammarNames.SupportedGrammars)
                using (var stream = ResourceLoader.OpenGrammarPackage(grammar))
                using (var reader = new StreamReader(stream))
                using (var jsonTextReader = new JsonTextReader(reader))
                {
                    var definition = serializer.Deserialize<GrammarDefinition>(jsonTextReader);
                    _availableGrammars.Add(grammar, definition);
                }
        }

        private string GetGrammarFile(string scopeName)
        {
            foreach (var grammarName in _availableGrammars.Keys)
            {
                var definition = _availableGrammars[grammarName];

                foreach (var grammar in definition.Contributes.Grammars)
                    if (scopeName.Equals(grammar.ScopeName))
                    {
                        var grammarPath = grammar.Path;

                        if (grammarPath.StartsWith("./"))
                            grammarPath = grammarPath.Substring(2);

                        grammarPath = grammarPath.Replace("/", ".");

                        return grammarName.ToLower() + "." + grammarPath;
                    }
            }

            return null;
        }

        private string GetThemeFile(ThemeName name)
        {
            switch (name)
            {
                case ThemeName.Abbys:
                    return "abyss-color-theme.json";
                case ThemeName.Dark:
                    return "dark_vs.json";
                case ThemeName.DarkPlus:
                    return "dark_plus.json";
                case ThemeName.DimmedMonokai:
                    return "dimmed-monokai-color-theme.json";
                case ThemeName.KimbieDark:
                    return "kimbie-dark-color-theme.json";
                case ThemeName.Light:
                    return "light_vs.json";
                case ThemeName.LightPlus:
                    return "light_plus.json";
                case ThemeName.Monokai:
                    return "monokai-color-theme.json";
                case ThemeName.QuietLight:
                    return "quietlight-color-theme.json";
                case ThemeName.Red:
                    return "Red-color-theme.json";
                case ThemeName.SolarizedDark:
                    return "solarized-dark-color-theme.json";
                case ThemeName.SolarizedLight:
                    return "solarized-light-color-theme.json";
                case ThemeName.TomorrowNightBlue:
                    return "tomorrow-night-blue-color-theme.json";
            }

            return null;
        }
    }
}
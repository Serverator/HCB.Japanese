#nullable disable warnings
using System.IO.Compression;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace HCB.Japanese;
public static class Dictionary
{
    private static bool _isInitialized;
    public static bool IsInitialized => _isInitialized;

    private static Kanji[] _kanji;

    /// <summary>
    /// Returns array of all kanji in the dictionary
    /// </summary>
    public static Kanji[] Kanji
    {
        get
        {
            if (!IsInitialized)
                InitDictionary();
            return _kanji;
        }
    }

    private static Word[] _words;

    /// <summary>
    /// Returns array of all words in the dictionary <br/>
    /// If dictionary is not initialized
    /// </summary>
    public static Word[] Words
    {
        get
        {
            if (!IsInitialized)
                InitDictionary();
            return _words;
        }
    }

    /// <summary>
    /// Initializes the dictionary from Dictionary.json file
    /// </summary
    public static void InitDictionaryFromFile(string jsonLocation = @"X:\")
    {
        if (string.IsNullOrWhiteSpace(jsonLocation))
            throw new ArgumentException("Location is not selected");

        if (jsonLocation[^1] != '\\')
            jsonLocation += '\\';

        if (!File.Exists($"{jsonLocation}Dictionary.json"))
            throw new FileNotFoundException($"Dictionary.gz file not found at \"{jsonLocation}\"");

        (Kanji[] Kanji, Word[] Words) dictionary;

        using (var json = File.OpenRead($"{jsonLocation}Dictionary.json"))
            dictionary = JsonSerializer.Deserialize<(Kanji[], Word[])>(json, serializerOptions);

        if (dictionary.Kanji == null || dictionary.Words == null)
            throw new Exception("Oopsie happened!");

        _kanji = dictionary.Kanji;
        _words = dictionary.Words;

        _isInitialized = true;
    }

    /// <summary>
    /// Initializes the dictionary from Resources
    /// </summary
    public static void InitDictionary()
    {
        (Kanji[] Kanji, Word[] Words) dictionary;

        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("HCB.Japanese.Dictionary.gz"))
        using (var decompressor = new GZipStream(stream, CompressionMode.Decompress))
            dictionary = JsonSerializer.Deserialize<(Kanji[], Word[])>(decompressor, serializerOptions);

        if (dictionary.Kanji == null || dictionary.Words == null)
            throw new Exception("Oopsie happened!");

        _kanji = dictionary.Kanji;
        _words = dictionary.Words;

        _isInitialized = true;
    }

    public static void ClearDictionary()
    {
        _isInitialized = false;

        _kanji = null;
        _words = null;
    }

    private static JsonSerializerOptions serializerOptions => new JsonSerializerOptions() { IncludeFields = true, IgnoreReadOnlyProperties = true, Converters = { new BaseClassConverter() }, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };

#if DEBUG
    /// <summary>
    /// Fully rebuilds the database <br/>
    /// Needs spare 1.5 gigs of ram and takes 10-60 seconds to complete <br/>
    /// Requires Kanjidic.xml, JMDict.xml and optionally WKKanji.json, WKVocab.json 
    /// </summary>
    public static void RegenerateDatabase(string inputLocation = @"X:\", string outputLocation = @"C:\Users\Serverator\source\repos\Japanese\")
    {

        if (!File.Exists($@"{inputLocation}Kanjidic.xml") || !File.Exists($@"{inputLocation}JMDict.xml"))
            throw new FileNotFoundException($"Kanjidic or JMDict not found at \"{inputLocation}\"");

        var KanjiDic = new XmlDocument();
        KanjiDic.Load($@"{inputLocation}Kanjidic.xml");

        var kanjiDic = KanjiDic.DocumentElement.SelectNodes("character").Cast<XmlNode>().AsParallel().Select(x =>
        {
            return new Kanji
            {
                Literal = x.SelectSingleNode("literal").InnerText[0],
                Meanings = x.SelectNodes("reading_meaning/rmgroup/meaning").Cast<XmlNode>().Where(x => x.Attributes.Count == 0).Select(node => node.InnerText).ToArray(),
                Grade = ParseOrNull(x.SelectSingleNode("misc/grade")?.InnerText),
                JLPT = ParseOrNull(x.SelectSingleNode("misc/jlpt")?.InnerText),
                Frequency = ParseOrNull(x.SelectSingleNode("misc/freq")?.InnerText),
                KunReadings = x.SelectNodes("reading_meaning/rmgroup/reading").Cast<XmlNode>().Where(x => x.Attributes[0].Value == "ja_kun").Select(node => node.InnerText).ToArray(),
                OnReadings = x.SelectNodes("reading_meaning/rmgroup/reading").Cast<XmlNode>().Where(x => x.Attributes[0].Value == "ja_on").Select(node => node.InnerText).ToArray()
            };
        }).OrderBy(x => x.Frequency != 0 ? x.Frequency : 9999).ToArray();

        var JMDict = new XmlDocument();
        JMDict.Load($@"{inputLocation}JMDict.xml");

        var wordDic = JMDict.DocumentElement.ChildNodes.Cast<XmlNode>().AsParallel().Select(x =>
        {
            var senses = x.SelectNodes("sense").Cast<XmlNode>().Select<XmlNode, (string[] Info, string[] Meaning)>(x =>
                new(
                    x.SelectNodes("pos").Cast<XmlNode>().Select(x => x.InnerXml[1..^1]).ToArray(),
                    x.SelectNodes("gloss").Cast<XmlNode>().Where(x => x.Attributes.Cast<XmlAttribute>().Any(x => x.Name == "xml:lang" && x.Value == "eng")).Select(x => x.InnerText).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray()
                    )
                ).Where(x => x.Meaning.Any()).ToArray();

            Word word = null;

            if (senses.Any(x => x.Info.Any(i => i.StartsWith("v") && i != "vulg" && i != "vidg")))
                word = new VerbWord();
            else
                word = new Word();

            word.Senses = senses;
            word.Kanji = x.SelectNodes("k_ele/keb").Cast<XmlNode>().Select(x => x.InnerText).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            word.Kana = x.SelectNodes("r_ele/reb").Cast<XmlNode>().Select(x => x.InnerText).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            word.WordFrequency = x.SelectNodes("k_ele/ke_pri").Cast<XmlNode>().Where(x => x.InnerText.Contains("nf"))
                .Select(x => int.Parse(x.InnerText[2..])).FirstOrDefault();
            word.OnlyKana = !word.Kanji.Any();

            if (word is VerbWord)
            {
                var verb = (VerbWord)word;

                if (verb.Infos.Any(i => i.StartsWith("v1")))
                {
                    verb.VerbType = VerbWord.VerbTypes.Ichidan;
                    if (verb.MainReading[^1] == 'る')
                        verb.Stem = verb.MainReading[..^1];
                }
                else if (verb.Infos.Any(i => i.StartsWith("v5") && i.Length == 3))
                {
                    verb.VerbType = VerbWord.VerbTypes.Godan;
                    var c = Typer.VerbEndings[verb.Infos.First(i => i.StartsWith("v5"))[2]];
                    if (verb.MainReading[^1] == c)
                        verb.Stem = verb.MainReading[..^1];
                }
                else if (verb.Infos.Any(i => i == "vs"))
                    verb.VerbType = VerbWord.VerbTypes.Suru;
                else
                    verb.VerbType = VerbWord.VerbTypes.Other;

                return verb;
            }
            else
            {
                return word;
            }
        }).OrderBy(x => x.WordFrequency != 0 ? x.WordFrequency : 99).ToArray();

        if (File.Exists($"{inputLocation}WKKanji.json") && File.Exists($"{inputLocation}WKVocab.json"))
        {
            // Parses WKKanji and runs search for each kanji element
            JsonDocument.Parse(File.ReadAllText($@"{inputLocation}WKKanji.json")).RootElement.EnumerateArray().AsParallel().Select(x => x.GetProperty("data")).ForAll(wkKanji =>
            {
                var kanjiChar = wkKanji.GetProperty("characters").GetString()[0];
                var kanji = kanjiDic.FirstOrDefault(x => x.Literal == kanjiChar);
                if (kanji != null)
                    kanji.WKLevel = wkKanji.GetProperty("level").GetInt32();
                else
                    wordDic.FirstOrDefault(x => x.Kanji.Any(k => k == kanjiChar.ToString())).WKLevel = wkKanji.GetProperty("level").GetInt32();
            });

            // Parses WKVocab to set level to every word
            JsonDocument.Parse(File.ReadAllText($@"{inputLocation}WKVocab.json")).RootElement.EnumerateArray().AsParallel().Select(x => x.GetProperty("data")).ForAll(wkWord =>
            {
                var kanjis = wkWord.GetProperty("characters").GetString();
                var word = wordDic.FirstOrDefault(x => x.Kanji.Any(kanji => string.Equals(kanji, kanjis)));
                if (word != null)
                    word.WKLevel = wkWord.GetProperty("level").GetInt32();
            });
        }

        wordDic = wordDic.OrderBy(x => x.WordFrequency != 0 ? x.WordFrequency : int.MaxValue).ThenBy(x => x.WKLevel != 0 ? x.WKLevel : int.MaxValue).ToArray();
        kanjiDic = kanjiDic.OrderBy(x => x.Frequency != 0 ? x.Frequency : int.MaxValue).ThenBy(x => x.JLPT == 0).ToArray();

        var json = JsonSerializer.SerializeToUtf8Bytes<(object[], object[])>(new(kanjiDic, wordDic), serializerOptions);
        //File.WriteAllBytes($@"X:\Dictionary.json", json);

        using (var writer = File.OpenWrite($@"{outputLocation}Dictionary.gz"))
        using (Stream stream = new MemoryStream(json))
        using (var compress = new GZipStream(writer, CompressionLevel.Optimal, false))
            stream.CopyTo(compress);


        //File.WriteAllText($@"{outputLocation}Words.json", JsonSerializer.Serialize(wordDic, serializerOptions));

        static int ParseOrNull(string value) => int.TryParse(value, out var number) ? number : 0;
    }
#endif
}

public class BaseClassConverter : JsonConverter<Word>
{
    private readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions() { IncludeFields = true, IgnoreReadOnlyProperties = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };
    private enum TypeDiscriminator
    {
        Word = 0,
        Verb = 1,
        Adverb = 2
    }

    public override bool CanConvert(Type type)
    {
        return typeof(Word).IsAssignableFrom(type);
    }

    public override Word Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject || !reader.Read() || reader.TokenType != JsonTokenType.PropertyName || reader.GetString() != "Type" || !reader.Read() || reader.TokenType != JsonTokenType.Number)
            throw new JsonException();

        Word baseClass;
        var typeDiscriminator = (TypeDiscriminator)reader.GetInt32();

        if (!reader.Read() || reader.GetString() != "Value" || !reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        switch (typeDiscriminator)
        {
            case TypeDiscriminator.Word:
                baseClass = (Word)JsonSerializer.Deserialize(ref reader, typeof(Word), serializerOptions);
                break;
            case TypeDiscriminator.Verb:
                baseClass = (VerbWord)JsonSerializer.Deserialize(ref reader, typeof(VerbWord), serializerOptions);
                break;
            case TypeDiscriminator.Adverb:
                throw new NotImplementedException();
            default:
                throw new NotSupportedException();
        }

        if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
            throw new JsonException();

        return baseClass;
    }

    public override void Write(Utf8JsonWriter writer, Word value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        if (value.GetType() == typeof(Word))
            writer.WriteNumber("Type", (int)TypeDiscriminator.Word);
        else if (value.GetType() == typeof(VerbWord))
            writer.WriteNumber("Type", (int)TypeDiscriminator.Verb);
        else
            throw new NotImplementedException();
        writer.WritePropertyName("Value");
        JsonSerializer.Serialize(writer, value, value.GetType(), serializerOptions);

        writer.WriteEndObject();
    }
}
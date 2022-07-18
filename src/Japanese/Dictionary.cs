using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace HCB.Japanese;

/// <summary>
/// Japanese dictionary that contains all of the <seealso cref="Kanji">Kanji</seealso> and <seealso cref="Words">Words</seealso> <br/>
/// Before accessing any of the dictionaries, make sure to run <seealso cref="Initialize"/>
/// </summary>
public static class Dictionary
{
    /// <summary>
    /// Asynchronous task of initializing
    /// </summary>
    private static Task InitTask { get; set; }

    /// <summary>
    /// Shows if dictionary is initialized
    /// </summary>
    public static bool IsInitialized 
    {
        get => _isInitialized;
        private set
        {
            if (_isInitialized == value)
                return;

            _isInitialized = value;

            if (_isInitialized)
                OnInitialize();
        }
    }
    private static bool _isInitialized;

    /// <summary>
    /// When dictionary initializes, inkoves this event
    /// </summary>
    public static event Action OnInitialize = delegate { };


    /// <summary>
    /// Returns array of all kanji in the dictionary <br/>
    /// Dictionary needs to be initialized or it will throw
    /// </summary>
    public static Kanji[] Kanji
    {
        get
        {
            if (IsInitialized)
                return _kanji;
            else if (InitTask != null)
            {
                InitTask.Wait();
                return _kanji;
            }
            else
                throw new Exception("Trying to access uninitialized dictionary! Run Dictionary.Initialize() before trying to access it");
        }
        private set => _kanji = value;
    }
    private static Kanji[] _kanji;


    /// <summary>
    /// Returns array of all words in the dictionary <br/>
    /// Dictionary needs to be initialized or it will throw
    /// </summary>
    public static Word[] Words
    {
        get
        {
            if (IsInitialized)
                return _words;
            else if (InitTask != null)
            {
                InitTask.Wait();
                return _words;
            }
            else
                throw new Exception("Trying to access uninitialized dictionary! Run Dictionary.Initialize() before trying to access it");
        }
        private set => _words = value;
    }
    private static Word[] _words;

    /// <summary>
    /// Initializes the dictionary from Resources <br/>
    /// Can take about 1-3 seconds to decompress and deserialize data
    /// </summary>
    public static void Initialize()
    {
        (Kanji[] Kanji, Word[] Words) dictionary;

        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("HCB.Dictionary.gz"))
        using (var decompressor = new GZipStream(stream, CompressionMode.Decompress))
            dictionary = JsonSerializer.Deserialize<(Kanji[], Word[])>(decompressor, serializerOptions);

        Kanji = dictionary.Kanji;
        Words = dictionary.Words;
        IsInitialized = true;
    }

    /// <summary>
    /// Initializes the dictionary asynchronously from Resources <br/>
    /// Can take about 1-3 seconds finish. If you try to get any of the dictionaries and initialization is not complete, main thread will wait for task completion. <br/>
    /// To mitigate that, use <seealso cref="IsInitialized"/> to check if Dictionary is ready for use
    /// </summary>  
    public static Task InitializeAsync() => InitTask = Task.Run(Initialize);

    /// <summary>
    /// Clears dictionary to free up memory
    /// </summary>
    public static void ClearDictionary()
    {
        InitTask = null;
        IsInitialized = false;

        Kanji = null;
        Words = null;
        
    }

    /// <summary>
    /// Initializes the dictionary from Dictionary.gz file at specified location <br/>
    /// Can take about 1-3 seconds finish.
    /// </summary>
    public static void Initialize(string dictionaryLocation = @"C:\")
    {
        if (string.IsNullOrWhiteSpace(dictionaryLocation))
            throw new ArgumentException("Location is not selected");

        if (dictionaryLocation[^1] != '\\')
            dictionaryLocation += '\\';

        if (!File.Exists($"{dictionaryLocation}Dictionary.gz"))
            throw new FileNotFoundException($"Dictionary.gz file not found at \"{dictionaryLocation}\"");

        (Kanji[] Kanji, Word[] Words) dictionary;

        using var json = File.OpenRead($"{dictionaryLocation}Dictionary.gz");
        using var gzipStream = new GZipStream(json, CompressionMode.Decompress);
        dictionary = JsonSerializer.Deserialize<(Kanji[], Word[])>(gzipStream, serializerOptions);

        Kanji = dictionary.Kanji;
        Words = dictionary.Words;
        IsInitialized = true;
    }

    /// <summary>
    /// Initializes the dictionary asynchronously from Dictionary.gz file at specified location <br/>
    /// Can take about 1-3 seconds finish. If you try to get any of the dictionaries and initialization is not complete, main thread will wait for task completion. <br/>
    /// To mitigate that, use <seealso cref="IsInitialized"/> to check if Dictionary is ready for use
    /// </summary>
    public static Task InitializeAsync(string dictionaryLocation = @"C:\") => InitTask = Task.Run(() => Initialize(dictionaryLocation));

    private static JsonSerializerOptions serializerOptions => new() { IncludeFields = true, IgnoreReadOnlyProperties = true, Converters = { new WordClassConverter() }, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };

    /// <summary>
    /// Fully rebuilds the dictionary file <br/>
    /// Needs spare 1.5 gigs of ram and takes 10-60 seconds to complete <br/>
    /// Downloads Kanjidic, JMDict and WKLevels and parses them into json, compressed in .gz file
    /// </summary>
    public static void RegenerateDictionaryFile(string outputLocation = @"C:\")
    {
        const string Kanjidic2Uri = "http://www.edrdg.org/kanjidic/kanjidic2.xml.gz";
        const string JmdictUri = "http://ftp.edrdg.org/pub/Nihongo/JMdict_e.gz";
        const string WKLevelUri = "http://hotcrow.bar/WKLevels.gz";

        Kanji[] kanjiDic = GetXDocFromUri(Kanjidic2Uri).Root.Elements("character").AsParallel().Select(x =>
           new Kanji
           {
               Literal = x.Element("literal").Value.First(),
               Meanings = x.Descendants("meaning").Select(x => x.Value).ToArray(),
               Grade = Parse(x.Descendants("grade").FirstOrDefault()?.Value),
               JLPT = Parse(x.Descendants("jlpt").FirstOrDefault()?.Value),
               Frequency = Parse(x.Descendants("freq").FirstOrDefault()?.Value),
               KunReadings = x.Descendants("reading").Where(x => x.FirstAttribute.Value == "ja_kun").Select(x => x.Value).ToArray(),
               OnReadings = x.Descendants("reading").Where(x => x.FirstAttribute.Value == "ja_on").Select(x => x.Value).ToArray()
           }).OrderBy(x => x.Frequency != 0 ? x.Frequency : int.MaxValue).ToArray();

        Word[] wordDic = GetXDocFromUri(JmdictUri, true).Root.Elements("entry").AsParallel().Select(x =>
            {
                Word word = null;
                var pos = x.Descendants("pos").Select(p => p.Value[..^1]).ToArray();
                if (pos.Any(p => p.StartsWith("v") && p != "vulg"))
                {
                    if (pos.Any(p => p.StartsWith("v1")))
                        word = new VerbWord() { Type = VerbWord.VerbTypes.Ichidan };
                    else if (pos.Any(p => p.StartsWith("v5") && p.Length == 3))
                        word = new VerbWord() { Type = VerbWord.VerbTypes.Godan };
                    else
                        word = new VerbWord() { Type = VerbWord.VerbTypes.Other };
                }
                else if (pos.Any(p => p.StartsWith("adj") && p != "adj-f"))
                {
                    if (pos.Any(p => p.StartsWith("adj-i")))
                        word = new AdjectiveWord() { Type = AdjectiveWord.AdjectiveType.I };
                    else if (pos.Any(p => p.StartsWith("adj-na")))
                        word = new AdjectiveWord() { Type = AdjectiveWord.AdjectiveType.Na };
                    else
                        word = new AdjectiveWord() { Type = AdjectiveWord.AdjectiveType.Other };
                }
                else
                    word = new Word();

                word.Kanji = x.Descendants("keb").Select(x => x.Value).ToArray();
                word.Kana = x.Descendants("reb").Select(x => x.Value).ToArray();
                word.WordFrequency = Parse(x.Descendants("ke_pri").FirstOrDefault(p => p.Value.StartsWith("nf"))?.Value[2..]);
                word.Senses = x.Elements("sense").Select<XElement, (string[], string[])>(s => new(s.Elements("pos").Select(x => x.Value).ToArray(), s.Elements("gloss").Select(x => x.Value).ToArray())).ToArray();

                return word;
            }).ToArray();

        var WKLevels = DeserializeJsonFromUri<((char literal, int level)[] kanjis, (string reading, int level)[] words)>(WKLevelUri);
        WKLevels.kanjis.AsParallel().ForAll(x => kanjiDic.FirstOrDefault(w => w.Literal == x.literal).WKLevel = x.level);
        WKLevels.words.AsParallel().ForAll(x => wordDic.FirstOrDefault(w => w.MainReading == x.reading).WKLevel = x.level);

        wordDic = wordDic.OrderBy(x => x.WordFrequency != 0 ? x.WordFrequency : int.MaxValue).ThenBy(x => x.WKLevel != 0 ? x.WKLevel : int.MaxValue).ToArray();
        kanjiDic = kanjiDic.OrderBy(x => x.Frequency != 0 ? x.Frequency : int.MaxValue).ThenBy(x => x.JLPT == 0).ToArray();

        var json = JsonSerializer.SerializeToUtf8Bytes<(object[], object[])>(new(kanjiDic, wordDic), serializerOptions);

        using (var writer = File.OpenWrite($@"{outputLocation}Dictionary.gz"))
        using (Stream stream = new MemoryStream(json))
        using (var compress = new GZipStream(writer, CompressionLevel.Optimal, false))
            stream.CopyTo(compress);

        static int Parse(string value) => int.TryParse(value, out var number) ? number : 0;


        static XDocument GetXDocFromUri(string url, bool needsParsing = false)
        {
            using var webClient = new WebClient();
            using var httpStream = webClient.OpenRead(url);
            using var gzipStream = new GZipStream(httpStream, CompressionMode.Decompress);
            // Because XML to Linq cannot give me a reference handle, I have to parse references out of the original file
            if (needsParsing)
            {
                using var memoryStream = new MemoryStream();
                gzipStream.CopyTo(memoryStream);
                var xmldoc = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray()).Replace("pos>&", "pos>");
                return XDocument.Parse(xmldoc);
            }
            return XDocument.Load(gzipStream);
        }

        static T DeserializeJsonFromUri<T>(string url)
        {
            using var webClient = new WebClient();
            using var httpStream = webClient.OpenRead(url);
            using var gzipStream = new GZipStream(httpStream, CompressionMode.Decompress);
            return JsonSerializer.Deserialize<T>(gzipStream, serializerOptions);
        }
    }

    /// <summary>
    /// Converter class to serialize and deserialize <seealso cref="Word"/> and it's derivatives
    /// </summary>
    private class WordClassConverter : JsonConverter<Word>
    {
        private readonly JsonSerializerOptions serializerOptions = new() { IncludeFields = true, IgnoreReadOnlyProperties = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };
        private enum WordType
        {
            Word = 0,
            Verb = 1,
            Adjective = 2
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
            var typeDiscriminator = (WordType)reader.GetInt32();

            if (!reader.Read() || reader.GetString() != "Value" || !reader.Read() || reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            switch (typeDiscriminator)
            {
                case WordType.Word:
                    baseClass = JsonSerializer.Deserialize<Word>(ref reader, serializerOptions);
                    break;
                case WordType.Verb:
                    baseClass = JsonSerializer.Deserialize<VerbWord>(ref reader, serializerOptions);
                    break;
                case WordType.Adjective:
                    baseClass = JsonSerializer.Deserialize<AdjectiveWord>(ref reader, serializerOptions);
                    break;
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
                writer.WriteNumber("Type", (int)WordType.Word);
            else if (value.GetType() == typeof(VerbWord))
                writer.WriteNumber("Type", (int)WordType.Verb);
            else if (value.GetType() == typeof(AdjectiveWord))
                writer.WriteNumber("Type", (int)WordType.Adjective);
            else
                throw new NotImplementedException();

            writer.WritePropertyName("Value");
            JsonSerializer.Serialize(writer, value, value.GetType(), serializerOptions);

            writer.WriteEndObject();
        }
    }
}
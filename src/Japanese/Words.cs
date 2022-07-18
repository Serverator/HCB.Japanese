#nullable disable warnings
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;

namespace HCB.Japanese;
/// <summary>
/// Contains every information of a japanese word <br/>
/// All kanji and kana readings, translations and other info
/// </summary>
[DebuggerDisplay("{MainReading} ({Kana[0]}) - {Meanings[0]}")]
public record Word
{
    public int WordFrequency = 0;
    public int WKLevel = 0;

    public string[] Kanji = null;
    public string[] Kana = null;
    public (string[] Info, string[] Meaning)[] Senses = null;

    #region Get properties
    [JsonIgnore]
    public bool OnlyKana => !Kanji.Any();
    [JsonIgnore]
    public bool UsuallyKana => OnlyKana || Senses.Any(s => s.Info.Any(i => i == "uk"));
    [JsonIgnore]
    public IEnumerable<Kanji> UsedKanji => Kanji.ConcatString().Where(x => !Japanese.Kana.IsKana(x)).Distinct().Select(x => Dictionary.Kanji.FirstOrDefault(y => y.Literal == x)).Where(x => x != null);
    [JsonIgnore]
    public string MainReading => UsuallyKana ? Kana.FirstOrDefault() : (Kanji.FirstOrDefault() ?? Kana.FirstOrDefault());
    [JsonIgnore]
    public string MainMeaning => Senses?.FirstOrDefault().Meaning?.FirstOrDefault() ?? "";
    [JsonIgnore]
    public string MainKana => OnlyKana && !Japanese.Kana.HasKatakana(Kana[0]) ? string.Empty : Converters.KatakanaToHiragana(Kana[0]);
    [JsonIgnore]
    public string[] Infos => Senses.SelectMany(x => x.Info).Distinct().ToArray();
    [JsonIgnore]
    public string[] ParsedInfo => Infos.Select(x => infoParser[x]).ToArray();
    [JsonIgnore]
    public string[] Meanings => Senses.SelectMany(x => x.Meaning).ToArray();
    #endregion

    #region Info dictionary
    private static readonly Dictionary<string, string> infoParser = new(new KeyValuePair<string, string>[]
        {
            new ("ateji","ateji (phonetic) reading"),
            new ("ik","word containing irregular kana/kanji usage"),
            new ("io","irregular okurigana usage"),
            new ("oK","word containing out-dated kanji or kanji usage"),
            new ("rK","rarely-used kanji form"),
            new ("abbr","abbreviation"),
            new ("arch","archaism"),
            new ("char","character"),
            new ("chn","children's language"),
            new ("col","colloquialism"),
            new ("company","company name"),
            new ("creat","creature"),
            new ("dated","dated term"),
            new ("dei","deity"),
            new ("derog","derogatory"),
            new ("doc","document"),
            new ("ev","event"),
            new ("fam","familiar language"),
            new ("fem","female term or language"),
            new ("fict","fiction"),
            new ("form","formal or literary term"),
            new ("given","given name or forename, gender not specified"),
            new ("group","group"),
            new ("hist","historical term"),
            new ("hon","honorific or respectful (sonkeigo) language"),
            new ("hum","humble (kenjougo) language"),
            new ("id","idiomatic expression"),
            new ("joc","jocular, humorous term"),
            new ("leg","legend"),
            new ("m-sl","manga slang"),
            new ("male","male term or language"),
            new ("myth","mythology"),
            new ("net-sl","Internet slang"),
            new ("obj","object"),
            new ("obs","obsolete term"),
            new ("obsc","obscure term"),
            new ("on-mim","onomatopoeic or mimetic word"),
            new ("organization","organization name"),
            new ("oth","other"),
            new ("person","full name of a particular person"),
            new ("place","place name"),
            new ("poet","poetical term"),
            new ("pol","polite (teineigo) language"),
            new ("product","product name"),
            new ("proverb","proverb"),
            new ("quote","quotation"),
            new ("rare","rare"),
            new ("relig","religion"),
            new ("sens","sensitive"),
            new ("serv","service"),
            new ("sl","slang"),
            new ("station","railway station"),
            new ("surname","family or surname"),
            new ("uk","word usually written using kana alone"),
            new ("unclass","unclassified name"),
            new ("vulg","vulgar expression or word"),
            new ("work","work of art, literature, music, etc. name"),
            new ("X","rude or X-rated term (not displayed in educational software)"),
            new ("yoji","yojijukugo"),
            new ("adj-f","noun or verb acting prenominally"),
            new ("adj-i","I-Adjective (keiyoushi)"),
            new ("adj-ix","I-Adjective (keiyoushi) - yoi/ii class"),
            new ("adj-kari","'kari' adjective (archaic)"),
            new ("adj-ku","'ku' adjective (archaic)"),
            new ("adj-na","Na-adjective (keiyodoshi)"),
            new ("adj-nari","Archaic/formal form of na-adjective"),
            new ("adj-no","nouns which may take the genitive case particle 'no'"),
            new ("adj-pn","pre-noun adjectival (rentaishi)"),
            new ("adj-shiku","'shiku' adjective (archaic)"),
            new ("adj-t","'taru' adjective"),
            new ("adv","adverb (fukushi)"),
            new ("adv-to","adverb taking the 'to' particle"),
            new ("aux","auxiliary"),
            new ("aux-adj","auxiliary adjective"),
            new ("aux-v","auxiliary verb"),
            new ("conj","conjunction"),
            new ("cop","copula"),
            new ("ctr","counter"),
            new ("exp","expressions (phrases, clauses, etc.)"),
            new ("int","interjection (kandoushi)"),
            new ("n","noun (common) (futsuumeishi)"),
            new ("n-adv","adverbial noun (fukushitekimeishi)"),
            new ("n-pr","proper noun"),
            new ("n-pref","noun, used as a prefix"),
            new ("n-suf","noun, used as a suffix"),
            new ("n-t","noun (temporal) (jisoumeishi)"),
            new ("num","numeric"),
            new ("pn","pronoun"),
            new ("pref","prefix"),
            new ("prt","particle"),
            new ("suf","suffix"),
            new ("unc","unclassified"),
            new ("v-unspec","verb unspecified"),
            new ("v1","Ichidan verb"),
            new ("v1-s","Ichidan verb - kureru special class"),
            new ("v2a-s","Nidan verb with 'u' ending (archaic)"),
            new ("v2b-k","Nidan verb (upper class) with 'bu' ending (archaic)"),
            new ("v2b-s","Nidan verb (lower class) with 'bu' ending (archaic)"),
            new ("v2d-k","Nidan verb (upper class) with 'dzu' ending (archaic)"),
            new ("v2d-s","Nidan verb (lower class) with 'dzu' ending (archaic)"),
            new ("v2g-k","Nidan verb (upper class) with 'gu' ending (archaic)"),
            new ("v2g-s","Nidan verb (lower class) with 'gu' ending (archaic)"),
            new ("v2h-k","Nidan verb (upper class) with 'hu/fu' ending (archaic)"),
            new ("v2h-s","Nidan verb (lower class) with 'hu/fu' ending (archaic)"),
            new ("v2k-k","Nidan verb (upper class) with 'ku' ending (archaic)"),
            new ("v2k-s","Nidan verb (lower class) with 'ku' ending (archaic)"),
            new ("v2m-k","Nidan verb (upper class) with 'mu' ending (archaic)"),
            new ("v2m-s","Nidan verb (lower class) with 'mu' ending (archaic)"),
            new ("v2n-s","Nidan verb (lower class) with 'nu' ending (archaic)"),
            new ("v2r-k","Nidan verb (upper class) with 'ru' ending (archaic)"),
            new ("v2r-s","Nidan verb (lower class) with 'ru' ending (archaic)"),
            new ("v2s-s","Nidan verb (lower class) with 'su' ending (archaic)"),
            new ("v2t-k","Nidan verb (upper class) with 'tsu' ending (archaic)"),
            new ("v2t-s","Nidan verb (lower class) with 'tsu' ending (archaic)"),
            new ("v2w-s","Nidan verb (lower class) with 'u' ending and 'we' conjugation (archaic)"),
            new ("v2y-k","Nidan verb (upper class) with 'yu' ending (archaic)"),
            new ("v2y-s","Nidan verb (lower class) with 'yu' ending (archaic)"),
            new ("v2z-s","Nidan verb (lower class) with 'zu' ending (archaic)"),
            new ("v4b","Yodan verb with 'bu' ending (archaic)"),
            new ("v4g","Yodan verb with 'gu' ending (archaic)"),
            new ("v4h","Yodan verb with 'hu/fu' ending (archaic)"),
            new ("v4k","Yodan verb with 'ku' ending (archaic)"),
            new ("v4m","Yodan verb with 'mu' ending (archaic)"),
            new ("v4n","Yodan verb with 'nu' ending (archaic)"),
            new ("v4r","Yodan verb with 'ru' ending (archaic)"),
            new ("v4s","Yodan verb with 'su' ending (archaic)"),
            new ("v4t","Yodan verb with 'tsu' ending (archaic)"),
            new ("v5aru","Godan verb - -aru special class"),
            new ("v5b","Godan verb with 'bu' ending"),
            new ("v5g","Godan verb with 'gu' ending"),
            new ("v5k","Godan verb with 'ku' ending"),
            new ("v5k-s","Godan verb - Iku/Yuku special class"),
            new ("v5m","Godan verb with 'mu' ending"),
            new ("v5n","Godan verb with 'nu' ending"),
            new ("v5r","Godan verb with 'ru' ending"),
            new ("v5r-i","Godan verb with 'ru' ending (irregular verb)"),
            new ("v5s","Godan verb with 'su' ending"),
            new ("v5t","Godan verb with 'tsu' ending"),
            new ("v5u","Godan verb with 'u' ending"),
            new ("v5u-s","Godan verb with 'u' ending (special class)"),
            new ("v5uru","Godan verb - Uru old class verb (old form of Eru)"),
            new ("vi","intransitive verb"),
            new ("vk","Kuru verb - special class"),
            new ("vn","irregular nu verb"),
            new ("vr","irregular ru verb, plain form ends with -ri"),
            new ("vs","noun or participle which takes the aux. verb suru"),
            new ("vs-c","su verb - precursor to the modern suru"),
            new ("vs-i","suru verb - included"),
            new ("vs-s","suru verb - special class"),
            new ("vt","transitive verb"),
            new ("vz","Ichidan verb - zuru verb (alternative form of -jiru verbs)"),
            new ("gikun","gikun (meaning as reading) or jukujikun (special kanji reading)"),
            new ("ok","out-dated or obsolete kana usage"),
            new ("uK","word usually written using kanji alone")
        });
    #endregion
}


public record VerbWord : Word
{
    public enum VerbTypes { Ichidan, Godan, Irregular, Suru, Nidan, Yodan, Other };
    public string Stem;
    public VerbTypes Type;
}

public record AdjectiveWord : Word
{
    public enum AdjectiveType { I, Na, Other };
    public AdjectiveType Type;
}

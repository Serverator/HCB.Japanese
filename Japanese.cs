#nullable disable
//#define generate_database
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;
using System.Diagnostics;
using static HCB.Japanese.Word;

using HCB.Helper;

namespace HCB.Japanese;






/// <summary>
/// Helper class to convert english letters to japanese
/// </summary>
public static class Typer
{
    #region Dictionaries
    public static readonly Dictionary<char, char> VerbEndings = new(new KeyValuePair<char, char>[]
        {
            new('u','う'),
            new('k','く'),
            new('s','す'),
            new('t','つ'),
            new('n','ぬ'),
            new('f','ふ'),
            new('m','む'),
            new('r','る'),
            new('g','ぐ'),
            new('z','ず'),
            //new('z','づ'),
            new('b','ぶ'),
            new('p','ぷ')
        });

    static readonly Dictionary<string, string> toHiragana = new(new KeyValuePair<string, string>[]
        {
            new("a","あ"),
            new("i","い"),
            new("u","う"),
            new("e","え"),
            new("o","お"),
            new("xa","ぁ"),
            new("xi","ぃ"),
            new("xu","ぅ"),
            new("xe","ぇ"),
            new("xo","ぉ"),
            new("ka","か"), new("ga","が"),
            new("ki","き"), new("gi","ぎ"),
            new("ku","く"), new("gu","ぐ"),
            new("ke","け"), new("ge","げ"),
            new("ko","こ"), new("go","ご"),
            new("sa","さ"), new("za","ざ"),
            new("shi","し"), new("ji","じ"),
            new("su","す"), new("zu","ず"),
            new("se","せ"), new("ze","ぜ"),
            new("so","そ"), new("zo","ぞ"),
            new("ta","た"), new("da","だ"),
            new("chi","ち"), new("di","ぢ"),
            new("tsu","つ"), new("du","づ"),
            new("te","て"), new("de","で"),
            new("to","と"), new("do","ど"),
            new("ha","は"), new("ba","ば"), new("pa","ぱ"),
            new("hi","ひ"), new("bi","び"), new("pi","ぴ"),
            new("fu","ふ"), new("bu","ぶ"), new("pu","ぷ"),
            new("he","へ"), new("be","べ"), new("pe","ぺ"),
            new("ho","ほ"), new("bo","ぼ"), new("po","ぽ"),
            new("na","な"), new("nna","んな"), new("nnna","んな"),
            new("ni","に"), new("nni","んに"), new("nnni","んに"),
            new("nu","ぬ"), new("nnu","んぬ"), new("nnnu","んぬ"),
            new("ne","ね"), new("nne","んね"), new("nnne","んね"),
            new("no","の"), new("nno","んの"), new("nnno","んの"),
            new("ma","ま"),
            new("mi","み"),
            new("mu","む"),
            new("me","め"),
            new("mo","も"),
            new("ra","ら"),
            new("ri","り"),
            new("ru","る"),
            new("re","れ"),
            new("ro","ろ"),
            new("wa","わ"),
            new("wo","を"),
            new("ya","や"),
            new("yu","ゆ"),
            new("yo","よ"),
            new("xya","ゃ"),
            new("xyu","ゅ"),
            new("xyo","ょ"),
            //new("n","ん"),
            new("nn","ん"),
            new("kya","きゃ"), new("kyu","きゅ"), new("kyo","きょ"),
            new("gya","ぎゃ"), new("gyu","ぎゅ"), new("gyo","ぎょ"),
            new("sha","しゃ"), new("shu","しゅ"), new("sho","しょ"),
            new("shya","しゃ"), new("shyu","しゅ"), new("shyo","しょ"),
            new("ja","じゃ"),  new("ju","じゅ"),  new("jo","じょ"),
            new("jya","じゃ"),  new("jyu","じゅ"),  new("jyo","じょ"),
            new("cha","ちゃ"), new("chu","ちゅ"), new("cho","ちょ"),
            new("chya","ちゃ"), new("chyu","ちゅ"), new("chyo","ちょ"),
            new("dya","ぢゃ"),  new("dyu","ぢゅ"),  new("dyo","ぢょ"),
            new("nya","にゃ"), new("nyu","にゅ"), new("nyo","にょ"),
            new("hya","ひゃ"), new("hyu","ひゅ"), new("hyo","ひょ"),
            new("bya","びゃ"), new("byu","びゅ"), new("byo","びょ"),
            new("pya","ぴゃ"), new("pyu","ぴゅ"), new("pyo","ぴょ"),
            new("mya","みゃ"), new("myu","みゅ"), new("myo","みょ"),
            new("rya","りゃ"), new("ryu","りゅ"), new("ryo","りょ"),
            new("xtsu","っ")
        });

    static readonly Dictionary<string, string> toKatakana = new(new KeyValuePair<string, string>[]
        {
            new("a","ア"),
            new("i","イ"),
            new("u","ウ"),
            new("e","エ"),
            new("o","オ"),
            new("xa","ァ"),
            new("xi","ィ"),
            new("xu","ゥ"),
            new("xe","ェ"),
            new("xo","ォ"),
            new("ka","カ"), new("ga","ガ"),
            new("ki","キ"), new("gi","ギ"),
            new("ku","ク"), new("gu","グ"),
            new("ke","ケ"), new("ge","ゲ"),
            new("ko","コ"), new("go","ゴ"),
            new("sa","サ"), new("za","ザ"),
            new("shi","シ"), new("ji","ジ"),
            new("su","ス"), new("zu","ズ"),
            new("se","セ"), new("ze","ゼ"),
            new("so","ソ"), new("zo","ゾ"),
            new("ta","タ"), new("da","ダ"),
            new("chi","チ"), new("di","ヂ"),
            new("tsu","ツ"), new("du","ヅ"),
            new("te","テ"), new("de","デ"),
            new("to","ト"), new("do","ド"),
            new("ha","ハ"), new("ba","バ"), new("pa","パ"),
            new("hi","ヒ"), new("bi","ビ"), new("pi","ピ"),
            new("fu","フ"), new("bu","ブ"), new("pu","プ"),
            new("he","ヘ"), new("be","ベ"), new("pe","ペ"),
            new("ho","ホ"), new("bo","ボ"), new("po","ポ"),
            new("na","ナ"), new("nna","ンナ"), new("nnna","ンナ"),
            new("ni","ニ"), new("nni","ンニ"), new("nnni","ンニ"),
            new("nu","ヌ"), new("nnu","ンヌ"), new("nnnu","ンヌ"),
            new("ne","ネ"), new("nne","ンネ"), new("nnne","ンネ"),
            new("no","ノ"), new("nno","ンノ"), new("nnno","ンノ"),
            new("ma","マ"),
            new("mi","ミ"),
            new("mu","ム"),
            new("me","メ"),
            new("mo","モ"),
            new("ra","ラ"),
            new("ri","リ"),
            new("ru","ル"),
            new("re","レ"),
            new("ro","ロ"),
            new("wa","ワ"),
            new("wo","ヲ"),
            new("ya","ヤ"),
            new("yu","ユ"),
            new("yo","ヨ"),
            new("xya","ャ"),
            new("xyu","ュ"),
            new("xyo","ョ"),
            //new("n","ン"),
            new("nn","ン"),
            new("kya","キャ"), new("kyu","キュ"), new("kyo","キョ"),
            new("gya","ギャ"), new("gyu","ギュ"), new("gyo","ギョ"),
            new("sha","シャ"), new("shu","シュ"), new("sho","ショ"),
            new("shya","シャ"), new("shyu","シュ"), new("shyo","ショ"),
            new("ja","ジャ"),  new("ju","ジュ"),  new("jo","ジョ"),
            new("jya","ジャ"),  new("jyu","ジュ"),  new("jyo","ジョ"),
            new("cha","チャ"), new("chu","チュ"), new("cho","チョ"),
            new("dya","ヂャ"),  new("dyu","ヂュ"),  new("dyo","ヂョ"),
            new("nya","ニャ"), new("nyu","ニュ"), new("nyo","ニョ"),
            new("hya","ヒャ"), new("hyu","ヒュ"), new("hyo","ヒョ"),
            new("bya","ビャ"), new("byu","ビュ"), new("byo","ビョ"),
            new("pya","ピャ"), new("pyu","ピュ"), new("pyo","ピョ"),
            new("mya","ミャ"), new("myu","ミュ"), new("myo","ミョ"),
            new("rya","リャ"), new("ryu","リュ"), new("ryo","リョ"),
            new("xtsu","ッ")
        });

    static Dictionary<string, string> fromKanaLookup = new(new KeyValuePair<string, string>[]
        {
            new("あ", "a"),
            new("い", "i"),
            new("う", "u"),
            new("え", "e"),
            new("お", "o"),
            new("か", "ka"), new("が", "ga"),
            new("き", "ki"), new("ぎ", "gi"),
            new("く", "ku"), new("ぐ", "gu"),
            new("け", "ke"), new("げ", "ge"),
            new("こ", "ko"), new("ご", "go"),
            new("さ", "sa"), new("ざ", "za"),
            new("し", "shi"), new("じ", "ji"),
            new("す", "su"), new("ず", "zu"),
            new("せ", "se"), new("ぜ", "ze"),
            new("そ", "so"), new("ぞ", "zo"),
            new("た", "ta"), new("だ", "da"),
            new("ち", "chi"), new("ぢ", "ji"),
            new("つ", "tsu"), new("づ", "zu"),
            new("て", "te"), new("で", "de"),
            new("と", "to"), new("ど", "do"),
            new("は", "ha"), new("ば", "ba"), new("ぱ", "pa"),
            new("ひ", "hi"), new("び", "bi"), new("ぴ", "pi"),
            new("ふ", "fu"), new("ぶ", "bu"), new("ぷ", "pu"),
            new("へ", "he"), new("べ", "be"), new("ぺ", "pe"),
            new("ほ", "ho"), new("ぼ", "bo"), new("ぽ", "po"),
            new("な", "na"),
            new("に", "ni"),
            new("ぬ", "nu"),
            new("ね", "ne"),
            new("の", "no"),
            new("ま", "ma"),
            new("み", "mi"),
            new("む", "mu"),
            new("め", "me"),
            new("も", "mo"),
            new("ら", "ra"),
            new("り", "ri"),
            new("る", "ru"),
            new("れ", "re"),
            new("ろ", "ro"),
            new("わ", "wa"),
            new("を", "wo"),
            new("や", "ya"),
            new("ゆ", "yu"),
            new("ん", "n"),
            new("きゃ", "kya"), new("きゅ", "kyu"), new("きょ", "kyo"),
            new("ぎゃ", "gya"), new("ぎゅ", "gyu"), new("ぎょ", "gyo"),
            new("しゃ", "sha"), new("しゅ", "shu"), new("しょ", "sho"),
            new("じゃ", "ja"),  new("じゅ", "ju"),  new("じょ", "jo"),
            new("ちゃ", "cha"), new("ちゅ", "chu"), new("ちょ", "cho"),
            new("ぢゃ", "ja"),  new("ぢゅ", "ju"),  new("ぢょ", "jo"),
            new("にゃ", "nya"), new("にゅ", "nyu"), new("にょ", "nyo"),
            new("ひゃ", "hya"), new("ひゅ", "hyu"), new("ひょ", "hyo"),
            new("びゃ", "bya"), new("びゅ", "byu"), new("びょ", "byo"),
            new("ぴゃ", "pya"), new("ぴゅ", "pyu"), new("ぴょ", "pyo"),
            new("みゃ", "mya"), new("みゅ", "myu"), new("みょ", "myo"),
            new("りゃ", "rya"), new("りゅ", "ryu"), new("りょ", "ryo"),
            new("ア", "A"),
            new("イ", "I"),
            new("ウ", "U"),
            new("エ", "E"),
            new("オ", "O"),
            new("カ", "KA"), new("ガ", "GA"),
            new("キ", "KI"), new("ギ", "GI"),
            new("ク", "KU"), new("グ", "GU"),
            new("ケ", "KE"), new("ゲ", "GE"),
            new("コ", "KO"), new("ゴ", "GO"),
            new("サ", "SA"), new("ザ", "ZA"),
            new("シ", "SHI"), new("ジ", "JI"),
            new("ス", "SU"), new("ズ", "ZU"),
            new("セ", "SE"), new("ゼ", "ZE"),
            new("ソ", "SO"), new("ゾ", "ZO"),
            new("タ", "TA"), new("ダ", "DA"),
            new("チ", "CHI"), new("ヂ", "JI"),
            new("ツ", "TSU"), new("ヅ", "ZU"),
            new("テ", "TE"), new("デ", "DE"),
            new("ト", "TO"), new("ド", "DO"),
            new("ハ", "HA"), new("バ", "BA"), new("パ", "PA"),
            new("ヒ", "HI"), new("ビ", "BI"), new("ピ", "PI"),
            new("フ", "FU"), new("ブ", "BU"), new("プ", "PU"),
            new("ヘ", "HE"), new("ベ", "BE"), new("ペ", "PE"),
            new("ホ", "HO"), new("ボ", "BO"), new("ポ", "PO"),
            new("ナ", "NA"),
            new("ニ", "NI"),
            new("ヌ", "NU"),
            new("ネ", "NE"),
            new("ノ", "NO"),
            new("マ", "MA"),
            new("ミ", "MI"),
            new("ム", "MU"),
            new("メ", "ME"),
            new("モ", "MO"),
            new("ラ", "RA"),
            new("リ", "RI"),
            new("ル", "RU"),
            new("レ", "RE"),
            new("ロ", "RO"),
            new("ワ", "WA"),
            new("ヲ", "WO"),
            new("ヤ", "YA"),
            new("ユ", "YU"),
            new("ン", "N"),
            new("キャ", "KYA"), new("キュ", "KYU"), new("キョ", "KYO"),
            new("ギャ", "GYA"), new("ギュ", "GYU"), new("ギョ", "GYO"),
            new("シャ", "SHA"), new("シュ", "SHU"), new("ショ", "SHO"),
            new("ジャ", "JA"),  new("ジュ", "JU"),  new("ジョ", "JO"),
            new("チャ", "CHA"), new("チュ", "CHU"), new("チョ", "CHO"),
            new("ヂャ", "JA"),  new("ヂュ", "JU"),  new("ヂョ", "JO"),
            new("ニャ", "NYA"), new("ニュ", "NYU"), new("ニョ", "NYO"),
            new("ヒャ", "HYA"), new("ヒュ", "HYU"), new("ヒョ", "HYO"),
            new("ビャ", "BYA"), new("ビュ", "BYU"), new("ビョ", "BYO"),
            new("ピャ", "PYA"), new("ピュ", "PYU"), new("ピョ", "PYO"),
            new("ミャ", "MYA"), new("ミュ", "MYU"), new("ミョ", "MYO"),
            new("リャ", "RYA"), new("リュ", "RYU"), new("リョ", "RYO")
        });

    /*
    static Dictionary<char, char> HiraganaToKatakana = new(new KeyValuePair<char, char>[]
        {
            new('あ','ア'),
            new('い','イ'),
            new('う','ウ'),
            new('え','エ'),
            new('お','オ'),
            new('か','カ'), new('が','ガ'),
            new('き','キ'), new('ぎ','ギ'),
            new('く','ク'), new('ぐ','グ'),
            new('け','ケ'), new('げ','ゲ'),
            new('こ','コ'), new('ご','ゴ'),
            new('さ','サ'), new('ざ','ザ'),
            new('し','シ'), new('じ','ジ'),
            new('す','ス'), new('ず','ズ'),
            new('せ','セ'), new('ぜ','ゼ'),
            new('そ','ソ'), new('ぞ','ゾ'),
            new('た','タ'), new('だ','ダ'),
            new('ち','チ'), new('ぢ','ヂ'),
            new('つ','ツ'), new('づ','ヅ'),
            new('て','テ'), new('で','デ'),
            new('と','ト'), new('ど','ド'),
            new('は','ハ'), new('ば','バ'), new('ぱ','パ'),
            new('ひ','ヒ'), new('び','ビ'), new('ぴ','ピ'),
            new('ふ','フ'), new('ぶ','ブ'), new('ぷ','プ'),
            new('へ','ヘ'), new('べ','ベ'), new('ぺ','ペ'),
            new('ほ','ホ'), new('ぼ','ボ'), new('ぽ','ポ'),
            new('な','ナ'),
            new('に','ニ'),
            new('ぬ','ヌ'),
            new('ね','ネ'),
            new('の','ノ'),
            new('ま','マ'),
            new('み','ミ'),
            new('む','ム'),
            new('め','メ'),
            new('も','モ'),
            new('ら','ラ'),
            new('り','リ'),
            new('る','ル'),
            new('れ','レ'),
            new('ろ','ロ'),
            new('わ','ワ'),
            new('を','ヲ'),
            new('や','ヤ'),
            new('ゆ','ユ'),
            new('ん','ン'),
            new('ゃ','ャ'),
            new('ゅ','ュ'),
            new('ょ','ョ'),
            new('ぁ','ァ'),
            new('ぃ','ィ'),
            new('ぅ','ゥ'),
            new('ぇ','ェ'),
            new('ぉ','ォ')
        });

    static Dictionary<char, char> KatakanaToHiragana = new(new KeyValuePair<char, char>[]
        {
            new('ア','あ'),
            new('イ','い'),
            new('ウ','う'),
            new('エ','え'),
            new('オ','お'),
            new('カ','か'), new('ガ','が'),
            new('キ','き'), new('ギ','ぎ'),
            new('ク','く'), new('グ','ぐ'),
            new('ケ','け'), new('ゲ','げ'),
            new('コ','こ'), new('ゴ','ご'),
            new('サ','さ'), new('ザ','ざ'),
            new('シ','し'), new('ジ','じ'),
            new('ス','す'), new('ズ','ず'),
            new('セ','せ'), new('ゼ','ぜ'),
            new('ソ','そ'), new('ゾ','ぞ'),
            new('タ','た'), new('ダ','だ'),
            new('チ','ち'), new('ヂ','ぢ'),
            new('ツ','つ'), new('ヅ','づ'),
            new('テ','て'), new('デ','で'),
            new('ト','と'), new('ド','ど'),
            new('ハ','は'), new('バ','ば'), new('パ','ぱ'),
            new('ヒ','ひ'), new('ビ','び'), new('ピ','ぴ'),
            new('フ','ふ'), new('ブ','ぶ'), new('プ','ぷ'),
            new('ヘ','へ'), new('ベ','べ'), new('ペ','ぺ'),
            new('ホ','ほ'), new('ボ','ぼ'), new('ポ','ぽ'),
            new('ナ','な'),
            new('ニ','に'),
            new('ヌ','ぬ'),
            new('ネ','ね'),
            new('ノ','の'),
            new('マ','ま'),
            new('ミ','み'),
            new('ム','む'),
            new('メ','め'),
            new('モ','も'),
            new('ラ','ら'),
            new('リ','り'),
            new('ル','る'),
            new('レ','れ'),
            new('ロ','ろ'),
            new('ワ','わ'),
            new('ヲ','を'),
            new('ヤ','や'),
            new('ユ','ゆ'),
            new('ン','ん'),
            new('ャ','ゃ'),
            new('ュ','ゅ'),
            new('ョ','ょ'),
            new('ァ','ぁ'),
            new('ィ','ぃ'),
            new('ゥ','ぅ'),
            new('ェ','ぇ'),
            new('ォ','ぉ')
        });
    */

    public const string Hiragana = "あいうえおかきくけこがぎぐげごさしすせそざじずぜぞたちつてとだぢづでどはひふへほばびぶべぼぱぴぷぺぽなにぬねのまみむめもらりるれろわをやゆんゃゅょぁぃぅぇぉっー";
    public const string Katakana = "アイウエオカキクケコガギグゲゴサシスセソザジズゼゾタチツテトダヂヅデドハヒフヘホバビブベボパピプペポナニヌネノマミムメモラリルレロワヲヤユンャュョァィゥェォッー";
    public const string Kana = Hiragana + Katakana;
    #endregion

    public static string ToKana(string text, bool forceConversion = false, bool isRealtimeInput = false)
    {
        string answer = string.Empty;
    //int 

    Start:
        while (text.Length != 0)
        {
            if (!char.IsLetter(text[0]))
            {
                if (text[0] == '-')
                {
                    answer += 'ー';
                    text = text[1..];
                    continue;
                }
                else
                {
                    answer += text[0];
                    text = text[1..];
                    continue;
                }
            }

            int startingChar = (text.Length >= 3 && text[0] == text[1] && char.ToLower(text[0]) != 'n' && !IsVowel(text[0])) ? 1 : 0;

            for (int i = 4; i >= 1; i--)
            {
                if (text.Length < i + startingChar)
                    continue;

                string key = text.Substring(startingChar, i);
                bool isKatakana = IsAllUpper(key);
                key = key.ToLower();

                if (isKatakana)
                {
                    if (toKatakana.TryGetValue(key, out string kana))
                    {
                        text = text[(i + startingChar)..];
                        if (startingChar == 1)
                            answer += 'ッ';
                        answer += kana;
                        goto Start;
                    }
                }
                else
                {
                    if (toHiragana.TryGetValue(key, out string kana))
                    {
                        text = text[(i + startingChar)..];
                        if (startingChar == 1)
                            answer += 'っ';
                        answer += kana;
                        goto Start;
                    }
                }
            }

            if (text[0] == 'n' && (!isRealtimeInput || text.Length > 1))
            {
                answer += 'ん';
                text = text[1..];
                continue;
            }
            else if (text[0] == 'N' && (!isRealtimeInput || text.Length > 1))
            {
                answer += 'ン';
                text = text[1..];
                continue;
            }

            // If nothing found
            answer += text[0];
            text = text[1..];
            continue;
        }

        if (forceConversion)
        {
            answer = answer.Replace('n', 'ん').Replace('N', 'ン');
        }

        return answer;

        bool IsAllUpper(string input)
        {
            return input.All(c => !char.IsLetter(c) || char.IsUpper(c));
        }

        bool IsVowel(char c)
        {
            return "aeiou".Contains(c, StringComparison.InvariantCultureIgnoreCase);
        }
    }

    public static string ToHiragana(string str) => string.Concat(str.Select(ToHiragana));

    public static char ToHiragana(char c)
    {
        int index = Katakana.IndexOf(c);
        if (index != -1)
            return Hiragana[index];
        else
            return c;
    }

    public static string ToKatakana(string str) => string.Concat(str.Select(ToKatakana));

    public static char ToKatakana(char c)
    {
        int index = Hiragana.IndexOf(c);
        if (index != -1)
            return Katakana[index];
        else
            return c;
    }

    public static bool HasKana(string str) => str.Any(c => Kana.Contains(c));
    public static bool IsKana(string str) => str.All(c => Kana.Contains(c));
    public static bool IsKana(char c) => !Kana.Contains(c);

    public static bool HasKatakana(string str) => str.Any(c => Katakana.Contains(c));
    public static bool IsKatakana(string str) => str.All(c => Katakana.Contains(c));
    public static bool IsKatakana(char c) => !Katakana.Contains(c);

    public static bool HasHiragana(string str) => str.Any(c => Hiragana.Contains(c));
    public static bool IsHiragana(string str) => str.All(c => Hiragana.Contains(c));
    public static bool IsHiragana(char c) => !Hiragana.Contains(c);



    private static bool PossibleEnglishWord(string str)
    {
        return Dictionary.Words.Where(x => x.WordFrequency != null)
            .Any(x => x.Senses[0].Meaning.Any(x => string.Equals(x, str, StringComparison.OrdinalIgnoreCase)));
    }


    public static IEnumerable<Word> SearchForWords(string searchString, out string searchArgs)
    {
        searchArgs = String.Empty;
        if (string.IsNullOrWhiteSpace(searchString)) return null;

        List<string> anyKanaSearch = new();
        List<string> strictKanaSearch = new();
        List<string> exactKanaSearch = new();
        List<string> meaningSearch = new();
        List<string> exactMeaningSearch = new();
        foreach (var str in searchString.Split('"')
                     .Select((element, index) => index % 2 == 0 // If even index
                         ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) // Split the item
                         : new string[] { "*" + element }) // Keep the entire item
                     .SelectMany(element => element))
        {
            if (str.Length >= 1)
            {
                if (str[0] == '*')
                {
                    string exactString = str[1..];
                    if (IsKana(exactString))
                        exactKanaSearch.Add(exactString);
                    else
                        exactMeaningSearch.Add(exactString);
                    continue;
                }

                if (str[0] == '!')
                {
                    string exactString = ToKana(str[1..]);
                    if (IsKana(exactString))
                        anyKanaSearch.Add(exactString);
                    else
                        meaningSearch.Add(str[1..]);
                    continue;
                }

                string kana = ToKana(str);
                if (IsKana(kana) && !PossibleEnglishWord(str))
                {
                    if (HasKatakana(kana))
                        strictKanaSearch.Add(kana);
                    else
                    {
                        anyKanaSearch.Add(kana);
                    }

                    continue;
                }
                else
                {
                    meaningSearch.Add(str);
                    continue;
                }
            }
        }

        var filteredWords = Dictionary.Words.AsParallel().AsOrdered().Where(word =>
                anyKanaSearch.All(searchTerm => word.Kana.Any(kana => ToHiragana(kana).StartsWith(searchTerm))) &&
                exactKanaSearch.All(searchTerm => word.Kana.Any(kana => kana.Equals(searchTerm))) &&
                meaningSearch.All(searchTerm => word.Senses.SelectMany(x => x.Meaning).SelectMany(x => x.Split(' '))
                    .Any(meaning => meaning.StartsWith(searchTerm, StringComparison.OrdinalIgnoreCase))) &&
                exactMeaningSearch.All(searchTerm =>
                    word.Senses.SelectMany(x => x.Meaning)
                        .Any(meaning => meaning.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))) &&
                exactMeaningSearch.SelectMany(x => x.Split(' ')).All(searchTerm =>
                    word.Senses.SelectMany(x => x.Meaning).SelectMany(x => x.Split(' ')).Any(meaning =>
                        meaning.Replace("(", "").Replace(")", "")
                            .Equals(searchTerm, StringComparison.OrdinalIgnoreCase))))
            .ToList();

        if (!filteredWords.Any()) return null;

        /*
        var filteredKana = filteredWords.AsParallel().AsOrdered().Where(word =>
            strictKanaSearch.All(searchTerm => word.Kana.Any(kana => kana.StartsWith(searchTerm)))).ToList();
        if (!filteredKana.Any())
            filteredKana = filteredWords.AsParallel().AsOrdered().Where(word => strictKanaSearch.All(searchTerm =>
                word.Kana.Any(kana => ToHiragana(kana).StartsWith(ToHiragana(searchTerm))))).ToList();

        // Filter exact matches first
        filteredKana = filteredKana.OrderByDescending(word =>
            anyKanaSearch.Concat(exactKanaSearch).Concat(strictKanaSearch).All(searchTerm =>
                word.Kana.Any(kana => ToHiragana(kana).Equals(ToHiragana(searchTerm))))).OrderByDescending(word =>
            meaningSearch.Concat(exactMeaningSearch).All(searchTerm =>
                word.Meaning.SelectMany(x => x)
                    .Any(meaning => meaning.Equals(searchTerm, StringComparison.OrdinalIgnoreCase)))).ToList();

        // Help me god
        filteredKana.OrderByDescending(word =>
            meaningSearch.Concat(exactMeaningSearch).Min(searchTerm =>
            {
                return word.Meaning.Where(sense => sense.Any(meaning => meaning.Equals(searchTerm)))
                    .Min(sense => word.Meaning.IndexOf(sense));
            }));
        
        filteredKana.OrderByDescending(word => 
        word.Meaning.Where(x => true).Select(x => 1).Any() ? 
        );
        */

        return filteredWords.OrderBy(word =>
        {
            var searchTerms = meaningSearch.Concat(exactMeaningSearch);
            var senses = word.Senses.Where(sense => sense.Meaning.Any(meaning => searchTerms.Any(term => term.Equals(meaning))));
            if (!senses.Any())
                return 100;
            return senses.Min(sense => word.Senses.IndexOf(sense));
        }).OrderBy(word =>
        {
            var searchTerms = anyKanaSearch.Concat(exactKanaSearch).Concat(strictKanaSearch);
            var kanas = word.Kana.Where(kana => searchTerms.Any(searchTerm => ToHiragana(kana).Equals(ToHiragana(searchTerm))));
            if (!kanas.Any())
                return 100;
            return kanas.Min(kana => word.Kana.IndexOf(kana));
        }).ToList();

        //meaningSearch.Concat(exactMeaningSearch)
        //        .Min(searchTerm => filteredWords.IndexOf(filteredWords.First(x => x.Meaning.Equals(searchTerm))));

        //filteredKana = filteredKana.AsParallel().AsOrdered().OrderByDescending(word =>
        //    anyKanaSearch.Concat(exactKanaSearch).Concat(strictKanaSearch).All(searchTerm =>
        //        word.Kana.Any(kana => ToHiragana(kana).Equals(ToHiragana(searchTerm)))) ||
        //    meaningSearch.Concat(exactMeaningSearch).All(searchTerm =>
        //        word.Meaning.Any(meaning => meaning.Equals(searchTerm, StringComparison.OrdinalIgnoreCase)))).ToList();

        //var test = filteredKana.ToList();

        //return filteredKana;
    }
}

public class Sentence
{
    public List<Token> Tokens = new();

    public Sentence()
    {

    }
    /// <summary>
    /// Tries to convert kanji string into separate words
    /// </summary>
    public static Sentence SeparateSentence(string KanjiString)
    {
        Sentence sentence = new();
        if (KanjiString == null)
            throw new ArgumentNullException("Baka! Sentence parcer cannot parse null string!");
        Word[] noPhrases = Dictionary.Words.AsParallel().Where(x => !x.Senses.SelectMany(x => x.Info).Any(y => y == "exp")).ToArray();
        Word[] particles = Dictionary.Words.AsParallel().Where(x => x.Senses.SelectMany(x => x.Info).Any(info => info == "prt")).ToArray();


        while (KanjiString.Length != 0)
        {
            Token lastExactMatch = null;
            Word[] lastMatches = noPhrases;
            Word[] matches = noPhrases;
            for (int i = 1; i <= KanjiString.Length; i++)
            {
                lastMatches = matches;
                string searchString = KanjiString[..i];
                //if (JapaneseTyper.IsKana(searchString))
                matches = lastMatches.AsParallel().Where(x => x.Kanji.Any(k => k.StartsWith(searchString)) || x.Kana.Any(k => k.StartsWith(searchString))).ToArray();

                Word match = matches.AsParallel().FirstOrDefault(x => x.Kanji.Any(k => k.Equals(searchString)) || x.Kana.Any(k => k.Equals(searchString)));

                if (match != null)
                    lastExactMatch = new Token() { isExact = true, isCorrect = true, String = searchString, Word = match };
                //else
                //    matches = lastMatches.AsParallel().Where(x => x.Kanji.Any(k => k.StartsWith(searchString))).ToArray();

                if (matches.Length == 1)
                {
                    int length = 0;
                    if (matches.First().Kanji.Concat(matches.First().Kana).Any(k => { length = k.Length; return k.Equals(KanjiString[..k.Length]); }))
                    {
                        sentence.Tokens.Add(new() { isCorrect = true, isExact = true, Word = matches.First(), String = KanjiString[..length] });
                        KanjiString = KanjiString[length..];
                        break;
                    }
                    else if (lastExactMatch != null)
                    {
                        lastExactMatch.isExact = true;
                        lastExactMatch.isCorrect = true;
                        KanjiString = KanjiString[lastExactMatch.String.Length..];
                        sentence.Tokens.Add(lastExactMatch);
                        break;
                    }
                    else
                    {
                        sentence.Tokens.Add(new() { isCorrect = false, isExact = false, Word = null, String = KanjiString[..1] });
                        KanjiString = KanjiString[1..];
                        break;
                    }
                }
                if (matches.Length == 0)
                {
                    if (lastExactMatch != null)
                    {
                        lastExactMatch.isExact = true;
                        lastExactMatch.isCorrect = true;
                        KanjiString = KanjiString[lastExactMatch.String.Length..];
                        sentence.Tokens.Add(lastExactMatch);
                        break;
                    }
                    else
                    {
                        sentence.Tokens.Add(new() { isCorrect = false, isExact = false, Word = null, String = KanjiString[..1] });
                        KanjiString = KanjiString[1..];
                        break;
                    }
                    //if (lastMatches == null)
                    //{
                    //    Tokens.Add(new() { isCorrect = false, String = searchString });
                    //    KanjiString = KanjiString[i..];
                    //    break;
                    //}
                    //Tokens.Add(new() { isCorrect = true, isExact = false, Word = lastMatches.OrderBy(x => x.WordFrequency).First(), String = searchString });
                    //KanjiString = KanjiString[i..];
                    //break;
                }
            }
        }
        return sentence;
    }

    public static Word[][] SeparateSentenceV2(string KanjiString)
    {
        //Dictionary<char, char> verbEndings = new(new KeyValuePair<char, char>[]
        //{
        //    new('u','う'),
        //    new('k','く'),
        //    new('s','す'),
        //    new('t','つ'),
        //    new('n','ぬ'),
        //    new('f','ふ'),
        //    new('m','む'),
        //    new('r','る'),
        //    new('g','ぐ'),
        //    new('z','ず'),
        //    //new('z','づ'),
        //    new('b','ぶ'),
        //    new('p','ぷ')
        //});

        //Word[] noPhrases = Database.Words.AsParallel().Where(x => !x.Senses.SelectMany(x => x.Info).All(y => y == "exp")).ToArray();
        Word[] particles = Dictionary.Words.AsParallel().Where(x => x.Senses.SelectMany(x => x.Info).Any(info => info == "prt")).ToArray();
        //(string stem, Word word)[] verbStems = Database.Words.AsParallel().Where(x => x.Infos.Any(i => i.StartsWith("v1") || (i.StartsWith("v5") && i.Length == 3))).Select<Word,(string stem,Word word)>(x =>
        //{
        //    string stem = null;
        //    if (x.Infos.Any(i => i.StartsWith("v1")))
        //    {
        //        if (x.MainReading[^1] == 'る')
        //            stem = x.MainReading[..^1];
        //    }
        //    else if (x.Infos.Any(i => i.StartsWith("v5")))
        //    {
        //        char c = verbEndings[x.Infos.First(i => i.StartsWith("v5"))[2]];
        //        if (x.MainReading[^1] == c)
        //            stem = x.MainReading[..^1];
        //    }
        //    return new(stem, x);

        //}).Where(x => x.stem != null).ToArray();
        if (string.IsNullOrWhiteSpace(KanjiString))
            throw new ArgumentNullException("Baka! Sentence parcer cannot parse empty string!");
        //string[] splitSentence = KanjiString.Split(new char[] {' ',',','\n','\r', '、', '。'}, 3);
        //Token[] tokens = splitSentence.AsParallel().
        Word[][] likelyTokens = KanjiString.AsParallel().Select((firstChar, i) =>
        {
            List<Word> words = new();
            int notFoundStreak = 0;
            bool verbFound = false;
            for (int length = 1; true; length++)
            {
                if (length + i > KanjiString.Length)
                    break;

                string searchString = KanjiString.Substring(i, length);
                bool isKana = Typer.IsKana(searchString);

                Word[] word = Dictionary.Words.Where(y => y.Kanji.Concat(y.Kana).Any(z => z == searchString)).ToArray();
                //Word[] verbs = verbStems.Where(x => x.stem == searchString).Select(x => x.word).ToArray();

                if (!word.Any())
                {
                    notFoundStreak++;

                    char lastChar = searchString[^1];
                    //if (Database.Kanji.Any(k => k.Literal == firstChar) && JapaneseTyper.IsKana(lastChar))

                    if (notFoundStreak > 4)
                        break;
                }
                else
                {
                    words.AddRange(word);
                    words = words.Distinct().ToList();
                    notFoundStreak = 0;
                }
                continue;

            }
            return words.ToArray();
        }).ToArray();

        return likelyTokens;
    }
}
[DebuggerDisplay("{String} | {Word.MainReading}")]
public class Token
{
    public string String;
    public bool isExact;
    public bool isCorrect;
    public bool isFound => Word != null;

    public Word Word;

    public Token()
    {

    }
}
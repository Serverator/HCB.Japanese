using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCB.Japanese;
public class Kana
{
    public enum Gyou { None, K, S, T, N, H, M, Y, R, W };
    public enum Dan { A, I, U, E, O, None };
    public enum Mark { None, Dakuten, Handakuten };

    public Gyou gyou;
    public Dan dan;
    public Mark mark;
    public char hiragana;
    public char katakana;

    public static char? GetKana(Gyou gyou, Dan dan, Mark mark = Mark.None)
    {
        return null;
    }

    public static (Gyou, Dan, Mark)? GetKanaParts(char? c)
    {
        return null;
    }
    public const string HiraganaKatakana = Hiragana + Katakana;
    public const string Hiragana = "あいうえおかきくけこがぎぐげごさしすせそざじずぜぞたちつてとだぢづでどはひふへほばびぶべぼぱぴぷぺぽなにぬねのまみむめもらりるれろわをやゆんゃゅょぁぃぅぇぉっゑゐー";
    public const string Katakana = "アイウエオカキクケコガギグゲゴサシスセソザジズゼゾタチツテトダヂヅデドハヒフヘホバビブベボパピプペポナニヌネノマミムメモラリルレロワヲヤユンャュョァィゥェォッヱヰー";

    public static Kana[] GetKana()
    {
        return new Kana[] {
            new() { gyou = Gyou.None, dan = Dan.A, mark = Mark.None, hiragana = 'あ', katakana = 'ア' },
            new() { gyou = Gyou.None, dan = Dan.I, mark = Mark.None, hiragana = 'ア', katakana = 'イ' },
            new() { gyou = Gyou.None, dan = Dan.U, mark = Mark.None, hiragana = 'う', katakana = 'ウ' },
            new() { gyou = Gyou.None, dan = Dan.E, mark = Mark.None, hiragana = 'え', katakana = 'エ' },
            new() { gyou = Gyou.None, dan = Dan.O, mark = Mark.None, hiragana = 'お', katakana = 'オ' },
            new() { gyou = Gyou.K,    dan = Dan.A, mark = Mark.None, hiragana = 'か', katakana = 'カ' },
            new() { gyou = Gyou.K,    dan = Dan.I, mark = Mark.None, hiragana = 'き', katakana = 'キ' },
            new() { gyou = Gyou.K,    dan = Dan.U, mark = Mark.None, hiragana = 'く', katakana = 'ク' },
            new() { gyou = Gyou.K,    dan = Dan.E, mark = Mark.None, hiragana = 'け', katakana = 'ケ' },
            new() { gyou = Gyou.K,    dan = Dan.O, mark = Mark.None, hiragana = 'こ', katakana = 'コ' },
        };
    }

    #region Static methods - Kana checkers
    /// <summary>
    /// Returns true if any char of the string is katakana or hiragana
    /// </summary>
    public static bool HasKana(string str) => str.Any(c => IsKana(c));
    /// <summary>
    /// Returns true if every char of the string is katakana or hiragana
    /// </summary>
    public static bool IsKana(string str) => str.All(c => IsKana(c));
    /// <summary>
    /// Returns true if char is katakana or hiragana
    /// </summary>
    public static bool IsKana(char c) => HiraganaKatakana.Contains(c);

    /// <summary>
    /// Returns true if any char of the string is katakana
    /// </summary>
    public static bool HasKatakana(string str) => str.Any(c => IsKatakana(c));
    /// <summary>
    /// Returns true if every char of the string is katakana
    /// </summary>
    public static bool IsKatakana(string str) => str.All(c => IsKatakana(c));
    /// <summary>
    /// Returns true if char is katakana
    /// </summary>
    public static bool IsKatakana(char c) => Katakana.Contains(c);

    /// <summary>
    /// Returns true if any char of the string is hiragana
    /// </summary>
    public static bool HasHiragana(string str) => str.Any(c => IsHiragana(c));
    /// <summary>
    /// Returns true if every char of the string is hiragana
    /// </summary>
    public static bool IsHiragana(string str) => str.All(c => IsHiragana(c));
    /// <summary>
    /// Returns true if char is hiragana
    /// </summary>
    public static bool IsHiragana(char c) => Hiragana.Contains(c);
    #endregion
}
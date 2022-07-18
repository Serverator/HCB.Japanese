using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCB.Japanese;

/// <summary>
/// Static method that helps convertation between hiragana, katakana, romaji <br/>
/// </summary>
public static class Converters
{

    /// <summary>
    /// Converts every Katakana character in a string in respective Hiragana character
    /// </summary>
    public static string KatakanaToHiragana(string str) => new(str.Select(KatakanaToHiragana).ToArray());
    /// <summary>
    /// Converts Katakana character in respective Hiragana character
    /// </summary>
    public static char KatakanaToHiragana(char c)
    {
        var index = Kana.Katakana.IndexOf(c);
        if (index != -1)
            return Kana.Hiragana[index];
        else
            return c;
    }
    /// <summary>
    /// Converts every Hiragana character in a string in respective Katakana character
    /// </summary>
    public static string HiraganaToKatakana(string str) => new(str.Select(HiraganaToKatakana).ToArray());
    /// <summary>
    /// Converts Hiragana character in respective Katakana character
    /// </summary>
    public static char HiraganaToKatakana(char c)
    {
        var index = Kana.Hiragana.IndexOf(c);
        if (index != -1)
            return Kana.Katakana[index];
        else
            return c;
    }


}

#nullable disable warnings
using System.Diagnostics;

namespace HCB.Japanese;

/// <summary>
/// Contains info of a kanji <br/>
/// Usual meanings and readings, and other info
/// </summary>
[DebuggerDisplay("{Literal.ToString()} - {Frequency??0}")]
public class Kanji
{
    public char Literal;
    public string[] Meanings = null;
    public string[] KunReadings = null;
    public string[] OnReadings = null;
    public int Grade = 0;
    public int JLPT = 0;
    public int Frequency = 0;
    public int WKLevel = 0;
}

public static class Kana
{
    public enum Gyou { None, K, S, T, N, H, M, Y, R, W };
    public enum Dan { A, I, U, E, O, None };
    public enum Mark { None, Dakuten, Handakuten };

    public static char? GetKana(Gyou gyou, Dan dan, Mark mark = Mark.None)
    {
        return null;
    }

    public static (Gyou, Dan, Mark)? GetKataParts(char? c)
    {

        return null;
    }

}
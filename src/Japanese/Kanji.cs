#nullable disable warnings
using System.Diagnostics;

namespace HCB.Japanese;

/// <summary>
/// Contains info of a kanji <br/>
/// Usual meanings and readings, and other info
/// </summary>
[DebuggerDisplay("{Literal.ToString()} - {Frequency}")]
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
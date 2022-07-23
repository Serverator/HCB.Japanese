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
    public char Literal { get; set; }
    public string[] Meanings { get; set; } = null;
    public string[] KunReadings { get; set; } = null;
    public string[] OnReadings { get; set; } = null;
    public int Grade { get; set; } = 0;
    public int JLPT { get; set; } = 0;
    public int Frequency { get; set; } = 0;
    public int WKLevel { get; set; } = 0;

    public override string ToString() => Literal.ToString();
}
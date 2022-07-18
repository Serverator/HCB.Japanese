using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HCB.Japanese.Typer2;

namespace HCB.Japanese;
public class Typer
{
    public string Sentence { get; set; } = string.Empty;

    public string Convert(string str)
    {
        throw new NotImplementedException();
    }

    public override string ToString() => Sentence;

    public static string ToKana(string text, bool forceConversion = false, bool isRealtimeInput = false)
    {
        var answer = string.Empty;

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

            var startingChar = text.Length >= 3 && text[0] == text[1] && char.ToLower(text[0]) != 'n' && !IsVowel(text[0]) ? 1 : 0;

            for (var i = 4; i >= 1; i--)
            {
                if (text.Length < i + startingChar)
                    continue;

                var key = text.Substring(startingChar, i);
                var isKatakana = IsAllUpper(key);
                key = key.ToLower();

                if (isKatakana)
                {
                    if (toKatakana.TryGetValue(key, out var kana))
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
                    if (toHiragana.TryGetValue(key, out var kana))
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
}


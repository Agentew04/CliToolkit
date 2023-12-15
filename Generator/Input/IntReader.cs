using System;
using System.Collections.Generic;
using System.Text;

namespace Cli.Toolkit.Input;

/// <summary>
/// A class with multiple utility methods to read input from the console.
/// </summary>
public static class IntReader {

    /// <summary>
    /// A struct to define a range of numbers.
    /// </summary>
    public class Range {

        /// <summary>
        /// The inclusive start of the range.
        /// </summary>
        public int Start { get; }

        /// <summary>
        /// The inclusive end of the range.
        /// </summary>
        public int End { get; }

        /// <summary>
        /// Creates a new range.
        /// </summary>
        /// <param name="start">The inclusive start</param>
        /// <param name="end">The inclusive end</param>
        public Range(int start, int end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// If the current range contains the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(int value) {
            return value >= Start && value <= End;
        }
    }

    public class Options {
        public string Prompt { get; set; } = "";
        public bool AllowNegative { get; set; } = false;
        public Range? Range { get; set; } = null;
        public int? DefaultValue { get; set; } = null;
        public bool AllowEmpty { get; set; } = false;
    }

    /// <summary>
    /// Reads a integer from the console.
    /// </summary>
    /// <param name="prompt">A string to be displayed immediately before the input</param>
    /// <param name="allowNegative"></param>
    /// <param name="range"></param>
    /// <param name="defaultValue"></param>
    /// <param name="allowEmpty"></param>
    /// <returns></returns>
    public static int? ReadInt(Options opt) {

        bool ok = false;
        Console.Write(opt.Prompt);
        string content = "";
        int? result = null;
        if (opt.DefaultValue is not null) {
            content = opt.DefaultValue.Value.ToString();
        }

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write(content);
        ConsoleKeyInfo key;
        do {
            string error = "";
            key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter) {
                bool canParse = int.TryParse(content, out var res);
                result = res;

                if (!canParse) {
                    error = "Valor inválido";
                }

                if (!opt.AllowNegative && result < 0) {
                    error = "Valor inválido. Digite um número positivo.";
                }

                if (opt.Range is not null && !opt.Range.Contains(result.Value)) {
                    error = $"Valor fora do intervalo [{opt.Range.Start},{opt.Range.End}]";
                }

                ok = canParse && error == "";
                if (content == "" && opt.AllowEmpty) {
                    ok = true;
                    error = "";
                    result = null;
                }
            } else if (key.Key == ConsoleKey.Backspace && content.Length > 0) {
                content = content.Substring(0, content.Length - 1);
            } else {
                if (key.KeyChar >= '0' && key.KeyChar <= '9'
                                       && content.Length < 10) {
                    content += key.KeyChar;
                }
            }

            Console.CursorLeft = opt.Prompt.Length;
            Console.Write(new string(' ', 10));
            Console.CursorLeft = opt.Prompt.Length;
            Console.ForegroundColor = error == "" ? ConsoleColor.Blue : ConsoleColor.Red;
            Console.Write(content);
            if (error != "") {
                Console.Write(" ");
                Console.Write(error);
            }
            Console.ResetColor();
        } while (!ok);
        Console.WriteLine();

        return result;
    }

}

#if !COLORLESS
#define COLORFUL
#endif

#if COLORFUL
using System;
using System.Collections.Generic;

namespace Talknet {
    public static class Exconsole {
        // "@x" to set foreground color
        // "#x" to set background color
        // x represents a character:
        //   r : red
        //   g : green
        //   b : blue
        //   c : cyan
        //   m : magenta
        //   y : yellow
        //   k : black
        //   w : white
        //   a : gray
        // Capital letter for dark version of color
        //   W = a
        //   K = k
        // Special character x
        //   ! : undo the last color setting
        //   - : undo all color setting
        // "@@" means output @, and "##" means output #
        // Invalid "@x" and "#x" will be outputted as is
        internal static readonly Dictionary<char, ConsoleColor> colorDict = new Dictionary<char, ConsoleColor> {
            ['r'] = ConsoleColor.Red, ['R'] = ConsoleColor.DarkRed,
            ['g'] = ConsoleColor.Green, ['G'] = ConsoleColor.DarkGreen,
            ['b'] = ConsoleColor.Blue, ['B'] = ConsoleColor.DarkBlue,
            ['c'] = ConsoleColor.Cyan, ['C'] = ConsoleColor.DarkCyan,
            ['m'] = ConsoleColor.Magenta, ['M'] = ConsoleColor.DarkMagenta,
            ['y'] = ConsoleColor.Yellow, ['Y'] = ConsoleColor.DarkYellow,
            ['k'] = ConsoleColor.Black, ['K'] = ConsoleColor.Black,
            ['w'] = ConsoleColor.White, ['W'] = ConsoleColor.Gray,
            ['a'] = ConsoleColor.Gray, ['A'] = ConsoleColor.DarkGray
        };

        public static ConsoleColor ForegroundColor { get => Console.ForegroundColor; set => Console.ForegroundColor = value; }
        public static ConsoleColor BackgroundColor { get => Console.BackgroundColor; set => Console.BackgroundColor = value; }

        public static void Write(string str) {
            int len = str.Length;

            ConsoleColor defaultForeColor = Console.ForegroundColor, defaultBackColor = Console.BackgroundColor;
            Stack<ConsoleColor> foreColorStack = new Stack<ConsoleColor>(), backColorStack = new Stack<ConsoleColor>();

            for (int i = 0; i < len; ++i) {
                char cur = str[i];

                if (str[i] == '@' || str[i] == '#') {
                    bool invalid = false;
                    bool fore = str[i] == '@';

                    if (i == len - 1) invalid = true;
                    else {
                        char nxt = str[i + 1];
                        var targetStack = fore ? foreColorStack : backColorStack;
                        var targetDefault = fore ? defaultForeColor : defaultBackColor;

                        if (colorDict.ContainsKey(nxt)) {
                            var newColor = colorDict[nxt];

                            targetStack.Push(newColor);
                            if (fore) Console.ForegroundColor = newColor;
                            else Console.BackgroundColor = newColor;
                        } else if (nxt == '!') {
                            if (targetStack.Count == 0) invalid = true;
                            else {
                                targetStack.Pop();
                                var newColor = targetStack.Count == 0 ? targetDefault : targetStack.Peek();

                                if (fore) Console.ForegroundColor = newColor;
                                else Console.BackgroundColor = newColor;
                            }
                        } else if (nxt == '-') {
                            targetStack.Clear();

                            if (fore) Console.ForegroundColor = defaultForeColor;
                            else Console.BackgroundColor = defaultBackColor;
                        } else if (nxt == cur) {
                            Console.Out.Write(nxt);
                        } else invalid = true;
                    }

                    if (invalid) Console.Out.Write(cur);
                    else ++i;
                } else {
                    Console.Out.Write(cur);
                }
            }
        }

        public static void WriteLine() => Console.WriteLine();
        public static void WriteLine(string str) {
            Write(str);
            Console.Out.WriteLine();
        }

        public static void WriteRaw(string str) => Console.Write(str);
        public static void WriteLineRaw(string str) => Console.WriteLine(str);
        public static void WriteLineRaw() => Console.WriteLine();
    }
}
#endif
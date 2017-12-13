using System;
using System.Collections.Generic;

namespace Talknet.Invoker {
    internal static class LineTokenizer {
        public class UnbalancedQuotationMarksException : Exception { }
        public class NeedMoreLineException : Exception { }

        //public static Dictionary<char, char> escapeChars = new Dictionary<char, char> {
        //    ['0'] = '\0',
        //    ['\''] = '\'',
        //    ['\"'] = '\"',
        //    ['\\'] = '\\',
        //    ['a'] = '\a',
        //    ['b'] = '\b',
        //    ['f'] = '\f',
        //    ['n'] = '\n',
        //    ['r'] = '\r',
        //    ['t'] = '\t',
        //    ['v'] = '\v'
        //};

        public static List<string> GetTokens(string str) {
            List<string> cache = new List<string>();

            int startIdx = 0;
            int length = str.Length;

            bool inQuotationMark = false;
            for (int i = 0; i < length; ++i) {
                if (char.IsWhiteSpace(str[i])) {
                    if (!inQuotationMark) {
                        if (startIdx < i) cache.Add(str.Substring(startIdx, i - startIdx));
                        startIdx = i + 1;
                    }
                } else if (str[i] == '\"') {
                    if (!inQuotationMark) {
                        if (startIdx < i) cache.Add(str.Substring(startIdx, i - startIdx));
                        startIdx = i;
                    } else {
                        cache.Add(str.Substring(startIdx + 1, i - startIdx - 1));
                        startIdx = i + 1;
                    }

                    inQuotationMark = !inQuotationMark;
                }
            }

            if (inQuotationMark) throw new UnbalancedQuotationMarksException();
            if (startIdx < length) cache.Add(str.Substring(startIdx, length - startIdx));

            return cache;
        }
    }
}
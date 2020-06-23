using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Test_GBooksAPI
{
    static class StaticUtilities
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)       
            => self.Select((item, index) => (item, index));

        public static string RemoveFilenameInvalidChars(this string input)
        {
            return string.Concat(input.Split(Path.GetInvalidFileNameChars()));
        }
    }
}

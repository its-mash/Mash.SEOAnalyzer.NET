using System;
using System.Collections.Generic;
using System.Text;

namespace Mash.SEOAnalyzer.NET
{
    public struct WordOccurrenceCount
    {
        public string Word;
        public int Count;

        public WordOccurrenceCount(string word, int count = 0)
        {
            this.Word = word;
            this.Count = count;
        }
    }
}

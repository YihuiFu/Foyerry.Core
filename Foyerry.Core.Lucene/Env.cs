using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Foyerry.Core.Lucene
{
    public class Env
    {
        public const string IndexPath = @"D:/ACG_News";
    }

    public enum AnalyzerType
    {
        StandardAnalyzer = 1,
        SimpleAnalyzer = 2,
        KeywordAnalyzer = 3,
        WhitespaceAnalyzer = 4,
        StopAnalyzer = 5,
    }
}

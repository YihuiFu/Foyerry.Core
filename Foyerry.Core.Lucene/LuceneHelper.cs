using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

namespace Foyerry.Core.Lucene
{
    public class LuceneHelper
    {
        
        /// <summary>
        /// 获取分词器
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Analyzer GetAnalyzer(AnalyzerType type)
        {

            switch (type)
            {
                case AnalyzerType.StandardAnalyzer:
                    return new StandardAnalyzer(Version.LUCENE_30);
                case AnalyzerType.SimpleAnalyzer:
                    return new SimpleAnalyzer();
                case AnalyzerType.KeywordAnalyzer:
                    return new KeywordAnalyzer();
                case AnalyzerType.WhitespaceAnalyzer:
                    return new WhitespaceAnalyzer();
                case AnalyzerType.StopAnalyzer:
                    return new StopAnalyzer(Version.LUCENE_30);
                default:
                    return new StandardAnalyzer(Version.LUCENE_30);
            }
        }

        public static Directory GetFsDirectory(string path)
        {
            return FSDirectory.Open(path);
        }

        public static Directory GetRamDirectory()
        {
            return new RAMDirectory();
        }

        public static IndexWriter GetIndexWriter(Analyzer analyzer, Directory directory)
        {
            return new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
        }

        public static IndexSearcher GetIndexSearcher(Directory directory)
        {
            return new IndexSearcher(directory);
        }
    }


    

  
}

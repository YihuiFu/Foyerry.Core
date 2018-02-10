using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;

namespace Foyerry.Core.Lucene
{
    public class LuceneSearch : ILuceneSearch
    {
        private Analyzer _analyzer { get; set; }
        private Directory _directory { get; set; }


        public LuceneSearch()
        {
            _analyzer = LuceneHelper.GetAnalyzer(AnalyzerType.StandardAnalyzer);
            _directory = LuceneHelper.GetFsDirectory(Env.IndexPath);
        }

        public bool AddIndex(Document doc)
        {
            try
            {
                using (var indexWriter = LuceneHelper.GetIndexWriter(_analyzer, _directory))
                {
                    indexWriter.AddDocument(doc);
                    _analyzer.Close();
                    indexWriter.Dispose();
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool UpdateIndex(Term term, Document doc)
        {
            try
            {
                using (var indexWriter = LuceneHelper.GetIndexWriter(_analyzer, _directory))
                {
                    indexWriter.UpdateDocument(term, doc);
                    _analyzer.Close();
                    indexWriter.Dispose();
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool RemoveIndexByTerm(Term term)
        {
            try
            {
                using (var indexWriter = LuceneHelper.GetIndexWriter(_analyzer, _directory))
                {
                    indexWriter.DeleteDocuments(term);
                    _analyzer.Close();
                    indexWriter.Dispose();
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public bool RemoveAllIndex()
        {
            try
            {
                using (var indexWriter = LuceneHelper.GetIndexWriter(_analyzer, _directory))
                {
                    indexWriter.DeleteAll();
                    _analyzer.Close();
                    indexWriter.Dispose();
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }

        }

        public IList<ScoreDoc> Search(Query query, int limits)
        {
            using (var searcher = new IndexSearcher(_directory, false))
            {
                var scoreDocs = searcher.Search(query, limits).ScoreDocs;
                _analyzer.Dispose();
                searcher.Dispose();
                return scoreDocs;
            }
        }
    }
}

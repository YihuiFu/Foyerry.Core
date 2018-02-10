using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;

namespace Foyerry.Core.Lucene
{
    interface ILuceneSearch
    {
        bool AddIndex(Document doc);
        bool UpdateIndex(Term term,Document doc);
        bool RemoveIndexByTerm(Term term);
        bool RemoveAllIndex();
        IList<ScoreDoc> Search(Query query,int limits);
    }
}

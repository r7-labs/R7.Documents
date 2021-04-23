using System;
using System.Collections;
using R7.Documents.Models;
using R7.Documents.Tests.Models;
using R7.Documents.ViewModels;
using Xunit;

namespace R7.Documents.Tests.ViewModels
{
    public class DocumentViewModelComparerTests
    {
        [Fact]
        void DateComparisonTest ()
        {
            var colOrder = new DocumentsSortColumn {
                ColumnName = DocumentsDisplayColumnInfo.COLUMN_SORTORDER,
                Direction = SortDirection.Ascending
            };

            var colModifiedDate = new DocumentsSortColumn {
                ColumnName = DocumentsDisplayColumnInfo.COLUMN_MODIFIEDDATE,
                Direction = SortDirection.Ascending
            };

            var today = DateTime.Today;
            var docA = new TestDocumentViewModel (new TestDocument {
                ModifiedDate = today.AddMinutes (20)
            });

            var docB = new TestDocumentViewModel (new TestDocument {
                ModifiedDate = today.AddMinutes (10)
            });

            var comparer1 = new DocumentViewModelComparer (new ArrayList {colOrder, colModifiedDate});
            Assert.Equal (0, comparer1.Compare (docA, docA));

            // A > B
            Assert.Equal (1, comparer1.Compare (docA, docB));

            var comparer2 = new DocumentViewModelComparer (new ArrayList {colModifiedDate, colOrder});
            Assert.Equal (0, comparer2.Compare (docB, docB));

            // A == B, even if A.ModifiedDate != B.ModifiedDate
            Assert.True (docA.ModifiedDate != docB.ModifiedDate);
            Assert.Equal (0, comparer2.Compare (docA, docB));
        }
    }
}

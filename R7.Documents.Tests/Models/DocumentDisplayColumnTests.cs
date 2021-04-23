using System.Collections.Generic;
using R7.Documents.Models;
using Xunit;

namespace R7.Documents.Tests.Models
{
    public class DocumentDisplayColumnTests
    {
        [Fact]
        public void ParseDisplayColumnsSettingsTest ()
        {
            var testDisplayColumns = new List<DocumentsDisplayColumn> () {
                new DocumentsDisplayColumn {
                    ColumnName = DocumentsDisplayColumn.COLUMN_TITLE,
                    LocalizedColumnName = "",
                    DisplayOrder = 1,
                    Visible = true
                },
                new DocumentsDisplayColumn {
                    ColumnName = DocumentsDisplayColumn.COLUMN_DESCRIPTION,
                    LocalizedColumnName = "",
                    DisplayOrder = 2,
                    Visible = false
                }
            };

            var displayColumns =
                DocumentsDisplayColumn.ParseDisplayColumns ("Title,,1,true#Description,,2,false");

            for (var i = 0; i < testDisplayColumns.Count; i++) {
                Assert.Equal (testDisplayColumns[i].ColumnName, displayColumns[i].ColumnName);
                Assert.Equal (testDisplayColumns[i].LocalizedColumnName, displayColumns[i].LocalizedColumnName);
                Assert.Equal (testDisplayColumns[i].DisplayOrder, displayColumns[i].DisplayOrder);
                Assert.Equal (testDisplayColumns[i].Visible, displayColumns[i].Visible);
            }
        }
    }
}

using System.Collections.Generic;
using R7.Documents.Models;
using Xunit;

namespace R7.Documents.Tests.Models
{
    public class DocumentDisplayColumnTests
    {
        IList<DocumentDisplayColumn> _testDisplayColumns = new List<DocumentDisplayColumn> {
            new DocumentDisplayColumn {
                ColumnName = DocumentDisplayColumn.COLUMN_TITLE,
                LocalizedColumnName = "",
                DisplayOrder = 1,
                Visible = true
            },
            new DocumentDisplayColumn {
                ColumnName = DocumentDisplayColumn.COLUMN_DESCRIPTION,
                LocalizedColumnName = "",
                DisplayOrder = 2,
                Visible = false
            }
        };

        [Fact]
        public void ParseDisplayColumnsSettingsTest ()
        {
            var displayColumns =
                DocumentDisplayColumn.ParseDisplayColumnsSettings ("Title,,1,True#Description,,2,False");

            for (var i = 0; i < _testDisplayColumns.Count; i++) {
                Assert.Equal (_testDisplayColumns[i].ColumnName, displayColumns[i].ColumnName);
                Assert.Equal (_testDisplayColumns[i].LocalizedColumnName, displayColumns[i].LocalizedColumnName);
                Assert.Equal (_testDisplayColumns[i].DisplayOrder, displayColumns[i].DisplayOrder);
                Assert.Equal (_testDisplayColumns[i].Visible, displayColumns[i].Visible);
            }
        }

        [Fact]
        public void FormatDisplayColumnsSettingsTest ()
        {
            var displayColumnsSettings = DocumentDisplayColumn.FormatDisplayColumnSettings (_testDisplayColumns);
            Assert.Equal ("Title,,1,True#Description,,2,False", displayColumnsSettings);
        }
    }
}

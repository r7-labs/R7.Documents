using System;

namespace R7.Documents.Models
{
    [Serializable]
    public class DocumentsSortColumnInfo : IDocumentsSortColumn
    {
        public SortDirection Direction { get; set; } = SortDirection.Ascending;

        public string ColumnName { get; set; }
    }
}

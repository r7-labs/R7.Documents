using R7.Dnn.Extensions.ViewModels;
using R7.Documents.Models;

namespace R7.Documents.ViewModels
{
    public class DocumentSortColumnViewModel : IDocumentsSortColumn
    {
        protected readonly IDocumentsSortColumn DocumentSortColumn;

        protected readonly ViewModelContext Dnn;

        public DocumentSortColumnViewModel(IDocumentsSortColumn documentSortColumn, ViewModelContext dnn)
        {
            DocumentSortColumn = documentSortColumn;
            Dnn = dnn;
        }

        #region IDocumentsSortColumn implementation

        public string ColumnName
        {
            get => DocumentSortColumn.ColumnName;
            set => DocumentSortColumn.ColumnName = value;
        }

        public SortDirection Direction
        {
            get => DocumentSortColumn.Direction;
            set => DocumentSortColumn.Direction = value;
        }

        #endregion

        public string LocalizedColumnName => Dnn.LocalizeString ($"{ColumnName}.Column");

        public string LocalizedDirection => Dnn.LocalizeString ($"SortOrder{Direction}.Text");
    }
}

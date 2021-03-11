using System.Collections;
using R7.Documents.Models;

namespace R7.Documents.ViewModels
{
    public class DocumentViewModelComparer : IComparer
    {
        ArrayList mobjSortColumns;

        public DocumentViewModelComparer (ArrayList sortColumns)
        {
            mobjSortColumns = sortColumns;
        }

        /// <summary>
        /// Compares two documents and returns a value indicating whether one is less than,
        /// equal to, or greater than the other. This method is of Comparison&lt;T&gt; delegate type
        /// </summary>
        /// <param name="x">First document.</param>
        /// <param name="y">Second document.</param>
        public int Compare (IDocumentViewModel x, IDocumentViewModel y)
        {
            if (mobjSortColumns.Count == 0) {
                return 0;
            }

            return Compare (0, x, y);
        }

        public int Compare (object x, object y)
        {
            if (mobjSortColumns.Count == 0) {
                return 0;
            }

            return Compare (0, (IDocumentViewModel) x, (IDocumentViewModel) y);
        }

        int Compare (int sortColumnIndex, IDocumentViewModel objX, IDocumentViewModel objY)
        {
            var objSortColumn = default(DocumentsSortColumnInfo);
            int intResult = 0;

            if (sortColumnIndex >= mobjSortColumns.Count) {
                return 0;
            }

            objSortColumn = (DocumentsSortColumnInfo) mobjSortColumns [sortColumnIndex];

            if (objSortColumn.Direction == SortDirection.Ascending) {
                intResult = CompareValues (objSortColumn.ColumnName, objX, objY);
            }
            else {
                intResult = CompareValues (objSortColumn.ColumnName, objY, objX);
            }

            // Difference not found, sort by next sort column
            if (intResult == 0) {
                return Compare (sortColumnIndex + 1, objX, objY);
            }

            return intResult;
        }

        int CompareValues (string columnName, IDocumentViewModel x, IDocumentViewModel y)
        {
            switch (columnName) {
                case DocumentsDisplayColumnInfo.COLUMN_SORTORDER:
                    return x.SortOrderIndex.CompareTo (y.SortOrderIndex);

                case DocumentsDisplayColumnInfo.COLUMN_CATEGORY:
                    return x.Category.CompareTo (y.Category);

                case DocumentsDisplayColumnInfo.COLUMN_CREATEDBY:
                    return x.CreatedByUser.CompareTo (y.CreatedByUser);

                case DocumentsDisplayColumnInfo.COLUMN_CREATEDDATE:
                    return x.CreatedDate.CompareTo (y.CreatedDate);

                case DocumentsDisplayColumnInfo.COLUMN_PUBLISHEDONDATE:
                    return x.PublishedOnDate.CompareTo (y.PublishedOnDate);

                case DocumentsDisplayColumnInfo.COLUMN_DESCRIPTION:
                    return x.Description.CompareTo (y.Description);

                case DocumentsDisplayColumnInfo.COLUMN_MODIFIEDBY:
                    return x.ModifiedByUser.CompareTo (y.ModifiedByUser);

                case DocumentsDisplayColumnInfo.COLUMN_MODIFIEDDATE:
                    return x.ModifiedDate.CompareTo (y.ModifiedDate);

                case DocumentsDisplayColumnInfo.COLUMN_OWNEDBY:
                    return x.OwnedByUser.CompareTo (y.OwnedByUser);

                case DocumentsDisplayColumnInfo.COLUMN_SIZE:
                    return x.Size.CompareTo (y.Size);

                case DocumentsDisplayColumnInfo.COLUMN_TITLE:
                    return x.Title.CompareTo (y.Title);

                case DocumentsDisplayColumnInfo.COLUMN_CLICKS:
                    return x.Clicks.CompareTo (y.Clicks);

                case DocumentsDisplayColumnInfo.COLUMN_ISFEATURED:
                    return x.IsFeatured.CompareTo (y.IsFeatured);
            }

            return 0;
        }
    }
}

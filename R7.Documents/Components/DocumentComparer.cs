//
// Copyright (c) 2002-2011 by DotNetNuke Corporation
// Copyright (c) 2014-2015 by Roman M. Yagodin <roman.yagodin@gmail.com>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections;
using R7.Documents.Models;

namespace R7.Documents
{
    public class DocumentComparer : IComparer
    {
        ArrayList mobjSortColumns;

        public DocumentComparer (ArrayList sortColumns)
        {
            mobjSortColumns = sortColumns;
        }

        /// <summary>
        /// Compares two documents and returns a value indicating whether one is less than, 
        /// equal to, or greater than the other. This method is of Comparison&lt;T&gt; delegate type
        /// </summary>
        /// <param name="x">First document.</param>
        /// <param name="y">Second document.</param>
        public int Compare (DocumentInfo x, DocumentInfo y)
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

            return Compare (0, (DocumentInfo) x, (DocumentInfo) y);
        }

        int Compare (int sortColumnIndex, DocumentInfo objX, DocumentInfo objY)
        {
            var objSortColumn = default(DocumentsSortColumnInfo);
            int intResult = 0;

            if (sortColumnIndex >= mobjSortColumns.Count) {
                return 0;
            }

            objSortColumn = (DocumentsSortColumnInfo) mobjSortColumns [sortColumnIndex];

            if (objSortColumn.Direction == DocumentsSortColumnInfo.SortDirection.Ascending) {
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

        int CompareValues (string columnName, DocumentInfo x, DocumentInfo y)
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
            }

            return 0;
        }
    }
}

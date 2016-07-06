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

using System;
using System.Collections.Generic;

namespace R7.Documents
{
    [Serializable]
    public class DocumentsDisplayColumnInfo : IComparable
    {

        public const string COLUMN_CREATEDBY = "CreatedBy";
        public const string COLUMN_CREATEDDATE = "CreatedDate";
        public const string COLUMN_TITLE = "Title";
        public const string COLUMN_CATEGORY = "Category";
        public const string COLUMN_OWNEDBY = "Owner";
        public const string COLUMN_MODIFIEDBY = "ModifiedBy";
        public const string COLUMN_MODIFIEDDATE = "ModifiedDate";
        public const string COLUMN_SORTORDER = "SortIndex";
        public const string COLUMN_DESCRIPTION = "Description";
        public const string COLUMN_SIZE = "Size";
        public const string COLUMN_DOWNLOADLINK = "DownloadLink";
        public const string COLUMN_CLICKS = "Clicks";
        public const string COLUMN_ICON = "Icon";

        public static HashSet<string> AvailableDisplayColumns = new HashSet<string> {
            COLUMN_ICON,
            COLUMN_TITLE,
            COLUMN_DESCRIPTION,
            COLUMN_CATEGORY,
            COLUMN_OWNEDBY,
            COLUMN_CREATEDDATE,
            COLUMN_CREATEDBY,
            COLUMN_MODIFIEDDATE,
            COLUMN_MODIFIEDBY,
            COLUMN_SIZE,
            COLUMN_CLICKS,
            COLUMN_DOWNLOADLINK
        };

        public static List<string> AvailableSortColumns = new List<string> {
            COLUMN_SORTORDER,
            COLUMN_TITLE,
            COLUMN_DESCRIPTION,
            COLUMN_CATEGORY,
            COLUMN_OWNEDBY,
            COLUMN_CREATEDDATE,
            COLUMN_CREATEDBY,
            COLUMN_MODIFIEDDATE,
            COLUMN_MODIFIEDBY,
            COLUMN_SIZE,
            COLUMN_CLICKS
        };

        private string _LocalizedColumnName;

        #region "Properties"

        public string ColumnName { get; set; }

        public string LocalizedColumnName { get; set; }

        public int DisplayOrder { get; set; }

        public bool Visible { get; set; }

        #endregion

        #region "ICompareable Interface"

        public int CompareTo (object obj)
        {
            DocumentsDisplayColumnInfo objYItem = null;

            objYItem = (DocumentsDisplayColumnInfo) obj;
            return DisplayOrder.CompareTo (objYItem.DisplayOrder);
        }

        #endregion
    }
}

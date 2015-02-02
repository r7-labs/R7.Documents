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
using System.Configuration;
using System.Data;
using DotNetNuke;

namespace R7.Documents
{
	[Serializable ()]
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
		// public const string COLUMN_URL = "Url";

		public static HashSet<string> AvailableDisplayColumns = new HashSet<string> () {
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
			COLUMN_DOWNLOADLINK,
			// ,COLUMN_URL
		};
		public static List<string> AvailableSortColumns = new List<string> () {
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
			// ,COLUMN_URL
		};

		#region "Private Members"

		private string _ColumnName;
		private int _DisplayOrder;
		private bool _Visible;

		#endregion

		private string _LocalizedColumnName;

		#region "Properties"

		public string ColumnName
		{
			get { return _ColumnName; }
			set { _ColumnName = value; }
		}

		public string LocalizedColumnName
		{
			get { return _LocalizedColumnName; }
			set { _LocalizedColumnName = value; }
		}

		public int DisplayOrder
		{
			get { return _DisplayOrder; }
			set { _DisplayOrder = value; }
		}

		public bool Visible
		{
			get { return _Visible; }
			set { _Visible = value; }
		}

		#endregion

		#region "ICompareable Interface"

		public int CompareTo (object obj)
		{
			DocumentsDisplayColumnInfo objYItem = null;

			objYItem = (DocumentsDisplayColumnInfo)obj;
			return this.DisplayOrder.CompareTo (objYItem.DisplayOrder);
		}

		#endregion


	}
}

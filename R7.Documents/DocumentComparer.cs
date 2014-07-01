//
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
// by DotNetNuke Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//

using System.Collections;

namespace R7.Documents
{
	public class DocumentComparer : IComparer
	{


		private ArrayList mobjSortColumns;

		public DocumentComparer (ArrayList SortColumns)
		{
			mobjSortColumns = SortColumns;
		}

		public int Compare (object x, object y)
		{
			if (mobjSortColumns.Count == 0)
				return 0;

			return Compare (0, (DocumentInfo)x, (DocumentInfo)y);
		}

		private int Compare (int SortColumnIndex, DocumentInfo objX, DocumentInfo objY)
		{
			DocumentsSortColumnInfo objSortColumn = default(DocumentsSortColumnInfo);
			int intResult = 0;

			if (SortColumnIndex >= mobjSortColumns.Count)
			{
				return 0;
			}

			objSortColumn = (DocumentsSortColumnInfo)mobjSortColumns [SortColumnIndex];

			if (objSortColumn.Direction == DocumentsSortColumnInfo.SortDirection.Ascending)
			{
				intResult = CompareValues (objSortColumn.ColumnName, objX, objY);
			}
			else
			{
				intResult = CompareValues (objSortColumn.ColumnName, objY, objX);
			}

			// Difference not found, sort by next sort column
			if (intResult == 0)
			{
				return Compare (SortColumnIndex + 1, objX, objY);
			}
			else
			{
				return intResult;
			}
		}

		private int CompareValues (string ColumnName, DocumentInfo ObjX, DocumentInfo ObjY)
		{
			switch (ColumnName)
			{
				case DocumentsDisplayColumnInfo.COLUMN_SORTORDER:
					if (ObjX.SortOrderIndex.CompareTo (ObjY.SortOrderIndex) != 0)
					{
						return ObjX.SortOrderIndex.CompareTo (ObjY.SortOrderIndex);
					}
					break;
				case DocumentsDisplayColumnInfo.COLUMN_CATEGORY:
					if (ObjX.Category.CompareTo (ObjY.Category) != 0)
					{
						return ObjX.Category.CompareTo (ObjY.Category);
					}
					break;
				case DocumentsDisplayColumnInfo.COLUMN_CREATEDBY:
					if (ObjX.CreatedByUser.CompareTo (ObjY.CreatedByUser) != 0)
					{
						return ObjX.CreatedByUser.CompareTo (ObjY.CreatedByUser);
					}
					break;
				case DocumentsDisplayColumnInfo.COLUMN_CREATEDDATE:
					if (ObjX.CreatedDate.CompareTo (ObjY.CreatedDate) != 0)
					{
						return ObjX.CreatedDate.CompareTo (ObjY.CreatedDate);
					}
					break;
				case DocumentsDisplayColumnInfo.COLUMN_DESCRIPTION:
					if (ObjX.Description.CompareTo (ObjY.Description) != 0)
					{
						return ObjX.Description.CompareTo (ObjY.Description);
					}
					break;
				case DocumentsDisplayColumnInfo.COLUMN_MODIFIEDBY:
					if (ObjX.ModifiedByUser.CompareTo (ObjY.ModifiedByUser) != 0)
					{
						return ObjX.ModifiedByUser.CompareTo (ObjY.ModifiedByUser);
					}
					break;
				case DocumentsDisplayColumnInfo.COLUMN_MODIFIEDDATE:
					if (ObjX.ModifiedDate.CompareTo (ObjY.ModifiedDate) != 0)
					{
						return ObjX.ModifiedDate.CompareTo (ObjY.ModifiedDate);
					}
					break;
				case DocumentsDisplayColumnInfo.COLUMN_OWNEDBY:
					if (ObjX.OwnedByUser.CompareTo (ObjY.OwnedByUser) != 0)
					{
						return ObjX.OwnedByUser.CompareTo (ObjY.OwnedByUser);
					}
					break;
				case DocumentsDisplayColumnInfo.COLUMN_SIZE:
					if (ObjX.Size.CompareTo (ObjY.Size) != 0)
					{
						return ObjX.Size.CompareTo (ObjY.Size);
					}
					break;
				case DocumentsDisplayColumnInfo.COLUMN_TITLE:
					if (ObjX.Title.CompareTo (ObjY.Title) != 0)
					{
						return ObjX.Title.CompareTo (ObjY.Title);
					}
					break;
				case DocumentsDisplayColumnInfo.COLUMN_CLICKS:
					if (ObjX.Clicks.CompareTo (ObjY.Clicks) != 0)
					{
						return ObjX.Clicks.CompareTo (ObjY.Clicks);
					}
					break;
			}

			return 0;
		}
	}
}

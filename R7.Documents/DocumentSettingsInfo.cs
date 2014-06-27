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

using System;
using System.Collections;
using System.Configuration;
using System.Data;
using DotNetNuke;
using DotNetNuke.Services.Localization;


namespace R7.Documents
{

	/// -----------------------------------------------------------------------------
	/// <summary>
	/// The DocumentSettings Class provides the Documents Settings Object
	/// </summary>
	/// <remarks>
	/// </remarks>
	/// <history>
	/// 	[aglenwright]	18 Feb 2006	Created
	/// </history>
	/// -----------------------------------------------------------------------------

	public class DocumentsSettingsInfo
	{


		public DocumentsSettingsInfo()
		{
		}

		public DocumentsSettingsInfo(string LocalResourceFile)
		{
			_LocalResourceFile = _LocalResourceFile;
		}

		#region "Private Members"
		private string _LocalResourceFile;
		private int _ModuleId;
		private bool _ShowTitleLink = true;
		private string _SortOrder;

		private string _DisplayColumns = DocumentsDisplayColumnInfo.COLUMN_TITLE + ";true," + DocumentsDisplayColumnInfo.COLUMN_OWNEDBY + ";true," + DocumentsDisplayColumnInfo.COLUMN_CATEGORY + ";true," + DocumentsDisplayColumnInfo.COLUMN_MODIFIEDDATE + ";true," + DocumentsDisplayColumnInfo.COLUMN_SIZE + ";true," + DocumentsDisplayColumnInfo.COLUMN_DOWNLOADLINK + ";true";
		private bool _UseCategoriesList = false;
		private string _DefaultFolder = "";
		private string _CategoriesListName = "Document Categories";

		private bool _AllowUserSort;
		#endregion

		#region "Properties"
		public string LocalResourceFile {
			get { return _LocalResourceFile; }
			set { _LocalResourceFile = value; }
		}

		public int ModuleId {
			get { return _ModuleId; }
			set { _ModuleId = value; }
		}

		public bool ShowTitleLink {
			get { return _ShowTitleLink; }
			set { _ShowTitleLink = value; }
		}

		public string SortOrder {
			get { return _SortOrder; }
			set { _SortOrder = value; }
		}

		public string DisplayColumns {
			get { return _DisplayColumns; }
			set { _DisplayColumns = value; }
		}

		public bool UseCategoriesList {
			get { return _UseCategoriesList; }
			set { _UseCategoriesList = value; }
		}

		public bool AllowUserSort {
			get { return _AllowUserSort; }
			set { _AllowUserSort = value; }
		}

		public string DefaultFolder {
			get { return _DefaultFolder; }
			set { _DefaultFolder = value; }
		}

		public string CategoriesListName {
			get { return _CategoriesListName; }
			set { _CategoriesListName = value; }
		}

		public ArrayList DisplayColumnList {
			get {
				string strColumnData = null;
				DocumentsDisplayColumnInfo objColumnInfo = null;
				ArrayList objColumnSettings = new ArrayList();

				if (this.DisplayColumns != string.Empty) {
					// read "saved" column sort orders in first
					foreach (string strColumnData_loopVariable in this.DisplayColumns.Split(char.Parse(","))) {
						strColumnData = strColumnData_loopVariable;
						objColumnInfo = new DocumentsDisplayColumnInfo();
						objColumnInfo.ColumnName = strColumnData.Split(char.Parse(";"))[0];
						objColumnInfo.DisplayOrder = objColumnSettings.Count + 1;
						objColumnInfo.Visible = bool.Parse(strColumnData.Split(char.Parse(";"))[1]);
						objColumnInfo.LocalizedColumnName = Localization.GetString(objColumnInfo.ColumnName + ".Header", _LocalResourceFile);

						objColumnSettings.Add(objColumnInfo);
					}
				}

				return objColumnSettings;
			}
		}

		public ArrayList SortColumnList {
			get {
				DocumentsSortColumnInfo objSortColumn = default(DocumentsSortColumnInfo);
				string strSortColumn = null;
				ArrayList objSortColumns = new ArrayList();

				//if (this.SortOrder != string.Empty) {
				if (!string.IsNullOrEmpty(this.SortOrder)) {
					foreach (string strSortColumn_loopVariable in this.SortOrder.Split(char.Parse(","))) {
						strSortColumn = strSortColumn_loopVariable;
						objSortColumn = new DocumentsSortColumnInfo();
						// REVIEW: Original: if (Strings.Left(strSortColumn, 1) == "-") {
						if (strSortColumn.StartsWith("-")) {
							objSortColumn.Direction = DocumentsSortColumnInfo.SortDirection.Descending;
							objSortColumn.ColumnName = strSortColumn.Substring(1);
						} else {
							objSortColumn.Direction = DocumentsSortColumnInfo.SortDirection.Ascending;
							objSortColumn.ColumnName = strSortColumn;
						}

						objSortColumn.LocalizedColumnName = Localization.GetString(objSortColumn.ColumnName + ".Header", _LocalResourceFile);

						objSortColumns.Add(objSortColumn);
					}
				}

				return objSortColumns;
			}
		}

		public static int FindColumn(string ColumnName, ArrayList List, bool VisibleOnly)
		{
			// Find a display column in the list and return it's index 
			int intIndex = 0;

			for (intIndex = 0; intIndex <= List.Count - 1; intIndex++) {
				var _with1 = (DocumentsDisplayColumnInfo)List[intIndex];
				if (_with1.ColumnName == ColumnName && (!VisibleOnly || _with1.Visible)) {
					return intIndex;
				}
			}

			return -1;
		}

		public static int FindGridColumn(string ColumnName, ArrayList List, bool VisibleOnly)
		{
			// Find a display column in the list and return it's "column" index 
			// as it will be displayed within the grid.  This function differs from FindColumn
			// in that it "ignores" invisible columns when counting which column index to 
			// return.
			int intIndex = 0;
			int intResult = 0;

			for (intIndex = 0; intIndex <= List.Count - 1; intIndex++) {
				var _with2 = (DocumentsDisplayColumnInfo)List[intIndex];
				if (_with2.ColumnName == ColumnName && (!VisibleOnly || _with2.Visible)) {
					return intResult;
				}
				if (_with2.Visible) {
					intResult = intResult + 1;
				}
			}

			return -1;
		}
		#endregion

	}
}

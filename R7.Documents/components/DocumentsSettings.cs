//
// DocumentsSettings.cs
//
// Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
// Copyright (c) 2014 
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
using System.Collections;
using DotNetNuke.Entities.Modules;
using DotNetNuke.UI.Modules;
using DotNetNuke.Services.Localization;

namespace R7.Documents
{
	/// <summary>
	/// Provides strong typed access to settings used by module
	/// </summary>
	public class DocumentsSettings : SettingsWrapper
	{
		public DocumentsSettings (IModuleControl module) : base (module)
		{
		}

		public DocumentsSettings (ModuleInfo module) : base (module)
		{
		}

		public string LocalResourceFile { get; set; }

		#region Tabmodule settings

		public bool ShowTitleLink
		{
			get { return ReadSetting<bool> ("Documents_ShowTitleLink", true, true); }
			set { WriteSetting<bool> ("Documents_ShowTitleLink", value, true); }
		}

		public string SortOrder
		{
			get { return ReadSetting<string> ("Documents_SortOrder", "", true); }
			set { WriteSetting<string> ("Documents_SortOrder", value, true); }
		}

		public string DisplayColumns
		{
			get
			{ 
				return ReadSetting<string> ("Documents_DisplayColumns", 
					DocumentsDisplayColumnInfo.COLUMN_ICON + ";true," +
					DocumentsDisplayColumnInfo.COLUMN_TITLE + ";true," +
					DocumentsDisplayColumnInfo.COLUMN_OWNEDBY + ";true," +
					DocumentsDisplayColumnInfo.COLUMN_CATEGORY + ";true," +
					DocumentsDisplayColumnInfo.COLUMN_MODIFIEDDATE + ";true," +
					DocumentsDisplayColumnInfo.COLUMN_SIZE + ";true," +
					DocumentsDisplayColumnInfo.COLUMN_DOWNLOADLINK + ";true", 
					true); 
			}
			set { WriteSetting<string> ("Documents_DisplayColumns", value, true); }
		}

		public bool AllowUserSort
		{
			get { return ReadSetting<bool> ("Documents_AllowUserSort", true, true); }
			set { WriteSetting<bool> ("Documents_AllowUserSort", value, true); }
		}

		#endregion

		#region Module settings

		public bool UseCategoriesList
		{
			get { return ReadSetting<bool> ("Documents_UseCategoriesList", false, false); }
			set { WriteSetting<bool> ("Documents_UseCategoriesList", value, false); }
		}

		public string CategoriesListName
		{
			get { return ReadSetting<string> ("Documents_CategoriesListName", "Document Categories", false); }
			set { WriteSetting<string> ("Documents_CategoriesListName", value, false); }
		}

		public string DefaultFolder
		{
			get { return ReadSetting<string> ("Documents_DefaultFolder", "", false); }
			set { WriteSetting<string> ("Documents_DefaultFolder", value, false); }
		}

		#endregion

		public ArrayList DisplayColumnList
		{
			get
			{
				string strColumnData;
				DocumentsDisplayColumnInfo objColumnInfo;
				var objColumnSettings = new ArrayList ();

				if (!string.IsNullOrEmpty (DisplayColumns))
				{
					// read "saved" column sort orders in first
					foreach (string strColumnData_loopVariable in this.DisplayColumns.Split(char.Parse(",")))
					{
						strColumnData = strColumnData_loopVariable;
						objColumnInfo = new DocumentsDisplayColumnInfo ();
						objColumnInfo.ColumnName = strColumnData.Split (char.Parse (";")) [0];
						objColumnInfo.DisplayOrder = objColumnSettings.Count + 1;
						objColumnInfo.Visible = bool.Parse (strColumnData.Split (char.Parse (";")) [1]);
						objColumnInfo.LocalizedColumnName = Localization.GetString (objColumnInfo.ColumnName + ".Header", LocalResourceFile);

						objColumnSettings.Add (objColumnInfo);
					}
				}

				return objColumnSettings;
			}
		}

		#region Calculated properties

		public ArrayList GetSortColumnList (string localResourceFile)
		{
			DocumentsSortColumnInfo objSortColumn = default(DocumentsSortColumnInfo);
			string strSortColumn = null;
			ArrayList objSortColumns = new ArrayList ();

			//if (this.SortOrder != string.Empty) {
			if (!string.IsNullOrEmpty (this.SortOrder))
			{
				foreach (string strSortColumn_loopVariable in this.SortOrder.Split(char.Parse(",")))
				{
					strSortColumn = strSortColumn_loopVariable;
					objSortColumn = new DocumentsSortColumnInfo ();
					// REVIEW: Original: if (Strings.Left(strSortColumn, 1) == "-") {
					if (strSortColumn.StartsWith ("-"))
					{
						objSortColumn.Direction = DocumentsSortColumnInfo.SortDirection.Descending;
						objSortColumn.ColumnName = strSortColumn.Substring (1);
					}
					else
					{
						objSortColumn.Direction = DocumentsSortColumnInfo.SortDirection.Ascending;
						objSortColumn.ColumnName = strSortColumn;
					}

					objSortColumn.LocalizedColumnName = Localization.GetString (objSortColumn.ColumnName + ".Header", localResourceFile);

					objSortColumns.Add (objSortColumn);
				}
			}

			return objSortColumns;
		}

		public static int FindColumn (string ColumnName, ArrayList List, bool VisibleOnly)
		{
			// Find a display column in the list and return it's index 
			int intIndex = 0;

			for (intIndex = 0; intIndex <= List.Count - 1; intIndex++)
			{
				var _with1 = (DocumentsDisplayColumnInfo)List [intIndex];
				if (_with1.ColumnName == ColumnName && (!VisibleOnly || _with1.Visible))
				{
					return intIndex;
				}
			}

			return -1;
		}

		public static int FindGridColumn (string ColumnName, ArrayList List, bool VisibleOnly)
		{
			// Find a display column in the list and return it's "column" index 
			// as it will be displayed within the grid.  This function differs from FindColumn
			// in that it "ignores" invisible columns when counting which column index to 
			// return.
			int intIndex = 0;
			int intResult = 0;

			for (intIndex = 0; intIndex <= List.Count - 1; intIndex++)
			{
				var _with2 = (DocumentsDisplayColumnInfo)List [intIndex];
				if (_with2.ColumnName == ColumnName && (!VisibleOnly || _with2.Visible))
				{
					return intResult;
				}
				if (_with2.Visible)
				{
					intResult = intResult + 1;
				}
			}

			return -1;
		}

		#endregion
	}
}


//
// Copyright (c) 2014-2017 by Roman M. Yagodin <roman.yagodin@gmail.com>
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
using System.Collections.Generic;
using DotNetNuke.Entities.Modules.Settings;
using DotNetNuke.Services.Localization;

namespace R7.Documents.Models
{
    /// <summary>
    /// Provides strong typed access to settings used by module
    /// </summary>
    [Serializable]
    public class DocumentsSettings
    {
        [TabModuleSetting (Prefix = "Documents_")]
        public bool ShowTitleLink { get; set; } = true;

        [TabModuleSetting (Prefix = "Documents_")]
        public string SortOrder { get; set; } = DocumentsDisplayColumnInfo.COLUMN_SORTORDER;

        [TabModuleSetting (Prefix = "Documents_")]
        public string DisplayColumns { get; set; } = DocumentsDisplayColumnInfo.COLUMN_ICON + ";true," +
                    DocumentsDisplayColumnInfo.COLUMN_TITLE + ";true," +
                    DocumentsDisplayColumnInfo.COLUMN_DOWNLOADLINK + ";true";

        [TabModuleSetting (Prefix = "Documents_")]
        public bool AllowUserSort { get; set; } = false;

        [TabModuleSetting (Prefix = "Documents_")]
        public string GridStyle { get; set; } = "bootstrap";

        [TabModuleSetting (Prefix = "Documents_")]
        public string DateTimeFormat { get; set; } = null;

        [ModuleSetting (Prefix = "Documents_")]
        public bool UseCategoriesList { get; set; } = false;

        [ModuleSetting (Prefix = "Documents_")]
        public string CategoriesListName { get; set; } = "Document Categories";

        [ModuleSetting (Prefix = "Documents_")]
        public int? DefaultFolder { get; set; }

        public List<DocumentsDisplayColumnInfo> GetDisplayColumnList (string localResourceFile)
        {
            var objColumnSettings = new List<DocumentsDisplayColumnInfo> ();
		
            if (!string.IsNullOrWhiteSpace (DisplayColumns)) {
                // read "saved" column sort orders in first
                foreach (var strColumn in DisplayColumns.Split( new [] {','}, StringSplitOptions.RemoveEmptyEntries)) {
                    var strColumnData = strColumn.Split (new [] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    var strColumnName = strColumnData [0];

                    if (DocumentsDisplayColumnInfo.AvailableDisplayColumns.Contains (strColumnName)) {
                        var objColumnInfo = new DocumentsDisplayColumnInfo {
                            ColumnName = strColumnName,
                            DisplayOrder = objColumnSettings.Count + 1,
                            Visible = bool.Parse (strColumnData [1]),
                            LocalizedColumnName = Localization.GetString (strColumnName + ".Header", localResourceFile)
                        };

                        objColumnSettings.Add (objColumnInfo);
                    }
                }
            }

            return objColumnSettings;
        }

        public ArrayList GetSortColumnList (string localResourceFile)
        {
            var objSortColumn = default (DocumentsSortColumnInfo);
            string strSortColumn = null;
            var objSortColumns = new ArrayList ();

            if (!string.IsNullOrEmpty (SortOrder)) {
                foreach (string strSortColumn_loopVariable in SortOrder.Split (',')) {
                    strSortColumn = strSortColumn_loopVariable;
                    objSortColumn = new DocumentsSortColumnInfo ();
                    if (strSortColumn.StartsWith ("-", StringComparison.InvariantCulture)) {
                        objSortColumn.Direction = DocumentsSortColumnInfo.SortDirection.Descending;
                        objSortColumn.ColumnName = strSortColumn.Substring (1);
                    }
                    else {
                        objSortColumn.Direction = DocumentsSortColumnInfo.SortDirection.Ascending;
                        objSortColumn.ColumnName = strSortColumn;
                    }

                    objSortColumn.LocalizedColumnName = Localization.GetString (
                        objSortColumn.ColumnName + ".Header",
                        localResourceFile);

                    objSortColumns.Add (objSortColumn);
                }
            }

            return objSortColumns;
        }

        public string GetDateTimeFormat ()
        {
            return (!string.IsNullOrEmpty (DateTimeFormat)) ? DateTimeFormat : "d";
        }

        #region Static methods

        public static int FindColumn (string columnName, List<DocumentsDisplayColumnInfo> columnList, bool visibleOnly)
        {
            // find a display column in the list and return it's index 
            var intIndex = 0;

            for (intIndex = 0; intIndex <= columnList.Count - 1; intIndex++) {
                var column = columnList [intIndex];
                if (column.ColumnName == columnName && (!visibleOnly || column.Visible)) {
                    return intIndex;
                }
            }

            return -1;
        }

        public static int FindGridColumn (string columnName, List<DocumentsDisplayColumnInfo> columnList, bool visibleOnly)
        {
            // find a display column in the list and return it's "column" index 
            // as it will be displayed within the grid.  This function differs from FindColumn
            // in that it "ignores" invisible columns when counting which column index to 
            // return.
            var intIndex = 0;
            var intResult = 0;

            for (intIndex = 0; intIndex <= columnList.Count - 1; intIndex++) {
                var column = columnList [intIndex];
                if (column.ColumnName == columnName && (!visibleOnly || column.Visible)) {
                    return intResult;
                }
                if (column.Visible) {
                    intResult = intResult + 1;
                }
            }

            return -1;
        }

        #endregion
    }
}

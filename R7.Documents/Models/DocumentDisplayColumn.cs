using System;
using System.Collections.Generic;
using System.Linq;

namespace R7.Documents.Models
{
    [Serializable]
    public class DocumentDisplayColumn : IComparable
    {
        public const string COLUMN_CREATEDBY = "CreatedBy";
        public const string COLUMN_CREATEDDATE = "CreatedDate";
        public const string COLUMN_PUBLISHEDONDATE = "PublishedOnDate";
        public const string COLUMN_TITLE = "Title";
        public const string COLUMN_SIGNATURE = "Signature";
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
        public const string COLUMN_ISFEATURED = "IsFeatured";

        public static HashSet<string> AvailableDisplayColumns = new HashSet<string> {
            COLUMN_ICON,
            COLUMN_TITLE,
            COLUMN_SIGNATURE,
            COLUMN_DESCRIPTION,
            COLUMN_CATEGORY,
            COLUMN_OWNEDBY,
            COLUMN_CREATEDDATE,
            COLUMN_PUBLISHEDONDATE,
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
            COLUMN_PUBLISHEDONDATE,
            COLUMN_CREATEDBY,
            COLUMN_MODIFIEDDATE,
            COLUMN_MODIFIEDBY,
            COLUMN_SIZE,
            COLUMN_CLICKS,
            COLUMN_ISFEATURED
        };

        #region "Properties"

        public string ColumnName { get; set; }

        public string LocalizedColumnName { get; set; }

        public int DisplayOrder { get; set; }

        public bool Visible { get; set; }

        #endregion

        #region IComparable implementation

        public int CompareTo (object obj)
        {
            DocumentDisplayColumn objYItem = null;

            objYItem = (DocumentDisplayColumn) obj;
            return DisplayOrder.CompareTo (objYItem.DisplayOrder);
        }

        #endregion

        public string FormatDisplayColumnSetting () => $"{ColumnName},{LocalizedColumnName},{DisplayOrder},{Visible}";

        public static List<DocumentDisplayColumn> ParseDisplayColumnsSettings (string displayColumnsSettings)
        {
            var displayColumns = new List<DocumentDisplayColumn> ();
            if (!string.IsNullOrEmpty (displayColumnsSettings)) {
                foreach (string strDisplayColumn in displayColumnsSettings.Split ('#')) {
                    var displayColumn = new DocumentDisplayColumn ();
                    displayColumn.ColumnName = strDisplayColumn.Split (',') [0];
                    displayColumn.LocalizedColumnName = strDisplayColumn.Split (',') [1];
                    displayColumn.DisplayOrder = Convert.ToInt32 (strDisplayColumn.Split (',') [2]);
                    displayColumn.Visible = Convert.ToBoolean (strDisplayColumn.Split (',') [3]);
                    displayColumns.Add (displayColumn);
                }
            }

            return displayColumns;
        }

        public static string FormatDisplayColumnSettings (IList<DocumentDisplayColumn> displayColumns)
            => string.Join ("#", displayColumns.Select (dc => dc.FormatDisplayColumnSetting ()));
    }
}

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DotNetNuke.UI.UserControls;
using DotNetNuke.UI.WebControls;
using DotNetNuke.Web.UI.WebControls;

namespace R7.Documents
{
	public partial class SettingsDocuments
	{
		protected System.Web.UI.WebControls.CheckBox chkShowTitleLink;
		protected System.Web.UI.WebControls.CheckBox chkUseCategoriesList;
		protected System.Web.UI.WebControls.DropDownList lstSortFields;
		private System.Web.UI.WebControls.DataGrid withEventsField_grdSortColumns;

		protected System.Web.UI.WebControls.DataGrid grdSortColumns
		{
			get { return withEventsField_grdSortColumns; }
			set
			{
				if (withEventsField_grdSortColumns != null)
				{
					withEventsField_grdSortColumns.ItemCreated -= grdSortColumns_ItemCreated;
					withEventsField_grdSortColumns.DeleteCommand -= grdSortColumns_DeleteCommand;
				}
				withEventsField_grdSortColumns = value;
				if (withEventsField_grdSortColumns != null)
				{
					withEventsField_grdSortColumns.ItemCreated += grdSortColumns_ItemCreated;
					withEventsField_grdSortColumns.DeleteCommand += grdSortColumns_DeleteCommand;
				}
			}
		}

		private System.Web.UI.WebControls.LinkButton withEventsField_lnkAddSortColumn;

		protected System.Web.UI.WebControls.LinkButton lnkAddSortColumn
		{
			get { return withEventsField_lnkAddSortColumn; }
			set
			{
				if (withEventsField_lnkAddSortColumn != null)
				{
					withEventsField_lnkAddSortColumn.Click -= lnkAddSortColumn_Click;
				}
				withEventsField_lnkAddSortColumn = value;
				if (withEventsField_lnkAddSortColumn != null)
				{
					withEventsField_lnkAddSortColumn.Click += lnkAddSortColumn_Click;
				}
			}
		}

		private System.Web.UI.WebControls.DataGrid withEventsField_grdDisplayColumns;

		protected System.Web.UI.WebControls.DataGrid grdDisplayColumns
		{
			get { return withEventsField_grdDisplayColumns; }
			set
			{
				if (withEventsField_grdDisplayColumns != null)
				{
					withEventsField_grdDisplayColumns.ItemCreated -= grdDisplayColumns_ItemCreated;
					withEventsField_grdDisplayColumns.ItemCommand -= grdDisplayColumns_ItemCommand;
				}
				withEventsField_grdDisplayColumns = value;
				if (withEventsField_grdDisplayColumns != null)
				{
					withEventsField_grdDisplayColumns.ItemCreated += grdDisplayColumns_ItemCreated;
					withEventsField_grdDisplayColumns.ItemCommand += grdDisplayColumns_ItemCommand;
				}
			}
		}

		protected System.Web.UI.WebControls.DropDownList cboCategoriesList;
		protected System.Web.UI.WebControls.DropDownList cboDefaultFolder;
		protected System.Web.UI.WebControls.HyperLink lnkEditLists;
		protected System.Web.UI.WebControls.Label lstNoListsAvailable;
		protected System.Web.UI.WebControls.DropDownList cboSortOrderDirection;
		protected System.Web.UI.WebControls.CheckBox chkAllowUserSort;
		protected System.Web.UI.WebControls.Label lblCannotEditLists;
	}
}

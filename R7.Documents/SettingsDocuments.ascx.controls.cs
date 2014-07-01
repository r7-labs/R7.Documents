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
		protected System.Web.UI.WebControls.DataGrid grdSortColumns;
		protected System.Web.UI.WebControls.LinkButton lnkAddSortColumn;
		protected System.Web.UI.WebControls.DataGrid grdDisplayColumns;
		protected System.Web.UI.WebControls.DropDownList cboCategoriesList;
		protected System.Web.UI.WebControls.DropDownList cboDefaultFolder;
		protected System.Web.UI.WebControls.HyperLink lnkEditLists;
		protected System.Web.UI.WebControls.Label lstNoListsAvailable;
		protected System.Web.UI.WebControls.DropDownList cboSortOrderDirection;
		protected System.Web.UI.WebControls.CheckBox chkAllowUserSort;
		protected System.Web.UI.WebControls.Label lblCannotEditLists;
	}
}

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
		protected CheckBox chkShowTitleLink;
        protected DropDownList comboGridStyle;
		protected CheckBox chkUseCategoriesList;
		protected DnnComboBox comboSortFields;
		protected DataGrid grdSortColumns;
		protected LinkButton lnkAddSortColumn;
		protected DataGrid grdDisplayColumns;
		protected DropDownList cboCategoriesList;
		protected DnnFolderDropDownList folderDefaultFolder;
		protected HyperLink lnkEditLists;
		protected Label lstNoListsAvailable;
		protected DnnComboBox comboSortOrderDirection;
		protected CheckBox chkAllowUserSort;
		protected Label lblCannotEditLists;
	}
}

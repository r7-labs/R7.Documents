using System.Web.UI.WebControls;
using DotNetNuke.Web.UI.WebControls;

namespace R7.Documents
{
    public partial class SettingsDocuments
	{
		protected CheckBox chkShowTitleAsLink;
        protected DropDownList ddlGridStyle;
		protected CheckBox chkUseCategoriesList;
		protected DropDownList ddlSortFields;
		protected DataGrid grdSortColumns;
		protected LinkButton lnkAddSortColumn;
		protected DataGrid grdDisplayColumns;
		protected DropDownList ddlCategoriesList;
		protected DnnFolderDropDownList ddlDefaultFolder;
		protected HyperLink lnkEditLists;
		protected Label lblNoListsAvailable;
		protected DropDownList ddlSortOrderDirection;
		protected CheckBox chkAllowUserSort;
		protected Label lblCannotEditLists;
        protected TextBox txtDateTimeFormat;
        protected TextBox txtFileFilter;
        protected CustomValidator valFileFilter;
        protected TextBox txtFilenameToTitleRules;
        protected CustomValidator valFilenameToTitleRules;
	}
}

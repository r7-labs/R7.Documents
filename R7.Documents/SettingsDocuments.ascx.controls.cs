using System.Web.UI.WebControls;
using DotNetNuke.Web.UI.WebControls;

namespace R7.Documents
{
    public partial class SettingsDocuments
	{
		protected CheckBox chkShowTitleLink;
        protected DropDownList comboGridStyle;
		protected CheckBox chkUseCategoriesList;
		protected DropDownList comboSortFields;
		protected DataGrid grdSortColumns;
		protected LinkButton lnkAddSortColumn;
		protected DataGrid grdDisplayColumns;
		protected DropDownList cboCategoriesList;
		protected DnnFolderDropDownList folderDefaultFolder;
		protected HyperLink lnkEditLists;
		protected Label lstNoListsAvailable;
		protected DropDownList comboSortOrderDirection;
		protected CheckBox chkAllowUserSort;
		protected Label lblCannotEditLists;
        protected TextBox textDateTimeFormat;
        protected TextBox textFileFilter;
        protected CustomValidator valFileFilter;
        protected TextBox textFilenameToTitleRules;
        protected CustomValidator valFilenameToTitleRules;
	}
}

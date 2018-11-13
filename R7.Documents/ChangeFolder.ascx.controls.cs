using System.Web.UI.WebControls;
using DotNetNuke.Web.UI.WebControls;

namespace R7.Documents
{
    public partial class ChangeFolder
	{
		protected DnnFolderDropDownList ddlFolder;
        protected CheckBox chkPublishUpdated;
        protected CheckBox chkDeleteOldFiles;
        protected CheckBox chkUnpublishSkipped;
		protected CheckBox chkUpdateDefaultFolder;
		protected HyperLink lnkCancel;
	}
}

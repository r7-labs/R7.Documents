using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DotNetNuke.UI.UserControls;
using DotNetNuke.UI.WebControls;
using DotNetNuke.Web.UI.WebControls;

namespace R7.Documents
{
	public partial class ChangeFolder
	{
		protected Label labelInfo;
		protected LabelControl labelFolder;
		protected DnnFolderDropDownList ddlFolder;
		protected LabelControl labelUnpublishSkipped;
		protected CheckBox checkUnpublishSkipped;
        protected LabelControl labelPublishUpdated;
        protected CheckBox checkPublishUpdated;
		protected LabelControl labelUpdateDefaultFolder;
		protected CheckBox checkUpdateDefaultFolder;
		protected LinkButton cmdUpdate;
		protected HyperLink linkCancel;
	}
}

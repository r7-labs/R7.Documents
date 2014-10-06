
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DotNetNuke.UI.UserControls;
using DotNetNuke.UI.WebControls;
using DotNetNuke.Web.UI.WebControls;

namespace R7.Documents
{
	public partial class ImportDocuments
	{
		protected LabelControl labelModule;
		protected DnnComboBox comboModule;
		protected Panel panelDocuments;
		protected LabelControl labelDocuments;
		protected CheckBoxList listDocuments;
		protected LinkButton buttonImport;
		protected HyperLink linkCancel;
	}
}

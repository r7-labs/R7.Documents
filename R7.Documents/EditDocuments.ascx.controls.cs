using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DotNetNuke.UI.UserControls;
using DotNetNuke.UI.WebControls;
using DotNetNuke.Web.UI.WebControls;

namespace R7.Documents
{
	public partial class EditDocuments
	{
		protected LabelControl plName;
		protected System.Web.UI.WebControls.TextBox txtName;
		protected System.Web.UI.WebControls.RequiredFieldValidator valName;
		protected LabelControl plUrl;
		protected UrlControl ctlUrl;
		protected LabelControl plCategory;
		protected System.Web.UI.WebControls.TextBox txtCategory;
		protected System.Web.UI.WebControls.TextBox txtDescription;
		protected System.Web.UI.WebControls.CheckBox chkForceDownload;
		protected System.Web.UI.WebControls.DropDownList lstCategory;
		protected System.Web.UI.WebControls.DropDownList lstOwner;
		//tasks
		protected System.Web.UI.WebControls.LinkButton cmdUpdate;
		protected System.Web.UI.WebControls.HyperLink linkCancel;
		protected System.Web.UI.WebControls.LinkButton cmdDelete;
		protected System.Web.UI.WebControls.LinkButton cmdUpdateOverride;
		//footer
		protected DotNetNuke.UI.UserControls.ModuleAuditControl ctlAudit;
		
		protected System.Web.UI.WebControls.TextBox txtSortIndex;
		protected System.Web.UI.WebControls.RangeValidator valSortIndex;
		protected System.Web.UI.WebControls.Label lblOwner;
		protected System.Web.UI.WebControls.LinkButton lnkChange;
		protected Panel panelUrlTracking;
		protected LabelControl labelUrlTracking;
		protected URLTrackingControl ctlUrlTracking;
		protected LabelControl labelIsPublished;
		protected CheckBox checkIsPublished;
        protected CheckBox checkDeleteWithResource;
		protected LabelControl labelCreatedDate;
		protected DnnDateTimePicker pickerCreatedDate;
		protected LabelControl labelLastModifiedDate;
		protected DnnDateTimePicker pickerLastModifiedDate;
        protected Panel panelDelete;
	}
}

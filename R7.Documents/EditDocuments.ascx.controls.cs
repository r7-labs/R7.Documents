using System.Web.UI.WebControls;
using DotNetNuke.UI.UserControls;
using DotNetNuke.Web.UI.WebControls;

namespace R7.Documents
{
    public partial class EditDocuments
	{
		protected TextBox txtName;
		protected RequiredFieldValidator valName;
		protected DnnUrlControl ctlUrl;
		protected TextBox txtCategory;
		protected TextBox txtDescription;
		protected CheckBox chkForceDownload;
        protected TextBox textLinkAttributes;
		protected DropDownList lstCategory;
		protected DropDownList lstOwner;
		protected LinkButton cmdUpdate;
		protected HyperLink linkCancel;
		protected LinkButton cmdDelete;
        protected LinkButton buttonDeleteWithResource;
		protected LinkButton cmdUpdateOverride;
		protected ModuleAuditControl ctlAudit;
		protected TextBox txtSortIndex;
		protected RangeValidator valSortIndex;
		protected Label lblOwner;
		protected LinkButton lnkChange;
		protected URLTrackingControl ctlUrlTracking;
		protected DnnDateTimePicker pickerCreatedDate;
		protected DnnDateTimePicker pickerLastModifiedDate;
        protected DnnDateTimePicker datetimeStartDate;
        protected DnnDateTimePicker datetimeEndDate;
        protected MultiView multiView;
        protected LinkButton linkAddMore;
        protected HyperLink linkClose;
	}
}

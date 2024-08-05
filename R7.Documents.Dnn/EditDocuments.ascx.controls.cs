using System.Web.UI.WebControls;
using DotNetNuke.UI.UserControls;
using DotNetNuke.Web.UI.WebControls;

namespace R7.Documents
{
    public partial class EditDocuments
	{
		protected TextBox txtTitle;
		protected RequiredFieldValidator valName;
		protected DnnUrlControl ctlUrl;
		protected TextBox txtCategory;
		protected TextBox txtDescription;
		protected CheckBox chkForceDownload;
        protected TextBox txtLinkAttributes;
		protected DropDownList lstCategory;
		protected DropDownList lstOwner;
        protected Label txtOwner;
        protected LinkButton btnAdd;
        protected LinkButton btnUpdate;
		protected HyperLink lnkCancel;
		protected LinkButton btnDelete;
        protected LinkButton btnDeleteWithFile;
		protected LinkButton cmdUpdateOverride;
		protected ModuleAuditControl ctlAudit;
		protected TextBox txtSortIndex;
		protected RangeValidator valSortIndex;
		protected LinkButton btnChangeOwner;
		protected URLTrackingControl ctlUrlTracking;
		protected TextBox dtCreatedDate;
		protected TextBox dtLastModifiedDate;
        protected TextBox dtStartDate;
        protected TextBox dtEndDate;
        protected MultiView mvEditDocument;
        protected LinkButton btnEdit;
        protected HyperLink lnkClose;
        protected CheckBox chkIsFeatured;
	}
}

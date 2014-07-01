
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
		private System.Web.UI.WebControls.LinkButton withEventsField_cmdUpdate;

		protected System.Web.UI.WebControls.LinkButton cmdUpdate
		{
			get { return withEventsField_cmdUpdate; }
			set
			{
				if (withEventsField_cmdUpdate != null)
				{
					withEventsField_cmdUpdate.Click -= cmdUpdate_Click;
				}
				withEventsField_cmdUpdate = value;
				if (withEventsField_cmdUpdate != null)
				{
					withEventsField_cmdUpdate.Click += cmdUpdate_Click;
				}
			}
		}

		private System.Web.UI.WebControls.LinkButton withEventsField_cmdCancel;

		protected System.Web.UI.WebControls.LinkButton cmdCancel
		{
			get { return withEventsField_cmdCancel; }
			set
			{
				if (withEventsField_cmdCancel != null)
				{
					withEventsField_cmdCancel.Click -= cmdCancel_Click;
				}
				withEventsField_cmdCancel = value;
				if (withEventsField_cmdCancel != null)
				{
					withEventsField_cmdCancel.Click += cmdCancel_Click;
				}
			}
		}

		private System.Web.UI.WebControls.LinkButton withEventsField_cmdDelete;

		protected System.Web.UI.WebControls.LinkButton cmdDelete
		{
			get { return withEventsField_cmdDelete; }
			set
			{
				if (withEventsField_cmdDelete != null)
				{
					withEventsField_cmdDelete.Click -= cmdDelete_Click;
				}
				withEventsField_cmdDelete = value;
				if (withEventsField_cmdDelete != null)
				{
					withEventsField_cmdDelete.Click += cmdDelete_Click;
				}
			}
		}

		private System.Web.UI.WebControls.LinkButton withEventsField_cmdUpdateOverride;

		protected System.Web.UI.WebControls.LinkButton cmdUpdateOverride
		{
			get { return withEventsField_cmdUpdateOverride; }
			set
			{
				if (withEventsField_cmdUpdateOverride != null)
				{
					withEventsField_cmdUpdateOverride.Click -= cmdUpdateOverride_Click;
				}
				withEventsField_cmdUpdateOverride = value;
				if (withEventsField_cmdUpdateOverride != null)
				{
					withEventsField_cmdUpdateOverride.Click += cmdUpdateOverride_Click;
				}
			}

		}
		//footer
		protected DotNetNuke.UI.UserControls.ModuleAuditControl ctlAudit;
		protected URLTrackingControl ctlTracking;
		protected System.Web.UI.WebControls.TextBox txtSortIndex;
		protected System.Web.UI.WebControls.RangeValidator valSortIndex;
		protected System.Web.UI.WebControls.Label lblOwner;
		private System.Web.UI.WebControls.LinkButton withEventsField_lnkChange;

		protected System.Web.UI.WebControls.LinkButton lnkChange
		{
			get { return withEventsField_lnkChange; }
			set
			{
				if (withEventsField_lnkChange != null)
				{
					withEventsField_lnkChange.Click -= lnkChange_Click;
				}
				withEventsField_lnkChange = value;
				if (withEventsField_lnkChange != null)
				{
					withEventsField_lnkChange.Click += lnkChange_Click;
				}
			}
		}

		protected System.Web.UI.WebControls.Label lblAudit;
	}
}

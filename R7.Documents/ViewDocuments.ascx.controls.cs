using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Linq;

using DotNetNuke.UI.UserControls;
using DotNetNuke.UI.WebControls;
using DotNetNuke.Web.UI.WebControls;

namespace R7.Documents
{
	public partial class ViewDocuments
	{
		private System.Web.UI.WebControls.DataGrid withEventsField_grdDocuments;

		protected System.Web.UI.WebControls.DataGrid grdDocuments {
			get { return withEventsField_grdDocuments; }
			set {
				if (withEventsField_grdDocuments != null) {
					withEventsField_grdDocuments.SortCommand -= grdDocuments_SortCommand;
					withEventsField_grdDocuments.ItemCreated -= grdDocuments_ItemCreated;
				}
				withEventsField_grdDocuments = value;
				if (withEventsField_grdDocuments != null) {
					withEventsField_grdDocuments.SortCommand += grdDocuments_SortCommand;
					withEventsField_grdDocuments.ItemCreated += grdDocuments_ItemCreated;
				}
			}
		}

		protected System.Web.UI.WebControls.Button button1;
	}
}

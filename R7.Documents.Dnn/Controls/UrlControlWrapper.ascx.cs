using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Web.UI.WebControls;

namespace R7.Documents.Controls
{
    public class UrlControlWrapper : UserControl
    {
        #region Controls

        protected DnnUrlControl ctlUrl;
        protected CheckBox chkNone;
        protected TextBox txtUrl;

        #endregion

        public string Url
        {
            get => GetUrl ();
            set => SetUrl (value);
        }

        string GetUrl ()
        {
            if (chkNone.Checked) {
                return string.Empty;
            }

            if (!string.IsNullOrEmpty (txtUrl.Text)) {
                return txtUrl.Text;
            }

            return ctlUrl.Url;
        }

        void SetUrl(string url)
        {
            if (string.IsNullOrEmpty (url)) {
                chkNone.Checked = true;
                ctlUrl.Url = string.Empty;
                ctlUrl.UrlType = "N";
                txtUrl.Text = string.Empty;
                return;
            }

            var urlType = Globals.GetURLType (url);
            if (urlType == TabType.File || urlType == TabType.Tab) {
                ctlUrl.Url = url;
                txtUrl.Text = string.Empty;
                chkNone.Checked = false;
                return;
            }

            ctlUrl.Url = string.Empty;
            ctlUrl.UrlType = "N";
            txtUrl.Text = url;
            chkNone.Checked = false;
        }

        public bool NewWindow {
            get => ctlUrl.NewWindow;
            set => ctlUrl.NewWindow = value;
        }

        public bool Log => ctlUrl.Log;

        public bool Track => ctlUrl.Track;

        public string UrlType => ctlUrl.UrlType;

        public DnnUrlControl BaseUrlControl => ctlUrl;
    }
}


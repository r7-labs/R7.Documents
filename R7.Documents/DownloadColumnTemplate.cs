//
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
// by DotNetNuke Corporation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

namespace R7.Documents
{

	public class DownloadColumnTemplate : System.Web.UI.ITemplate
	{

		//Shared itemcount As Integer = 0
		private System.Web.UI.WebControls.ListItemType mobjTemplateType;
		private string mstrID;

		private string mstrCaption;

		public DownloadColumnTemplate (string ID, string Caption, System.Web.UI.WebControls.ListItemType Type)
		{
			mobjTemplateType = Type;
			mstrID = ID;
			mstrCaption = Caption;
			if (mstrCaption == string.Empty)
			{
				mstrCaption = "Download";
			}
		}

		public void InstantiateIn (System.Web.UI.Control container)
		{
			System.Web.UI.WebControls.HyperLink objButton = default(System.Web.UI.WebControls.HyperLink);

			switch (mobjTemplateType)
			{
				case System.Web.UI.WebControls.ListItemType.Item:
				case System.Web.UI.WebControls.ListItemType.AlternatingItem:
				case System.Web.UI.WebControls.ListItemType.SelectedItem:
					objButton = new System.Web.UI.WebControls.HyperLink ();
					objButton.Text = mstrCaption;
					objButton.ID = mstrID;

					container.Controls.Add (objButton);
					break;
			}
			//itemcount += 1
		}
	}
}

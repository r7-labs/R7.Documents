//
// UrlHistory.cs
//
// Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
// Copyright (c) 2017 Roman M. Yagodin
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.FileSystem;

namespace R7.Documents.Components
{
    // TODO: Move to the base library
    public class UrlHistory
    {
        string _variableName;

        HttpSessionState _session;

        const string _separator = ";";

        static char [] _separators = { char.Parse (_separator) };
 
        public UrlHistory (HttpSessionState session, string variableName = "r7_Share_UrlHistory")
        {
            _session = session;
            _variableName = variableName;
        }

        public void AddUrl (string url)
        {
            if (string.IsNullOrEmpty (url)) {
                return;
            }

            var sessionObject = _session [_variableName];
            if (sessionObject != null) {
                if (_session.Mode == SessionStateMode.InProc) {
                    var urlList = (IList<string>) sessionObject;
                    if (!urlList.Contains (url)) {
                        urlList.Insert (0, url);
                        _session [_variableName] = urlList;
                    }
                }
                else {
                    var urls = (string) sessionObject;
                    if (!urls.Contains (_separator + url + _separator)) {
                        _session [_variableName] = _separator + url + urls;
                    }
                }
            }
            else {
                if (_session.Mode == SessionStateMode.InProc) {
                    _session [_variableName] = new List<string> { url };
                }
                else {
                    _session [_variableName] = _separator + url + _separator;
                }
            }
        }

        public IList<ListItem> GetBindableUrls ()
        {
            var portalId = PortalSettings.Current.PortalId;
            var urlList = new List<ListItem> ();
            var sessionObject = _session [_variableName];
            if (sessionObject != null) {

                IEnumerable<string> urls;
                if (_session.Mode == SessionStateMode.InProc) {
                    urls = (IList<string>) sessionObject;
                }
                else {
                    urls = ((string) sessionObject).Split (_separators, StringSplitOptions.RemoveEmptyEntries);
                }

                foreach (var url in urls) {
                    urlList.Add (new ListItem (GetUrlName (url, portalId), url));
                }
            }

            return urlList;
        }

        protected virtual string GetUrlName (string url, int portalId)
        {
            switch (Globals.GetURLType (url)) {
                case TabType.File:
                    var file = FileManager.Instance.GetFile (Utils.GetResourceId (url));
                    if (file != null) {
                        return file.Folder + file.FileName;
                    }
                    break;

                case TabType.Tab:
                    var tab = TabController.Instance.GetTab (int.Parse (url), portalId);
                    if (tab != null) {
                        return tab.TabName;
                    }
                    break;

                case TabType.Member:
                    var user = UserController.Instance.GetUserById (portalId, Utils.GetResourceId (url));
                    if (user != null) {
                        return user.DisplayName;
                    }
                    break;
            }

            return url;
        }
    }
}

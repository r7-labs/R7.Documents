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
using System.Linq;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.FileSystem;

namespace R7.Documents.Components
{
    public abstract class UrlHistoryBackend
    {
        public abstract void Init (HttpSessionState session, string variableName);

        public abstract void StoreUrl (string url);

        public abstract IEnumerable<string> GetUrls ();
    }

    /// <summary>
    /// UrlHistory backend for InProc sessions
    /// </summary>
    public class UrlHistoryInProcBackend : UrlHistoryBackend
    {
        HttpSessionState _session;

        string _variableName;

        public override void Init (HttpSessionState session, string variableName)
        {
            _session = session;
            _variableName = variableName;
        }

        public override void StoreUrl (string url)
        {
            var sessionObject = _session [_variableName];
            if (sessionObject != null) {
                var urlList = (IList<string>) sessionObject;
                var index = urlList.IndexOf (url);
                if (index >= 0) {
                    urlList.RemoveAt (index);
                }
                urlList.Insert (0, url);
                _session [_variableName] = urlList;
            }
            else {
                _session [_variableName] = new List<string> { url };
            }
        }

        public override IEnumerable<string> GetUrls ()
        {
            var sessionObject = _session [_variableName];
            if (sessionObject != null) {
                return (IEnumerable<string>) sessionObject;
            }

            return Enumerable.Empty<string> ();
        }
    }

    /// <summary>
    /// UrlHistory backend for non-InProc sessions
    /// </summary>
    public class UrlHistoryDefaultBackend : UrlHistoryBackend
    {
        HttpSessionState _session;

        string _variableName;

        const string _separator = ";";

        static readonly char [] _separators = { char.Parse (_separator) };

        public override void Init (HttpSessionState session, string variableName)
        {
            _session = session;
            _variableName = variableName;
        }

        public override void StoreUrl (string url)
        {
            var sessionObject = _session [_variableName];
            if (sessionObject != null) {
                var urls = (string) sessionObject;
                var quotedUrl = _separator + url + _separator;
                var index = urls.IndexOf (quotedUrl, StringComparison.InvariantCulture);
                if (index >= 0) {
                    urls = urls.Remove (index, quotedUrl.Length - 1);
                }
                _session [_variableName] = _separator + url + urls;
            }
            else {
                _session [_variableName] = _separator + url + _separator;
            }
        }

        public override IEnumerable<string> GetUrls ()
        {
            var sessionObject = _session [_variableName];
            if (sessionObject != null) {
                return ((string) sessionObject).Split (_separators, StringSplitOptions.RemoveEmptyEntries);
            }

            return Enumerable.Empty<string> ();
        }
    }

    // TODO: Move to the base library
    public class UrlHistory
    {
        protected readonly UrlHistoryBackend Backend;

        public UrlHistory (HttpSessionState session, string variableName = "r7_Shared_UrlHistory")
        {
            if (session.Mode == SessionStateMode.InProc) {
                Backend = new UrlHistoryInProcBackend ();
            }
            else {
                Backend = new UrlHistoryDefaultBackend ();
            }

            Backend.Init (session, variableName);
        }

        public void StoreUrl (string url)
        {
            if (string.IsNullOrEmpty (url)) {
                return;
            }

            Backend.StoreUrl (url);
        }

        public IList<ListItem> GetBindableUrls ()
        {
            var portalId = PortalSettings.Current.PortalId;
            var urlList = new List<ListItem> ();
            foreach (var url in Backend.GetUrls ()) {
                urlList.Add (new ListItem (GetUrlName (url, portalId), url));
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

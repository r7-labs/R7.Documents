//
// DocumentsDataProvider.cs
//
// Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
// Copyright (c) 2016 Roman M. Yagodin
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
using DotNetNuke.R7;
using DotNetNuke.Data;
using System.Collections.Generic;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Common;
using DotNetNuke.Services.FileSystem;
using System.Linq;

namespace R7.Documents.Data
{
    public class DocumentsDataProvider: Dal2DataProvider
    {
        #region Singleton implementation

        static readonly Lazy<DocumentsDataProvider> instance = new Lazy<DocumentsDataProvider> ();

        public static DocumentsDataProvider Instance
        {
            get { return instance.Value; }
        }

        #endregion

        public DocumentInfo GetDocument (int ItemId, int ModuleId)
        {
            DocumentInfo document;

            using (var ctx = DataContext.Instance ())
            {
                document = ctx.ExecuteSingleOrDefault<DocumentInfo> (
                    System.Data.CommandType.StoredProcedure, "Documents_GetDocument", ItemId, ModuleId);
            }

            return document;
        }

        public IEnumerable<DocumentInfo> GetDocuments (int ModuleId, int PortalId)
        {
            IEnumerable<DocumentInfo> documents;

            using (var ctx = DataContext.Instance ())
            {
                documents = ctx.ExecuteQuery<DocumentInfo> (
                    System.Data.CommandType.StoredProcedure, "Documents_GetDocuments", ModuleId, PortalId);
            }

            return documents;
        }

        /// <summary>
        /// Gets documents from DNN Documents module
        /// </summary>
        /// <returns>The DNN documents.</returns>
        /// <param name="ModuleId">Module identifier.</param>
        /// <param name="PortalId">Portal identifier.</param>
        public IEnumerable<DocumentInfo> GetDNNDocuments (int ModuleId, int PortalId)
        {
            IEnumerable<DocumentInfo> documents;

            using (var ctx = DataContext.Instance ())
            {
                documents = ctx.ExecuteQuery<DocumentInfo> (
                    System.Data.CommandType.StoredProcedure, "GetDocuments", ModuleId, PortalId);
            }

            return documents;
        }

        public DocumentInfo GetDNNDocument (int ItemId, int ModuleId)
        {
            DocumentInfo document;

            using (var ctx = DataContext.Instance ())
            {
                document = ctx.ExecuteSingleOrDefault<DocumentInfo> (
                    System.Data.CommandType.StoredProcedure, "GetDocument", ItemId, ModuleId);
            }

            return document;
        }

        /// <summary>
        /// Deletes the resource, accociated with the document (only files and URLs are supported).
        /// </summary>
        /// <param name="document">Document.</param>
        /// <param name="portalId">Portal identifier.</param>
        public int DeleteDocumentResource (DocumentInfo document, int portalId)
        {
            // count resource references
            var count = GetObjects<DocumentInfo> ("WHERE [ItemID] <> @0 AND [Url] = @1", document.ItemId, document.Url).Count ();

            // if no other document references it
            if (count == 0)
            {
                switch (Globals.GetURLType (document.Url))
                {
                    // delete file
                    case TabType.File:
                        var file = FileManager.Instance.GetFile (Utils.GetResourceId (document.Url));
                        if (file != null)
                            FileManager.Instance.DeleteFile (file);
                        break;

                        // delete URL
                    case TabType.Url:
                        new UrlController ().DeleteUrl (portalId, document.Url);
                        break;
                }
            }

            return count;
        }

        public void DeleteDocumentUrl (string oldUrl, int PortalId, int ModuleId)
        {
            // NOTE: we shouldn't delete URL itself as is can be used in other modules
            // DataProvider.Instance().DeleteUrl (PortalId, document.Url);

            // delete URL tracking data
            DataProvider.Instance ().DeleteUrlTracking (PortalId, oldUrl, ModuleId);
        }

        public void UpdateDocumentUrl (DocumentInfo document, string oldUrl, int PortalId, int ModuleId)
        {
            if (document.Url != oldUrl)
            {
                var ctrlUrl = new UrlController ();

                // get tracking data for the old URL
                var url = ctrlUrl.GetUrlTracking (PortalId, oldUrl, ModuleId);

                // delete old URL tracking data
                DataProvider.Instance ().DeleteUrlTracking (PortalId, oldUrl, ModuleId);

                // create new URL tracking data
                ctrlUrl.UpdateUrl (PortalId, document.Url, url.UrlType, url.LogActivity, url.TrackClicks, ModuleId, url.NewWindow);
            }
        }
    }
}


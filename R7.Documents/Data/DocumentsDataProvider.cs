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
using System.Linq;
using DotNetNuke.Data;
using System.Collections.Generic;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Common;
using DotNetNuke.Services.FileSystem;
using R7.DotNetNuke.Extensions.Data;
using R7.Documents.Models;

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

        public DocumentInfo GetDocument (int itemId, int moduleId)
        {
            DocumentInfo document;

            using (var ctx = DataContext.Instance ()) {
                document = ctx.ExecuteSingleOrDefault<DocumentInfo> (
                    System.Data.CommandType.StoredProcedure, "{objectQualifier}Documents_GetDocument", itemId, moduleId);
            }

            return document;
        }

        public IEnumerable<DocumentInfo> GetDocuments (int moduleId, int portalId)
        {
            IEnumerable<DocumentInfo> documents;

            using (var ctx = DataContext.Instance ()) {
                documents = ctx.ExecuteQuery<DocumentInfo> (
                    System.Data.CommandType.StoredProcedure, "{objectQualifier}Documents_GetDocuments", moduleId, portalId);
            }

            return documents;
        }

        /// <summary>
        /// Gets documents from DNN Documents module
        /// </summary>
        /// <returns>The DNN documents.</returns>
        /// <param name="moduleId">Module identifier.</param>
        /// <param name="portalId">Portal identifier.</param>
        public IEnumerable<DocumentInfo> GetDNNDocuments (int moduleId, int portalId)
        {
            IEnumerable<DocumentInfo> documents;

            using (var ctx = DataContext.Instance ()) {
                documents = ctx.ExecuteQuery<DocumentInfo> (
                    System.Data.CommandType.StoredProcedure, "{objectQualifier}GetDocuments", moduleId, portalId);
            }

            return documents;
        }

        public DocumentInfo GetDNNDocument (int itemId, int moduleId)
        {
            DocumentInfo document;

            using (var ctx = DataContext.Instance ()) {
                document = ctx.ExecuteSingleOrDefault<DocumentInfo> (
                    System.Data.CommandType.StoredProcedure, "{objectQualifier}GetDocument", itemId, moduleId);
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
            if (count == 0) {
                switch (Globals.GetURLType (document.Url)) {
                // delete file
                    case TabType.File:
                        var file = FileManager.Instance.GetFile (Utils.GetResourceId (document.Url));
                        if (file != null) {
                            FileManager.Instance.DeleteFile (file);
                        }
                        break;

                // delete URL
                    case TabType.Url:
                        new UrlController ().DeleteUrl (portalId, document.Url);
                        break;
                }
            }

            return count;
        }

        public void DeleteDocumentUrl (string oldUrl, int portalId, int moduleId)
        {
            // NOTE: we shouldn't delete URL itself as is can be used in other modules
            // DataProvider.Instance().DeleteUrl (PortalId, document.Url);

            // delete URL tracking data
            DataProvider.Instance ().DeleteUrlTracking (portalId, oldUrl, moduleId);
        }

        public void UpdateDocumentUrl (DocumentInfo document, string oldUrl, int portalId, int moduleId)
        {
            if (document.Url != oldUrl) {
                var ctrlUrl = new UrlController ();

                // get tracking data for the old URL
                var url = ctrlUrl.GetUrlTracking (portalId, oldUrl, moduleId);

                // delete old URL tracking data
                DataProvider.Instance ().DeleteUrlTracking (portalId, oldUrl, moduleId);

                // create new URL tracking data
                ctrlUrl.UpdateUrl (
                    portalId,
                    document.Url,
                    url.UrlType,
                    url.LogActivity,
                    url.TrackClicks,
                    moduleId,
                    url.NewWindow);
            }
        }
    }
}


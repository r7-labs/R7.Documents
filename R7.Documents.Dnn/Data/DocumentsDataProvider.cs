using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.FileSystem;
using R7.Dnn.Extensions.Data;
using R7.Dnn.Extensions.Urls;
using R7.Documents.Models;
using R7.University.Components;

namespace R7.Documents.Data
{
    public class DocumentsDataProvider: Dal2DataProvider
    {
        #region Singleton implementation

        static readonly Lazy<DocumentsDataProvider> instance = new Lazy<DocumentsDataProvider> ();

        public static DocumentsDataProvider Instance {
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
        /// Deletes the file, accociated with the document.
        /// </summary>
        /// <param name="document">Document.</param>
        /// <param name="portalId">Portal identifier.</param>
        public int DeleteDocumentFile (DocumentInfo document, int portalId)
        {
            // count resource references
            var count = GetObjects<DocumentInfo> ("WHERE [ItemID] <> @0 AND [Url] = @1", document.ItemId, document.Url).Count ();

            // if no other document references it
            if (count == 0) {
                if (Globals.GetURLType (document.Url) == TabType.File) {
                    var file = FileManager.Instance.GetFile (UrlHelper.GetResourceId (document.Url).Value);
                    if (file != null) {
                        FileManager.Instance.DeleteFile (file);
                    }
                }
            }

            return count;
        }

        public void DeleteDocumentUrl (string oldUrl, int portalId, int moduleId)
        {
            // TODO: shouldn't delete URL itself as is can be used in other modules?
            // DataProvider.Instance().DeleteUrl (PortalId, document.Url);

            // delete URL tracking data
            DataProvider.Instance ().DeleteUrlTracking (portalId, oldUrl, moduleId);
        }

        public void DeleteDocument (int documentId, bool withFile, int portalId, int moduleId)
        {
            var document = GetDocument (documentId, moduleId);
            if (document != null) {
                Delete (document);
                DeleteDocumentUrl (document.Url, portalId, moduleId);
                if (withFile) {
                    DeleteDocumentFile (document, portalId);
                }
            }
        }

        public void UpdateDocumentUrl (DocumentInfo document, string oldUrl, int portalId, int moduleId)
        {
            if (document.Url != oldUrl) {
                var ctrlUrl = new UrlController ();

                // get tracking data for the old URL
                var urlTracking = ctrlUrl.GetUrlTracking (portalId, oldUrl, moduleId);
                if (urlTracking != null) {
                    // delete old URL tracking data
                    DataProvider.Instance ().DeleteUrlTracking (portalId, oldUrl, moduleId);

                    // create new URL tracking data
                    ctrlUrl.UpdateUrl (
                        portalId,
                        document.Url,
                        urlTracking.UrlType,
                        urlTracking.LogActivity,
                        urlTracking.TrackClicks,
                        moduleId,
                        urlTracking.NewWindow);
                }
            }
        }

        public IEnumerable<DocumentInfo> CreateDocumentsFromFolder (int folderId, int portalId, int moduleId, string fileFilter, IEnumerable<string []> rules)
        {
            var folder = FolderManager.Instance.GetFolder (folderId);
            if (folder == null) {
                return Enumerable.Empty<DocumentInfo> ();
            }

            var urlController = new UrlController ();
            var files = FolderManager.Instance.GetFiles (folder);
            var documents = files.Where (f => Regex.IsMatch (f.FileName, fileFilter))
                                 .Select (f => new DocumentInfo {
                ItemId = 0,
                Url = "FileID=" + f.FileId,
                Title = FilenameToTitle (f.FileName, rules),
                Size = f.Size,
                CreatedByUserId = f.CreatedByUserID,
                CreatedDate = f.CreatedOnDate,
                ModifiedByUserId = f.LastModifiedByUserID,
                ModifiedDate = f.LastModifiedOnDate,
                OwnedByUserId = f.CreatedByUserID,
                Clicks = urlController.GetUrlTracking (portalId, "FileID=" + f.FileId, moduleId)?.Clicks ?? 0,
                ModuleId = moduleId,
                TrackClicks = true,
                NewWindow = DocumentsConfig.Instance.NewWindow
            });

            var urlCtrl = new UrlController ();
            foreach (var document in documents) {
                urlCtrl.UpdateUrl (portalId, document.Url, "F", false, document.TrackClicks, document.ModuleId, document.NewWindow);
            }

            return documents;
        }

        string FilenameToTitle (string filename, IEnumerable<string []> rules)
        {
            if (rules != null) {
                foreach (var rule in rules) {
                    filename = Regex.Replace (filename, rule [0], rule [1]);
                }
            }

            return filename;
        }

    }
}


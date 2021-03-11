using System;
using System.Collections.Generic;
using R7.Documents.Data;

namespace R7.Documents.Commands
{
    public class DocumentBulkCommands
    {
        public void Duplicate (IEnumerable<int> documentIds, int moduleId, string copySuffix)
        {
            foreach (var documentId in documentIds) {
                var document = DocumentsDataProvider.Instance.GetDocument (documentId, moduleId);
                if (document != null) {
                    document.ItemId = 0;
                    document.Title += copySuffix;
                    DocumentsDataProvider.Instance.Add (document);
                }
            }
        }

        public void Delete (IEnumerable<int> documentIds, int portalId, int moduleId)
        {
            foreach (var documentId in documentIds) {
                DocumentsDataProvider.Instance.DeleteDocument (documentId, false, portalId, moduleId);
            }
        }

        public void DeleteWithFile (IEnumerable<int> documentIds, int portalId, int moduleId)
        {
            foreach (var documentId in documentIds) {
                DocumentsDataProvider.Instance.DeleteDocument (documentId, true, portalId, moduleId);
            }
        }

        public void Publish (IEnumerable<int> documentIds, int moduleId)
        {
            foreach (var documentId in documentIds) {
                var document = DocumentsDataProvider.Instance.GetDocument (documentId, moduleId);
                var now = DateTime.Now;
                if (document != null) {
                    document.Publish ();
                    document.ModifiedDate = now;
                    DocumentsDataProvider.Instance.Update (document);
                }
            }
        }

        public void UnPublish (IEnumerable<int> documentIds, int moduleId)
        {
            foreach (var documentId in documentIds) {
                var document = DocumentsDataProvider.Instance.GetDocument (documentId, moduleId);
                var now = DateTime.Now;
                var today = now.Date;
                if (document != null) {
                    document.UnPublish (today);
                    document.ModifiedDate = now;
                    DocumentsDataProvider.Instance.Update (document);
                }
            }
        }
    }
}

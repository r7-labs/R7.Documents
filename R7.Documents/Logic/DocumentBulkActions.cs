//
// DocumentBulkActions.cs
//
// Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
// Copyright (c) 2018-2020 Roman M. Yagodin
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
using R7.Documents.Data;

namespace R7.Documents.Logic
{
    public class DocumentBulkActions
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

        public void DeleteWithAsset (IEnumerable<int> documentIds, int portalId, int moduleId)
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

//
// Copyright (c) 2002-2013 by DotNetNuke Corporation
// Copyright (c) 2014-2017 by Roman M. Yagodin <roman.yagodin@gmail.com>
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
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.FileSystem;
using R7.Dnn.Extensions.Models;
using R7.Dnn.Extensions.Utilities;

namespace R7.Documents.Models
{
    /// <summary>
    /// Holds the information about a single document
    /// </summary>
    [TableName ("Documents_Documents")]
    [PrimaryKey ("ItemId", AutoIncrement = true)]
    [Scope ("ModuleId")]
    public class DocumentInfo: IDocument
    {
        #region IDocument implementation

        public int ItemId { get; set; }

        public int ModuleId { get; set; }

        public int CreatedByUserId { get; set; }

        public int ModifiedByUserId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public string Category { get; set; }

        public int OwnedByUserId { get; set; }

        public int SortOrderIndex { get; set; }

        public string Description { get; set; }

        public bool ForceDownload { get; set; }

        public string LinkAttributes { get; set; }

        [ReadOnlyColumn]
        public bool TrackClicks { get; set; }

        [ReadOnlyColumn]
        public bool NewWindow { get; set; }

        [ReadOnlyColumn]
        public int Size { get; set; }

        [ReadOnlyColumn]
        public string CreatedByUser { get; set; }

        [ReadOnlyColumn]
        public string OwnedByUser { get; set; }

        [ReadOnlyColumn]
        public string ModifiedByUser { get; set; }

        int _clicks;
        [ReadOnlyColumn]
        public int Clicks {
            get { return (_clicks < 0) ? 0 : _clicks; }
            set { _clicks = value; }
        }

        [IgnoreColumn]
        public DateTime PublishedOnDate {
            get {
                return ModelHelper.PublishedOnDate (StartDate, CreatedDate);
            }
        }

        #endregion

        #region Methods

        public DocumentInfo Clone ()
        {
            return (DocumentInfo) MemberwiseClone ();
        }

        public void Publish ()
        {
            EndDate = null;
            if (StartDate != null && StartDate > DateTime.Now) {
                StartDate = null;
            }
        }

        public void UnPublish ()
        {
            EndDate = DateTime.Today;
            if (StartDate != null && StartDate > EndDate) {
                StartDate = null;
            }
        }

        #endregion
    }
}

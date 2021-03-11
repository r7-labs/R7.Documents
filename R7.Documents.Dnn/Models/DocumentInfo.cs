using System;
using DotNetNuke.ComponentModel.DataAnnotations;
using R7.Dnn.Extensions.Models;

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

        public int? ParentDocumentId { get; set; }

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

        public bool IsFeatured { get; set; }

        [ReadOnlyColumn]
        public bool TrackClicks { get; set; }

        [ReadOnlyColumn]
        public bool NewWindow { get; set; }

        [ReadOnlyColumn]
        public int Size { get; set; }

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

        public void UnPublish (DateTime? today = null)
        {
            if (today == null) {
                today = DateTime.Today;
            }

            EndDate = today;
            if (StartDate != null && StartDate > EndDate) {
                StartDate = null;
            }
        }

        #endregion
    }
}

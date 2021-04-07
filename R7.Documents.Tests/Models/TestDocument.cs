using System;
using R7.Documents.Models;

namespace R7.Documents.Tests.Models
{
    public class TestDocument: IDocument
    {
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
        public bool IsFeatured { get; set; }
        public bool TrackClicks { get; set; }
        public bool NewWindow { get; set; }
        public int Size { get; set; }
        public int Clicks { get; set; }
        public DateTime PublishedOnDate { get; set; }
    }
}

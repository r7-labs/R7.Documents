using System;

namespace R7.Documents.Models
{
    public interface IDocument
    {
        int ItemId { get; }

        int ModuleId { get; }

        int CreatedByUserId { get; }

        int ModifiedByUserId { get; }

        DateTime CreatedDate { get; }

        DateTime ModifiedDate { get; }

        DateTime? StartDate { get; }

        DateTime? EndDate { get; }

        string Url { get; }

        string Title { get; }

        string Category { get; }

        int OwnedByUserId { get; }

        int SortOrderIndex { get; }

        string Description { get; }

        bool ForceDownload { get; }

        string LinkAttributes { get; }

        bool IsFeatured { get; }

        bool TrackClicks { get; }

        bool NewWindow { get; }

        int Size { get; }

        int Clicks { get; }
    }

    public interface IDocumentMutable : IDocument
    {
        new int ItemId { get; set; }

        new int ModuleId { get; set; }

        new int CreatedByUserId { get; set; }

        new int ModifiedByUserId { get; set; }

        new DateTime CreatedDate { get; set; }

        new DateTime ModifiedDate { get; set; }

        new DateTime? StartDate { get; set; }

        new DateTime? EndDate { get; set; }

        new string Url { get; set; }

        new string Title { get; set; }

        new string Category { get; set; }

        new int OwnedByUserId { get; set; }

        new int SortOrderIndex { get; set; }

        new string Description { get; set; }

        new bool ForceDownload { get; set; }

        new string LinkAttributes { get; set; }

        new bool IsFeatured { get; set; }

        new bool TrackClicks { get; set; }

        new bool NewWindow { get; set; }

        new int Size { get; set; }

        new int Clicks { get; set; }
    }
}

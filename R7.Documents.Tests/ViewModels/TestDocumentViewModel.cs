using System;
using R7.Documents.Models;
using R7.Documents.ViewModels;

namespace R7.Documents.Tests.ViewModels
{
    class TestDocumentViewModel : IDocumentViewModel
    {
        private readonly IDocument _document;

        public TestDocumentViewModel (IDocument document)
        {
            _document = document;
        }

        public int ItemId => _document.ItemId;

        public int ModuleId => _document.ModuleId;

        public int CreatedByUserId => _document.CreatedByUserId;

        public int ModifiedByUserId => _document.ModifiedByUserId;

        public DateTime CreatedDate => _document.CreatedDate;

        public DateTime ModifiedDate => _document.ModifiedDate;

        public DateTime? StartDate => _document.StartDate;

        public DateTime? EndDate => _document.EndDate;

        public string Url => _document.Url;

        public string Title => _document.Title;

        public string Category => _document.Category;

        public int OwnedByUserId => _document.OwnedByUserId;

        public int SortOrderIndex => _document.SortOrderIndex;

        public string Description => _document.Description;

        public bool ForceDownload => _document.ForceDownload;

        public string LinkAttributes => _document.LinkAttributes;

        public bool IsFeatured => _document.IsFeatured;

        public bool TrackClicks => _document.TrackClicks;

        public bool NewWindow => _document.NewWindow;

        public int Size => _document.Size;

        public int Clicks => _document.Clicks;

        public DateTime PublishedOnDate => _document.PublishedOnDate;

        public string CreatedByUser => throw new NotImplementedException ();

        public string ModifiedByUser => throw new NotImplementedException ();

        public string OwnedByUser => throw new NotImplementedException ();
    }
}

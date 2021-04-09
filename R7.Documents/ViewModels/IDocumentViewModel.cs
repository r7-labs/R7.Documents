using System;
using R7.Documents.Models;

namespace R7.Documents.ViewModels
{
    public interface IDocumentViewModel: IDocument
    {
        string CreatedByUser { get; }

        string ModifiedByUser { get; }

        string OwnedByUser { get; }

        DateTime PublishedOnDate { get; }
    }
}

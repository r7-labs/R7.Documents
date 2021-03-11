//
// IDocument.cs
//
// Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
// Copyright (c) 2017 Roman M. Yagodin
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

namespace R7.Documents.Models
{
    public interface IDocument
    {
        int ItemId { get; }

        int? ParentDocumentId { get; }

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

        DateTime PublishedOnDate { get; }
    }
}

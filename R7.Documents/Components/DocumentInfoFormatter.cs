//
// DocumentInfoFormatter.cs
//
// Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
// Copyright (c) 2015 
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
using R7.Documents.Models;

namespace R7.Documents
{
    public class DocumentInfoFormatter
    {
        // charsets for string.Split ()
        static readonly char [] attributeQuotes = { '\"', '\'' };
        static readonly char [] attributeSeparators = { ';', ',' };

        public DocumentInfo Document { get; protected set; }

        public DocumentInfoFormatter (DocumentInfo document) {
            Document = document;
        }

        public IEnumerable<Tuple<string,string>> LinkAttributesCollection
        {
            get {
                var attrs = new List<Tuple<string,string>> ();

                if (!string.IsNullOrWhiteSpace (Document.LinkAttributes)) {
                    // for earch attribute name / value pair
                    foreach (var attr in Document.LinkAttributes.Split (attributeSeparators, StringSplitOptions.RemoveEmptyEntries)) {
                        var attrPair = attr.Split ('=');
                        if (attrPair.Length == 2)
                            attrs.Add (new Tuple<string, string> (attrPair [0], attrPair [1].Trim (attributeQuotes)));
                        else if (attrPair.Length == 1)
                            attrs.Add (new Tuple<string, string> (attrPair [0], string.Empty));
                    }
                }

                return attrs;
            }
        }
    }
}

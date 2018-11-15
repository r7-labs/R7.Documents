//
// DocumentsPortalConfig.cs
//
// Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
// Copyright (c) 2018 Roman M. Yagodin
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

using System.Collections.Generic;

namespace R7.Documents.Models
{
    public class DocumentsPortalConfig
    {
        public int DataCacheTime { get; set; } = 3600;

        public bool NewWindow { get; set; } = true;

        public List<GridStyle> GridStyles { get; set; } = new List<GridStyle> {
            new GridStyle {
                Name = "dnn",
                GridCssClass = "dnnGrid",
                HeaderCssClass = "dnnGridHeader",
                FooterCssClass = "dnnGridFooter",
                ItemCssClass = "dnnGridItem",
                AltItemCssClass = "dnnGridAltItem",
                GridLines = "None",
                Width = "100%"
            },
            new GridStyle {
                Name = "bootstrap",
                GridCssClass = "table table-bordered table-striped table-hover",
                HeaderCssClass = "",
                FooterCssClass = "",
                ItemCssClass = "",
                AltItemCssClass = "",
                GridLines = "Both",
                Width = "100%"
            },
            new GridStyle {
                Name = "bootstrap-condensed",
                GridCssClass = "table table-bordered table-striped table-hover table-condensed",
                HeaderCssClass = "",
                FooterCssClass = "",
                ItemCssClass = "",
                AltItemCssClass = "",
                GridLines = "Both",
                Width = "100%"
            }
        };
    }
}

//
// HttpOffContextHelper.cs
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

using System.Linq;
using DotNetNuke.Entities.Portals;

namespace R7.Documents.Components
{
    // TODO: Move to the base library
    public static class HttpOffContextHelper
    {
        /// <summary>
        /// Gets the portal settings with portal alias info outside HTTP context.
        /// Could be useful along with e.g. Globals.NavigateURL().
        /// </summary>
        /// <returns>The portal settings.</returns>
        /// <param name="portalId">Portal identifier.</param>
        /// <param name="tabId">Tab identifier.</param>
        /// <param name="cultureCode">Culture code.</param>
        public static PortalSettings GetPortalSettings (int portalId, int tabId, string cultureCode)
        {
            var portalAliases = PortalAliasController.Instance.GetPortalAliasesByPortalId (portalId).ToList ();
            var portalSettings = new PortalSettings (portalId);
            var portalAlias = default (PortalAliasInfo);

            if (!string.IsNullOrEmpty (cultureCode)) {
                portalAlias = portalAliases.FirstOrDefault (pa => pa.IsPrimary && pa.CultureCode == cultureCode);
                if (portalAlias == null) {
                    portalAlias = portalAliases.FirstOrDefault (pa => pa.CultureCode == cultureCode);
                }
            }
            else {
                portalAlias = portalAliases.FirstOrDefault (pa => pa.IsPrimary && pa.CultureCode == portalSettings.DefaultLanguage);
                if (portalAlias == null) {
                    portalAlias = portalAliases.FirstOrDefault (pa => pa.IsPrimary && pa.CultureCode == "");
                }
            }

            if (portalAlias == null) {
                portalAlias = portalAliases.FirstOrDefault (pa => pa.IsPrimary);
                if (portalAlias == null) {
                    portalAlias = portalAliases.FirstOrDefault ();
                }
            }

            return new PortalSettings (tabId, portalAlias);
        }
    }
}

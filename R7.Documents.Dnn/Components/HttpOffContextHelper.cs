using System.Linq;
using DotNetNuke.Entities.Portals;

namespace R7.Documents.Components
{
    // TODO: Replace with PortalHelper.GetPortalSettingsOutOfHttpContext
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

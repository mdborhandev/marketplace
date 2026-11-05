# Design System Migration Notes

## Implemented in this change

- Added an app-owned semantic token and Bootstrap component layer at `src/market.Application/wwwroot/css/design-system.css`.
- Added the Clearance Market (dark) and New Collections (light) theme modes on `data-market-theme`, synchronized with Bootstrap/Sneat's `data-bs-theme`.
- Added early theme initialization and theme controls in the marketplace Razor layout. User selection persists in local storage under `poshakghor-theme` and synchronizes Sneat's legacy theme key to avoid a competing preference. The first visit migrates a prior Sneat light/dark selection if present.
- Switched the marketplace layout's Bootstrap bundle from the external 5.3.3 CDN reference to the repository's local Bootstrap 5.3.3 distribution. No package was installed or removed.
- Added `/design-system`, implemented by `HomeController.DesignSystem` and `Views/Home/DesignSystem.cshtml`, with interactive examples bound to production tokens and classes.
- Replaced the placeholder `wwwroot/js/site.js` comments with marketplace-scoped theme and showcase event handling. Other layouts return before initialization, preserving Halda behavior.
- Added this guide and `docs/DESIGN_SYSTEM.md`.

## Changed files

| File | Change |
|---|---|
| `src/market.Application/Views/Shared/_LayoutMarketplace.cshtml` | Theme pre-initialization, storefront navigation, design-system stylesheet, local Bootstrap bundle. |
| `src/market.Application/Controllers/HomeController.cs` | New `/design-system` view action. |
| `src/market.Application/Views/Home/DesignSystem.cshtml` | Shared component showcase page. |
| `src/market.Application/wwwroot/css/design-system.css` | Central palette, themes, Bootstrap mappings, responsive component rules. |
| `src/market.Application/wwwroot/js/site.js` | Theme persistence and page interactions. |
| `docs/DESIGN_SYSTEM.md` | Architecture, tokens, components and customization instructions. |
| `docs/DESIGN_SYSTEM_MIGRATION.md` | This implementation and migration record. |

No Halda admin files, APIs, database models, business rules, compiled theme files, or package manifests were changed.

## Moving existing marketplace UI onto the system

1. Confirm which runtime views belong to the public storefront and which are admin surfaces. The separate Halda tree must remain independently themed.
2. Replace hardcoded colors and one-off spacing in marketplace Razor views with semantic `--pg-*` tokens, Bootstrap utilities, and the shared component classes.
3. Extract recurring Razor markup into partials/ViewComponents when product/catalog flows are introduced. The showcase is a reference and uses no business data.
4. Add real product photographs and accessible alt text at the production component boundary. The showcase uses CSS-generated textile/editorial visual examples because the foundation currently has no product image/content source.
5. Validate contrast, keyboard navigation, modal focus handling, and content at representative mobile/tablet/desktop sizes using real application pages.
6. Once route and layout usage is known, audit the alternate stock `_Layout.cshtml` and static vendor copies for actual reachability. Do not remove assets based only on their current apparent usage.

## Compatibility and risks

- The active marketplace theme CSS identifies as Bootstrap 5.3.3, and the local bundle is 5.3.3. The alternate stock `_Layout.cshtml` still references Bootstrap 5.3.8 and was intentionally left unchanged; do not mix those styles/scripts on a single rendered page.
- CSS theme files are selected asynchronously when `data-bs-theme` changes. The app-owned stylesheet follows them in document order and reasserts semantic tokens. Verify both modes after any upstream theme refresh.
- The sample product illustration is an abstract CSS silhouette, not real catalog photography. Replace it with actual image-backed markup when catalog data is available.
- Browser storage can be disabled; theme switching still applies for the current page, but persistence is unavailable in that case.
- The showcase uses client-side validation only for demonstration. It does not submit data or alter server-side validation behavior.
- No external UI package was added. The implementation depends on the existing local Bootstrap 5.3.3 bundle and the existing Sneat theme assets.


# PoshakGhor Design System Audit

**Scope:** Read-only audit of the checked-in ASP.NET Core MVC front end and its bundled theme assets. The repository currently labels its app/project `market`; the audit target is the codebase supplied for PoshakGhor. No application files were changed for this audit.

## Executive summary

The repository has two parallel front-end shells rather than one unified design system:

1. The active MVC application shell is `src/market.Application/Views/Shared/_LayoutMarketplace.cshtml`, selected for application views by `Views/_ViewStart.cshtml`. It loads a Sneat-derived, locally bundled Bootstrap 5.3.3 theme (core, component theme, demo CSS), a small Pickr integration, and `wwwroot/css/site.css`. The current home page is a minimal “market” placeholder.
2. A larger Halda administration shell lives under the repository path `layout theme/`. Its layout loads a separately bundled Sneat theme, global site CSS, vendor styles, and numerous third-party libraries. It is not selected by the application project's `_ViewStart.cshtml`; verify its runtime/project integration before treating its files as the marketplace's active UI.
3. A stock ASP.NET template `_Layout.cshtml` also exists and references Bootstrap 5.3.8 from jsDelivr, plus `site.css` and generated scoped CSS. It is not the default layout selected by `_ViewStart.cshtml`.

There is no checked-in npm manifest, SCSS build pipeline, or centralized PoshakGhor token/component layer found in the inspected project. The active shell carries a dark-by-default, runtime-switchable theme, but there is no Clearance Market or New Collections theme architecture or dedicated component showcase page in the current app.

## 1. Application and asset architecture

| Area | Files | Observed role |
|---|---|---|
| MVC app | `src/market.Application/` | ASP.NET Core Web project targeting `net10.0`; Razor views and `wwwroot` assets. |
| Active default layout | `src/market.Application/Views/_ViewStart.cshtml`, `Views/Shared/_LayoutMarketplace.cshtml` | `_ViewStart` selects `_LayoutMarketplace`; it supplies the marketplace navbar/content wrapper and theme toggle. |
| Active view | `src/market.Application/Views/Home/Index.cshtml` | Placeholder page with a heading and short status paragraph. |
| Marketplace theme bundle | `src/market.Application/wwwroot/marketplaceTheme/assets/` | Sneat-derived core/theme CSS, demo stylesheet, JS helpers/config/customizer, Pickr, images. |
| Base application CSS/JS | `src/market.Application/wwwroot/css/site.css`, `wwwroot/js/site.js` | Small app-level files. `site.css` retains template defaults; `site.js` is loaded by the layout. |
| Bootstrap and jQuery static libraries | `src/market.Application/wwwroot/lib/` | Vendored Bootstrap 5.3.3 CSS/JS distributions and jQuery / validation distributions. |
| Legacy/default template layout | `src/market.Application/Views/Shared/_Layout.cshtml`, `_Layout.cshtml.css` | Stock MVC-style layout using a Bootstrap 5.3.8 CDN reference; not selected by `_ViewStart`. |
| Halda administration theme | `layout theme/wwwroot/adminLayoutHalda/assets/`, `layout theme/Views/Shared/_LayoutAdminHalda.cshtml` | Larger, separately bundled Sneat/admin experience with its own theme assets, vendor libraries, and page scripts. |

The application project file is `src/market.Application/market.Application.csproj`; its direct PackageReferences are server-side ASP.NET/API/logging packages, not Bootstrap. `global.json` pins the .NET SDK. No `package.json`, `libman.json`, Bower manifest, Vite/Webpack config, or SCSS/Sass source was found within the inspected repository paths. Static vendor copies and CDN links therefore act as the front-end dependency declarations.

## 2. Bootstrap implementation and dependencies

### Version and loading paths

- The vendored Bootstrap distribution under `src/market.Application/wwwroot/lib/bootstrap/dist/` identifies itself as **Bootstrap 5.3.3** in `css/bootstrap.css` and `js/bootstrap.js`.
- The active marketplace shell does **not** link the vendored `lib/bootstrap/dist/css/bootstrap.css`. Its `core.css` / `core-dark.css` theme bundles identify themselves as Bootstrap 5.3.3 and provide Bootstrap-style foundations and component styling.
- The active marketplace shell loads `https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js` at the end of the body. This bundle includes Bootstrap's Popper dependency.
- The alternate `Views/Shared/_Layout.cshtml` links Bootstrap CSS and JS **5.3.8** from jsDelivr with integrity attributes.
- The Halda layout comments out a Bootstrap 5.3.3 CSS CDN link and instead loads local Sneat core/theme CSS. Its own bundled `assets/vendor/js/bootstrap.js` is a compiled theme vendor bundle; the inspected minified bundle does not expose an easy standalone version marker. Other Bootstrap 5 integrations are visible in vendor paths and CDN assets (for example Tabulator Bootstrap 5 styling).

This yields multiple Bootstrap artifacts/versions in the repository, but not all are active in the same page. Confirm the intended shell and runtime references before migration; avoid assuming the static library copy drives the active page.

### Other observed dependencies

**Active marketplace shell:** Hugeicons stylesheet from `use.hugeicons.com`; local Sneat `helpers.js`, `template-customizer.js`, `config.js`; local Pickr theme CSS/JS; Bootstrap bundle CDN.

**Halda layout / admin tree:** Google Fonts (Inter and Public Sans), Remix Icon, Font Awesome, Bootstrap Icons, Boxicons, flag and Iconify font styles; jQuery UI; Toastr; Tabulator 6.3.1 and Bootstrap 5 adapter; Dropzone; Perfect Scrollbar; Flatpickr; Bootstrap Date Range Picker; jQuery Timepicker; Select2; Tagify; Bootstrap Select; Driver.js; intl-tel-input; and many optional vendor packages under `assets/vendor/libs/` (charts, calendars, editors, maps, validation, tours, notifications, etc.). The layout also loads several of these from CDNs. Presence in the tree does not prove each vendor plugin is used by a live page.

## 3. CSS/SCSS structure and theme files

There are no authored SCSS/Sass sources in the inspected app. CSS is organized as follows:

- `src/market.Application/wwwroot/css/site.css`: small app stylesheet (base font-size adjustment, focus ring, page/body defaults, floating-label placeholder alignment).
- `src/market.Application/Views/Shared/_Layout.cshtml.css`: generated Razor scoped stylesheet associated with the alternate `_Layout.cshtml`; contains the stock template's link/footer/navbar styling.
- `src/market.Application/wwwroot/marketplaceTheme/assets/vendor/css/core.css` and `core-dark.css`: large Bootstrap-compatible Sneat foundation files, including Bootstrap variables, resets, components, utilities, responsive rules, and template layout styles. These files are theme distributions, not hand-authored PoshakGhor source.
- `src/market.Application/wwwroot/marketplaceTheme/assets/vendor/css/theme-default.css` and `theme-default-dark.css`: theme-specific component and template overrides.
- `src/market.Application/wwwroot/marketplaceTheme/assets/css/demo.css`: template/demo rules loaded by the active marketplace shell.
- `src/market.Application/wwwroot/marketplaceTheme/assets/vendor/libs/pickr/pickr-themes.css`: colour-picker styles.
- `layout theme/wwwroot/adminLayoutHalda/assets/vendor/css/`: parallel Halda theme (`core*.css`, `theme-default*.css`), `customCss_haldaLayout.css`, `tabulatorStyle_halda.css`, and many page-specific styles under `css/pages/`.
- `layout theme/wwwroot/adminLayoutHalda/assets/css/demo.css`: admin template demo rules.
- `layout theme/wwwroot/adminLayoutHalda/assets/vendor/libs/`: third-party CSS grouped by plugin.
- `layout theme/Views/Shared/PartialView/_CompanySetupChecklist.cshtml`: substantial page/component-local `<style>` rules for a setup wizard and floating launcher.
- `layout theme/Views/Shared/_LayoutAdminHalda.cshtml`: a `<style>` block for file previews, many inline styles, and embedded assistant/chat styling.

The `layout theme` directory name includes a space. Preserve quoting in shell tooling and ensure any future asset pipeline handles this path or moves assets deliberately.

## 4. Colours, variables, and current palette

The clearest current token source is the Bootstrap variable block in the marketplace `core.css` and `core-dark.css`; the default page sets `data-bs-theme="dark"` in `_LayoutMarketplace.cshtml`.

| Token / role | Light core value | Dark core value |
|---|---:|---:|
| Primary | `#696cff` | `#696cff` |
| Secondary | `#8592a3` | `#8592a3` |
| Success | `#4AA147` | `#71dd37` |
| Info | `#0DCAF0` | `#03c3ec` |
| Warning | `#FFC107` | `#ffab00` |
| Danger | `#DC3545` | `#ff3e1d` |
| Body background | `#f5f5f9` | `#2b2c40` |
| Body text | `#646e78` | `#b2b2c4` |
| Heading text | `#384551` | `#d5d5e2` |
| Border | `#e4e6e8` | `#4e4f6c` |

The bundles also define the wider `--bs-*` system: semantic text/background/border variants, gray scales, RGB companions, emphasis colours, form validation colours, links, radii, shadows, and focus ring values. The dark and light definitions are not perfectly symmetrical: the two files differ in semantic values and font setup, and the active shell swaps both core and theme stylesheets when the root `data-bs-theme` attribute changes.

The marketplace `assets/js/config.js` separately defines `defaultPrimaryColor: '#4AA147'`, which differs from the Bootstrap core's `--bs-primary: #696cff`; the precise runtime effect should be verified against the customizer if that setting is retained. This is a concrete colour-source inconsistency to resolve during token design.

Additional hardcoded colours occur in the stock scoped layout stylesheet and in Halda layout/partial styles and scripts (for example green `#22c55e`, `#52a167`, gray chat surfaces, and white text). These appear outside a single shared token registry. The default app `site.css` also hardcodes the focus halo `#258cfb` and white, which diverges from the theme focus ring.

## 5. Typography, spacing, and breakpoints

### Typography

- Marketplace light core defines `--bs-font-inter` and sets `--bs-body-font-family` to it, but `_LayoutMarketplace.cshtml` does not itself load the Inter font stylesheet. Its fallback stack is system fonts. The light theme's `--bs-font-sans-serif` is Public Sans, but Bootstrap body text points at Inter.
- Marketplace dark core uses Public Sans for `--bs-font-sans-serif` and body text. The active dark default therefore uses Public Sans, but the active layout does not load a Public Sans webfont stylesheet. Local/system fallback behavior will apply unless another stylesheet loads the font.
- Halda explicitly requests Inter and Public Sans from Google Fonts. It also loads several icon fonts.
- Bootstrap-derived body type in theme core is `0.9375rem` (15px at 16px root), weight 400, line-height 1.375; root size is 16px. Heading rules are part of the vendor bundle and are responsive. `site.css` globally changes `html` to 14px, then 16px at `min-width: 768px`, but loaded core CSS has `:root { font-size: var(--bs-root-font-size) }`; because `html` is the root element, cascade order matters. Since `site.css` is loaded after core/theme/demo CSS, its `html { font-size: ... }` can override the root declaration and makes `rem` sizing viewport-dependent in the active shell.

### Spacing and responsive breakpoints

Spacing primarily comes from Bootstrap utilities (for example `container-xxl`, `container-p-y`, `gap-*`, `py-*`, `px-*`, `ms-*`) and the Sneat layout's own spacing rules. No PoshakGhor-specific spacing scale is defined in app CSS. Bootstrap's standard responsive utility and grid conventions are used, including `sm`, `xl`, and `xxl` classes. The active marketplace shell uses `navbar-expand-xl` and `container-xxl`; the admin layout uses the same breakpoint for menu/navbar behavior. No separate custom Sass breakpoint map was found. The admin layout and setup partial add custom responsive breakpoints in their styles; catalogue those before centralizing responsive rules.

## 6. Layout, navigation, and responsive structure

**Marketplace shell:** compact, no-sidebar (`layout-without-menu`) layout; detached navbar; brand label; theme toggle; `container-xxl` content wrapper; footer/backdrop scaffold. It uses Bootstrap/Sneat utility and layout classes, with responsive navbar expansion at `xl`. Current home content is a placeholder.

**Halda shell:** vertical sidebar and menu partials selected from request cookies (`viewMode`), fixed/compact layout classes, detached top navbar, module dropdowns, user menu, notification dropdowns, company switcher, profile actions, assistant overlay, and a file-preview modal. It uses Bootstrap navbar, dropdown, badge, modal, grid, and responsive display utilities together with Sneat-specific layout/menu classes. Its navigation is split into `_UserMenuNav.cshtml` and `_ApplicantMenuNav.cshtml` partials.

**Alternate scaffold shell:** standard Bootstrap navbar with collapse toggler, container, body content, and footer in `_Layout.cshtml`.

The shell distinction should be made explicit for the later design-system work: establish which shell is the customer storefront and which admin shell remains independently branded/maintained. A future Clearance Market / New Collections theme should be scoped at a stable app/theme boundary (for example root data attribute), not applied globally to both shells without an explicit requirement.

## 7. Component inventory

The active marketplace page itself currently contributes almost no reusable domain components. Bootstrap/Sneat components are available by styling and markup convention, but the inspected home view does not demonstrate a PoshakGhor component library.

| Component | Evidence in repository | Current state |
|---|---|---|
| Buttons | Active theme toggle in `_LayoutMarketplace.cshtml`; extensive Bootstrap buttons in admin layout and setup partial. | Bootstrap `.btn*` classes plus Sneat variants (`btn-icon`, `btn-text-secondary`, `wizard-btn-soft-primary`); no central app button partial/component found. |
| Cards | Numerous admin template card classes/scripts and page CSS; setup tasks have custom card rules. | Primarily theme/vendor conventions and local page styling. |
| Forms | Bootstrap `.form-control` in layouts; validation libraries; Select2, Tagify, Flatpickr and other admin form plugins. | ASP.NET validation partial plus Bootstrap conventions; form patterns vary by feature. |
| Tables | Tabulator 6.3.1 and `tabulatorStyle_halda.css`; vendored DataTables Bootstrap integrations in Halda. | Multiple table implementations/styles; page-level usage should be mapped before consolidation. |
| Badges | Notification/status badges in admin layout; wizard status badges in setup partial. | Bootstrap badges with theme label/dot variants and custom wizard badge styling. |
| Modals | Bootstrap file preview modal in admin layout; company setup wizard modal in partial. | Bootstrap modal primitives with feature-specific CSS/inline styles. |
| Navigation | Marketplace navbar; Halda sidebar/menu partials, navbar and dropdowns; scaffold navbar. | Three shell patterns across codebase, with only marketplace selected by app `_ViewStart`. |
| Alerts/toasts | Toastr CDN and Notyf/Notiflix vendor assets in admin tree. | Several notification systems are present; usage and ownership are not centralized. |
| Theme controls | Marketplace toggle and admin theme chooser/customizer helpers. | Both shells use `data-bs-theme`, but theme persistence and theme CSS mechanisms differ. |

Reusable domain component architecture (shared Razor partials, ViewComponents, Tag Helpers, or a frontend component package) was not evident in active marketplace views. The admin setup checklist is the clearest localized custom UI component found.

## 8. Custom overrides, inline styles, and architecture issues

- `_LayoutMarketplace.cshtml` directly manages stylesheet swapping with a `MutationObserver`, while its theme toggle persists a `marketplace` theme via `window.Helpers`. Theme behavior therefore spans Razor markup, inline JS, helper scripts, and paired CSS files.
- `site.css` is still close to the ASP.NET starter template and contains global html/body/form/focus rules rather than a storefront design system.
- The admin layout includes an inline preview style block, conditional inline fullscreen CSS injection, numerous `style="..."` attributes, and embedded chat/assistant style/script sections. This makes visual overrides difficult to inventory and test centrally.
- `_CompanySetupChecklist.cshtml` embeds a substantial `<style>` block (wizard launcher, progress, task cards, modal buttons, responsive adjustments) alongside markup and script.
- The alternate `_Layout.cshtml.css` contains stock app styles that can conflict with current theme colours if that alternate layout is used.
- `core.css` and `core-dark.css` are vendor theme distributions, while `theme-default*.css` also supplies theme-layer overrides. Editing these directly would create upgrade and provenance risk; build any brand layer after them, using explicit app-owned overrides/tokens.
- Some custom admin CSS may use common selectors. Global `site.css` is conditionally included by the Halda layout unless `ViewData["SkipSiteCss"]` is set, so changes to that shared file can affect both shells if both are active in a deployment.

## 9. Duplicated, conflicting, unused, and hardcoded styles

### Confirmed overlap/conflict

1. **Bootstrap version split:** local theme and vendored distribution are 5.3.3, alternate CDN layout is 5.3.8. A mixed page could load mismatched CSS and JS behavior.
2. **Parallel layout/theme trees:** marketplace theme assets are under the application project, while admin assets and views are in a sibling `layout theme/` tree. They use overlapping Sneat/Bootstrap patterns but separate files and selectors.
3. **Brand-primary split:** core Bootstrap primary is purple `#696cff`, while customizer config's default primary is green `#4AA147`; other hardcoded green values appear in admin UI.
4. **Focus colour split:** `site.css` uses blue `#258cfb`; theme core defines its own focus ring values.
5. **Typography mismatch:** theme files declare Inter/Public Sans differently; active layout does not load the Google Fonts stylesheet loaded by Halda.
6. **Theme implementation split:** active marketplace swaps core/theme CSS files; Halda uses a different template customizer/theme setup. Both use `data-bs-theme`, but that alone does not guarantee equivalent component tokens.

### Potential unused/legacy assets (requires runtime confirmation)

- Bootstrap local `wwwroot/lib/bootstrap` and jQuery assets may be unused by `_LayoutMarketplace`; the visible active shell loads Bootstrap JS from CDN and theme CSS locally.
- The alternate scaffold `_Layout.cshtml` and `_Layout.cshtml.css` are not selected by `_ViewStart` in this project, but may be used explicitly elsewhere.
- The Halda assets tree contains many page-specific CSS/JS and optional vendor plugins; presence alone does not show which are loaded by individual pages. Inventory actual view references before deleting or optimizing.
- `core_old.css`, light/dark variants, many admin page CSS files, and a large plugin directory may include unused template material. Do not classify as dead code solely from filenames.

### Hardcoded values

Hardcoded colors, radii, spacing, sizes, and display behaviors are present in `site.css`, the scaffold scoped stylesheet, admin Razor inline styles/scripts, and the setup partial's local CSS. This increases drift risk when introducing the two storefront themes and makes global brand changes difficult to audit.

## 10. Migration impact map and recommendations

### Files/dependencies likely affected

**Marketplace foundation:**

- `src/market.Application/Views/Shared/_LayoutMarketplace.cshtml` (asset order, root theme attributes, toggle/persistence, navbar/footer, showcase entry point)
- `src/market.Application/Views/_ViewStart.cshtml` (layout selection)
- `src/market.Application/wwwroot/css/site.css` (currently global starter rules; likely migration boundary for app-owned base styles)
- `src/market.Application/wwwroot/marketplaceTheme/assets/vendor/css/core*.css`, `theme-default*.css`, `assets/css/demo.css` (vendor/template baseline; treat as upstream, avoid direct brand edits)
- `src/market.Application/wwwroot/marketplaceTheme/assets/js/config.js` and `assets/vendor/js/helpers.js` / `template-customizer.js` (theme defaults, persisted modes, colour customizer behavior)
- `src/market.Application/wwwroot/js/site.js`
- `src/market.Application/Views/Home/Index.cshtml` and future storefront views/components

**Parallel/admin surfaces to assess and keep scoped:**

- `layout theme/Views/Shared/_LayoutAdminHalda.cshtml` and `PartialView/_CompanySetupChecklist.cshtml`
- `layout theme/wwwroot/adminLayoutHalda/assets/vendor/css/customCss_haldaLayout.css`, `tabulatorStyle_halda.css`, `core*.css`, `theme-default*.css`, page CSS, vendor plugin CSS
- `src/market.Application/Views/Shared/_Layout.cshtml` and `_Layout.cshtml.css` if the scaffold shell remains reachable

**Dependency references and supply chain:**

- Bootstrap 5.3.3 vendored `wwwroot/lib/bootstrap` assets and Bootstrap bundle CDN in `_LayoutMarketplace`
- Bootstrap 5.3.8 CSS/JS CDN in `_Layout.cshtml`
- Halda compiled Bootstrap bundle and Bootstrap-adapted Tabulator/DataTables/vendor styles
- Google Fonts, icon font CDNs, Tabulator, Toastr, Driver.js, and other plugin CDN references in the admin layout
- jQuery / validation libraries in `wwwroot/lib` and `_ValidationScriptsPartial.cshtml`

### Recommended migration sequence

1. **Confirm runtime scope:** identify live routes/projects that use marketplace, Halda, and scaffold layouts. Capture current computed styles and key pages before altering CSS. Decide which shell owns the PoshakGhor storefront.
2. **Pin and document Bootstrap:** choose one intended Bootstrap baseline and document the source for CSS/JS. Avoid simultaneous CDN/local versions or a mismatched major/minor between theme CSS and JS. Retain Bootstrap as the requested foundation.
3. **Create app-owned semantic tokens outside vendor bundles:** define source tokens for brand, semantic states, surfaces, text, borders, focus, radii, elevation, typography, spacing, and motion. Map them to Bootstrap custom properties where possible. Keep upstream template assets distinguishable from PoshakGhor overrides.
4. **Model Clearance Market and New Collections as explicit theme variants:** use a scoped root attribute/class and token overrides, with a shared component contract. Keep layout/structure shared unless product requirements establish a real structural difference. Define how selected theme persists and what default applies.
5. **Normalize typography and responsive rules:** explicitly load the selected font(s), define a type scale and spacing scale, and make the root font-size behavior intentional. Document Bootstrap breakpoint usage and any app-specific responsive rules.
6. **Inventory and centralize reusable components:** establish repeatable Bootstrap-based button, card/product tile, form/control, table, badge/status, modal, navigation, alert/toast patterns with states. Separate app components from Halda/template demo components.
7. **Build the dedicated interactive showcase page:** make it render real shared components and show both themes, responsive variants, interactive states, validation/disabled/loading states, and accessibility-relevant focus behavior. Avoid a static mockup that can drift from production components.
8. **Migrate incrementally and validate by surface:** replace hardcoded declarations with tokens, extract embedded styles where useful, verify theme parity and Bootstrap JS behavior, and compare key viewport sizes. Remove stale assets only after references and production routes are proven unused.

## 11. Risks and verification items

- **Shell ambiguity:** the active MVC `_ViewStart` chooses the marketplace shell, but the large Halda tree suggests another UI surface. Incorrectly applying tokens globally can unintentionally restyle admin routes.
- **CSS/JS mismatch:** Bootstrap 5.3.3 theme CSS and 5.3.8 scaffold CSS/JS coexist. Theme components and data APIs may diverge if mixed on one rendered page.
- **Theme flashes and persistence:** CSS is switched after a mutation observer sees `data-bs-theme`; inspect helper initialization and local storage behavior to ensure the correct theme is applied before paint. The inline theme script is not accompanied by a CSP analysis in this audit.
- **Vendor provenance and regeneration:** theme files look compiled/distributed; edits may be overwritten or make future updates hard. Establish source-of-truth strategy before touching them.
- **Specificity and cascade:** Sneat styles, Bootstrap utilities, app `site.css`, scoped layout CSS, page CSS, inline styles, and plugin CSS compete. New tokens alone will not resolve existing selector precedence.
- **Theme contrast/accessibility:** dark/light palettes and custom green/purple overrides require contrast and focus verification, especially status badges, icon-only buttons, forms, and interactive states.
- **Third-party components:** Tabulator, Select2, Bootstrap Select, date/time controls, and notification plugins may need explicit theme adapters; their presence in a shared layout can make a storefront theme migration broader than the core Bootstrap layer.
- **Static asset references:** files under the separate `layout theme/` path and CDN URLs need deployment/publishing verification; this report does not establish production availability or runtime use.
- **No automated style inventory:** no CSS build/test pipeline or centralized token source was located. Add migration validation and visual regression practices as a separate planned engineering task, not as part of this audit.

## 12. Audit boundary

Findings reflect repository contents and visible references in the inspected layouts/styles. This audit did not run the app, inspect deployed pages, resolve CDN assets, or infer plugin usage from mere file presence. Runtime route coverage, production CSS cascade, accessibility outcomes, and unused-asset status should be verified during migration discovery.

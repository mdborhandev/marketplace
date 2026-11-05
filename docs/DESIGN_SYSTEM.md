# PoshakGhor Design System

## Architecture

The system is built in the existing ASP.NET Core MVC/Razor application and retains Bootstrap 5 and the Sneat foundation. The marketplace default layout is `src/market.Application/Views/Shared/_LayoutMarketplace.cshtml`; it loads the bundled Sneat theme first, then `wwwroot/css/site.css`, then the app-owned `wwwroot/css/design-system.css`. The last file is the central customization layer. The Halda administration theme is a separate UI and is not changed by this design system.

Bootstrap's local 5.3.3 bundle is used by the marketplace layout for interactive components. Bootstrap's CSS-compatible variables and existing Sneat layout/utilities remain the structural foundation; design-system styles override semantic variables and component presentation without editing compiled vendor CSS.

## Themes

The root `data-market-theme` attribute selects `clearance` or `new-collections`. `data-bs-theme` is kept in sync (`dark` for Clearance Market, `light` for New Collections) so the corresponding existing Sneat core and component-theme stylesheets are selected.

- **Clearance Market:** dark plum/neutral surfaces, high-contrast text and action states. Components highlight offer pricing, discounts, stock availability, and quantity selection.
- **New Collections:** warm ivory/white surfaces, muted plum accents, editorial serif display type, and product/brand presentation.

The selected mode is stored as `poshakghor-theme` in local storage and mirrored to Sneat's existing marketplace theme key so its template customizer cannot restore a conflicting light/dark mode. A small early script in the shared marketplace layout sets the theme attributes before theme CSS loads. On first visit it migrates the prior Sneat light/dark selection when present; otherwise it defaults to Clearance Market. Both modes share component markup and semantic component tokens.

## Tokens and customization

All application design tokens are in `src/market.Application/wwwroot/css/design-system.css` under `:root` / `[data-market-theme="new-collections"]`. The initial light palette is:

| Token | Value |
|---|---|
| Primary | `#806B78` |
| Primary hover | `#6E5966` |
| Secondary | `#B7A4AE` |
| Background | `#F7F4EF` |
| Surface | `#FFFFFF` |
| Neutral | `#B8AA9B` |
| Text | `#51463F` |
| Heading | `#302D2B` |
| Border | `#E6DFD7` |
| Success | `#667D68` |
| Warning | `#A58A5A` |
| Danger | `#A56E70` |
| Info | `#718697` |

The token layer also defines dark-mode semantic surfaces and text; body/display font stacks; spacing (`--pg-space-1` through `--pg-space-8`); small/medium/large/pill radii; three elevation levels; content width; transition timing; and focus colour. Semantic values are mapped onto Bootstrap variables such as `--bs-primary`, `--bs-body-bg`, `--bs-body-color`, and `--bs-border-color` so Bootstrap utilities and controls inherit the current theme.

To customize the brand palette, edit the `--pg-*` values in the first token block. For Clearance-specific values, update the `[data-market-theme="clearance"]` block. Avoid changing Sneat's `core*.css` or `theme-default*.css`; they are bundled foundation assets. Components should use semantic tokens, not copy hex codes.

## Shared components

The app-owned component layer is CSS in `design-system.css`, built on Bootstrap markup and utilities. It includes:

- Button variants: Bootstrap primary/outline, soft, icon, disabled and interactive/loading states.
- Product cards, merchandising visuals, brand/title/price treatments, offer labels and stock indicators.
- Semantic badges and status dots for primary, success, warning, danger and info states.
- Form labels, controls, select, focus/disabled/validation styling, product-filtering search composition and quantity stepper with quantity-tier pricing treatment.
- Bootstrap responsive tables, pagination, breadcrumbs and shared navigation styles.
- Alerts, Bootstrap modal surfaces and Bootstrap toast surfaces.
- Loading spinner/skeleton and empty states.
- Editorial hero and responsive grid helpers.

The interactive reference page is `src/market.Application/Views/Home/DesignSystem.cshtml`, served at `/design-system` by `HomeController.DesignSystem`. It uses the same production classes and token variables as the marketplace shell, with Bootstrap's bundled modal/toast behavior. The global `wwwroot/js/site.js` handles theme selection/persistence, showcase form validation, quantity changes, loading button state, and toast invocation.

When building product or collection pages, reuse the component classes and Bootstrap grid rather than styling an isolated mockup. Extract repeated Razor markup into partials or ViewComponents as real product use cases emerge; the current foundation app contains no product data model to bind to.

## Responsive and accessibility behavior

Layouts use Bootstrap's responsive grid/utilities. Product examples use two columns on compact viewports and three at large widths. The showcase's section links scroll horizontally on small screens. Controls have visible `:focus-visible` treatment, labels and accessible names, semantic status roles, and native disabled/validation states. Reduced-motion preferences are respected for transitions and animations.

## Main implementation files

- `src/market.Application/Views/Shared/_LayoutMarketplace.cshtml` — app shell, pre-paint theme initialization, navigation, Bootstrap JS.
- `src/market.Application/Controllers/HomeController.cs` — conventional page action plus `/design-system` route.
- `src/market.Application/Views/Home/DesignSystem.cshtml` — interactive component reference.
- `src/market.Application/wwwroot/css/design-system.css` — app-owned tokens and component styles.
- `src/market.Application/wwwroot/js/site.js` — theme and example interactions.
- `src/market.Application/wwwroot/marketplaceTheme/assets/vendor/css/` — retained Sneat foundation (do not edit for brand customizations).


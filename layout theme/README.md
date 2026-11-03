# Halda layout theme starter

This folder is a reusable copy of the Halda admin layout. It keeps the Razor layout, its sidebar and navigation partials, template customizer, styles, scripts, vendor libraries, fonts, and images together without changing the running application.

## Contents

- `Views/Shared/_LayoutAdminHalda.cshtml` — main page shell with navbar, sidebar, content area, footer, and Razor style/script sections.
- `Views/Shared/PartialView/` — user navigation, applicant navigation, and company setup checklist partials used by the layout.
- `wwwroot/adminLayoutHalda/assets/` — theme CSS and JavaScript, including `template-customizer.js`, `menu.js`, `helpers.js`, plus vendor dependencies and theme images/fonts.

## Reuse in another ASP.NET Core MVC app

1. Copy `Views/Shared/_LayoutAdminHalda.cshtml` and the partials into the new app's `Views/Shared` folder, preserving the `PartialView` subfolder.
2. Copy `wwwroot/adminLayoutHalda` into the new app's `wwwroot` folder. The layout's asset URLs expect this path.
3. Set the target app's `Views/_ViewStart.cshtml` to use `_LayoutAdminHalda` (or set `Layout = "_LayoutAdminHalda"` in the view).
4. Keep or replace the three navigation/checklist partials. They use Halda-specific view data and services, so update them for the new app. Remove their calls from the layout if they are not needed.
5. Review the layout's imports and injected services at the top of the Razor file. It currently uses Halda DTOs, configuration, JSON, and antiforgery services. Adapt those imports/data references to the target app.
6. The layout also loads several libraries and helper scripts from CDNs and the app's `wwwroot/js` folder. Copy the required helpers or replace those references; keep Bootstrap, jQuery, and theme scripts loaded before page scripts.

The original source files are `Halda.Application/Views/Shared/_LayoutAdminHalda.cshtml` and `Halda.Application/wwwroot/adminLayoutHalda/assets/`.

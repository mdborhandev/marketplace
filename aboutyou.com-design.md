---
version: alpha
name: About You
description: A clean fashion retail system with editorial contrast, restrained color, and bold promotional moments.
colors:
  primary: "#000000"
  secondary: "#ffffff"
  tertiary: "#f5f5f5"
  neutral: "#d9d9d9"
  surface: "#ffffff"
  on-surface: "#000000"
  error: "#d92d20"
  accent-warm: "#f7e08a"
  muted-brown: "#8b7769"
typography:
  headline-display:
    fontFamily: "ABC Whyte"
    fontSize: "32px"
    fontWeight: 700
    lineHeight: "38px"
    letterSpacing: "0px"
  headline-lg:
    fontFamily: "ABC Whyte"
    fontSize: "24px"
    fontWeight: 700
    lineHeight: "29px"
    letterSpacing: "0px"
  headline-md:
    fontFamily: "ABC Whyte"
    fontSize: "20px"
    fontWeight: 600
    lineHeight: "24px"
    letterSpacing: "0px"
  headline-sm:
    fontFamily: "ABC Whyte"
    fontSize: "18px"
    fontWeight: 600
    lineHeight: "22px"
    letterSpacing: "0px"
  body-lg:
    fontFamily: "ABC Whyte"
    fontSize: "16px"
    fontWeight: 500
    lineHeight: "24px"
    letterSpacing: "0.4px"
  body-md:
    fontFamily: "ABC Whyte"
    fontSize: "16px"
    fontWeight: 400
    lineHeight: "24px"
    letterSpacing: "0px"
  body-sm:
    fontFamily: "ABC Whyte"
    fontSize: "14px"
    fontWeight: 400
    lineHeight: "20px"
    letterSpacing: "0px"
  label-lg:
    fontFamily: "ABC Whyte"
    fontSize: "16px"
    fontWeight: 700
    lineHeight: "20px"
    letterSpacing: "0px"
  label-md:
    fontFamily: "ABC Whyte"
    fontSize: "14px"
    fontWeight: 600
    lineHeight: "18px"
    letterSpacing: "0px"
  label-sm:
    fontFamily: "ABC Whyte"
    fontSize: "12px"
    fontWeight: 600
    lineHeight: "16px"
    letterSpacing: "0.04em"
  nav-sm:
    fontFamily: "ABC Whyte"
    fontSize: "14px"
    fontWeight: 400
    lineHeight: "20px"
    letterSpacing: "0px"
  promo-lg:
    fontFamily: "Helvetica Neue Condensed Bold"
    fontSize: "30px"
    fontWeight: 700
    lineHeight: "36px"
    letterSpacing: "0px"
rounded:
  none: "0px"
  sm: "4px"
  md: "8px"
  lg: "16px"
  xl: "24px"
  full: "9999px"
spacing:
  xs: "8px"
  sm: "16px"
  md: "32px"
  lg: "60px"
  xl: "80px"
components:
  button-primary:
    backgroundColor: "{colors.primary}"
    textColor: "{colors.secondary}"
    typography: "{typography.label-lg}"
    rounded: "{rounded.xl}"
    padding: "0px 4px"
    height: "36px"
  button-primary-hover:
    backgroundColor: "{colors.primary}"
    textColor: "{colors.secondary}"
    typography: "{typography.label-lg}"
    rounded: "{rounded.xl}"
    padding: "0px 4px"
    height: "36px"
  button-secondary:
    backgroundColor: "{colors.secondary}"
    textColor: "{colors.primary}"
    typography: "{typography.label-lg}"
    rounded: "{rounded.xl}"
    padding: "0px 4px"
    height: "36px"
  button-link:
    backgroundColor: "transparent"
    textColor: "{colors.primary}"
    typography: "{typography.body-md}"
    rounded: "{rounded.none}"
    padding: "0px"
  card:
    backgroundColor: "{colors.surface}"
    textColor: "{colors.on-surface}"
    rounded: "{rounded.sm}"
    padding: "16px"
  input:
    backgroundColor: "{colors.tertiary}"
    textColor: "{colors.on-surface}"
    typography: "{typography.body-md}"
    rounded: "{rounded.sm}"
    padding: "12px"
  chip:
    backgroundColor: "{colors.secondary}"
    textColor: "{colors.on-surface}"
    typography: "{typography.label-sm}"
    rounded: "{rounded.full}"
    padding: "4px 12px"
---

# About You

## Overview

About You feels polished, commercial, and highly navigable, with a fashion-first editorial tone. The interface balances a very clean white header and utility system with large merchandising panels and bold promotional accents. It targets mainstream shopping users who expect speed, clarity, and trust, while still giving product and campaign imagery room to feel aspirational.

## Colors

- **Primary (#000000):** The dominant ink tone used for text, key bars, buttons, and the strongest contrast moments. It gives the brand its sharp retail edge.
- **Secondary (#ffffff):** The base surface color for the page, cards, and most content areas. It keeps the layout bright and legible.
- **Tertiary (#f5f5f5):** A soft neutral used for subtle input surfaces, inactive states, and low-emphasis UI blocks.
- **Neutral (#d9d9d9):** A light divider and border gray that helps separate controls without feeling heavy.
- **Surface (#ffffff):** The standard canvas for cards and content containers, reinforcing the clean storefront feel.
- **On-surface (#000000):** The default readable text color on light backgrounds.
- **Error (#d92d20):** A restrained alert red for notifications and count badges.
- **Accent-warm (#f7e08a):** A warm promotional background used for the consent/banner area and other attention-grabbing notices.
- **Muted-brown (#8b7769):** A natural, earthy support tone that appears in lifestyle imagery and brand campaign contexts.

## Typography

The system uses ABC Whyte as the primary UI family, giving the interface a modern, neutral, retail-friendly voice. Headings are bold and compact, with clear size steps from `headline-display` through `headline-sm`, and the body text stays highly readable with a 16px base and modest 0.4px tracking in `body-lg`.

Labels and navigation rely on tighter, more functional styles such as `label-md`, `label-sm`, and `nav-sm`. The brand also uses a condensed promotional display style in `promo-lg` for highly visible marketing moments, echoing the bold “ABOUT YOU” treatment seen in the header and footer. Uppercase usage is common in small utility labels, section tags, and promotional bars, but letter spacing remains subtle rather than overly expanded.

## Layout

The layout is built on a fluid, full-width storefront structure with strong horizontal alignment and clear segmentation. A compact top utility area, a dense navigation zone, and a separate black service strip create stacked information bands before the main merchandising content appears.

Spacing follows a restrained rhythm based on 8px increments, then scales up through 16px, 32px, 60px, and 80px for section breaks and major page zones. Cards and product tiles use modest internal padding, while broader sections rely on generous outer gutters to keep the page from feeling crowded despite the amount of navigation and promotional content.

## Elevation & Depth

The system is intentionally low-elevation and mostly flat. Rather than relying on strong shadows, it uses contrast, background blocks, and borders to separate layers: white surfaces sit against black strips, and soft gray dividers define navigation and controls. Where depth is needed, it appears as very subtle shadowing or thin borders on cards and interactive elements, keeping the UI crisp and uncluttered.

## Shapes

The shape language is soft and modern, with small radii on cards and controls and fuller radii on pills and buttons. `rounded.sm` and `rounded.xl` create the most visible contrast: product cards feel lightly framed, while utility chips and action buttons read as friendly and touchable.

Overall, the shape feel is pragmatic rather than decorative. Most elements are rectilinear with only enough curvature to soften the retail grid.

## Components

Buttons favor strong, highly legible forms. `button-primary` uses a black fill, white text, and `rounded.xl` for the main call to action. `button-secondary` flips to a white fill with black text and should be used for secondary actions or contrast against dark surfaces. `button-link` is plain and text-only for low-emphasis actions like settings, policies, and inline navigation. Button sizing is compact in height but visually confident, and padding stays tight to preserve the streamlined retail look.

Cards use a white background, thin border, and `rounded.sm`, with modest padding for content such as product tiles or promotional modules. They should remain visually quiet and not compete with imagery.

Inputs are understated and functional, using soft neutral backgrounds, no heavy decoration, and clear text contrast. Search fields should look approachable and lightweight, with subtle borders or tonal surfaces rather than pronounced outlines.

Chips and pills should be used for short filters, tags, and status labels. They should remain low-height, rounded-full or near-full in shape, and use small label typography for a compact, efficient feel.

Navigation items are simple text links with minimal chrome. Active states should rely on underline, black text, or subtle weight shifts rather than boxed treatment. Badges and counts should use the error color sparingly so they remain meaningful.

## Do's and Don'ts

- Do keep the interface bright, editorial, and commercially focused.
- Do use black-and-white contrast for the most important actions and bars.
- Do preserve the restrained typography scale; let weight and spacing do the work.
- Do use soft borders and subtle containers instead of deep shadows.
- Don't introduce saturated brand colors that compete with merchandise imagery.
- Don't make buttons overly tall, playful, or heavily rounded beyond the established pill shape.
- Don't clutter the header or navigation with decorative effects.
- Don't use large shadow stacks or heavy gradients to imply depth.
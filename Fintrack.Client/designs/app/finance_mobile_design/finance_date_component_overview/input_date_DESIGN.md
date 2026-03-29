# Design System Specification: High-End Editorial Fintech

## 1. Overview & Creative North Star
**The Creative North Star: "Precision Luminescence"**

This design system rejects the "template-first" mentality of traditional fintech. Instead, it treats financial data as a high-end editorial experience. We move away from the rigid, boxed-in layouts of legacy banking and toward a "Precision Luminescence" aesthetic—where the UI feels like a dark, sophisticated instrument panel illuminated by neon-exact data points. 

The experience is defined by **intentional asymmetry** and **tonal depth**. By breaking the standard grid with overlapping typography and varying surface heights, we create a sense of technological mastery and professional curation.

---

## 2. Colors & Surface Logic

Our palette is rooted in the deep void of `surface-dim`, allowing the high-frequency `primary` (Cero Green) to act as a functional beacon.

### Core Palette
*   **Primary (Cero Green):** `#05E699` | The pulse of the system. Use for FABs, active states, and growth.
*   **Background:** `#111317` | The foundation. A deep, desaturated obsidian.
*   **Surface:** `#1E2228` | The standard container for high-level data.
*   **Danger (Cero Danger):** `#FF4D4D` | Reserved strictly for debt, negative balances, and critical errors.

### The "No-Line" Rule
**Prohibit 1px solid borders for sectioning.** Conventional borders create visual noise that cheapens the "High-End" feel. Instead, define boundaries through background color shifts:
*   Place a `surface-container-high` (`#282a2e`) element on top of a `surface` (`#111317`) background to define its shape.
*   Use the `outline-variant` at 10% opacity only if a "Ghost Border" is required for extreme accessibility needs.

### Surface Hierarchy & Nesting
Treat the UI as a series of physical layers. 
1.  **Level 0 (Base):** `surface-dim` (`#111317`)
2.  **Level 1 (Sections):** `surface-container-low` (`#1a1c20`)
3.  **Level 2 (Cards):** `surface-container` (`#1e2024`)
4.  **Level 3 (Pop-overs/Modals):** `surface-container-highest` (`#333539`)

### The "Glass & Gradient" Rule
To escape the "flat" look, apply a **0.5% - 1% linear gradient** to primary CTA buttons, transitioning from `primary` (`#77ffbb`) to `primary-container` (`#05e699`). For floating navigation or header blurs, use `surface-container` with a `20px` backdrop-blur and 80% opacity to create a "frosted obsidian" effect.

---

## 3. Typography
The system utilizes a high-contrast pairing: **Manrope** for architectural headlines and **Inter** for data-heavy precision.

*   **Display (Manrope):** Large, bold, and authoritative. Use `display-lg` (3.5rem) for total net worth or hero statements. 
*   **Headline (Manrope):** `headline-md` (1.75rem) provides the editorial structure. Use tight letter-spacing (-0.02em) to maintain a "tech" feel.
*   **Title & Body (Inter):** `title-md` (1.125rem) for section headers; `body-md` (0.875rem) for all transactional data. 
*   **Hierarchy Note:** Always pair `Text Primary` (`#F3F4F6`) for amounts with `Text Muted` (`#9CA3AF`) for labels to ensure the eye hits the most important data first.

---

## 4. Elevation & Depth

### The Layering Principle
Depth is achieved through **Tonal Layering**, not shadows. A "floating" card shouldn't have a black drop shadow; it should simply be a lighter shade of grey (`surface-container-high`) than the layer beneath it.

### Ambient Shadows
If a component must float (e.g., a bottom sheet or modal), use a **Luminous Shadow**:
*   **Blur:** 40px - 60px
*   **Opacity:** 4% - 8%
*   **Color:** Use a tinted version of the surface color, never pure black. This mimics natural ambient light refraction in a dark room.

### Glassmorphism
Apply `backdrop-filter: blur(12px)` to any element that sits above the main content flow. This ensures the "Cero Green" accents from the background bleed through subtly, maintaining a cohesive color story even in complex layouts.

---

## 5. Component Signatures

### Buttons
*   **Primary:** Solid `primary-container` (`#05E699`) with `on-primary` text. No border. `0.375rem` (md) corner radius.
*   **Secondary:** `surface-container-highest` background with `Text Primary` labels. This creates a subtle "button-within-a-surface" look.
*   **Tertiary:** Ghost style. No background, only `primary` text.

### Cards & Lists
*   **Strict Rule:** No dividers. Separate list items using `spacing-4` (1rem) of vertical white space or by alternating background tones between `surface-container-low` and `surface-container`.
*   **Interactions:** On hover, transition the background to `surface-hover` (`#2A2F37`) and shift the entire card 2px upward for a tactile "lift" effect.

### Input Fields
*   **Idle:** `surface-container-lowest` background with a 10% opacity `outline-variant`.
*   **Focus:** The "Ghost Border" becomes `primary` at 40% opacity with a subtle 2px outer glow of the same color.

### Financial Progress Bars
*   **Track:** `surface-container-highest`.
*   **Indicator:** A gradient from `primary-fixed` to `primary-container`. This gives the data a "liquid light" feel.

---

## 6. Do’s and Don’ts

### Do
*   **Use asymmetrical padding.** For example, give a header more top padding than bottom padding to create an "editorial" flow.
*   **Embrace white space.** High-end design requires "room to breathe." If a screen feels crowded, increase the `spacing` tokens.
*   **Layer your surfaces.** Always place lighter surfaces on darker ones to indicate importance.

### Don’t
*   **Don't use 100% opaque borders.** They create a "grid-lock" feel that destroys the premium aesthetic.
*   **Don't use "System Blue" or "System Gray."** Stick strictly to the desaturated, tech-focused palette provided.
*   **Don't use standard drop shadows.** Use tonal shifts or diffused ambient blurs only.
*   **Don't center-align everything.** Use left-aligned typography for a more professional, structured look.
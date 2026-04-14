# CeroBase Client - AI Agent Guide

This document provides specialized context and rules for AI agents working within the `Fintrack.Client` directory. It overrides general knowledge and ensures code aligns with the established frontend architecture.

## 1. Context & Tech Stack
The `Fintrack.Client` is a Single Page Application (SPA) built with:
- **Framework:** Vue 3 (Composition API ONLY, `<script setup>`)
- **Language:** TypeScript
- **State Management:** Pinia
- **Routing:** Vue Router
- **Styling:** Tailwind CSS 3, PostCSS, SCSS
- **Build Tool:** Vite
- **Testing:** Vitest, Vue Test Utils, Playwright

## 2. Architecture: 4-Domain Client Pattern
The client strictly adheres to a 4-domain architecture. Agents MUST respect these boundaries:
- **`app/`**: Private, authenticated application core (dashboard, transactions, budgets).
- **`auth/`**: Authentication and authorization flows (login, register, forgot password).
- **`public/`**: Public-facing pages (landing, pricing, about).
- **`admin/`**: (If applicable) Administrative features.

Shared resources (components, composables, stores, services) that cross domain boundaries belong in the **`shared/`** directory.

## 3. UI-Specific Skills
When working in this directory, leverage the following skills (located in `../.agents/skills/`):

- [`vue-best-practices`](../.agents/skills/vue-best-practices/SKILL.md): Core guidelines for Vue 3 (Composition API).
- [`vue-client-architecture`](../.agents/skills/vue-client-architecture/SKILL.md): Enforces 4-domain client architecture.
- [`vue-router-best-practices`](../.agents/skills/vue-router-best-practices/SKILL.md): State and routing guidelines.
- [`vue-pinia-best-practices`](../.agents/skills/vue-pinia-best-practices/SKILL.md): State management patterns.
- [`vue-testing-best-practices`](../.agents/skills/vue-testing-best-practices/SKILL.md): Testing with Vitest and Vue Test Utils.
- [`vue-debug-guides`](../.agents/skills/vue-debug-guides/SKILL.md): Handling runtime errors, hydration issues, etc.
- [`create-adaptable-composable`](../.agents/skills/create-adaptable-composable/SKILL.md): Standards for writing robust composables.
- [`pwa-development`](../.agents/skills/pwa-development/SKILL.md): Progressive Web Apps guidelines.

## 4. Coding Conventions
- **Composition API:** Never use the Options API.
- **TypeScript:** Enforce strict typing. Use interfaces for complex objects.
- **Components:** Keep components small, focused, and pure where possible.
- **State:** Use Pinia for global state; avoid deeply nested prop drilling.
- **Styling:** Prefer Tailwind CSS utility classes; use SCSS only when necessary for complex, scoped styling.

## 5. Design Integration Guidelines
When translating design prototypes (from the `designs/` directory) into Vue views:
- **Tailwind Config Awareness:** Never blindly copy Tailwind classes from the raw HTML prototypes without verifying `tailwind.config.js`. Pay special attention to custom mappings (e.g., if the app overrides `xl` border-radius to mean 24px, you must use `rounded` to achieve an 8px visual look from the design). Focus on **visual equivalence**, not literal class name matching.
- **Layouts vs. Views:** Distinctly separate global structure from page content. TopAppBars and BottomNavBars generally belong in Layout components (like `AppLayout.vue` or `FocusedLayout.vue`), while the specific data grids and localized footers belong in the View.
- **Mobile Safeties:** Always respect device safe areas. Use custom utilities like `pb-safe` strictly on **global Layout wrappers** or **fixed Navigation components** (like `AppLayout.vue` or `Sidebar.vue`). Never apply safe-area paddings inside individual Views, as the Layout should handle device boundaries.
- **Cohesive Theming:** Ensure newly integrated designs adhere to the root theme tokens (e.g., using `bg-background text-on-background`) rather than hardcoding colors that conflict with global layouts.

---
name: vue-view-refactor
description: Step-by-step workflow for refactoring, splitting, or cleaning up existing Vue 3 views in Fintrack.Client—extracting child components and composables, deduplicating UI with shared components (SurfaceCard, AppButton, AmountInputCard), removing unused imports and dead code, fixing domain placement and routes, and verifying tests. Use whenever the user mentions refactoring a view, breaking up a large SFC, reducing template complexity, tech debt in a page, migrating duplicate markup, removing unused code, reorganizing views, or improving maintainability of Fintrack screens—even if they do not say "refactor" explicitly. Also use when comparing "before/after" structure for a page or moving logic out of a `.vue` file.
---

# Vue View Refactor (Fintrack.Client)

Use this skill **before** editing files when the task is about changing structure or maintainability of an **existing** route-level view (`.vue` under `src/*/views/` or similar), not when greenfield-building a new screen from scratch (combine with `vue-view-generator` for that).

## 0. Load companion skills (in order)

1. **`vue-best-practices`** — Composition API, `<script setup>`, component boundaries, when to split.
2. **`vue-view-generator`** — Layout rules, shared components (`SurfaceCard`, `AppButton`, `AmountInputCard`, `LoadingIndicator`, etc.), spacing, UUID typing.
3. **`vue-client-architecture`** — If moving files between `public` / `auth` / `app` / `admin`, or changing route modules.
4. **`vue-router-best-practices`** — If navigation guards, params, or lazy imports change.
5. **`vue-testing-best-practices`** — If specs exist or must be added/updated next to the view.

Do not duplicate those documents here; follow their rules when this skill says to apply them.

## 1. Clarify the goal (short)

Answer in your head or in the reply:

- **Scope**: One view file, or a small cluster (view + 1–2 local components)?
- **Why**: Readability, reuse, testability, performance (rare), or consistency with the design system?
- **Non-goals**: Explicitly avoid drive-by changes in unrelated views, APIs, or backend projects.

If intent is ambiguous or the change is mostly new product behavior, use **`brainstorming`** first, then return here.

## 2. Read-only inventory

1. Open the **view SFC** and note: template sections (hero, list, forms, modals), `computed`/`watch`/lifecycle density, and API calls.
2. Find **route(s)** that import this view (`routes.ts` in the same domain, and root router if needed). Record `name`, `path`, `meta` (title, layout).
3. Find **tests** (`*.spec.ts` near the view or component). Note what they assert today.
4. List **duplicated** markup or classes that match patterns in `vue-view-generator` (raw buttons vs `AppButton`, ad-hoc cards vs `SurfaceCard`, amount fields vs `AmountInputCard`).

## 3. Choose split lines

Prefer splits that match **one clear responsibility** each (see `vue-best-practices`):

| Candidate child | Responsibility |
|-----------------|----------------|
| Presentational block | Markup + props/emits only; no API |
| Form section | Fields + validation display; may emit updates |
| List / row item | Repeated row UI |
| Composable `useXxx` | Shared state, fetch, derived values, side effects |

Keep **route views** as orchestrators: compose children and composables; avoid new API calls deep in dumb components unless the project already does that.

## 4. Execute the refactor (typical order)

1. **Types** — Extract shared interfaces to a `types.ts` or colocated file if multiple files need them.
2. **Composable** — If orchestration is > ~80 lines or reused, move to `composables/use<Feature>.ts` in the same feature folder.
3. **Child components** — Under `src/<domain>/components/<feature>/` for domain-specific UI; `src/app/components/common/` only for true cross-feature reuse (align with `vue-client-architecture`).
4. **Replace ad-hoc UI** — Swap in `SurfaceCard`, `AppButton`, `AmountInputCard`, etc., per `vue-view-generator` so borders/padding stay consistent.
5. **View root** — Do **not** add global container padding / safe-area on the view root; layouts own that.
6. **Routes** — Update lazy `import()` paths if files moved; keep `meta` accurate.
7. **IDs** — Domain entity IDs stay `string` (UUIDs); no numeric parsing from `route.params`.
8. **Remove unused code** — Before finishing, strip dead weight introduced or left behind by the refactor:
   - **Script**: Unused imports (`vue` helpers, components, composables), `ref`/`computed`/`watch`/`function` symbols never read, props/emits that no longer apply after extraction.
   - **Template**: Unused conditional branches, duplicate wrappers, commented-out markup (delete unless the user asked to keep a deliberate TODO).
   - **Style**: Scoped rules that no longer match any class in the file after markup moves (remove orphaned selectors).
   - **Tests**: Imports, mocks, and assertions that only served removed code.

   Rely on ESLint/TypeScript for “unused” signals; do not delete code that is only used indirectly (e.g. `defineExpose`, template refs by string) without confirming.

## 5. Verification checklist

- [ ] `vue-tsc` / typecheck passes for `Fintrack.Client`.
- [ ] ESLint passes for touched files (including no unused vars/imports where the rule applies).
- [ ] Tests updated or added for behavior that moved (mount paths, stubs for new children); test-only dead code removed.
- [ ] Unused code removed per section 4.8 (imports, script symbols, template, scoped CSS, tests).
- [ ] Manual smoke: route still loads, primary actions and back navigation behave as before.

## 6. What not to do

- Do not refactor the **server** or **Pinia** stores in the same pass unless the user asked for it.
- Do not rename routes or delete features without explicit user confirmation.
- Do not strip accessibility (labels, `input-id` pairs, modal patterns) when extracting components.

## 7. Suggested commit framing

Use **`conventional-commit`** when committing: `refactor(client): …` scoped to the view or feature.

---

**Bundled resources:** See `evals/evals.json` for example evaluation prompts when tuning this skill or testing agents against refactor tasks.

---
name: vue-view-generator
description: Enforces Fintrack UI guidelines. Use this skill whenever generating or restructuring a Vue view/page inside Fintrack.Client. It provides specific instructions on layout selection, common component usage, padding rules, and UUID identifier practices.
---

# Vue View Generator

Whenever you are asked to create a new page, view, or build a UI in the `Fintrack.Client` project, you MUST strictly follow the guidelines regarding layouts, spacing, and shared components defined in this skill.

## 1. Layout Selection & Integration

Vue pages in Fintrack must be nested inside an appropriate layout to inherit the correct application frame.

- **`AppLayout`**: Use this for top-level navigational pages that require the global bottom navigation bar (e.g., Dashboards, Activity lists).
- **`FocusedLayout`**: Use this for task-oriented views, detailed forms, CRUD operations, or specialized edit pages that should NOT display the global bottom navigation. It provides its own top back-navigation header.

**Action**: When creating a new view, ensure the route defined in `src/app/routes.ts` or corresponding domain routes assigns the correct layout as a parent.

## 2. Spacing & Padding Rules

- **Do NOT declare global container paddings** (`px-4`, `pt-20`, `max-w-2xl`, `mx-auto`) or mobile safe areas (`pb-safe`) on the root wrapper of the view component.
- The parent layouts (`AppLayout` or `FocusedLayout`) already enforce these bounds automatically to guarantee visual consistency.
- **Internal Spacing**: Views should only manage spacing *between* their internal elements, typically using utility classes like `space-y-6` on the main container or gap/margin classes on children.

## 3. Shared UI Components

Fintrack has established shared UI components that must be utilized to maintain a consistent experience. Before building custom UI logic for these scenarios, import them from `src/app/components/common/`.

- **Page Data Fetching**: Use `<LoadingIndicator>` while waiting for asynchronous context or component data.
  ```vue
  <LoadingIndicator :is-loading="isLoading" message="Cargando Datos" />
  ```
- **Destructive Actions**: Use `<ConfirmationModal>` to wrap delete, discard, or irreversible actions instead of generic alerts.
  ```vue
  <ConfirmationModal
    :show="showDeleteConfirm"
    title="¿Eliminar Registro?"
    description="Esta acción eliminará el registro de forma permanente."
    confirm-text="Eliminar"
    cancel-text="Cancelar"
    :is-loading="isDeleting"
    variant="danger"
    @confirm="handleDelete"
    @cancel="showDeleteConfirm = false"
  >
    <template #item-preview v-if="selectedItem">
      <!-- Optional layout for showing what you are confirming against -->
    </template>
  </ConfirmationModal>
  ```
- **Category Selection**: When a form requires expense category selection, use `<CategorySelector>` which handles nested groups, styling, and search internally.
- **Surface cards (grouped content blocks)**: Use `<SurfaceCard>` whenever you need a bordered, padded block on the surface color (settings rows, toggles, summaries, etc.). It only provides the shared shell; put your markup in the default slot.
  ```vue
  <SurfaceCard>
    <div class="flex items-center justify-between">
      <!-- any structure -->
    </div>
  </SurfaceCard>
  ```

*(Note: Provide imports relative to `@/app/components/common/...`)*

## 4. Data Type Standards (UUIDs)

All resource identifiers representing domain entities (e.g., `budgetId`, `categoryId`, `expenseId`) have been migrated to UUIDs.

- You MUST statically type these IDs as `string`.
- Do NOT use `parseInt()`, `Number()`, or any numeric casting when extracting IDs from route parameters.
- Handle fallback ID types manually by ensuring they are typed as `string | null` instead of `number | null`.

## 5. Route Metadata Setup

In the route definitions (`routes.ts`), accurately define the `meta` object to drive the Layout's header logic.

```typescript
// Typical Route Example for AppLayout
{
  path: 'dashboard',
  name: 'Dashboard',
  component: () => import('@/app/views/dashboard/DashboardView.vue'),
  meta: { title: 'Dashboard', subtitle: '¡Bienvenido de nuevo!', showMenu: true }
}

// Typical Route Example for FocusedLayout
{
  path: 'new',
  name: 'BudgetCreate',
  component: () => import('@/app/views/budget/form/BudgetFormView.vue'),
  meta: { title: 'Configurar Presupuesto', subtitle: 'Define tus límites de gasto.' }
}
```
- Set `showMenu: true` primarily for routes using `AppLayout`.

By following this skill, Fintrack maintains a robust, centralized design system where components and layouts serve their intended responsibilities cleanly.

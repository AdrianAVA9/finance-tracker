---
name: vue-client-architecture
description: Enforces the 4-domain client architecture (public, auth, app, admin). Use this skill whenever generating new views, components, layouts, or routes for the Vue frontend to ensure they are placed in the correct domain folder.
---

# Vue Client Architecture Structure

This project strictly follows a 4-domain modular architecture for the Vue frontend. Whenever you are asked to create a new page, view, component, or route, you MUST place it in the appropriate domain folder under `src/`. 

Avoid adding files to a generic `src/views/` folder. Every feature must belong to a domain.

## The 4 Domains

### 1. `src/public`
**Purpose**: The "Face" of the App. Landing pages, informative pages, pricing, contact us, privacy policy, etc.
- **Routing**: Maps to root `/` and open paths.
- **Components**: Components specific to marketing or public landing pages go in `src/public/components`.

### 2. `src/auth`
**Purpose**: The "Gatekeeper". All views related to authentication workflows (login, registration, forgot password, 2FA).
- **Routing**: Prefix with `/auth` (e.g., `/auth/login`).
- **Layout**: Uses a specific AuthLayout.

### 3. `src/app`
**Purpose**: The "Core Engine". The main user application where the primary business logic lives (e.g., dashboards, budgets, expenses).
- **Routing**: Prefix with `/app` (e.g., `/app/dashboard`).
- **Security**: Must be protected by authentication routing guards.

### 4. `src/admin`
**Purpose**: The "Command Center". Site administration, user management, global analytics, and master settings.
- **Routing**: Prefix with `/admin` (e.g., `/admin/users`).
- **Security**: Must be protected by strict admin-role routing guards. Code for this branch should ideally be lazy-loaded to prevent standard users from downloading admin bundles.

## Implementation Rules
1. **Routing**: Each domain should maintain its own route definitions in `src/<domain>/routes.ts`. These are then imported and orchestrated by the main `src/router/index.ts`.
2. **Components**: If a component is ONLY used by one domain, it belongs in `src/<domain>/components/`. If it is used across multiple domains (e.g., a generic primary button), place it in the global `src/components/` directory.
3. **Layouts**: Domains usually have their own root layout (e.g., `AppLayout.vue`, `PublicLayout.vue`).

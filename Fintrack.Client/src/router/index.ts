import { createRouter, createWebHistory } from 'vue-router';
import { useAuth } from '@/composables/useAuth';
import { routes as publicRoutes } from '@/public/routes';
import { routes as authRoutes } from '@/auth/routes';
import { routes as appRoutes } from '@/app/routes';

const routes = [
  ...publicRoutes,
  ...authRoutes,
  ...appRoutes,
  { path: '/:pathMatch(.*)*', redirect: '/' }
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

router.beforeEach(async (to, from, next) => {
  const { isAuthenticated, checkSession } = useAuth();

  // Resolve auth state once before making any routing decision
  if (!isAuthenticated.value) {
    await checkSession();
  }

  const isAuthDomain   = to.path.startsWith('/auth'); // login, register, forgot/reset password
  const isAppDomain    = to.path.startsWith('/app');  // protected app pages
  const isAdminDomain  = to.path.startsWith('/admin'); // protected admin pages
  const isPublicDomain = !isAuthDomain && !isAppDomain && !isAdminDomain; // '/' landing etc.

  // Authenticated users visiting the public landing or any auth page → send to app
  if ((isPublicDomain || isAuthDomain) && isAuthenticated.value) {
    return next('/app');
  }

  // Unauthenticated users visiting protected domains → gate with login
  if ((isAppDomain || isAdminDomain) && !isAuthenticated.value) {
    return next('/auth/login');
  }

  next();
});

export default router;

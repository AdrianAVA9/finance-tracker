import { h } from 'vue';
import { createRouter, createWebHistory } from 'vue-router';
import type { RouteRecordRaw } from 'vue-router';
import { useAuth } from '@/composables/useAuth';
import { getAuthRedirectForPath, getInstalledPwaRedirectForPath } from '@/router/authRedirect';
import { routes as publicRoutes } from '@/public/routes';
import { routes as authRoutes } from '@/auth/routes';
import { routes as appRoutes } from '@/app/routes';

/** If the host fell through to the SPA for these URLs, repeat the request so static files in wwwroot / Vite public can be served. */
const voidStatic = { render: () => h('div', { 'aria-hidden': 'true' }) };
const staticFileEscapeRoutes: RouteRecordRaw[] = [
  { path: '/robots.txt', name: 'WellKnownRobots' },
  { path: '/sitemap.xml', name: 'WellKnownSitemap' }
].map((r) => ({
  ...r,
  beforeEnter: (to) => {
    window.location.replace(to.fullPath);
    return false;
  },
  component: voidStatic
}));

const routes = [
  ...publicRoutes,
  ...authRoutes,
  ...appRoutes,
  ...staticFileEscapeRoutes,
  { path: '/:pathMatch(.*)*', redirect: '/' }
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
  scrollBehavior(to, from, savedPosition) {
    if (savedPosition) {
      return savedPosition;
    } else {
      return { top: 0 };
    }
  },
});

router.beforeEach((to, from, next) => {
  const pwaEntry = getInstalledPwaRedirectForPath(to.path);
  if (pwaEntry) {
    return next({ path: pwaEntry, replace: true });
  }

  const { isAuthenticated, isInitialized } = useAuth();

  // Allow the initial navigation to proceed without blocking
  // App.vue shows SessionBootstrapOverlay while !isInitialized
  if (!isInitialized.value) {
    return next();
  }

  const redirect = getAuthRedirectForPath(to.path, isAuthenticated.value);
  if (redirect) {
    return next(redirect);
  }

  next();
});

export default router;

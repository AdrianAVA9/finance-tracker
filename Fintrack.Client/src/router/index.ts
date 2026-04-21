import { createRouter, createWebHistory } from 'vue-router';
import { useAuth } from '@/composables/useAuth';
import { getAuthRedirectForPath } from '@/router/authRedirect';
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
  scrollBehavior(to, from, savedPosition) {
    if (savedPosition) {
      return savedPosition;
    } else {
      return { top: 0 };
    }
  },
});

router.beforeEach((to, from, next) => {
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

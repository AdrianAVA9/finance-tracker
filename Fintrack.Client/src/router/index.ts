import { createRouter, createWebHistory } from 'vue-router';
import { useAuth } from '@/composables/useAuth';
import { routes as publicRoutes } from '@/public/routes';
import { routes as authRoutes } from '@/auth/routes';
import { routes as appRoutes } from '@/app/routes';
import { routes as budgetRoutes } from '@/app/budgets/routes';
import { routes as expenseRoutes } from '@/app/expenses/routes';

const routes = [
  ...publicRoutes,
  ...authRoutes,
  ...appRoutes,
  ...budgetRoutes,
  ...expenseRoutes,
  // Dashboard Redirect
  { 
    path: '/dashboard', 
    redirect: '/app/budgets' // Redirect to budgets for now until dashboard exists
  },
  // Fallback
  { path: '/:pathMatch(.*)*', redirect: '/' }
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

router.beforeEach(async (to, from, next) => {
  const { isAuthenticated, checkSession } = useAuth();
  
  // We check the session on every route change if they aren't authenticated yet
  // but navigating to a protected route
  const publicPages = ['/auth/login', '/auth/register', '/'];
  const authRequired = !publicPages.includes(to.path);

  if (authRequired) {
    if (!isAuthenticated.value) {
      await checkSession();
    }
    
    if (!isAuthenticated.value) {
      return next('/auth/login');
    }
  }

  next();
});

export default router;

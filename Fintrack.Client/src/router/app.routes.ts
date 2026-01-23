import type { RouteRecordRaw } from 'vue-router';
import AppLayout from '@/layouts/AppLayout.vue';

export const appRoutes: RouteRecordRaw[] = [
  {
    path: '/app', // Base path for app
    alias: '/dashboard', // Convenience alias
    component: AppLayout,
    children: [
      {
        path: '',
        name: 'Dashboard',
        component: () => import('@/budgets/list/BudgetListView.vue'), // Using BudgetList as Dashboard for now
        meta: { layout: AppLayout, requiresAuth: true }
      }
    ]
  }
];

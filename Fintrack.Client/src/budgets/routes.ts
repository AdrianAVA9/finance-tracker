import type { RouteRecordRaw } from 'vue-router';
import AppLayout from '@/layouts/AppLayout.vue';

export const routes: RouteRecordRaw[] = [
  {
    path: '/budgets',
    component: AppLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        name: 'BudgetList',
        component: () => import('@/budgets/list/BudgetListView.vue'), // Placeholder
        meta: { layout: AppLayout }
      },
      {
        path: 'create',
        name: 'BudgetCreate',
        component: () => import('@/budgets/list/BudgetListView.vue'), // Placeholder
        meta: { layout: AppLayout }
      },
      {
        path: ':id',
        name: 'BudgetDetails',
        component: () => import('@/budgets/list/BudgetListView.vue'), // Placeholder
        meta: { layout: AppLayout }
      }
    ]
  }
];

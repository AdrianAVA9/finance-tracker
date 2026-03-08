import type { RouteRecordRaw } from 'vue-router';
import AppLayout from '@/app/layouts/AppLayout.vue';

export const routes: RouteRecordRaw[] = [
  {
    path: '/app/budgets',
    component: AppLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        name: 'BudgetList',
        component: () => import('@/app/budgets/list/BudgetListView.vue'), // Placeholder
        meta: { layout: AppLayout }
      },
      {
        path: 'create',
        name: 'BudgetCreate',
        component: () => import('@/app/budgets/list/BudgetListView.vue'), // Placeholder
        meta: { layout: AppLayout }
      },
      {
        path: ':id',
        name: 'BudgetDetails',
        component: () => import('@/app/budgets/list/BudgetListView.vue'), // Placeholder
        meta: { layout: AppLayout }
      }
    ]
  }
];

import type { RouteRecordRaw } from 'vue-router';
import AppLayout from '@/layouts/AppLayout.vue';

export const routes: RouteRecordRaw[] = [
  {
    path: '/expenses',
    component: AppLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        name: 'ExpenseList', // Or Manual View
        component: () => import('@/budgets/list/BudgetListView.vue'), // Placeholder
        meta: { layout: AppLayout }
      },
      {
        path: 'manual',
        name: 'ManualExpense',
        component: () => import('@/budgets/list/BudgetListView.vue'), // Placeholder
        meta: { layout: AppLayout }
      },
      {
        path: 'upload',
        name: 'InvoiceUpload',
        component: () => import('@/budgets/list/BudgetListView.vue'), // Placeholder
        meta: { layout: AppLayout }
      },
      {
        path: 'review/:id',
        name: 'InvoiceReview',
        component: () => import('@/budgets/list/BudgetListView.vue'), // Placeholder
        meta: { layout: AppLayout }
      }
    ]
  }
];

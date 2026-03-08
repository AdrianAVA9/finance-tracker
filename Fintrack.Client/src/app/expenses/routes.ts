import type { RouteRecordRaw } from 'vue-router';
import AppLayout from '@/app/layouts/AppLayout.vue';

export const routes: RouteRecordRaw[] = [
  {
    path: '/app/expenses',
    component: AppLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        name: 'ExpenseList', // Or Manual View
        component: () => import('@/app/budgets/list/BudgetListView.vue'), // Placeholder
        meta: { layout: AppLayout }
      },
      {
        path: 'manual',
        name: 'ManualExpense',
        component: () => import('@/app/budgets/list/BudgetListView.vue'), // Placeholder
        meta: { layout: AppLayout }
      },
      {
        path: 'upload',
        name: 'InvoiceUpload',
        component: () => import('@/app/budgets/list/BudgetListView.vue'), // Placeholder
        meta: { layout: AppLayout }
      },
      {
        path: 'review/:id',
        name: 'InvoiceReview',
        component: () => import('@/app/budgets/list/BudgetListView.vue'), // Placeholder
        meta: { layout: AppLayout }
      }
    ]
  }
];

import type { RouteRecordRaw } from 'vue-router';
import AppLayout from '@/app/layouts/AppLayout.vue';

export const routes: RouteRecordRaw[] = [
  {
    path: '/app',
    component: AppLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        redirect: '/app/dashboard'
      },
      {
        path: 'dashboard',
        name: 'Dashboard',
        component: () => import('@/app/views/DashboardView.vue'),
        meta: { title: 'Dashboard', subtitle: 'Welcome back' }
      },
      {
        path: 'expenses/new',
        name: 'ExpenseRegistration',
        component: () => import('@/app/views/ExpenseRegistrationView.vue'),
        meta: { title: 'Register Expense', subtitle: 'Record a new transaction or log an itemized receipt.' }
      },
      {
        path: 'budgets',
        name: 'BudgetRegistration',
        component: () => import('@/app/views/BudgetRegistrationView.vue'),
        meta: { title: 'Plan Presupuestario', subtitle: 'Define tus límites mensuales y mantén el control.' }
      }
    ]
  }
];

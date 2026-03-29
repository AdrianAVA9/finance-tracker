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
        meta: { title: 'Dashboard', subtitle: '¡Bienvenido de nuevo!' }
      },
      {
        path: 'activity',
        name: 'Activity',
        component: () => import('@/app/views/TransactionsView.vue'),
        meta: { title: 'Actividad', subtitle: 'Revisa tu historial de movimientos.' }
      },
      {
        path: 'expenses/new',
        name: 'ExpenseRegistration',
        component: () => import('@/app/views/ExpenseRegistrationView.vue'),
        meta: { title: 'Registrar Gasto', subtitle: 'Registra un nuevo gasto o sube un recibo desglosado.' }
      },
      {
        path: 'incomes/new',
        name: 'IncomeRegistration',
        component: () => import('@/app/views/IncomeRegistrationView.vue'),
        meta: { title: 'Registro de Ingresos', subtitle: 'Complete los detalles para su nueva entrada de capital.' }
      },
      {
        path: 'budgets',
        name: 'BudgetRegistration',
        component: () => import('@/app/views/BudgetRegistrationView.vue'),
        meta: { title: 'Plan Presupuestario', subtitle: 'Define tus límites mensuales y mantén el control.' }
      },
      {
        path: 'budgets/new',
        name: 'BudgetCreate',
        component: () => import('@/app/views/BudgetFormView.vue'),
        meta: { title: 'Configurar Presupuesto', subtitle: 'Define tus límites de gasto.' }
      },
      {
        path: 'budgets/:id/edit',
        name: 'BudgetEdit',
        component: () => import('@/app/views/BudgetFormView.vue'),
        meta: { title: 'Ajustar Presupuesto', subtitle: 'Refina tus metas financieras.' }
      },
      {
        path: 'budgets/:id',
        name: 'BudgetDetails',
        component: () => import('@/app/views/BudgetDetailsView.vue'),
        meta: { title: 'Detalles del Presupuesto', subtitle: 'Revisa tu balance e historial.' }
      }
    ]
  }
];

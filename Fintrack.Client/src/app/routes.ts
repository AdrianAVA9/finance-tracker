import type { RouteRecordRaw } from 'vue-router';
import AppLayout from '@/app/layouts/AppLayout.vue';
import FocusedLayout from '@/app/layouts/FocusedLayout.vue';

export const routes: RouteRecordRaw[] = [
  {
    path: '/app/focused',
    component: AppLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        name: 'BudgetSimulationDesignPreview',
        component: () => import('@/app/views/budget/BudgetSimulationDesignView.vue'),
        meta: { title: 'Simulador de Presupuesto' }
      }
    ]
  },
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
        component: () => import('@/app/views/dashboard/DashboardView.vue'),
        meta: { title: 'Dashboard', subtitle: '¡Bienvenido de nuevo!', showMenu: true }
      },
      {
        path: 'activity',
        name: 'Activity',
        component: () => import('@/app/views/transaction/TransactionsView.vue'),
        meta: { title: 'Actividad', subtitle: 'Revisa tu historial de movimientos.', showMenu: true }
      },
      {
        path: 'expenses/new',
        name: 'ExpenseRegistration',
        component: () => import('@/app/views/expense/ExpenseRegistrationView.vue'),
        meta: { title: 'Registrar Gasto', subtitle: 'Registra un nuevo gasto o sube un recibo desglosado.' }
      },
      {
        path: 'expenses/:id/edit',
        name: 'ExpenseEdit',
        component: () => import('@/app/views/expense/ExpenseRegistrationView.vue'),
        meta: { title: 'Editar Gasto', subtitle: 'Modifica los detalles de tu gasto.' }
      },
      {
        path: 'incomes/new',
        name: 'IncomeRegistration',
        component: () => import('@/app/views/income/IncomeRegistrationView.vue'),
        meta: { title: 'Registro de Ingresos', subtitle: 'Complete los detalles para su nueva entrada de capital.' }
      },
      {
        path: 'incomes/:id/edit',
        name: 'IncomeEdit',
        component: () => import('@/app/views/income/IncomeRegistrationView.vue'),
        meta: { title: 'Editar Ingreso', subtitle: 'Ajusta los detalles de tu entrada de capital.' }
      },
      {
        path: 'budgets',
        name: 'BudgetRegistration',
        component: () => import('@/app/views/budget/BudgetRegistrationView.vue'),
        meta: { title: 'Plan Presupuestario', subtitle: 'Define tus límites mensuales y mantén el control.', showMenu: true }
      },
      {
        path: 'budgets/new',
        name: 'BudgetCreate',
        component: () => import('@/app/views/budget/BudgetFormView.vue'),
        meta: { title: 'Configurar Presupuesto', subtitle: 'Define tus límites de gasto.' }
      },
      {
        path: 'budgets/:id/edit',
        name: 'BudgetEdit',
        component: () => import('@/app/views/budget/BudgetFormView.vue'),
        meta: { title: 'Ajustar Presupuesto', subtitle: 'Refina tus metas financieras.' }
      },
      {
        path: 'budgets/:id',
        name: 'BudgetDetails',
        component: () => import('@/app/views/budget/BudgetDetailsView.vue'),
        meta: { title: 'Detalles del Presupuesto', subtitle: 'Revisa tu balance e historial.', showMenu: true }
      },
      {
        path: 'budgets/simulate',
        name: 'BudgetSimulation',
        component: () => import('@/app/views/budget/BudgetSimulationView.vue'),
        meta: { title: 'Simulador de Presupuesto', subtitle: 'Ajusta y visualiza el impacto en tiempo real.' }
      }
    ]
  }
];

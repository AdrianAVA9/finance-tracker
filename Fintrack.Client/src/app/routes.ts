import type { RouteRecordRaw } from 'vue-router';
import AppLayout from '@/app/layouts/AppLayout.vue';
import FocusedLayout from '@/app/layouts/FocusedLayout.vue';
import { RouterView } from 'vue-router';

export const routes: RouteRecordRaw[] = [
  {
    path: '/app/budgets/bulk',
    component: FocusedLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        name: 'BudgetBulkEdit',
        component: () => import('@/app/views/budget/bulk/BudgetBulkEditView.vue'),
        meta: { title: 'Distribución de Gastos', subtitle: 'Ajusta y visualiza el impacto en tiempo real.' }
      }
    ]
  },
  {
    path: '/app/budgets/new',
    component: FocusedLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        name: 'BudgetCreate',
        component: () => import('@/app/views/budget/form/BudgetFormView.vue'),
        meta: { title: 'Registrar Presupuesto', subtitle: 'Planificación de Precisión' }
      }
    ]
  },
  {
    path: '/app/budgets/:id/edit',
    component: FocusedLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        name: 'BudgetEdit',
        component: () => import('@/app/views/budget/form/BudgetFormView.vue'),
        meta: { title: 'Ajustar Presupuesto', subtitle: 'Refina tus metas financieras.' }
      }
    ]
  },
  {
    path: '/app/expenses/new',
    component: FocusedLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        name: 'ExpenseRegistration',
        component: () => import('@/app/views/expense/ExpenseRegistrationView.vue'),
        meta: { title: 'Registrar Gasto', subtitle: 'Registra un nuevo gasto o sube un recibo desglosado.' }
      }
    ]
  },
  {
    path: '/app/expenses/:id/edit',
    component: FocusedLayout,
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        name: 'ExpenseEdit',
        component: () => import('@/app/views/expense/ExpenseRegistrationView.vue'),
        meta: { title: 'Editar Gasto', subtitle: 'Modifica los detalles de tu gasto.' }
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
        component: RouterView,
        children: [
          {
            path: '',
            name: 'BudgetList',
            component: () => import('@/app/views/budget/list/BudgetListView.vue'),
            meta: { title: 'Plan Presupuestario', subtitle: 'Define tus límites mensuales y mantén el control.', showMenu: true }
          },

          {
            path: ':id',
            name: 'BudgetDetails',
            component: () => import('@/app/views/budget/details/BudgetDetailsView.vue'),
            meta: { title: 'Detalles del Presupuesto', subtitle: 'Revisa tu balance e historial.', showMenu: true }
          },
        ]
      }
    ]
  }
];

import type { RouteRecordRaw } from 'vue-router';
import AppShell from '@/app/layouts/AppShell.vue';
import AppLayout from '@/app/layouts/AppLayout.vue';
import FocusedLayout from '@/app/layouts/FocusedLayout.vue';
import { RouterView } from 'vue-router';

export const routes: RouteRecordRaw[] = [
  {
    path: '/app',
    component: AppShell,
    meta: { requiresAuth: true },
    children: [
      {
        path: 'budgets/bulk',
        component: FocusedLayout,
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
        path: 'budgets/new',
        component: FocusedLayout,
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
        path: 'budgets/:id/edit',
        component: FocusedLayout,
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
        path: 'budgets/:id',
        component: FocusedLayout,
        children: [
          {
            path: '',
            name: 'BudgetDetails',
            component: () => import('@/app/views/budget/details/BudgetDetailsView.vue'),
            meta: { title: 'Detalles del Presupuesto', subtitle: 'Revisa tu balance e historial.' }
          }
        ]
      },
      {
        path: 'expenses/new',
        component: FocusedLayout,
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
        path: 'expenses/:id/edit',
        component: FocusedLayout,
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
        path: 'incomes/new',
        component: FocusedLayout,
        children: [
          {
            path: '',
            name: 'IncomeRegistration',
            component: () => import('@/app/views/income/IncomeRegistrationView.vue'),
            meta: { title: 'Registro de Ingresos', subtitle: 'Complete los detalles para su nueva entrada de capital.' }
          }
        ]
      },
      {
        path: 'incomes/:id/edit',
        component: FocusedLayout,
        children: [
          {
            path: '',
            name: 'IncomeEdit',
            component: () => import('@/app/views/income/IncomeRegistrationView.vue'),
            meta: { title: 'Editar Ingreso', subtitle: 'Ajusta los detalles de tu entrada de capital.' }
          }
        ]
      },
      {
        path: 'settings',
        component: FocusedLayout,
        children: [
          {
            path: '',
            name: 'Settings',
            component: () => import('@/app/views/settings/SettingsView.vue'),
            meta: { title: 'Ajustes', hasHelp: true }
          }
        ]
      },
      {
        path: '',
        component: AppLayout,
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
            path: 'budgets',
            component: RouterView,
            children: [
              {
                path: '',
                name: 'BudgetList',
                component: () => import('@/app/views/budget/list/BudgetListView.vue'),
                meta: { title: 'Plan Presupuestario', subtitle: 'Define tus límites mensuales y mantén el control.', showMenu: true }
              },
            ]
          }
        ]
      }
    ]
  }
];

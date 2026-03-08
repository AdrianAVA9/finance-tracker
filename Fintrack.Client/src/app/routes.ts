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
        redirect: '/app/budgets'
      }
    ]
  }
];

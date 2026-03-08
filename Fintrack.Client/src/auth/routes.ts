import type { RouteRecordRaw } from 'vue-router';
import AuthLayout from '@/layouts/AuthLayout.vue';

export const routes: RouteRecordRaw[] = [
  {
    path: '/auth',
    component: AuthLayout,
    children: [
      {
        path: 'login',
        name: 'Login',
        component: () => import('@/views/Login.vue'),
        meta: { layout: AuthLayout }
      },
      {
        path: 'register',
        name: 'Register',
        component: () => import('@/views/Register.vue'),
        meta: { layout: AuthLayout }
      },
      {
        path: 'password-reset',
        name: 'PasswordReset',
        component: () => import('@/views/Login.vue'), // Still using Login as placeholder for now
        meta: { layout: AuthLayout }
      }
    ]
  }
];

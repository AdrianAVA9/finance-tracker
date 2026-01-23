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
        component: () => import('@/auth/login/LoginView.vue'),
        meta: { layout: AuthLayout }
      },
      {
        path: 'register',
        name: 'Register',
        component: () => import('@/auth/login/LoginView.vue'), // Using Login as placeholder
        meta: { layout: AuthLayout }
      },
      {
        path: 'password-reset',
        name: 'PasswordReset',
        component: () => import('@/auth/login/LoginView.vue'), // Using Login as placeholder
        meta: { layout: AuthLayout }
      }
    ]
  }
];

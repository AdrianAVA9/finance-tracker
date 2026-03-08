import type { RouteRecordRaw } from 'vue-router';
import AuthLayout from '@/auth/layouts/AuthLayout.vue';

export const routes: RouteRecordRaw[] = [
  {
    path: '/auth',
    component: AuthLayout,
    children: [
      {
        path: 'login',
        name: 'Login',
        component: () => import('@/auth/login/LoginView.vue')
      },
      {
        path: 'register',
        name: 'Register',
        component: () => import('@/auth/register/RegisterView.vue')
      },
      {
        path: 'password-reset',
        name: 'PasswordReset',
        component: () => import('@/auth/login/LoginView.vue') // Still using Login as placeholder for now
      }
    ]
  }
];

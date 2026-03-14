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
        path: 'forgot-password',
        name: 'ForgotPassword',
        component: () => import('@/auth/password-reset/ForgotPasswordView.vue')
      },
      {
        // email and resetCode arrive as query params: /auth/reset-password?email=...&resetCode=...
        path: 'reset-password',
        name: 'ResetPassword',
        component: () => import('@/auth/password-reset/ResetPasswordView.vue')
      }
    ]
  }
];

import type { RouteRecordRaw } from 'vue-router';
import AuthLayout from '@/layouts/AuthLayout.vue';

export const authRoutes: RouteRecordRaw[] = [
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
        component: () => import('@/auth/login/LoginView.vue'), // Reusing for now
        meta: { layout: AuthLayout }
      }
    ]
  }
];

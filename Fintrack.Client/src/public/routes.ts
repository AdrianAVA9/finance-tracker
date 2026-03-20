import type { RouteRecordRaw } from 'vue-router';
import PublicLayout from '@/public/layouts/PublicLayout.vue';

export const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: PublicLayout,
    children: [
      {
        path: '',
        name: 'Landing',
        component: () => import('@/public/landing/LandingView.vue')
      },
    ]
  }
];

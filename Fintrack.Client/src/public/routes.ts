import type { RouteRecordRaw } from 'vue-router';
import PublicLayout from '@/layouts/PublicLayout.vue';

export const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: PublicLayout,
    children: [
      {
        path: '',
        name: 'Landing',
        component: () => import('@/public/landing/LandingView.vue'),
        meta: { layout: PublicLayout }
      },
      {
        path: 'pricing',
        name: 'Pricing',
        component: () => import('@/public/pricing/PricingView.vue'),
      }
    ]
  }
];

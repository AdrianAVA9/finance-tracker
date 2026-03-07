import type { RouteRecordRaw } from 'vue-router';
import PublicLayout from '@/layouts/PublicLayout.vue';

export const publicRoutes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'Landing',
    component: () => import('@/public/landing/LandingView.vue'),
    meta: { layout: 'div' } // Standalone layout for the landing page
  },
  // {
  //   path: '/pricing',
  //   name: 'Pricing',
  //   // component: () => import('@/public/pricing/PricingView.vue'), // Placeholder import
  //   meta: { layout: PublicLayout }
  // }
];

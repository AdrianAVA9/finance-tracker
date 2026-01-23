import type { RouteRecordRaw } from 'vue-router';
import PublicLayout from '@/layouts/PublicLayout.vue';

export const publicRoutes: RouteRecordRaw[] = [
  {
    path: '/',
    component: PublicLayout,
    children: [
      {
        path: '',
        name: 'Landing',
        component: () => import('@/public/landing/LandingView.vue'),
        meta: { layout: PublicLayout } // Explicitly redundant or used by App.vue
      },
      {
        path: 'pricing',
        name: 'Pricing',
        component: () => import('@/public/pricing/PricingView.vue'), // Placeholder import
      }
    ]
  }
];

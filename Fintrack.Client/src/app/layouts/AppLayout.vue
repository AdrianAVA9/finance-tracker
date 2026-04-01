<script setup lang="ts">
import { useRoute } from 'vue-router';
import Sidebar from '@/app/components/Sidebar.vue';
import Header from '@/app/components/Header.vue';
import OfflineAlert from '@/shared/ui/OfflineAlert.vue';
import NewEntryActionSheet from '@/app/components/NewEntryActionSheet.vue';

const route = useRoute();
</script>

<template>
  <div :class="['bg-background-light dark:bg-background-dark min-h-screen h-full relative font-body text-slate-900 dark:text-white', { 'pb-32': route.meta.showMenu }]">
    <Header />
    <OfflineAlert />

    <main class="pt-24 px-6 max-w-md mx-auto space-y-8 h-full relative">
      <router-view />
    </main>

    <Sidebar v-if="route.meta.showMenu" />
    <NewEntryActionSheet />
  </div>
</template>

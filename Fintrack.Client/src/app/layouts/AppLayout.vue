<script setup lang="ts">
import { useRoute } from 'vue-router';
import Sidebar from '@/app/components/Sidebar.vue';
import Header from '@/app/components/Header.vue';
import OfflineAlert from '@/shared/ui/OfflineAlert.vue';
import NewEntryActionSheet from '@/app/components/NewEntryActionSheet.vue';

const route = useRoute();
</script>

<template>
  <div :class="[
    'bg-background text-on-background font-body min-h-screen relative overflow-x-hidden selection:bg-primary-container selection:text-on-primary-container',
    route.meta.showMenu ? 'pb-[100px]' : 'pb-safe'
  ]">
    <Header />
    <OfflineAlert />

    <main class="pt-20 px-4 max-w-2xl mx-auto h-full relative">
      <router-view />
    </main>

    <Sidebar v-if="route.meta.showMenu" />
    <NewEntryActionSheet />
  </div>
</template>

<script setup lang="ts">
import Sidebar from '@/app/components/Sidebar.vue';
import Header from '@/app/components/Header.vue';
</script>

<template>
  <div class="bg-background-light dark:bg-background-dark min-h-screen pb-32 h-full relative font-body text-slate-900 dark:text-white">
    <Header />

    <main class="pt-24 px-6 max-w-md mx-auto space-y-8 h-full relative">
      <router-view />
    </main>

    <Sidebar />
  </div>
</template>

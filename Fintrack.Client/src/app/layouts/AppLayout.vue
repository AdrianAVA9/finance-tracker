<script setup lang="ts">
import Sidebar from '@/app/components/Sidebar.vue';
import Header from '@/app/components/Header.vue';
</script>

<template>
  <div
    class="bg-background-light dark:bg-background-dark text-slate-900 dark:text-white font-display overflow-hidden h-screen flex transition-colors duration-300">
    <Sidebar />

    <main class="flex-1 flex flex-col h-full relative overflow-hidden">
      <Header />

      <!-- Scrollable Content -->
      <div class="flex-1 overflow-y-auto p-6 md:p-8 scroll-smooth">
        <div class="max-w-[1400px] mx-auto space-y-6">
          <!-- The pages render here -->
          <router-view />
        </div>
      </div>
    </main>
  </div>
</template>

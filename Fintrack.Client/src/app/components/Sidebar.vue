<script setup lang="ts">
import { ref } from 'vue';
import { useSidebarStore } from '@/stores/useSidebarStore';
import logo from '@/assets/logo.png';

const sidebarStore = useSidebarStore();
</script>

<template>
    <!-- Background Overlay for Mobile -->
    <div v-if="sidebarStore.isOpen" 
        class="fixed inset-0 bg-black/50 backdrop-blur-sm z-[25] lg:hidden transition-opacity"
        @click="sidebarStore.close">
    </div>

    <aside
        :class="sidebarStore.isOpen ? 'translate-x-0' : '-translate-x-full lg:translate-x-0'"
        class="fixed lg:static top-0 left-0 w-64 h-full flex flex-col border-r border-slate-200 dark:border-slate-800 bg-white dark:bg-[#181b1e] shrink-0 z-30 transition-transform duration-300 ease-in-out">
        <div class="p-6 flex items-center justify-between gap-3">
            <div class="flex items-center gap-3">
                <div
                    class="size-10 rounded-xl overflow-hidden shadow-glow">
                    <img :src="logo" alt="CeroBase Logo" class="w-full h-full object-cover" />
                </div>
                <div>
                    <h1 class="text-lg font-bold leading-none tracking-tight">CeroBase</h1>
                    <span class="text-xs font-medium text-primary dark:text-accent-lime">Pro Plan</span>
                </div>
            </div>
            <!-- Close button for mobile -->
            <button @click="sidebarStore.close" class="lg:hidden p-1.5 rounded-lg hover:bg-slate-100 dark:hover:bg-[#2B3036] text-slate-500">
                <span class="material-symbols-outlined">close</span>
            </button>
        </div>

        <nav class="flex-1 px-4 flex flex-col gap-2 overflow-y-auto py-4">
            <router-link to="/app/dashboard"
                @click="sidebarStore.close"
                class="flex items-center gap-3 px-4 py-3 rounded-xl hover:bg-slate-100 dark:hover:bg-[#2B3036] transition-all group"
                active-class="bg-primary text-white shadow-glow hover:bg-primary dark:hover:bg-primary font-semibold"
                exact-active-class="bg-primary text-white shadow-glow hover:bg-primary dark:hover:bg-primary font-semibold">
                <span class="material-symbols-outlined group-hover:scale-110 transition-transform">dashboard</span>
                <span class="text-sm">Dashboard</span>
            </router-link>

            <router-link to="/app/expenses/new"
                @click="sidebarStore.close"
                class="flex items-center gap-3 px-4 py-3 rounded-xl text-slate-600 dark:text-text-secondary-dark hover:bg-slate-100 dark:hover:bg-[#2B3036] hover:text-primary dark:hover:text-white transition-all group"
                active-class="bg-primary text-white shadow-glow hover:bg-primary dark:hover:bg-primary font-semibold">
                <span class="material-symbols-outlined group-hover:scale-110 transition-transform">receipt_long</span>
                <span class="font-medium text-sm">Gastos</span>
            </router-link>

            <router-link to="/app/incomes/new"
                @click="sidebarStore.close"
                class="flex items-center gap-3 px-4 py-3 rounded-xl text-slate-600 dark:text-text-secondary-dark hover:bg-slate-100 dark:hover:bg-[#2B3036] hover:text-primary dark:hover:text-white transition-all group"
                active-class="bg-primary text-white shadow-glow hover:bg-primary dark:hover:bg-primary font-semibold">
                <span class="material-symbols-outlined group-hover:scale-110 transition-transform">add_circle</span>
                <span class="font-medium text-sm">Ingresos</span>
            </router-link>

            <router-link to="/app/budgets"
                @click="sidebarStore.close"
                class="flex items-center gap-3 px-4 py-3 rounded-xl text-slate-600 dark:text-text-secondary-dark hover:bg-slate-100 dark:hover:bg-[#2B3036] hover:text-primary dark:hover:text-white transition-all group"
                active-class="bg-primary text-white shadow-glow hover:bg-primary dark:hover:bg-primary font-semibold">
                <span class="material-symbols-outlined group-hover:scale-110 transition-transform">pie_chart</span>
                <span class="font-medium text-sm">Presupuestos</span>
            </router-link>
        </nav>

        <div class="p-4 mt-auto">
            <a href="#"
                class="flex items-center gap-3 px-4 py-3 rounded-xl text-slate-600 dark:text-text-secondary-dark hover:bg-slate-100 dark:hover:bg-[#2B3036] hover:text-primary dark:hover:text-white transition-all mb-2">
                <span class="material-symbols-outlined">settings</span>
                <span class="font-medium text-sm">Configuración</span>
            </a>
        </div>
    </aside>
</template>

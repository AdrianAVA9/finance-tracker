<script setup lang="ts">
import { ref } from 'vue';
</script>

<template>
    <aside
        class="w-64 h-full hidden lg:flex flex-col border-r border-slate-200 dark:border-slate-800 bg-white dark:bg-[#181b1e] shrink-0 z-20">
        <div class="p-6 flex items-center gap-3">
            <div
                class="size-10 rounded-xl bg-gradient-to-br from-primary to-emerald-600 flex items-center justify-center shadow-glow text-white">
                <span class="material-symbols-outlined">account_balance_wallet</span>
            </div>
            <div>
                <h1 class="text-lg font-bold leading-none tracking-tight">Fintrack</h1>
                <span class="text-xs font-medium text-primary dark:text-accent-lime">Pro Plan</span>
            </div>
        </div>

        <nav class="flex-1 px-4 flex flex-col gap-2 overflow-y-auto py-4">
            <router-link to="/app/dashboard"
                class="flex items-center gap-3 px-4 py-3 rounded-xl hover:bg-slate-100 dark:hover:bg-[#2B3036] transition-all group"
                active-class="bg-primary text-white shadow-glow hover:bg-primary dark:hover:bg-primary font-semibold"
                exact-active-class="bg-primary text-white shadow-glow hover:bg-primary dark:hover:bg-primary font-semibold">
                <span class="material-symbols-outlined group-hover:scale-110 transition-transform">dashboard</span>
                <span class="text-sm">Dashboard</span>
            </router-link>

            <router-link to="/app/expenses/new"
                class="flex items-center gap-3 px-4 py-3 rounded-xl text-slate-600 dark:text-text-secondary-dark hover:bg-slate-100 dark:hover:bg-[#2B3036] hover:text-primary dark:hover:text-white transition-all group"
                active-class="bg-primary text-white shadow-glow hover:bg-primary dark:hover:bg-primary font-semibold">
                <span class="material-symbols-outlined group-hover:scale-110 transition-transform">receipt_long</span>
                <span class="font-medium text-sm">Gastos</span>
            </router-link>

            <router-link to="/app/incomes/new"
                class="flex items-center gap-3 px-4 py-3 rounded-xl text-slate-600 dark:text-text-secondary-dark hover:bg-slate-100 dark:hover:bg-[#2B3036] hover:text-primary dark:hover:text-white transition-all group"
                active-class="bg-primary text-white shadow-glow hover:bg-primary dark:hover:bg-primary font-semibold">
                <span class="material-symbols-outlined group-hover:scale-110 transition-transform">add_circle</span>
                <span class="font-medium text-sm">Ingresos</span>
            </router-link>

            <router-link to="/app/budgets"
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

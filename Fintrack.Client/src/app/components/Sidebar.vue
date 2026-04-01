<script setup lang="ts">
import { useActionSheetStore } from '@/stores/useActionSheetStore';

const store = useActionSheetStore();
</script>

<template>
    <nav class="fixed bottom-0 w-full rounded-t-3xl z-50 bg-[#1E2228]/90 backdrop-blur-md no-border border-t border-white/5 shadow-[0_-4px_40px_rgba(5,230,153,0.05)] flex justify-around items-center px-4 pb-8 pt-3">
        <!-- Dashboard -->
        <router-link to="/app/dashboard" class="sidebar-link">
            <span class="material-symbols-outlined">dashboard</span>
            <span class="text-[10px] font-medium tracking-wide uppercase mt-1">Inicio</span>
        </router-link>
        
        <!-- Activity -->
        <router-link to="/app/activity" class="sidebar-link">
            <span class="material-symbols-outlined">receipt_long</span>
            <span class="text-[10px] font-medium tracking-wide uppercase mt-1">Actividad</span>
        </router-link>

        <!-- Add -->
        <button 
            @click="store.openNewEntry"
            :class="[
                'flex flex-col items-center justify-center transition-all duration-200 active:scale-90',
                store.isNewEntryOpen ? 'text-[#05E699]' : 'text-[#9CA3AF] hover:text-[#05E699]'
            ]">
            <span class="material-symbols-outlined transition-transform duration-300" :class="{ 'rotate-45': store.isNewEntryOpen }">add_circle</span>
            <span class="text-[10px] font-medium tracking-wide uppercase mt-1">Agregar</span>
        </button>

        <!-- Budgets -->
        <router-link to="/app/budgets" class="sidebar-link">
            <span class="material-symbols-outlined">account_balance_wallet</span>
            <span class="text-[10px] font-medium tracking-wide uppercase mt-1">Presupuesto</span>
        </router-link>

        <!-- Profile / Cuentas -->
        <router-link to="/app/settings" class="sidebar-link">
            <span class="material-symbols-outlined">wallet</span>
            <span class="text-[10px] font-medium tracking-wide uppercase mt-1">Cuentas</span>
        </router-link>
    </nav>
</template>

<style scoped>
.sidebar-link {
    @apply flex flex-col items-center justify-center relative transition-all duration-200 text-[#9CA3AF] hover:text-[#05E699] active:scale-90;
}

.router-link-exact-active {
    @apply !text-[#05E699];
}

.router-link-exact-active::after {
    content: '';
    @apply absolute -bottom-3 w-1 h-1 bg-[#05E699] rounded-full shadow-[0_0_8px_#05E699];
}

.router-link-exact-active .material-symbols-outlined {
    font-variation-settings: 'FILL' 1 !important;
}
</style>

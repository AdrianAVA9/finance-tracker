<script setup lang="ts">
</script>

<template>
    <nav class="fixed bottom-0 w-full rounded-t-3xl z-50 bg-[#1E2228]/90 backdrop-blur-md no-border border-t border-white/5 shadow-[0_-4px_40px_rgba(5,230,153,0.05)] flex justify-around items-center px-4 pb-8 pt-3">
        <!-- Dashboard -->
        <router-link to="/app/dashboard"
            class="flex flex-col items-center justify-center relative active:scale-90 transition-transform duration-200"
            active-class="text-[#05E699] after:content-[''] after:absolute after:-bottom-2 after:w-1 after:h-1 after:bg-[#05E699] after:rounded-full"
            exact-active-class="text-[#05E699] after:content-[''] after:absolute after:-bottom-2 after:w-1 after:h-1 after:bg-[#05E699] after:rounded-full">
            <span class="material-symbols-outlined" style="font-variation-settings: 'FILL' 1;">dashboard</span>
            <span class="font-['Inter'] text-[10px] font-medium tracking-wide uppercase mt-1">Dashboard</span>
        </router-link>
        
        <!-- Activity -->
        <router-link to="/app/expenses"
            class="flex flex-col items-center justify-center text-[#9CA3AF] hover:text-[#05E699] transition-colors active:scale-90 transition-transform duration-200"
            active-class="text-[#05E699]">
            <span class="material-symbols-outlined">receipt_long</span>
            <span class="font-['Inter'] text-[10px] font-medium tracking-wide uppercase mt-1">Actividad</span>
        </router-link>

        <!-- Add -->
        <router-link to="/app/expenses/new"
            class="flex flex-col items-center justify-center text-[#9CA3AF] hover:text-[#05E699] transition-colors active:scale-90 transition-transform duration-200"
            active-class="text-[#05E699]">
            <span class="material-symbols-outlined">add_circle</span>
            <span class="font-['Inter'] text-[10px] font-medium tracking-wide uppercase mt-1">Agregar</span>
        </router-link>

        <!-- Budgets -->
        <router-link to="/app/budgets"
            class="flex flex-col items-center justify-center text-[#9CA3AF] hover:text-[#05E699] transition-colors active:scale-90 transition-transform duration-200"
            active-class="text-[#05E699]">
            <span class="material-symbols-outlined">account_balance_wallet</span>
            <span class="font-['Inter'] text-[10px] font-medium tracking-wide uppercase mt-1">Presupuestos</span>
        </router-link>

        <!-- Profile -->
        <router-link to="/app/settings"
            class="flex flex-col items-center justify-center text-[#9CA3AF] hover:text-[#05E699] transition-colors active:scale-90 transition-transform duration-200"
            active-class="text-[#05E699]">
            <span class="material-symbols-outlined">person</span>
            <span class="font-['Inter'] text-[10px] font-medium tracking-wide uppercase mt-1">Perfil</span>
        </router-link>
    </nav>
</template>

<script setup lang="ts">
import { useAuth } from '@/composables/useAuth';
import { useRouter, useRoute } from 'vue-router';
import { computed } from 'vue';
import { useSidebarStore } from '@/stores/useSidebarStore';

const sidebarStore = useSidebarStore();

const { logout } = useAuth();
const router = useRouter();
const route = useRoute();

const pageTitle = computed(() => route.meta.title as string || 'Dashboard');
const pageSubtitle = computed(() => route.meta.subtitle as string || '¡Bienvenido de nuevo!');

const handleLogout = async () => {
    await logout();
    router.push('/auth/login');
};
</script>

<template>
    <header
        class="h-20 px-8 flex items-center justify-between border-b border-slate-200 dark:border-slate-800 bg-white/80 dark:bg-[#1c1f22]/90 backdrop-blur-md z-10 sticky top-0">
        <div class="flex items-center gap-4">
            <!-- Mobile Toggle -->
            <button
                @click="sidebarStore.toggle"
                class="lg:hidden size-10 flex items-center justify-center rounded-xl hover:bg-slate-100 dark:hover:bg-[#2B3036] text-slate-600 dark:text-slate-300 transition-colors">
                <span class="material-symbols-outlined">menu</span>
            </button>

            <div>
                <h2 class="text-xl font-bold dark:text-white tracking-tight">{{ pageTitle }}</h2>
                <p v-if="pageSubtitle" class="text-xs text-slate-500 dark:text-slate-400">{{ pageSubtitle }}</p>
            </div>
        </div>

        <div class="flex items-center gap-4">
            <div
                class="hidden md:flex items-center gap-2 bg-slate-100 dark:bg-[#2B3036] rounded-lg p-1 border border-slate-200 dark:border-slate-700">
                <button
                    class="px-3 py-1.5 rounded-md bg-white dark:bg-primary text-slate-900 dark:text-white shadow-sm text-xs font-bold transition-all">CRC</button>
            </div>

            <button
                class="size-10 flex items-center justify-center rounded-xl hover:bg-slate-100 dark:hover:bg-[#2B3036] text-slate-600 dark:text-slate-300 transition-colors relative">
                <span class="material-symbols-outlined">notifications</span>
                <span
                    class="absolute top-2.5 right-2.5 size-2 bg-red-500 rounded-full border-2 border-white dark:border-[#1c1f22]"></span>
            </button>

            <div class="h-8 w-px bg-slate-200 dark:bg-slate-700 mx-1"></div>

            <div class="flex items-center gap-3 group relative cursor-pointer">
                <div
                    class="size-10 rounded-full bg-slate-200 dark:bg-slate-700 border-2 border-slate-200 dark:border-slate-700 group-hover:border-primary transition-colors flex items-center justify-center overflow-hidden">
                    <span class="material-symbols-outlined text-gray-500 dark:text-gray-400">person</span>
                </div>

                <div
                    class="absolute right-0 top-full mt-2 w-48 bg-white dark:bg-[#2B3036] rounded-xl shadow-lg border border-slate-200 dark:border-slate-700 opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all duration-200 z-50">
                    <button @click="handleLogout"
                        class="w-full text-left px-4 py-3 text-sm text-primary hover:bg-slate-50 dark:hover:bg-slate-800 rounded-xl transition-colors font-bold flex items-center gap-2">
                        <span class="material-symbols-outlined text-sm">logout</span>
                        Cerrar sesión
                    </button>
                </div>
            </div>
        </div>
    </header>
</template>

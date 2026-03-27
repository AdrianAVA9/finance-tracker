<script setup lang="ts">
import { useAuth } from '@/composables/useAuth';
import { useRouter } from 'vue-router';
import logo from '@/assets/logo.png';

const { logout } = useAuth();
const router = useRouter();

const handleLogout = async () => {
    await logout();
    router.push('/auth/login');
};
</script>

<template>
    <header class="fixed top-0 w-full z-50 flex justify-between items-center px-6 h-16 bg-[#111317]/80 backdrop-blur-xl no-border bg-gradient-to-b from-[#111317] to-transparent">
        <div class="flex items-center gap-2">
            <img alt="CERO Logo" class="h-6 w-auto" :src="logo" />
            <span class="text-xl font-black text-[#05E699] tracking-tighter font-['Manrope']">CERO</span>
        </div>
        
        <div class="flex items-center gap-4">
            <button class="text-[#F3F4F6] hover:opacity-80 transition-opacity active:transition-all scale-95">
                <span class="material-symbols-outlined">notifications</span>
            </button>
            
            <div class="flex items-center group relative cursor-pointer">
                <div class="size-8 rounded-full bg-[#333539] border border-[#333539] group-hover:border-[#05E699] transition-colors flex items-center justify-center overflow-hidden">
                    <span class="material-symbols-outlined text-[#bacbbe] text-sm">person</span>
                </div>

                <div class="absolute right-0 top-full mt-2 w-48 bg-[#282a2e] rounded-xl shadow-lg border border-white/5 opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all duration-200 z-50">
                    <button @click="handleLogout"
                        class="w-full text-left px-4 py-3 text-sm text-[#FF4D4D] hover:bg-[#333539] rounded-xl transition-colors font-bold flex items-center gap-2">
                        <span class="material-symbols-outlined text-sm">logout</span>
                        Cerrar sesión
                    </button>
                </div>
            </div>
        </div>
    </header>
</template>

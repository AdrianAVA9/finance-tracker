<script setup lang="ts">
import { useActionSheetStore } from '@/stores/useActionSheetStore';
import { useRouter } from 'vue-router';

const store = useActionSheetStore();
const router = useRouter();

const close = () => {
    store.closeNewEntry();
};

const handleNavigation = (route: string) => {
    close();
    router.push(route);
};
</script>

<template>
    <Transition name="fade">
        <div v-if="store.isNewEntryOpen" class="fixed inset-0 z-[100] flex flex-col justify-end">
            <!-- Backdrop -->
            <div class="absolute inset-0 bg-[#111317]/80 backdrop-blur-[12px] transition-opacity" @click="close"></div>
            
            <!-- Modal Content -->
            <div class="relative w-full max-w-md mx-auto bg-[#111317] rounded-t-[32px] p-6 pb-12 shadow-[0_-10px_60px_rgba(30,34,40,0.5)] border-t border-white/5" @click.stop>
                
                <!-- Header -->
                <div class="mb-8">
                    <h2 class="text-[#F3F4F6] font-[Manrope] font-extrabold text-3xl tracking-tight leading-tight">Nuevo Registro</h2>
                    <p class="text-[#9CA3AF] font-semibold text-[10px] tracking-[0.15em] uppercase mt-2">Tipo de Acción</p>
                </div>

                <!-- Action List -->
                <div class="space-y-4">
                    <!-- AI Receipt Scan -->
                    <button 
                        @click="handleNavigation('/app/expenses/upload')"
                        class="w-full bg-gradient-to-r from-[#77ffbb] to-[#05E699] p-3.5 rounded-xl flex items-center justify-between group active:scale-[0.98] transition-all duration-200">
                        <div class="flex items-center gap-3">
                            <div class="w-10 h-10 rounded-xl flex items-center justify-center bg-[#003822]/20 shrink-0">
                                <span class="material-symbols-outlined text-[#003822] text-[20px]">document_scanner</span>
                            </div>
                            <div class="text-left">
                                <h3 class="text-[#003822] font-bold text-sm leading-tight">Escaneo Recibo (IA)</h3>
                                <p class="text-[#003822]/70 font-semibold text-[10px] tracking-wide uppercase mt-0.5 max-w-[200px] truncate sm:max-w-none">Extracción automática de datos</p>
                            </div>
                        </div>
                        <span class="material-symbols-outlined text-[#003822] text-lg opacity-70 group-hover:opacity-100 transition-opacity">chevron_right</span>
                    </button>

                    <!-- Expense -->
                    <button 
                        @click="handleNavigation('/app/expenses/new')"
                        class="w-full bg-[#1e2024] hover:bg-[#2A2F37] border border-transparent hover:border-white/5 p-3.5 rounded-xl flex items-center justify-between group active:scale-[0.98] transition-all duration-200 shadow-[0_4px_20px_rgba(0,0,0,0.2)]">
                        <div class="flex items-center gap-3">
                            <div class="w-10 h-10 rounded-xl bg-[#FF4D4D]/10 flex items-center justify-center shrink-0">
                                <span class="material-symbols-outlined text-[#FF4D4D] text-[20px]">receipt_long</span>
                            </div>
                            <div class="text-left">
                                <h3 class="text-[#F3F4F6] font-bold text-sm leading-tight">Gasto</h3>
                                <p class="text-[#9CA3AF] font-semibold text-[10px] tracking-wide uppercase mt-0.5">Registrar un pago</p>
                            </div>
                        </div>
                        <span class="material-symbols-outlined text-[#9CA3AF] text-lg group-hover:text-[#F3F4F6] transition-colors">chevron_right</span>
                    </button>

                    <!-- Income -->
                    <button 
                        @click="handleNavigation('/app/incomes/new')"
                        class="w-full bg-[#1e2024] hover:bg-[#2A2F37] border border-transparent hover:border-white/5 p-3.5 rounded-xl flex items-center justify-between group active:scale-[0.98] transition-all duration-200 shadow-[0_4px_20px_rgba(0,0,0,0.2)]">
                        <div class="flex items-center gap-3">
                            <div class="w-10 h-10 rounded-xl bg-[#05E699]/10 flex items-center justify-center shrink-0">
                                <span class="material-symbols-outlined text-[#05E699] text-[20px]">payments</span>
                            </div>
                            <div class="text-left">
                                <h3 class="text-[#F3F4F6] font-bold text-sm leading-tight">Ingreso</h3>
                                <p class="text-[#9CA3AF] font-semibold text-[10px] tracking-wide uppercase mt-0.5">Registrar depósito</p>
                            </div>
                        </div>
                        <span class="material-symbols-outlined text-[#9CA3AF] text-lg group-hover:text-[#F3F4F6] transition-colors">chevron_right</span>
                    </button>
                </div>

                <!-- Close Button (Dark Mode Friendly) -->
                <div class="flex justify-center mt-8 relative z-10">
                    <button 
                        @click="close"
                        class="w-14 h-14 bg-[#1e2024] border border-white/10 hover:bg-[#2A2F37] rounded-[20px] flex items-center justify-center shadow-[0_8px_32px_rgba(0,0,0,0.4)] transition-all duration-200 active:scale-90">
                        <span class="material-symbols-outlined text-[#F3F4F6] text-2xl">close</span>
                    </button>
                </div>
            </div>
        </div>
    </Transition>
</template>

<style scoped>
.fade-enter-active,
.fade-leave-active {
    transition: opacity 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.fade-enter-from,
.fade-leave-to {
    opacity: 0;
}

.fade-enter-active > div:last-child {
    transition: transform 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.1);
}
.fade-leave-active > div:last-child {
    transition: transform 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.fade-enter-from > div:last-child,
.fade-leave-to > div:last-child {
    transform: translateY(100%);
}
</style>

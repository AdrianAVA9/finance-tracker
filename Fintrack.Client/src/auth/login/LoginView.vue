<script setup lang="ts">
import { ref } from 'vue';
import { useAuth } from '@/composables/useAuth';
import { useRouter } from 'vue-router';

const { login } = useAuth();
const router = useRouter();

const email = ref('');
const password = ref('');
const errorMsg = ref('');
const loading = ref(false);

const handleLogin = async () => {
    loading.value = true;
    errorMsg.value = '';

    const success = await login(email.value, password.value);

    loading.value = false;
    if (success) {
        router.push('/app'); // Redirect to dashboard after successful login
    } else {
        errorMsg.value = 'Correo o contraseña inválidos.';
    }
};
</script>

<template>
    <div class="bg-[#0e0e1b] text-white h-screen w-full overflow-hidden flex flex-row font-display relative z-50">
        <!-- Left Side: Marketing / Visuals -->
        <div class="hidden lg:flex lg:w-[60%] relative h-full bg-[#0F0F1A] items-center justify-center overflow-hidden">
            <!-- Abstract Background Image -->
            <div class="absolute inset-0 w-full h-full bg-cover bg-center opacity-80"
                style="background-image: url('https://lh3.googleusercontent.com/aida-public/AB6AXuDERtLs02LEU4puPj5Yr231Z1mqpASVb3qTfzJH8hzYPvNyopc4uPXzhAH_iRN3wXarrhiSW1VATYYB6ur5HYskypAR8vtln6Phv_e8LOLzpqY7FnJ3Gn62anMJ3Nb6mxzeA2Ij5GC5XbfY8xflxvx9kW0FNsr-5JBXTJws5Tu5viMP20Sfpg2xKcsKXU8FPLfHiCPZTbUgMHEcH7ZaTTS-eiCkVlagJkJyrzLmhWsPU9dmHeSE-OHbY9mnjhTC71BSrvFm5gbyqAnb');">
            </div>
            <!-- Gradient Overlays for depth and text readability -->
            <div class="absolute inset-0 bg-[#191971]/40 mix-blend-multiply"></div>
            <div class="absolute inset-0 bg-gradient-to-t from-[#0e0e1b] via-[#0e0e1b]/40 to-transparent"></div>
            <div class="absolute inset-0 bg-gradient-to-r from-transparent via-transparent to-[#0F0F1A]"></div>

            <!-- Content Overlay -->
            <div class="relative z-10 p-12 max-w-2xl text-left self-end mb-20">
                <div class="flex items-center gap-3 mb-6">
                    <span class="material-symbols-outlined text-4xl text-blue-400">security</span>
                    <p class="text-2xl font-bold tracking-wide">Fintrack</p>
                </div>
                <h2 class="text-4xl md:text-5xl font-bold leading-tight mb-4 text-white">
                    Claridad financiera,<br />impulsada por inteligencia.
                </h2>
                <p class="text-lg text-[#9898c3] max-w-lg">
                    Experimenta la próxima generación de gestión presupuestaria donde la precisión se une a la seguridad.
                </p>
            </div>
        </div>

        <!-- Right Side: Login Form -->
        <div class="w-full lg:w-[40%] h-full bg-[#0F0F1A] flex flex-col overflow-y-auto">
            <div class="flex-1 flex flex-col justify-center items-center px-6 py-12 sm:px-12">

                <!-- Mobile Logo (Visible only on small screens) -->
                <div class="lg:hidden mb-8 flex items-center gap-2">
                    <span class="material-symbols-outlined text-3xl text-[#191971]">security</span>
                    <span class="text-xl font-bold">Fintrack</span>
                </div>

                <!-- Login Card -->
                <div
                    class="w-full max-w-[440px] bg-[#1A1A30] rounded-2xl p-8 shadow-2xl border border-[#393960]/30 relative overflow-hidden group">
                    <!-- Decorative top border glow -->
                    <div
                        class="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-transparent via-[#191971] to-transparent opacity-50 group-hover:opacity-100 transition-opacity duration-500">
                    </div>

                    <!-- Header -->
                    <div class="mb-8">
                        <h1 class="text-[32px] font-bold leading-tight text-white mb-2">Acceso Seguro</h1>
                        <p class="text-[#9898c3] text-sm">Gestiona tu presupuesto con precisión.</p>
                        <p v-if="errorMsg" class="mt-2 text-red-500 text-sm font-semibold">{{ errorMsg }}</p>
                    </div>

                    <!-- Form -->
                    <form @submit.prevent="handleLogin" class="flex flex-col gap-5">
                        <!-- Email Field -->
                        <div class="flex flex-col gap-2">
                            <label for="email" class="text-white text-sm font-medium">Correo Electrónico</label>
                            <div class="relative">
                                <input v-model="email" type="email" id="email" placeholder="nombre@ejemplo.com"
                                    class="w-full h-12 rounded-lg bg-[#0F0F1A] border border-[#393960] text-white placeholder-[#9898c3] px-4 focus:outline-none focus:border-[#191971] focus:ring-1 focus:ring-[#191971] transition-all duration-200"
                                    required>
                            </div>
                        </div>

                        <!-- Password Field -->
                        <div class="flex flex-col gap-2">
                            <label for="password" class="text-white text-sm font-medium">Contraseña</label>
                            <div class="relative flex items-center">
                                <input v-model="password" type="password" id="password" placeholder="••••••••"
                                    class="w-full h-12 rounded-lg bg-[#0F0F1A] border border-[#393960] text-white placeholder-[#9898c3] pl-4 pr-12 focus:outline-none focus:border-[#191971] focus:ring-1 focus:ring-[#191971] transition-all duration-200"
                                    required>
                                <button type="button"
                                    class="absolute right-3 text-[#9898c3] hover:text-white transition-colors flex items-center justify-center p-1">
                                    <span class="material-symbols-outlined text-[20px]">visibility</span>
                                </button>
                            </div>
                        </div>

                        <!-- Remember & Forgot Password -->
                        <div class="flex items-center justify-between mt-1">
                            <label class="flex items-center gap-2 cursor-pointer group/check">
                                <div class="relative flex items-center">
                                    <input type="checkbox"
                                        class="peer h-4 w-4 appearance-none rounded border border-[#393960] bg-[#0F0F1A] checked:border-[#191971] checked:bg-[#191971] focus:ring-0 focus:ring-offset-0 transition-colors">
                                    <span
                                        class="material-symbols-outlined absolute text-white opacity-0 peer-checked:opacity-100 pointer-events-none text-sm top-0 left-0">check</span>
                                </div>
                                <span
                                    class="text-sm text-[#9898c3] group-hover/check:text-white transition-colors">Recordarme</span>
                            </label>
                            <router-link
                                to="/auth/forgot-password"
                                class="text-sm text-[#85B2B2] hover:text-[#A5D2D2] transition-colors font-medium">¿Olvidaste tu contraseña?</router-link>
                        </div>

                        <!-- Submit Button -->
                        <button type="submit" :disabled="loading"
                            class="mt-4 w-full h-12 bg-[#191971] hover:bg-[#191971]/90 text-white font-medium rounded-lg shadow-lg shadow-[#191971]/20 transition-all transform active:scale-[0.99] flex items-center justify-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed">
                            <span v-if="loading">Iniciando Sesión...</span>
                            <span v-else>Iniciar Sesión</span>
                            <span v-if="!loading" class="material-symbols-outlined text-lg">login</span>
                        </button>
                    </form>

                    <!-- Footer Link -->
                    <div class="mt-8 text-center">
                        <p class="text-sm text-[#9898c3]">
                            ¿No tienes una cuenta?
                            <router-link to="/auth/register"
                                class="text-[#85B2B2] hover:text-[#A5D2D2] font-medium transition-colors">Regístrate</router-link>
                        </p>
                    </div>
                </div>

                <p class="mt-8 text-xs text-[#9898c3]/50 text-center">
                    © 2024 Fintrack Inc. Todos los derechos reservados.
                </p>
            </div>
        </div>
    </div>
</template>

<style scoped>
/* Custom scrollbar for subtle look */
::-webkit-scrollbar {
    width: 8px;
}

::-webkit-scrollbar-track {
    background: #0F0F1A;
}

::-webkit-scrollbar-thumb {
    background: #393960;
    border-radius: 4px;
}

::-webkit-scrollbar-thumb:hover {
    background: #505080;
}
</style>

<script setup lang="ts">
import { ref } from 'vue';
import { useAuth } from '@/composables/useAuth';
import { useRouter } from 'vue-router';

const { register } = useAuth();
const router = useRouter();

const fullName = ref('');
const email = ref('');
const password = ref('');
const terms = ref(false);
const errorMsg = ref('');
const loading = ref(false);

const handleRegister = async () => {
    if (!terms.value) {
        errorMsg.value = 'You must agree to the Terms of Service to register.';
        return;
    }

    loading.value = true;
    errorMsg.value = '';

    const success = await register(email.value, password.value);

    loading.value = false;
    if (success) {
        router.push('/'); // Redirect to dashboard or home after successful registration/login
    } else {
        errorMsg.value = 'Registration failed. Please check your details.';
    }
};

const getPasswordStrength = () => {
    if (password.value.length === 0) return { label: 'None', width: 'w-0', color: 'bg-white/10' };
    if (password.value.length < 6) return { label: 'Weak', width: 'w-1/4', color: 'bg-red-500' };
    if (password.value.length < 10) return { label: 'Medium', width: 'w-2/4', color: 'bg-[#85B2B2]' };
    if (password.value.match(/[A-Z]/) && password.value.match(/[0-9]/)) return { label: 'Strong', width: 'w-full', color: 'bg-[#191971]' };
    return { label: 'Good', width: 'w-3/4', color: 'bg-blue-400' };
};

</script>

<template>
    <div class="bg-[#0F0F1A] text-slate-100 font-display selection:bg-[#191971]/40 min-h-screen relative z-50">
        <div class="flex min-h-screen w-full flex-col lg:flex-row">
            <!-- Left Side: Marketing Image (60%) -->
            <div class="relative hidden lg:flex lg:w-[60%] flex-col items-start justify-center p-16 overflow-hidden">
                <!-- Background Image with Overlay -->
                <div class="absolute inset-0 z-0">
                    <div class="absolute inset-0 bg-[#0F0F1A]/60 z-10"></div>
                    <img src="https://lh3.googleusercontent.com/aida-public/AB6AXuB7ReZf5Sq5Fzs7O6OE-yOiB0wEaFhv3jQ8c7OVoLoeGcJb0ip8t_cLZoEIb_7yCmru6C7wZVjlV9DrKyGKvJZeBL56vO8kZmMWeYaO4wj6sFSoppf1tusQ7g9OnCdXK049-xVdZ26KXnETmwex4vcbEPeKnGyGWW5SSPKC8aSMEitVX8BfaMZTMJQMMd19i6fSehKcWKQYafZeSIUvB3208cS8YjDVDjt7gyP1aAUwFs7EkI6rnhC87mC8qk4y41x9MhvIbVM3bz3k"
                        alt="Abstract digital network security visualization" class="h-full w-full object-cover">
                </div>
                <!-- Content -->
                <div class="relative z-20 max-w-2xl">
                    <div class="mb-12 flex items-center gap-3">
                        <div class="flex h-10 w-10 items-center justify-center rounded-md bg-[#191971]">
                            <span class="material-symbols-outlined text-white">shield_person</span>
                        </div>
                        <span class="text-2xl font-bold tracking-tight text-white">Sentinel Secure</span>
                    </div>

                    <h1 class="mb-6 text-5xl font-extrabold leading-[1.1] tracking-tight text-white lg:text-6xl">
                        Start your financial journey with AI precision.
                    </h1>

                    <p class="text-xl font-medium leading-relaxed text-slate-300">
                        Create an account to protect and grow your wealth with advanced AI-driven security. Our sentinel
                        technology monitors your assets 24/7.
                    </p>

                    <div class="mt-12 flex gap-8">
                        <div class="flex flex-col">
                            <span class="text-3xl font-bold text-white">99.9%</span>
                            <span class="text-sm uppercase tracking-widest text-[#85B2B2]">Threat Detection</span>
                        </div>
                        <div class="flex flex-col">
                            <span class="text-3xl font-bold text-white">256-bit</span>
                            <span class="text-sm uppercase tracking-widest text-[#85B2B2]">Encryption</span>
                        </div>
                    </div>
                </div>

                <!-- Subtle Decorative Bottom Element -->
                <div
                    class="absolute bottom-8 left-16 z-20 flex items-center gap-2 text-xs font-semibold uppercase tracking-[0.2em] text-[#AAAAAA]/50">
                    <span>Enterprise Grade Security</span>
                    <span class="h-px w-12 bg-[#AAAAAA]/30"></span>
                    <span>Powered by Neural Intelligence</span>
                </div>
            </div>

            <!-- Right Side: Registration Form (40%) -->
            <div class="flex w-full flex-col items-center justify-center px-6 py-12 lg:w-[40%] lg:px-12 bg-[#0F0F1A]">

                <!-- Mobile Logo (visible only on small screens) -->
                <div class="mb-10 flex items-center gap-2 lg:hidden">
                    <div class="flex h-8 w-8 items-center justify-center rounded-md bg-[#191971]">
                        <span class="material-symbols-outlined text-sm text-white">shield_person</span>
                    </div>
                    <span class="text-xl font-bold text-white">Sentinel Secure</span>
                </div>

                <!-- Form Card -->
                <div class="w-full max-w-md rounded-md bg-[#1A1A30] p-8 layered-obsidian-shadow border border-white/5">
                    <div class="mb-8">
                        <h2 class="text-2xl font-bold tracking-tight text-white">Create Account</h2>
                        <p class="mt-2 text-sm text-[#AAAAAA]">Secure your digital assets with enterprise-grade
                            protection.</p>
                        <p v-if="errorMsg" class="mt-2 text-red-500 text-sm font-semibold">{{ errorMsg }}</p>
                    </div>

                    <form @submit.prevent="handleRegister" class="space-y-5">
                        <!-- Full Name -->
                        <div class="space-y-2">
                            <label for="full-name" class="text-sm font-semibold tracking-wide text-white">Full
                                Name</label>
                            <input v-model="fullName" type="text" id="full-name" placeholder="Enter your full name"
                                class="w-full rounded-md border border-white/10 bg-[#0F0F1A]/50 px-4 py-3 text-white placeholder:text-[#AAAAAA]/50 focus:border-[#191971] focus:ring-1 focus:ring-[#191971] outline-none transition-all input-focus-inset font-display"
                                required>
                        </div>

                        <!-- Email Address -->
                        <div class="space-y-2">
                            <label for="email" class="text-sm font-semibold tracking-wide text-white">Email
                                Address</label>
                            <input v-model="email" type="email" id="email" placeholder="name@example.com"
                                class="w-full rounded-md border border-white/10 bg-[#0F0F1A]/50 px-4 py-3 text-white placeholder:text-[#AAAAAA]/50 focus:border-[#191971] focus:ring-1 focus:ring-[#191971] outline-none transition-all input-focus-inset font-display"
                                required>
                        </div>

                        <!-- Password -->
                        <div class="space-y-2">
                            <div class="flex items-center justify-between">
                                <label for="password"
                                    class="text-sm font-semibold tracking-wide text-white">Password</label>
                            </div>
                            <div class="relative">
                                <input v-model="password" type="password" id="password" placeholder="••••••••"
                                    class="w-full rounded-md border border-white/10 bg-[#0F0F1A]/50 px-4 py-3 text-white placeholder:text-[#AAAAAA]/50 focus:border-[#191971] focus:ring-1 focus:ring-[#191971] outline-none transition-all input-focus-inset font-display"
                                    required>
                                <button type="button"
                                    class="absolute right-3 top-1/2 -translate-y-1/2 text-[#AAAAAA] hover:text-white">
                                    <span class="material-symbols-outlined text-xl">visibility</span>
                                </button>
                            </div>
                            <!-- Strength Indicator -->
                            <div class="mt-2 space-y-1.5">
                                <div class="w-full bg-white/10 h-1 flex rounded-full overflow-hidden">
                                    <div :class="[getPasswordStrength().width, getPasswordStrength().color]"
                                        class="h-full transition-all duration-300"></div>
                                </div>
                                <p class="text-[10px] font-bold uppercase tracking-widest text-[#85B2B2]">Strength: {{
                                    getPasswordStrength().label }}</p>
                            </div>
                        </div>

                        <!-- Terms Agreement -->
                        <div class="flex items-start gap-3 py-2">
                            <div class="flex h-5 items-center">
                                <input v-model="terms" id="terms" name="terms" type="checkbox"
                                    class="h-4 w-4 rounded border-white/20 bg-[#0F0F1A] text-[#191971] focus:ring-[#191971] focus:ring-offset-[#0F0F1A]">
                            </div>
                            <label for="terms" class="text-xs leading-relaxed text-[#AAAAAA]">
                                I agree to the <a href="#" class="text-[#85B2B2] hover:underline">Terms of Service</a>
                                and <a href="#" class="text-[#85B2B2] hover:underline">Privacy Policy</a>.
                            </label>
                        </div>

                        <!-- Submit Button -->
                        <button type="submit" :disabled="loading"
                            class="w-full rounded-md bg-[#191971] py-4 text-sm font-bold tracking-[0.05em] text-white transition-all hover:bg-[#191971]/90 active:scale-[0.98] disabled:opacity-50 disabled:cursor-not-allowed">
                            <span v-if="loading">CREATING ACCOUNT...</span>
                            <span v-else>CREATE ACCOUNT</span>
                        </button>
                    </form>



                    <!-- Footer Link -->
                    <p class="mt-8 text-center text-sm font-medium text-[#AAAAAA]">
                        Already have an account?
                        <router-link to="/auth/login" class="font-bold text-[#85B2B2] hover:underline ml-1">Sign
                            in</router-link>
                    </p>
                </div>

                <!-- Accessibility/Legal Footer -->
                <div class="mt-12 flex gap-6 text-[10px] font-bold uppercase tracking-widest text-[#AAAAAA]/40">
                    <a href="#" class="hover:text-[#AAAAAA] transition-colors">Security</a>
                    <a href="#" class="hover:text-[#AAAAAA] transition-colors">Support</a>
                    <a href="#" class="hover:text-[#AAAAAA] transition-colors">Privacy</a>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
.layered-obsidian-shadow {
    box-shadow: 0 10px 30px -5px rgba(25, 25, 113, 0.2);
}

.input-focus-inset:focus {
    box-shadow: inset 0 2px 4px 0 rgba(0, 0, 0, 0.06);
}
</style>

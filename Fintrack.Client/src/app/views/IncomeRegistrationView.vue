<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import api from '@/services/api';

interface IncomeCategory {
    id: number;
    name: string;
    icon: string;
    color: string;
}

const router = useRouter();

// Form State
const amount = ref<number | null>(null);
const source = ref('');
const date = ref(new Date().toISOString().split('T')[0]);
const categoryId = ref<number | null>(null);
const notes = ref('');
const isRecurring = ref(false);
const frequency = ref('Monthly');
const nextDate = ref('');

// UI state
const categories = ref<IncomeCategory[]>([]);
const isLoading = ref(false);
const isSaving = ref(false);
const showSuccess = ref(false);

const frequencies = [
    { value: 'Daily', label: 'Diario' },
    { value: 'Weekly', label: 'Semanal' },
    { value: 'BiWeekly', label: 'Quincenal' },
    { value: 'Monthly', label: 'Mensual' },
    { value: 'Quarterly', label: 'Trimestral' },
    { value: 'Yearly', label: 'Anual' }
];

const loadCategories = async () => {
    try {
        isLoading.value = true;
        const response = await api.get('/api/v1/incomes/categories');
        categories.value = response.data;
        if (categories.value.length > 0 && categories.value[0]) {
            categoryId.value = categories.value[0].id;
        }
    } catch (error) {
        console.error('Failed to load categories', error);
    } finally {
        isLoading.value = false;
    }
};

const handleSave = async (andReset: boolean = false) => {
    if (!amount.value || !source.value || !categoryId.value || !date.value) return;

    try {
        isSaving.value = true;
        await api.post('/api/v1/incomes', {
            source: source.value,
            amount: amount.value,
            categoryId: categoryId.value,
            date: date.value,
            notes: notes.value,
            isRecurring: isRecurring.value,
            frequency: isRecurring.value ? frequency.value : null,
            nextDate: isRecurring.value && nextDate.value ? nextDate.value : null
        });

        showSuccess.value = true;
        setTimeout(() => (showSuccess.value = false), 3000);

        if (andReset) {
            resetForm();
        } else {
            router.push('/app/dashboard');
        }
    } catch (error) {
        console.error('Failed to save income', error);
    } finally {
        isSaving.value = false;
    }
};

const resetForm = () => {
    amount.value = null;
    source.value = '';
    date.value = new Date().toISOString().split('T')[0];
    notes.value = '';
    isRecurring.value = false;
    nextDate.value = '';
};

onMounted(loadCategories);
</script>

<template>
    <div class="max-w-xl mx-auto py-8 px-4">
        <!-- Success Alert -->
        <Transition
            enter-active-class="transform transition duration-300 ease-out"
            enter-from-class="-translate-y-4 opacity-0"
            enter-to-class="translate-y-0 opacity-100"
            leave-active-class="transition duration-200 ease-in"
            leave-from-class="opacity-100"
            leave-to-class="opacity-0"
        >
            <div v-if="showSuccess" class="mb-6 p-4 bg-accent-lime/10 border border-accent-lime/20 rounded-xl flex items-center gap-3 text-accent-lime">
                <span class="material-symbols-outlined">check_circle</span>
                <p class="text-sm font-medium">¡Ingreso registrado exitosamente!</p>
            </div>
        </Transition>

        <div class="bg-white dark:bg-card-dark border border-slate-200 dark:border-slate-800 rounded-2xl shadow-soft overflow-hidden">
            <!-- Header -->
            <div class="p-6 md:p-8 border-b border-slate-100 dark:border-slate-800">
                <h1 class="text-xs font-bold tracking-widest text-primary uppercase mb-1">Registro de Ingresos</h1>
                <p class="text-sm text-slate-500 dark:text-text-secondary-dark">Complete los detalles para su nueva entrada de capital.</p>
            </div>

            <form @submit.prevent="handleSave(false)" class="p-6 md:p-8 space-y-6">
                <!-- Amount Section -->
                <div class="bg-slate-50 dark:bg-background-dark/50 border border-slate-200 dark:border-slate-800 rounded-xl p-6 text-center">
                    <label class="block text-sm font-medium text-slate-500 mb-2">Monto del Ingreso</label>
                    <div class="flex items-center justify-center gap-2">
                        <span class="text-4xl font-bold text-primary">+</span>
                        <span class="text-4xl font-bold text-slate-400 dark:text-slate-500">₡</span>
                        <input
                            v-model="amount"
                            type="number"
                            step="0.01"
                            class="bg-transparent border-none text-4xl font-bold text-slate-900 dark:text-white focus:ring-0 w-32 p-0 placeholder-slate-200 dark:placeholder-slate-700"
                            placeholder="0.00"
                            required
                        />
                    </div>
                </div>

                <!-- Form Fields -->
                <div class="grid grid-cols-1 gap-5">
                    <!-- Source/Payer -->
                    <div class="space-y-2">
                        <label class="block text-sm font-medium text-slate-700 dark:text-slate-300">Fuente o Pagador</label>
                        <input
                            v-model="source"
                            type="text"
                            class="w-full bg-white dark:bg-background-dark border border-slate-200 dark:border-slate-800 rounded-lg py-3 px-4 focus:ring-2 focus:ring-accent-lime/20 focus:border-accent-lime outline-none transition-all dark:text-white"
                            placeholder="Ej. Salario Quincenal, Freelance..."
                            required
                        />
                    </div>

                    <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <!-- Date -->
                        <div class="space-y-2">
                            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300">Fecha del depósito</label>
                            <input
                                v-model="date"
                                type="date"
                                class="w-full bg-white dark:bg-background-dark border border-slate-200 dark:border-slate-800 rounded-lg py-3 px-4 focus:ring-2 focus:ring-accent-lime/20 focus:border-accent-lime outline-none transition-all dark:text-white"
                                required
                            />
                        </div>

                        <!-- Category -->
                        <div class="space-y-2">
                            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300">Categoría</label>
                            <select
                                v-model="categoryId"
                                class="w-full bg-white dark:bg-background-dark border border-slate-200 dark:border-slate-800 rounded-lg py-3 px-4 focus:ring-2 focus:ring-accent-lime/20 focus:border-accent-lime outline-none transition-all dark:text-white"
                                required
                            >
                                <option v-for="cat in categories" :key="cat.id" :value="cat.id">
                                    {{ cat.name }}
                                </option>
                            </select>
                        </div>
                    </div>

                    <!-- Notes -->
                    <div class="space-y-2">
                        <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 font-normal">Notas <span class="text-slate-400">(Opcional)</span></label>
                        <textarea
                            v-model="notes"
                            rows="2"
                            class="w-full bg-white dark:bg-background-dark border border-slate-200 dark:border-slate-800 rounded-lg py-3 px-4 focus:ring-2 focus:ring-accent-lime/20 focus:border-accent-lime outline-none transition-all dark:text-white resize-none"
                            placeholder="Detalles adicionales..."
                        ></textarea>
                    </div>
                </div>

                <!-- Automation Toggle -->
                <div class="border-t border-slate-100 dark:border-slate-800 pt-6">
                    <div class="flex items-center justify-between mb-4">
                        <div class="flex flex-col">
                            <span class="text-sm font-semibold dark:text-white">🔁 Automatización</span>
                            <span class="text-xs text-slate-500">Convertir en ingreso recurrente</span>
                        </div>
                        <button
                            type="button"
                            @click="isRecurring = !isRecurring"
                            class="relative inline-flex h-6 w-11 items-center rounded-full transition-colors focus:outline-none"
                            :class="isRecurring ? 'bg-primary' : 'bg-slate-200 dark:bg-slate-700'"
                        >
                            <span
                                class="inline-block h-4 w-4 transform rounded-full bg-white transition-transform"
                                :class="isRecurring ? 'translate-x-6' : 'translate-x-1'"
                            />
                        </button>
                    </div>

                    <!-- Recurrence Fields (Progressive Revelation) -->
                    <Transition
                        enter-active-class="transition duration-300 ease-out"
                        enter-from-class="opacity-0 -translate-y-2 scale-95"
                        enter-to-class="opacity-100 translate-y-0 scale-100"
                        leave-active-class="transition duration-200 ease-in"
                        leave-from-class="opacity-100 translate-y-0 scale-100"
                        leave-to-class="opacity-0 -translate-y-2 scale-95"
                    >
                        <div v-if="isRecurring" class="bg-slate-50 dark:bg-background-dark/30 border border-slate-200 dark:border-slate-800 rounded-xl p-4 space-y-4">
                            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                                <div class="space-y-2">
                                    <label class="block text-[10px] font-bold text-slate-400 uppercase tracking-wider">Frecuencia</label>
                                    <select
                                        v-model="frequency"
                                        class="w-full bg-white dark:bg-background-dark border border-slate-200 dark:border-slate-800 rounded-lg py-2 px-3 text-sm focus:ring-2 focus:ring-accent-lime/20 focus:border-accent-lime outline-none transition-all dark:text-white"
                                    >
                                        <option v-for="f in frequencies" :key="f.value" :value="f.value">{{ f.label }}</option>
                                    </select>
                                </div>
                                <div class="space-y-2">
                                    <label class="block text-[10px] font-bold text-slate-400 uppercase tracking-wider">Próximo depósito</label>
                                    <input
                                        v-model="nextDate"
                                        type="date"
                                        class="w-full bg-white dark:bg-background-dark border border-slate-200 dark:border-slate-800 rounded-lg py-2 px-3 text-sm focus:ring-2 focus:ring-accent-lime/20 focus:border-accent-lime outline-none transition-all dark:text-white"
                                    />
                                </div>
                            </div>
                        </div>
                    </Transition>
                </div>

                <!-- Actions -->
                <div class="flex flex-col gap-3 pt-4">
                    <button
                        type="submit"
                        :disabled="isSaving"
                        class="w-full bg-primary text-white font-bold py-4 rounded-xl hover:bg-primary-hover transition-all shadow-glow active:scale-[0.98] disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
                    >
                        <span v-if="isSaving" class="material-symbols-outlined animate-spin shadow-none">sync</span>
                        {{ isSaving ? 'Guardando...' : 'Guardar Ingreso' }}
                    </button>
                    <button
                        @click="handleSave(true)"
                        type="button"
                        :disabled="isSaving"
                        class="w-full bg-transparent border border-slate-200 dark:border-slate-800 text-slate-600 dark:text-slate-400 font-medium py-3 rounded-xl hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-all"
                    >
                        Guardar y registrar otro
                    </button>
                </div>
            </form>
        </div>

        <!-- Footer -->
        <footer class="mt-8 text-center">
            <div class="inline-flex items-center gap-2 text-slate-400 dark:text-slate-600 text-[10px] font-medium uppercase tracking-widest">
                <span class="material-symbols-outlined text-sm">lock</span>
                <span>Secure Session Active • Sentinel App v2.4</span>
            </div>
        </footer>
    </div>
</template>

<style scoped>
/* Chrome, Safari, Edge, Opera */
input::-webkit-outer-spin-button,
input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  appearance: none;
  margin: 0;
}

/* Firefox */
input[type=number] {
  -moz-appearance: textfield;
  appearance: textfield;
}
</style>

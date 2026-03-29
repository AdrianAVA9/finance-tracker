<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import api from '@/services/api';
import CategorySelector from '@/app/components/common/CategorySelector.vue';

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
    <div class="space-y-10 pb-20">
        <!-- Success Alert (Editorial Style) -->
        <Transition
            enter-active-class="transform transition duration-500 ease-out"
            enter-from-class="-translate-y-10 opacity-0"
            enter-to-class="translate-y-0 opacity-100"
            leave-active-class="transition duration-300 ease-in"
            leave-from-class="opacity-100"
            leave-to-class="opacity-0 scale-95"
        >
            <div v-if="showSuccess" class="fixed top-24 left-6 right-6 z-[110] p-4 bg-primary-container/10 border border-primary-container/20 backdrop-blur-xl rounded-xl flex items-center gap-3 text-primary-container luminous-shadow">
                <span class="material-symbols-outlined">check_circle</span>
                <p class="text-xs font-bold uppercase tracking-widest">Ingreso registrado exitosamente</p>
            </div>
        </Transition>

        <!-- Page Header -->
        <header class="flex items-center justify-between px-1">
            <div class="flex items-center gap-4">
                <button 
                    @click="router.back()"
                    class="p-2 rounded-lg hover:bg-surface-container-high transition-colors text-on-surface-variant"
                    aria-label="Volver"
                >
                    <span class="material-symbols-outlined">arrow_back</span>
                </button>
                <div class="space-y-1">
                    <h1 class="font-headline text-2xl font-black tracking-tighter text-on-surface">Registro de Ingresos</h1>
                    <p class="text-xs font-bold text-on-surface-variant uppercase tracking-widest">Nueva Entrada de Capital</p>
                </div>
            </div>
        </header>

        <form @submit.prevent="handleSave(false)" class="space-y-8 animate-fade-in-up">
            <!-- Hero Amount Section -->
            <section class="relative group">
                <label class="block text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant mb-4 px-1">Monto del Ingreso</label>
                <div class="bg-surface-container-low p-10 rounded-xl border border-white/[0.03] luminous-shadow-sm flex flex-col items-center justify-center transition-all duration-500 focus-within:bg-primary-container/[0.02] focus-within:border-primary-container/20">
                    <div class="flex items-baseline gap-2">
                        <span class="font-headline text-4xl font-black text-primary-container">+</span>
                        <span class="font-headline text-4xl font-black text-primary-container">₡</span>
                        <input
                            v-model="amount"
                            type="number"
                            step="0.01"
                            class="bg-transparent border-none text-center font-headline text-6xl font-black tracking-tighter focus:ring-0 text-on-surface w-full max-w-[280px] placeholder:text-on-surface-variant/20"
                            placeholder="0"
                            required
                        />
                    </div>
                </div>
            </section>

            <!-- Main Form Fields -->
            <div class="grid grid-cols-1 gap-6">
                <!-- Source/Payer -->
                <section class="space-y-4">
                    <label class="block text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant px-1">Fuente o Pagador</label>
                    <input
                        v-model="source"
                        type="text"
                        class="w-full bg-surface-container p-5 rounded-xl border border-white/[0.03] focus:border-primary-container/30 focus:ring-0 transition-all font-body text-on-surface font-semibold"
                        placeholder="Ej. Salario Quincenal, Freelance..."
                        required
                    />
                </section>

                <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <!-- Date -->
                    <section class="space-y-4">
                        <label class="block text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant px-1">Fecha del Depósito</label>
                        <input
                            v-model="date"
                            type="date"
                            class="w-full bg-surface-container p-5 rounded-xl border border-white/[0.03] focus:border-primary-container/30 focus:ring-0 transition-all font-body text-on-surface font-semibold [color-scheme:dark]"
                            required
                        />
                    </section>

                    <!-- Category Selector -->
                    <section class="space-y-4">
                        <label class="block text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant px-1">Categoría</label>
                        <CategorySelector 
                            v-model="categoryId" 
                            :categories="categories" 
                            placeholder="Selecciona una categoría"
                        />
                    </section>
                </div>

                <!-- Notes -->
                <section class="space-y-4">
                    <label class="block text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant px-1">Notas <span class="opacity-40">(Opcional)</span></label>
                    <textarea
                        v-model="notes"
                        rows="2"
                        class="w-full bg-surface-container p-5 rounded-xl border border-white/[0.03] focus:border-primary-container/30 focus:ring-0 transition-all font-body text-on-surface font-semibold resize-none"
                        placeholder="Detalles adicionales..."
                    ></textarea>
                </section>
            </div>

            <!-- Automation Toggle -->
            <section class="bg-surface-container-low p-6 rounded-xl border border-white/[0.02]">
                <div class="flex items-center justify-between mb-2">
                    <div class="flex gap-4 items-center">
                        <div class="w-12 h-12 rounded-xl bg-primary-container/10 flex items-center justify-center text-primary-container">
                            <span class="material-symbols-outlined text-2xl">sync</span>
                        </div>
                        <div class="space-y-0.5">
                            <h4 class="text-sm font-bold text-on-surface">Automatización</h4>
                            <p class="text-[10px] font-medium text-on-surface-variant uppercase tracking-wide">Ingreso Recurrente</p>
                        </div>
                    </div>
                    <!-- Toggle Switch -->
                    <label class="relative inline-flex items-center cursor-pointer group">
                        <input
                            type="checkbox"
                            v-model="isRecurring"
                            class="sr-only peer"
                        />
                        <div class="w-12 h-6 bg-surface-container-highest rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:start-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-primary-container shadow-lg group-active:scale-95"></div>
                    </label>
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
                    <div v-if="isRecurring" class="mt-6 pt-6 border-t border-white/[0.03] grid grid-cols-1 md:grid-cols-2 gap-6">
                        <div class="space-y-3">
                            <label class="block text-[10px] font-bold text-on-surface-variant uppercase tracking-wider">Frecuencia</label>
                            <div class="relative">
                                <select
                                    v-model="frequency"
                                    class="w-full appearance-none bg-surface-container-high p-4 rounded-xl border border-white/[0.03] text-sm font-bold text-on-surface focus:ring-1 focus:ring-primary-container/30 outline-none transition-all"
                                >
                                    <option v-for="f in frequencies" :key="f.value" :value="f.value">{{ f.label }}</option>
                                </select>
                                <span class="material-symbols-outlined absolute right-3 top-1/2 -translate-y-1/2 text-on-surface-variant pointer-events-none text-xl">unfold_more</span>
                            </div>
                        </div>
                        <div class="space-y-3">
                            <label class="block text-[10px] font-bold text-on-surface-variant uppercase tracking-wider">Próximo Depósito</label>
                            <input
                                v-model="nextDate"
                                type="date"
                                class="w-full bg-surface-container-high p-4 rounded-xl border border-white/[0.03] text-sm font-bold text-on-surface focus:ring-1 focus:ring-primary-container/30 outline-none transition-all [color-scheme:dark]"
                            />
                        </div>
                    </div>
                </Transition>
            </section>

            <!-- Actions -->
            <div class="pt-6 space-y-4">
                <button
                    type="submit"
                    :disabled="isSaving"
                    class="w-full bg-primary-container text-on-primary-container font-headline font-black py-5 rounded-xl hover:brightness-110 active:scale-[0.98] transition-all shadow-xl shadow-primary-container/20 disabled:opacity-50 flex items-center justify-center gap-3"
                >
                    <span v-if="isSaving" class="material-symbols-outlined animate-spin shadow-none">sync</span>
                    <span v-else class="material-symbols-outlined">save</span>
                    {{ isSaving ? 'Guardando...' : 'Guardar Ingreso' }}
                </button>
                <button
                    @click="handleSave(true)"
                    type="button"
                    :disabled="isSaving"
                    class="w-full bg-surface-container-high text-on-surface font-headline font-bold py-5 rounded-xl hover:bg-surface-variant active:scale-[0.98] transition-all"
                >
                    Guardar y Registrar Otro
                </button>
            </div>
        </form>

        <!-- Editorial Info -->
        <footer class="mt-8 text-center flex flex-col items-center gap-2">
            <div class="inline-flex items-center gap-2 text-on-surface-variant/40 text-[9px] font-bold uppercase tracking-[0.2em]">
                <span class="material-symbols-outlined text-sm">lock</span>
                <span>Sentinel Secure v2.4 • Session Active</span>
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

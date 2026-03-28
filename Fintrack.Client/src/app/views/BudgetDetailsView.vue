<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRoute } from 'vue-router';
import budgetService, { type BudgetDetailsDto } from '@/services/budgetService';

const route = useRoute();
const budgetData = ref<BudgetDetailsDto | null>(null);
const isLoading = ref(true);
const openMonthIndex = ref<number | null>(null);

const fetchDetails = async () => {
    try {
        isLoading.value = true;
        const id = parseInt(route.params.id as string, 10);
        const now = new Date();
        // Use current month/year as the target for the details view
        budgetData.value = await budgetService.getBudgetDetails(id, now.getMonth() + 1, now.getFullYear());
    } catch (error) {
        console.error('Failed to fetch budget details:', error);
    } finally {
        isLoading.value = false;
    }
};

onMounted(() => {
    fetchDetails();
});

const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('es-CR', {
        style: 'currency',
        currency: 'CRC',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0,
    }).format(value);
};

const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('es-ES', { month: 'short', day: '2-digit', year: 'numeric' });
};

// Data for the Chart Component
const chartData = computed(() => {
    if (!budgetData.value || !Array.isArray(budgetData.value.monthlyHistory)) return [];
    if (!budgetData.value?.monthlyHistory || !Array.isArray(budgetData.value.monthlyHistory)) return [];
    // The dto gives recent first. We want chronological for the chart (left to right)
    return [...budgetData.value.monthlyHistory].reverse();
});

const maxExpense = computed(() => {
    if (!budgetData.value?.monthlyHistory || !Array.isArray(budgetData.value.monthlyHistory)) return budgetData.value?.limitAmount || 0;
    const maxVal = budgetData.value.monthlyHistory.length > 0
        ? Math.max(...budgetData.value.monthlyHistory.map(m => m.totalExpense || 0))
        : 0;
    return Math.max(maxVal, budgetData.value.limitAmount || 0);
});

const totalSpentYTD = computed(() => {
    if (!budgetData.value?.monthlyHistory || !Array.isArray(budgetData.value.monthlyHistory)) return 0;
    const currentYear = new Date().getFullYear();
    return budgetData.value.monthlyHistory
        .filter(m => m.year === currentYear)
        .reduce((sum, m) => sum + (m.totalExpense || 0), 0);
});

const toggleMonth = (index: number) => {
    openMonthIndex.value = openMonthIndex.value === index ? null : index;
};

const getMonthName = (month: number) => {
    const date = new Date();
    date.setMonth(month - 1);
    return date.toLocaleString('es-ES', { month: 'short' });
};
</script>

<template>
    <div v-if="budgetData" class="space-y-8 pb-10">
        <!-- Header Section -->
        <section class="space-y-4">
            <h1 class="text-3xl font-extrabold font-headline tracking-tight text-[#F3F4F6]">
                {{ budgetData.categoryName }}
            </h1>

            <!-- Total Spent YTD Widget -->
            <div class="bg-[#1a1c20] border border-white/5 rounded-lg p-4 flex items-center justify-between mt-6 shadow-sm shadow-black/20">
                <span class="text-[#bacbbe] font-label text-[11px] font-bold tracking-[0.1em] uppercase">
                    Total Gastado Este Año
                </span>
                <span class="text-[#05e699] font-headline font-semibold text-xl">
                    {{ formatCurrency(totalSpentYTD) }}
                </span>
            </div>
        </section>

        <!-- Chart Section -->
        <section class="bg-[#1a1c20] border border-white/5 p-5 rounded-lg flex flex-col gap-4 relative overflow-hidden">
            <div class="absolute inset-0 pointer-events-none" 
                 style="background: linear-gradient(180deg, rgba(5, 230, 153, 0.08) 0%, rgba(5, 230, 153, 0) 100%);">
            </div>
            <h3 class="font-headline font-bold text-lg text-[#F3F4F6] relative z-10">Resumen 12 Meses</h3>
            
            <div class="relative h-48 w-full pt-6 z-10">
                <!-- Max label placeholder line  -->
                <div class="absolute top-0 left-0 w-full flex items-center gap-2 opacity-50 z-0">
                    <span class="text-[10px] font-label">{{ formatCurrency(maxExpense) }}</span>
                    <div class="h-px flex-1 border-dashed border-b border-[#bacbbe]/30"></div>
                </div>

                <!-- Custom Bar Chart items -->
                <div class="flex items-end justify-between gap-1 h-full w-full relative z-10">
                    <div v-for="(item, index) in chartData" :key="index" class="flex flex-col items-center flex-1 h-full justify-end group cursor-pointer relative">
                        <!-- Tooltip approximation (visible on hover) -->
                        <div class="opacity-0 group-hover:opacity-100 transition-opacity absolute -top-10 bg-[#333539] text-[#e2e2e8] text-[10px] py-1 px-2 rounded font-label whitespace-nowrap z-20 pointer-events-none mb-1 shadow-lg border border-[rgba(255,255,255,0.05)]">
                            {{ formatCurrency(item.totalExpense) }}
                        </div>

                        <!-- Bar -->
                        <div class="w-full max-w-[20px] rounded-t-sm flex items-end justify-center relative transition-all duration-300 group-hover:-translate-y-1 shadow-[0_0_10px_rgba(5,230,153,0.1)]"
                             :style="{ height: maxExpense > 0 ? ((item.totalExpense / maxExpense) * 100) + '%' : '0px' }"
                             :class="item.totalExpense > 0 ? 'bg-[#05e699]' : 'bg-[#333539]'"
                             style="min-height: 4px;">
                        </div>

                        <!-- Month Label -->
                        <span class="text-[10px] font-label uppercase mt-2 opacity-80" 
                              :class="{ 'font-bold text-[#F3F4F6] opacity-100': item.totalExpense > 0 }">
                            {{ getMonthName(item.month).charAt(0) }}
                        </span>
                    </div>
                </div>
            </div>
        </section>

        <!-- Detailed Monthly Report (Accordion) -->
        <section class="space-y-4">
            <h3 class="font-headline font-bold text-[10px] uppercase tracking-[0.2em] text-[#bacbbe]">Historial Detallado</h3>
            
            <div class="space-y-3">
                <div v-for="(monthData, index) in budgetData.monthlyHistory" :key="index"
                     class="rounded-lg overflow-hidden transition-all duration-300 border border-white/5"
                     :class="[
                        openMonthIndex === index 
                            ? 'bg-[#282a2e] shadow-2xl border-[#05e699]/10' 
                            : 'bg-[#1e2024] hover:bg-[#222428]'
                     ]">
                    
                    <!-- Accordion Header -->
                    <button class="w-full flex items-center justify-between p-4 text-left"
                            @click="toggleMonth(index)">
                        <div>
                            <span class="block font-headline font-bold text-[#e2e2e8] capitalize">
                                {{ getMonthName(monthData.month) }} {{ monthData.year }}
                            </span>
                            <span class="block text-xs font-medium text-[#bacbbe]">
                                {{ budgetData.limitAmount > 0 ? Math.round((monthData.totalExpense / budgetData.limitAmount) * 100) : 0 }}% gastado
                            </span>
                        </div>
                        <div class="flex items-center gap-4">
                            <span class="font-headline font-bold text-[#F3F4F6]">
                                {{ formatCurrency(monthData.totalExpense) }}
                            </span>
                            <span class="material-symbols-outlined text-[#bacbbe] transition-transform duration-300"
                                  :class="{ 'rotate-180': openMonthIndex === index }">
                                expand_more
                            </span>
                        </div>
                    </button>

                    <!-- Accordion Content -->
                    <div v-if="openMonthIndex === index" class="px-4 pb-4 space-y-2 border-t border-white/5 pt-3">
                        <div v-if="monthData.expenses.length === 0" class="text-center py-4 text-[#bacbbe] text-sm font-label">
                            No hay gastos registrados este mes.
                        </div>
                        <div v-for="expense in monthData.expenses" :key="expense.id"
                             class="flex justify-between items-center py-2">
                            <div class="truncate mr-4">
                                <p class="text-sm font-bold text-[#e2e2e8] truncate">{{ expense.description }}</p>
                                <p class="text-xs text-[#bacbbe]">{{ formatDate(expense.date) }}</p>
                            </div>
                            <span class="text-sm font-bold text-[#FF4D4D] whitespace-nowrap">
                                -{{ formatCurrency(expense.amount) }}
                            </span>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <!-- Loading State -->
    <div v-else-if="isLoading" class="flex flex-col items-center justify-center min-h-[400px]">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-[#05E699]"></div>
        <p class="mt-4 text-[#bacbbe] font-label">Cargando detalles...</p>
    </div>

    <!-- Error State -->
    <div v-else class="flex flex-col items-center justify-center min-h-[400px]">
        <span class="material-symbols-outlined text-[#FF4D4D] text-5xl mb-4">error</span>
        <h4 class="text-lg font-bold text-[#F3F4F6] font-headline">Presupuesto no encontrado</h4>
        <p class="text-[#bacbbe] text-sm font-label">Retrocede e intenta nuevamente.</p>
    </div>
</template>

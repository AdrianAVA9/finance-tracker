<script setup lang="ts">
import { ref, onMounted } from 'vue';
import dashboardService, { type DashboardSummaryDto } from '@/services/dashboardService';

const dashboardData = ref<DashboardSummaryDto | null>(null);
const isLoading = ref(true);

const fetchDashboardData = async () => {
    try {
        isLoading.value = true;
        dashboardData.value = await dashboardService.getDashboardSummary();
    } catch (error) {
        console.error('Failed to fetch dashboard data:', error);
    } finally {
        isLoading.value = false;
    }
};

onMounted(() => {
    fetchDashboardData();
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
    const now = new Date();
    const isToday = date.toDateString() === now.toDateString();
    
    if (isToday) {
        return `Hoy, ${date.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' })}`;
    }
    
    return date.toLocaleDateString('es-ES', {
        month: 'short',
        day: '2-digit',
        year: date.getFullYear() !== now.getFullYear() ? 'numeric' : undefined
    }) + `, ${date.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' })}`;
};


</script>

<template>
    <div v-if="dashboardData" class="space-y-8">
        <!-- Hero Balance Section -->
        <section class="space-y-1">
            <p class="text-[#bacbbe] font-label text-xs uppercase tracking-[0.2em] font-medium">Balance Total</p>
            <h1 class="text-4xl font-extrabold font-headline tracking-tight text-[#F3F4F6]">
                {{ formatCurrency(dashboardData.totalBalance) }}
            </h1>
            
            <div class="flex gap-6 pt-4">
                <div class="flex flex-col">
                    <span class="text-[10px] text-[#bacbbe] uppercase font-bold tracking-wider">Ingresos</span>
                    <span class="text-[#05E699] font-headline font-bold text-lg">+{{ formatCurrency(dashboardData.monthlyIncome) }}</span>
                </div>
                <div class="flex flex-col">
                    <span class="text-[10px] text-[#bacbbe] uppercase font-bold tracking-wider">Gastos</span>
                    <span class="text-[#FF4D4D] font-headline font-bold text-lg">-{{ formatCurrency(dashboardData.monthlyExpenses) }}</span>
                </div>
            </div>
        </section>

        <!-- Top Budgets List -->
        <section class="space-y-4">
            <div class="flex justify-between items-center">
                <h3 class="font-headline font-bold text-xl tracking-tight">Presupuestos</h3>
            </div>
            
            <div class="space-y-2">
                <router-link v-for="budget in dashboardData.topBudgets" :key="budget.id" 
                     :to="`/app/budgets/${budget.id}`"
                     class="block bg-[#1a1c20] p-3.5 rounded-xl space-y-3 hover:bg-[#222428] transition-colors cursor-pointer group">
                    <div class="flex justify-between items-center">
                        <div class="flex items-center gap-3">
                            <div class="w-10 h-10 rounded-xl flex items-center justify-center bg-[#333539]"
                                 :style="budget.color ? { backgroundColor: budget.color + '25' } : {}">
                                <span class="material-symbols-outlined text-[20px]"
                                      :style="budget.color ? { color: budget.color } : { color: '#bacbbe' }">
                                    {{ budget.icon || 'category' }}
                                </span>
                            </div>
                            <div>
                                <h4 class="font-bold text-sm text-[#e2e2e8] group-hover:text-white transition-colors">{{ budget.categoryName }}</h4>
                                <p class="text-xs text-[#bacbbe] mt-0.5">{{ formatCurrency(budget.remainingAmount) }} remaining</p>
                            </div>
                        </div>
                        <div class="flex items-center gap-2">
                             <span class="font-headline font-bold text-sm text-[#e2e2e8]">{{ Math.round(budget.percentage) }}%</span>
                             <span class="material-symbols-outlined text-[#bacbbe] text-sm opacity-0 group-hover:opacity-100 transition-opacity -translate-x-2 group-hover:translate-x-0">chevron_right</span>
                        </div>
                    </div>
                    
                    <div class="w-full bg-[#333539] h-1.5 rounded-full overflow-hidden">
                        <div class="h-full rounded-full" 
                             :style="{ 
                                width: Math.min(budget.percentage, 100) + '%', 
                                backgroundColor: budget.color || '#05e699' 
                             }"></div>
                    </div>
                </router-link>
                
                <!-- Empty State -->
                <div v-if="dashboardData.topBudgets.length === 0" class="bg-[#1a1c20] p-4 rounded-xl text-center border border-white/5">
                    <p class="text-[#bacbbe] text-sm">No active budgets for this month.</p>
                </div>
            </div>
        </section>

        <!-- Recent Activity -->
        <section class="space-y-4">
            <h3 class="font-headline font-bold text-xl tracking-tight">Actividad Reciente</h3>
            <div class="space-y-1">
                <div v-for="tx in dashboardData.recentTransactions" :key="tx.id" 
                     class="flex items-center justify-between p-4 bg-[#1a1c20] rounded-xl">
                    <div class="flex items-center gap-4 max-w-[60%]">
                        <div class="w-12 h-12 rounded-xl bg-[#1e2024] flex items-center justify-center shrink-0">
                            <span class="material-symbols-outlined text-white" 
                                  :style="{ color: tx.categoryColor || '#fff' }">
                                {{ tx.categoryIcon || 'payments' }}
                            </span>
                        </div>
                        <div class="truncate">
                            <p class="font-bold text-sm truncate">{{ tx.description }}</p>
                            <p class="text-xs text-[#bacbbe]">{{ tx.categoryName }} • {{ formatDate(tx.date) }}</p>
                        </div>
                    </div>
                    <span :class="[
                        'font-headline font-bold text-sm sm:text-base',
                        tx.amount >= 0 ? 'text-[#05E699]' : 'text-[#FF4D4D]'
                    ]">
                        {{ tx.amount >= 0 ? '+' : '' }}{{ formatCurrency(tx.amount) }}
                    </span>
                </div>
            </div>
        </section>


    </div>

    <!-- Loading State -->
    <div v-else-if="isLoading" class="flex flex-col items-center justify-center min-h-[400px]">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-[#05E699]"></div>
        <p class="mt-4 text-[#bacbbe]">Cargando datos del tablero...</p>
    </div>

    <!-- Error State -->
    <div v-else class="flex flex-col items-center justify-center min-h-[400px]">
        <span class="material-symbols-outlined text-[#FF4D4D] text-5xl mb-4">error</span>
        <h4 class="text-lg font-bold text-[#F3F4F6]">Error al cargar datos</h4>
        <p class="text-[#bacbbe] text-sm">Por favor intenta recargar la página más tarde.</p>
    </div>
</template>

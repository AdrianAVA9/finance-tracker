<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
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
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD',
    }).format(value);
};

const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    const now = new Date();
    const isToday = date.toDateString() === now.toDateString();
    
    if (isToday) {
        return `Today, ${date.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' })}`;
    }
    
    return date.toLocaleDateString('en-US', {
        month: 'short',
        day: '2-digit',
        year: date.getFullYear() !== now.getFullYear() ? 'numeric' : undefined
    }) + `, ${date.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' })}`;
};

const maxChartValue = computed(() => {
    if (!dashboardData.value || dashboardData.value.monthlyData.length === 0) return 1000;
    const values = dashboardData.value.monthlyData.flatMap(d => [d.income, d.expense]);
    const max = Math.max(...values);
    return max === 0 ? 1000 : max * 1.1; // 10% padding
});

const getBarHeight = (value: number) => {
    return `${(value / maxChartValue.value) * 100}%`;
};

const getCategoryPercentage = (amount: number) => {
    if (!dashboardData.value || dashboardData.value.monthlyExpenses === 0) return 0;
    return Math.round((amount / dashboardData.value.monthlyExpenses) * 100);
};

const getCategoryDashOffset = (index: number) => {
    if (!dashboardData.value) return 0;
    let offset = 0;
    for (let i = 0; i < index; i++) {
        const cat = dashboardData.value.topSpendingCategories[i];
        if (cat) {
            offset += getCategoryPercentage(cat.amount);
        }
    }
    return -offset;
};
</script>

<template>
    <template v-if="dashboardData">
    <!-- Stats Grid -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <!-- Total Balance -->
        <div
            class="bg-card-light dark:bg-card-dark p-6 rounded-2xl border border-slate-200 dark:border-slate-800 shadow-soft relative overflow-hidden group">
            <div class="absolute right-0 top-0 p-6 opacity-10 group-hover:opacity-20 transition-opacity">
                <span class="material-symbols-outlined text-[80px] text-primary">account_balance</span>
            </div>
            <div class="relative z-10">
                <p class="text-slate-500 dark:text-text-secondary-dark text-sm font-medium mb-1">Total Balance</p>
                <h3 class="text-3xl font-extrabold text-slate-900 dark:text-white tracking-tight mb-4">
                    {{ formatCurrency(dashboardData.totalBalance) }}
                </h3>
                <div class="flex items-center gap-2">
                    <span
                        :class="[
                            dashboardData.totalBalance >= 0 ? 'bg-emerald-100 dark:bg-emerald-500/10 text-emerald-600 dark:text-accent-lime' : 'bg-rose-100 dark:bg-rose-500/10 text-rose-600 dark:text-rose-400',
                            'px-2 py-0.5 rounded-full text-xs font-bold flex items-center gap-1'
                        ]">
                        <span class="material-symbols-outlined text-[14px]">
                            {{ dashboardData.totalBalance >= 0 ? 'trending_up' : 'trending_down' }}
                        </span> 
                        {{ dashboardData.totalBalance >= 0 ? 'Active' : 'Negative' }}
                    </span>
                    <span class="text-slate-400 text-xs">Current health</span>
                </div>
            </div>
        </div>

        <!-- Monthly Income -->
        <div
            class="bg-card-light dark:bg-card-dark p-6 rounded-2xl border border-slate-200 dark:border-slate-800 shadow-soft relative overflow-hidden group">
            <div class="absolute right-0 top-0 p-6 opacity-10 group-hover:opacity-20 transition-opacity">
                <span class="material-symbols-outlined text-[80px] text-emerald-500">payments</span>
            </div>
            <div class="relative z-10">
                <p class="text-slate-500 dark:text-text-secondary-dark text-sm font-medium mb-1">Monthly Income</p>
                <h3 class="text-3xl font-extrabold text-slate-900 dark:text-white tracking-tight mb-4">
                    {{ formatCurrency(dashboardData.monthlyIncome) }}
                </h3>
                <div class="flex items-center gap-2">
                    <span
                        :class="[
                            dashboardData.incomeChangePercentage >= 0 ? 'bg-emerald-100 dark:bg-emerald-500/10 text-emerald-600 dark:text-accent-lime' : 'bg-rose-100 dark:bg-rose-500/10 text-rose-600 dark:text-rose-400',
                            'px-2 py-0.5 rounded-full text-xs font-bold flex items-center gap-1'
                        ]">
                        <span class="material-symbols-outlined text-[14px]">
                            {{ dashboardData.incomeChangePercentage >= 0 ? 'arrow_upward' : 'arrow_downward' }}
                        </span> 
                        {{ Math.abs(Math.round(dashboardData.incomeChangePercentage)) }}%
                    </span>
                    <span class="text-slate-400 text-xs">vs last month</span>
                </div>
            </div>
        </div>

        <!-- Monthly Expenses -->
        <div
            class="bg-card-light dark:bg-card-dark p-6 rounded-2xl border border-slate-200 dark:border-slate-800 shadow-soft relative overflow-hidden group">
            <div class="absolute right-0 top-0 p-6 opacity-10 group-hover:opacity-20 transition-opacity">
                <span class="material-symbols-outlined text-[80px] text-rose-500">credit_card</span>
            </div>
            <div class="relative z-10">
                <p class="text-slate-500 dark:text-text-secondary-dark text-sm font-medium mb-1">Monthly Expenses</p>
                <h3 class="text-3xl font-extrabold text-slate-900 dark:text-white tracking-tight mb-4">
                    {{ formatCurrency(dashboardData.monthlyExpenses) }}
                </h3>
                <div class="flex items-center gap-2">
                    <span
                        :class="[
                            dashboardData.expenseChangePercentage <= 0 ? 'bg-emerald-100 dark:bg-emerald-500/10 text-emerald-600 dark:text-accent-lime' : 'bg-rose-100 dark:bg-rose-500/10 text-rose-600 dark:text-rose-400',
                            'px-2 py-0.5 rounded-full text-xs font-bold flex items-center gap-1'
                        ]">
                        <span class="material-symbols-outlined text-[14px]">
                            {{ dashboardData.expenseChangePercentage <= 0 ? 'arrow_downward' : 'arrow_upward' }}
                        </span> 
                        {{ Math.abs(Math.round(dashboardData.expenseChangePercentage)) }}%
                    </span>
                    <span class="text-slate-400 text-xs">vs last month</span>
                </div>
            </div>
        </div>
    </div>

    <!-- Bento Grid Row 2 -->
    <div class="grid grid-cols-1 lg:grid-cols-12 gap-6">
        <!-- Main Chart -->
        <div
            class="lg:col-span-8 bg-card-light dark:bg-card-dark p-6 rounded-2xl border border-slate-200 dark:border-slate-800 shadow-soft flex flex-col">
            <div class="flex items-center justify-between mb-6">
                <div>
                    <h4 class="text-lg font-bold text-slate-900 dark:text-white">Income vs Expenses</h4>
                    <p class="text-sm text-slate-500 dark:text-text-secondary-dark">Net positive flow for current
                        quarter</p>
                </div>
                <button
                    class="text-primary dark:text-white hover:bg-slate-100 dark:hover:bg-slate-700 p-2 rounded-lg transition-colors">
                    <span class="material-symbols-outlined">more_horiz</span>
                </button>
            </div>

            <div class="flex-1 min-h-[250px] w-full relative pt-4">
                <!-- Custom CSS Chart -->
                <div class="absolute inset-0 flex items-end justify-between gap-2 md:gap-4 px-2">
                    <!-- Grid Lines -->
                    <div class="absolute inset-0 flex flex-col justify-between pointer-events-none opacity-10">
                        <div class="w-full h-px bg-slate-500"></div>
                        <div class="w-full h-px bg-slate-500"></div>
                        <div class="w-full h-px bg-slate-500"></div>
                        <div class="w-full h-px bg-slate-500"></div>
                        <div class="w-full h-px bg-slate-500"></div>
                    </div>
                    <!-- Bars (Pairs) -->
                    <div class="w-full h-full flex items-end justify-around gap-2 z-10 pb-6">
                        <div v-for="item in dashboardData.monthlyData" :key="item.month" class="flex flex-col items-center gap-2 h-full justify-end w-1/6 group">
                            <div class="flex items-end gap-1 h-full w-full justify-center">
                                <div class="w-3 md:w-5 bg-primary rounded-t-sm opacity-90 group-hover:opacity-100 transition-all bar-animate"
                                    :style="{ height: getBarHeight(item.income) }"></div>
                                <div class="w-3 md:w-5 bg-slate-300 dark:bg-slate-600 rounded-t-sm opacity-90 group-hover:opacity-100 transition-all bar-animate"
                                    :style="{ height: getBarHeight(item.expense) }"></div>
                            </div>
                            <span class="text-xs font-bold text-slate-400">{{ item.month }}</span>
                        </div>
                    </div>
                </div>
            </div>
            <div class="flex items-center gap-6 mt-4 justify-center">
                <div class="flex items-center gap-2">
                    <div class="size-3 rounded-full bg-primary"></div>
                    <span class="text-xs font-medium text-slate-500 dark:text-slate-400">Income</span>
                </div>
                <div class="flex items-center gap-2">
                    <div class="size-3 rounded-full bg-slate-300 dark:bg-slate-600"></div>
                    <span class="text-xs font-medium text-slate-500 dark:text-slate-400">Expense</span>
                </div>
            </div>
        </div>

        <!-- AI Quick Scan & Mini Categories Pie -->
        <div class="lg:col-span-4 flex flex-col gap-6">
            <!-- Scan Card -->
            <div
                class="flex-1 bg-gradient-to-br from-slate-50 to-slate-100 dark:from-[#2B3036] dark:to-[#1c1f22] p-1 rounded-2xl shadow-soft border border-slate-200 dark:border-slate-800 relative group overflow-hidden">
                <div
                    class="absolute inset-0 bg-gradient-to-r from-transparent via-primary/5 to-transparent -translate-x-full group-hover:translate-x-full transition-transform duration-1000">
                </div>
                <router-link to="/app/expenses/upload"
                    class="block h-full border-2 border-dashed border-slate-300 dark:border-slate-600 hover:border-primary dark:hover:border-primary rounded-xl flex flex-col items-center justify-center text-center p-6 transition-colors bg-white/50 dark:bg-transparent cursor-pointer">
                    <div
                        class="size-16 rounded-full bg-primary/10 dark:bg-primary/20 flex items-center justify-center mb-4 group-hover:scale-110 transition-transform">
                        <span class="material-symbols-outlined text-primary text-3xl">document_scanner</span>
                    </div>
                    <h4 class="text-lg font-bold text-slate-900 dark:text-white mb-2">AI Quick Scan</h4>
                    <p class="text-sm text-slate-500 dark:text-text-secondary-dark mb-6 max-w-[200px]">Drop your receipt
                        here to auto-log expenses instantly.</p>
                    <button
                        class="px-6 py-2.5 bg-primary hover:bg-[#17525e] text-white rounded-lg text-sm font-bold shadow-lg shadow-primary/20 transition-all flex items-center gap-2">
                        <span class="material-symbols-outlined text-lg">upload_file</span>
                        Upload File
                    </button>
                </router-link>
            </div>

            <!-- Mini Categories Pie -->
            <div
                class="h-64 bg-card-light dark:bg-card-dark p-6 rounded-2xl border border-slate-200 dark:border-slate-800 shadow-soft flex flex-col">
                <h4 class="text-base font-bold text-slate-900 dark:text-white mb-4">Top Spending</h4>
                <div class="flex-1 flex items-center gap-4">
                    <!-- Donut Chart Representation -->
                    <div class="relative size-32 shrink-0">
                        <svg class="w-full h-full rotate-[-90deg]" viewBox="0 0 36 36">
                            <!-- Background Ring -->
                            <path class="text-slate-800"
                                d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831"
                                fill="none" stroke="#2B3036" stroke-width="4.5"></path>
                            
                            <!-- Dynamic Rings -->
                            <path v-for="(cat, index) in dashboardData.topSpendingCategories" 
                                :key="cat.categoryName"
                                :style="{ color: cat.color || 'var(--color-primary)' }"
                                d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831"
                                fill="none" 
                                stroke="currentColor" 
                                :stroke-dasharray="`${getCategoryPercentage(cat.amount)}, 100`" 
                                :stroke-dashoffset="getCategoryDashOffset(index)"
                                stroke-width="4.5"></path>
                        </svg>
                        <div class="absolute inset-0 flex flex-col items-center justify-center">
                            <span class="text-xl font-bold dark:text-white">{{ getCategoryPercentage(dashboardData.topSpendingCategories[0]?.amount || 0) }}%</span>
                            <span class="text-[10px] uppercase text-slate-500 font-bold">{{ dashboardData.topSpendingCategories[0]?.categoryName || 'None' }}</span>
                        </div>
                    </div>
                    <div class="flex flex-col gap-3 w-full">
                        <div v-for="cat in dashboardData.topSpendingCategories" :key="cat.categoryName" class="flex items-center justify-between text-xs">
                            <div class="flex items-center gap-2">
                                <div class="size-2 rounded-full" :style="{ backgroundColor: cat.color || 'var(--color-primary)' }"></div>
                                <span class="text-slate-600 dark:text-slate-300 font-medium">{{ cat.categoryName }}</span>
                            </div>
                            <span class="font-bold dark:text-white">{{ formatCurrency(cat.amount) }}</span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Recent Transactions Table -->
    <div
        class="bg-card-light dark:bg-card-dark rounded-2xl border border-slate-200 dark:border-slate-800 shadow-soft overflow-hidden">
        <div class="p-6 border-b border-slate-200 dark:border-slate-800 flex items-center justify-between">
            <h4 class="text-lg font-bold text-slate-900 dark:text-white">Recent Transactions</h4>
            <router-link to="/app/expenses" class="text-primary text-sm font-bold hover:underline">View
                All</router-link>
        </div>
        <div class="overflow-x-auto">
            <table class="w-full text-left border-collapse">
                <thead>
                    <tr
                        class="bg-slate-50 dark:bg-[#23272b] text-slate-500 dark:text-slate-400 text-xs uppercase tracking-wider">
                        <th class="p-4 font-semibold rounded-tl-lg">Transaction</th>
                        <th class="p-4 font-semibold">Category</th>
                        <th class="p-4 font-semibold">Date</th>
                        <th class="p-4 font-semibold text-right rounded-tr-lg">Amount</th>
                    </tr>
                </thead>
                <tbody class="text-sm">
                    <tr v-for="tx in dashboardData.recentTransactions" :key="tx.id"
                        class="border-b border-slate-100 dark:border-slate-800 hover:bg-slate-50 dark:hover:bg-[#32383f] transition-colors group">
                        <td class="p-4">
                            <div class="flex items-center gap-3">
                                <div
                                    class="size-10 rounded-full flex items-center justify-center shrink-0"
                                    :style="{ backgroundColor: (tx.categoryColor || '#64748b') + '20', color: tx.categoryColor || '#64748b' }">
                                    <span class="material-symbols-outlined text-[20px]">{{ tx.categoryIcon || 'receipt_long' }}</span>
                                </div>
                                <div>
                                    <p
                                        class="font-bold text-slate-900 dark:text-white group-hover:text-primary transition-colors">
                                        {{ tx.description }}</p>
                                    <p class="text-xs text-slate-500 dark:text-slate-400">{{ tx.type }}</p>
                                </div>
                            </div>
                        </td>
                        <td class="p-4">
                            <span
                                class="px-2.5 py-1 rounded-full text-xs font-bold border"
                                :style="{ 
                                    backgroundColor: (tx.categoryColor || '#64748b') + '10', 
                                    borderColor: (tx.categoryColor || '#64748b') + '30',
                                    color: tx.categoryColor || '#64748b'
                                }">
                                {{ tx.categoryName }}
                            </span>
                        </td>
                        <td class="p-4 text-slate-600 dark:text-slate-400 font-medium">
                            {{ formatDate(tx.date) }}
                        </td>
                        <td :class="['p-4 text-right font-bold', tx.amount >= 0 ? 'text-emerald-600 dark:text-emerald-400' : 'text-slate-900 dark:text-white']">
                            {{ tx.amount >= 0 ? '+' : '' }}{{ formatCurrency(tx.amount) }}
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    </template>

    <div v-else-if="isLoading" class="flex flex-col items-center justify-center min-h-[400px]">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
        <p class="mt-4 text-slate-400">Loading dashboard data...</p>
    </div>

    <div v-else class="flex flex-col items-center justify-center min-h-[400px]">
        <span class="material-symbols-outlined text-rose-500 text-5xl mb-4">error</span>
        <h4 class="text-lg font-bold text-slate-900 dark:text-white">Error loading data</h4>
        <p class="text-slate-500 text-sm">Please try refreshing the page later.</p>
    </div>

    <footer class="mt-12 mb-6 text-center">
        <p class="text-xs text-slate-400 dark:text-slate-600 font-medium">© 2024 Fintrack Inc. All rights reserved.</p>
    </footer>
</template>

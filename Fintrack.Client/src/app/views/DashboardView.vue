<script setup lang="ts">
// Main Dashboard component containing the mock data visualizers
</script>

<template>
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
                <h3 class="text-3xl font-extrabold text-slate-900 dark:text-white tracking-tight mb-4">$12,450.00</h3>
                <div class="flex items-center gap-2">
                    <span
                        class="bg-emerald-100 dark:bg-emerald-500/10 text-emerald-600 dark:text-accent-lime px-2 py-0.5 rounded-full text-xs font-bold flex items-center gap-1">
                        <span class="material-symbols-outlined text-[14px]">trending_up</span> 12%
                    </span>
                    <span class="text-slate-400 text-xs">vs last month</span>
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
                <h3 class="text-3xl font-extrabold text-slate-900 dark:text-white tracking-tight mb-4">$5,200.00</h3>
                <div class="flex items-center gap-2">
                    <span
                        class="bg-emerald-100 dark:bg-emerald-500/10 text-emerald-600 dark:text-accent-lime px-2 py-0.5 rounded-full text-xs font-bold flex items-center gap-1">
                        <span class="material-symbols-outlined text-[14px]">arrow_upward</span> 2%
                    </span>
                    <span class="text-slate-400 text-xs">from average</span>
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
                <h3 class="text-3xl font-extrabold text-slate-900 dark:text-white tracking-tight mb-4">$3,150.00</h3>
                <div class="flex items-center gap-2">
                    <span
                        class="bg-rose-100 dark:bg-rose-500/10 text-rose-600 dark:text-rose-400 px-2 py-0.5 rounded-full text-xs font-bold flex items-center gap-1">
                        <span class="material-symbols-outlined text-[14px]">arrow_downward</span> 5%
                    </span>
                    <span class="text-slate-400 text-xs">higher than usual</span>
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
                        <!-- Jan -->
                        <div class="flex flex-col items-center gap-2 h-full justify-end w-1/6 group">
                            <div class="flex items-end gap-1 h-full w-full justify-center">
                                <div class="w-3 md:w-5 bg-primary rounded-t-sm opacity-90 group-hover:opacity-100 transition-all bar-animate"
                                    style="--h: 60%"></div>
                                <div class="w-3 md:w-5 bg-slate-300 dark:bg-slate-600 rounded-t-sm opacity-90 group-hover:opacity-100 transition-all bar-animate"
                                    style="--h: 40%"></div>
                            </div>
                            <span class="text-xs font-bold text-slate-400">Jan</span>
                        </div>

                        <!-- Feb -->
                        <div class="flex flex-col items-center gap-2 h-full justify-end w-1/6 group">
                            <div class="flex items-end gap-1 h-full w-full justify-center">
                                <div class="w-3 md:w-5 bg-primary rounded-t-sm opacity-90 group-hover:opacity-100 transition-all bar-animate"
                                    style="--h: 75%"></div>
                                <div class="w-3 md:w-5 bg-slate-300 dark:bg-slate-600 rounded-t-sm opacity-90 group-hover:opacity-100 transition-all bar-animate"
                                    style="--h: 45%"></div>
                            </div>
                            <span class="text-xs font-bold text-slate-400">Feb</span>
                        </div>

                        <!-- Mar -->
                        <div class="flex flex-col items-center gap-2 h-full justify-end w-1/6 group">
                            <div class="flex items-end gap-1 h-full w-full justify-center">
                                <div class="w-3 md:w-5 bg-primary rounded-t-sm opacity-90 group-hover:opacity-100 transition-all bar-animate"
                                    style="--h: 50%"></div>
                                <div class="w-3 md:w-5 bg-slate-300 dark:bg-slate-600 rounded-t-sm opacity-90 group-hover:opacity-100 transition-all bar-animate"
                                    style="--h: 55%"></div>
                            </div>
                            <span class="text-xs font-bold text-slate-400">Mar</span>
                        </div>

                        <!-- Apr -->
                        <div class="flex flex-col items-center gap-2 h-full justify-end w-1/6 group">
                            <div class="flex items-end gap-1 h-full w-full justify-center">
                                <div class="w-3 md:w-5 bg-primary rounded-t-sm opacity-90 group-hover:opacity-100 transition-all bar-animate"
                                    style="--h: 80%"></div>
                                <div class="w-3 md:w-5 bg-slate-300 dark:bg-slate-600 rounded-t-sm opacity-90 group-hover:opacity-100 transition-all bar-animate"
                                    style="--h: 30%"></div>
                            </div>
                            <span class="text-xs font-bold text-slate-400">Apr</span>
                        </div>

                        <!-- May -->
                        <div class="flex flex-col items-center gap-2 h-full justify-end w-1/6 group">
                            <div class="flex items-end gap-1 h-full w-full justify-center">
                                <div class="w-3 md:w-5 bg-primary rounded-t-sm opacity-90 group-hover:opacity-100 transition-all bar-animate"
                                    style="--h: 65%"></div>
                                <div class="w-3 md:w-5 bg-slate-300 dark:bg-slate-600 rounded-t-sm opacity-90 group-hover:opacity-100 transition-all bar-animate"
                                    style="--h: 35%"></div>
                            </div>
                            <span class="text-xs font-bold text-slate-400">May</span>
                        </div>

                        <!-- Jun -->
                        <div class="flex flex-col items-center gap-2 h-full justify-end w-1/6 group">
                            <div class="flex items-end gap-1 h-full w-full justify-center">
                                <div class="w-3 md:w-5 bg-primary rounded-t-sm opacity-90 group-hover:opacity-100 transition-all bar-animate"
                                    style="--h: 90%"></div>
                                <div class="w-3 md:w-5 bg-slate-300 dark:bg-slate-600 rounded-t-sm opacity-90 group-hover:opacity-100 transition-all bar-animate"
                                    style="--h: 40%"></div>
                            </div>
                            <span class="text-xs font-bold text-slate-400">Jun</span>
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
                            <!-- Ring 1 Housing -->
                            <path class="text-slate-800"
                                d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831"
                                fill="none" stroke="#2B3036" stroke-width="4.5"></path>
                            <path class="text-primary"
                                d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831"
                                fill="none" stroke="currentColor" stroke-dasharray="40, 100" stroke-width="4.5"></path>
                            <!-- Ring 2 Food -->
                            <path class="text-emerald-500"
                                d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831"
                                fill="none" stroke="currentColor" stroke-dasharray="20, 100" stroke-dashoffset="-40"
                                stroke-width="4.5"></path>
                            <!-- Ring 3 Transport -->
                            <path class="text-sky-400"
                                d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831"
                                fill="none" stroke="currentColor" stroke-dasharray="15, 100" stroke-dashoffset="-60"
                                stroke-width="4.5"></path>
                        </svg>
                        <div class="absolute inset-0 flex flex-col items-center justify-center">
                            <span class="text-xl font-bold dark:text-white">40%</span>
                            <span class="text-[10px] uppercase text-slate-500 font-bold">Housing</span>
                        </div>
                    </div>
                    <div class="flex flex-col gap-3 w-full">
                        <div class="flex items-center justify-between text-xs">
                            <div class="flex items-center gap-2">
                                <div class="size-2 rounded-full bg-primary"></div>
                                <span class="text-slate-600 dark:text-slate-300 font-medium">Housing</span>
                            </div>
                            <span class="font-bold dark:text-white">$1,260</span>
                        </div>
                        <div class="flex items-center justify-between text-xs">
                            <div class="flex items-center gap-2">
                                <div class="size-2 rounded-full bg-emerald-500"></div>
                                <span class="text-slate-600 dark:text-slate-300 font-medium">Food</span>
                            </div>
                            <span class="font-bold dark:text-white">$630</span>
                        </div>
                        <div class="flex items-center justify-between text-xs">
                            <div class="flex items-center gap-2">
                                <div class="size-2 rounded-full bg-sky-400"></div>
                                <span class="text-slate-600 dark:text-slate-300 font-medium">Transport</span>
                            </div>
                            <span class="font-bold dark:text-white">$472</span>
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
                    <tr
                        class="border-b border-slate-100 dark:border-slate-800 hover:bg-slate-50 dark:hover:bg-[#32383f] transition-colors group">
                        <td class="p-4">
                            <div class="flex items-center gap-3">
                                <div
                                    class="size-10 rounded-full bg-orange-100 dark:bg-orange-500/20 text-orange-600 dark:text-orange-400 flex items-center justify-center shrink-0">
                                    <span class="material-symbols-outlined text-[20px]">coffee</span>
                                </div>
                                <div>
                                    <p
                                        class="font-bold text-slate-900 dark:text-white group-hover:text-primary transition-colors">
                                        Starbucks</p>
                                    <p class="text-xs text-slate-500 dark:text-slate-400">Card •••• 4242</p>
                                </div>
                            </div>
                        </td>
                        <td class="p-4">
                            <span
                                class="px-2.5 py-1 rounded-full text-xs font-bold bg-orange-50 dark:bg-transparent border border-orange-200 dark:border-orange-500/30 text-orange-600 dark:text-orange-400">
                                Food & Drink
                            </span>
                        </td>
                        <td class="p-4 text-slate-600 dark:text-slate-400 font-medium">
                            Today, 10:23 AM
                        </td>
                        <td class="p-4 text-right font-bold text-slate-900 dark:text-white">
                            -$5.50
                        </td>
                    </tr>

                    <tr
                        class="border-b border-slate-100 dark:border-slate-800 hover:bg-slate-50 dark:hover:bg-[#32383f] transition-colors group">
                        <td class="p-4">
                            <div class="flex items-center gap-3">
                                <div
                                    class="size-10 rounded-full bg-blue-100 dark:bg-blue-500/20 text-blue-600 dark:text-blue-400 flex items-center justify-center shrink-0">
                                    <span class="material-symbols-outlined text-[20px]">shopping_bag</span>
                                </div>
                                <div>
                                    <p
                                        class="font-bold text-slate-900 dark:text-white group-hover:text-primary transition-colors">
                                        Whole Foods Market</p>
                                    <p class="text-xs text-slate-500 dark:text-slate-400">Card •••• 4242</p>
                                </div>
                            </div>
                        </td>
                        <td class="p-4">
                            <span
                                class="px-2.5 py-1 rounded-full text-xs font-bold bg-blue-50 dark:bg-transparent border border-blue-200 dark:border-blue-500/30 text-blue-600 dark:text-blue-400">
                                Groceries
                            </span>
                        </td>
                        <td class="p-4 text-slate-600 dark:text-slate-400 font-medium">
                            Yesterday, 6:45 PM
                        </td>
                        <td class="p-4 text-right font-bold text-slate-900 dark:text-white">
                            -$124.80
                        </td>
                    </tr>

                    <tr
                        class="border-b border-slate-100 dark:border-slate-800 hover:bg-slate-50 dark:hover:bg-[#32383f] transition-colors group">
                        <td class="p-4">
                            <div class="flex items-center gap-3">
                                <div
                                    class="size-10 rounded-full bg-purple-100 dark:bg-purple-500/20 text-purple-600 dark:text-purple-400 flex items-center justify-center shrink-0">
                                    <span class="material-symbols-outlined text-[20px]">subscriptions</span>
                                </div>
                                <div>
                                    <p
                                        class="font-bold text-slate-900 dark:text-white group-hover:text-primary transition-colors">
                                        Netflix Subscription</p>
                                    <p class="text-xs text-slate-500 dark:text-slate-400">Auto-Pay</p>
                                </div>
                            </div>
                        </td>
                        <td class="p-4">
                            <span
                                class="px-2.5 py-1 rounded-full text-xs font-bold bg-purple-50 dark:bg-transparent border border-purple-200 dark:border-purple-500/30 text-purple-600 dark:text-purple-400">
                                Entertainment
                            </span>
                        </td>
                        <td class="p-4 text-slate-600 dark:text-slate-400 font-medium">
                            Oct 24, 2023
                        </td>
                        <td class="p-4 text-right font-bold text-slate-900 dark:text-white">
                            -$15.99
                        </td>
                    </tr>

                    <tr class="hover:bg-slate-50 dark:hover:bg-[#32383f] transition-colors group">
                        <td class="p-4">
                            <div class="flex items-center gap-3">
                                <div
                                    class="size-10 rounded-full bg-emerald-100 dark:bg-emerald-500/20 text-emerald-600 dark:text-emerald-400 flex items-center justify-center shrink-0">
                                    <span class="material-symbols-outlined text-[20px]">work</span>
                                </div>
                                <div>
                                    <p
                                        class="font-bold text-slate-900 dark:text-white group-hover:text-primary transition-colors">
                                        Freelance Payment</p>
                                    <p class="text-xs text-slate-500 dark:text-slate-400">Direct Deposit</p>
                                </div>
                            </div>
                        </td>
                        <td class="p-4">
                            <span
                                class="px-2.5 py-1 rounded-full text-xs font-bold bg-emerald-50 dark:bg-transparent border border-emerald-200 dark:border-emerald-500/30 text-emerald-600 dark:text-emerald-400">
                                Income
                            </span>
                        </td>
                        <td class="p-4 text-slate-600 dark:text-slate-400 font-medium">
                            Oct 20, 2023
                        </td>
                        <td class="p-4 text-right font-bold text-emerald-600 dark:text-emerald-400">
                            +$1,500.00
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>

    <footer class="mt-12 mb-6 text-center">
        <p class="text-xs text-slate-400 dark:text-slate-600 font-medium">© 2024 Fintrack Inc. All rights reserved.</p>
    </footer>
</template>

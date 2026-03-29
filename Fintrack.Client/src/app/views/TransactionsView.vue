<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import transactionService, { type TransactionDto } from '@/services/transactionService'
import MonthPicker from '@/app/components/common/MonthPicker.vue'

// State
const router = useRouter()
const transactions = ref<TransactionDto[]>([])
const isLoading = ref(true)
const selectedYear = ref(new Date().getFullYear())
const selectedMonth = ref(new Date().getMonth() + 1) // 1-indexed
const selectedType = ref('All')
const isSearchOpen = ref(false)
const searchQuery = ref('')

// Fetching
const fetchTransactions = async () => {
  try {
    isLoading.value = true
    transactions.value = await transactionService.getTransactions(selectedYear.value, selectedMonth.value, selectedType.value)
  } catch (error) {
    console.error('Failed to fetch transactions', error)
  } finally {
    isLoading.value = false
  }
}

// Local Filtering (Search)
const filteredTransactions = computed(() => {
  if (!searchQuery.value.trim()) return transactions.value
  
  const normalize = (str: string) => 
    str.normalize("NFD").replace(/[\u0300-\u036f]/g, "").toLowerCase()
    
  const query = normalize(searchQuery.value)
  return transactions.value.filter(tx => 
    normalize(tx.description).includes(query) || 
    normalize(tx.categoryName).includes(query)
  )
})

// Grouping logic
const groupedTransactions = computed(() => {
  const groups: Record<string, TransactionDto[]> = {}
  const locale = navigator.language || 'es-ES'
  
  filteredTransactions.value.forEach(tx => {
    const date = new Date(tx.date)
    const today = new Date()
    const yesterday = new Date()
    yesterday.setDate(today.getDate() - 1)
    
    let groupKey = ''
    if (date.toDateString() === today.toDateString()) {
      groupKey = 'Hoy'
    } else if (date.toDateString() === yesterday.toDateString()) {
      groupKey = 'Ayer'
    } else {
      groupKey = date.toLocaleDateString(locale, { weekday: 'long', day: 'numeric', month: 'long' })
      // Capitalize first letter
      groupKey = groupKey.charAt(0).toUpperCase() + groupKey.slice(1)
    }
    
    if (!groups[groupKey]) {
      groups[groupKey] = []
    }
    groups[groupKey]!.push(tx)
  })
  
  return Object.entries(groups).map(([date, items]) => ({ date, items }))
})

// UI Actions
const navigateToEdit = (tx: TransactionDto) => {
  const routeName = tx.type === 'Income' ? 'IncomeEdit' : 'ExpenseEdit'
  // Strip any type prefix (e.g., 'exp_3' -> '3')
  const idValue = tx.id.includes('_') ? tx.id.split('_')[1] : tx.id
  router.push({ name: routeName, params: { id: idValue } })
}

const toggleSearch = () => {
  isSearchOpen.value = !isSearchOpen.value
  if (!isSearchOpen.value) searchQuery.value = ''
}

const selectType = (type: string) => {
  selectedType.value = type
}

// Initial fetch and watchers
onMounted(fetchTransactions)
watch([selectedYear, selectedMonth, selectedType], fetchTransactions)

// Formatting
const formatCurrency = (amount: number) => {
  const locale = navigator.language || 'es-ES'
  return new Intl.NumberFormat(locale, { style: 'currency', currency: 'USD' }).format(amount)
}
</script>

<template>
  <div class="space-y-10 pb-20 animate-fade-in">
    <!-- Activity Header -->
    <header class="flex items-center justify-between px-1">
      <div class="space-y-1">
        <h1 class="font-headline text-2xl font-black tracking-tighter text-[#05E699]">Actividad</h1>
        <p class="text-[10px] font-bold text-on-surface-variant uppercase tracking-[0.2em]">CEROBASE • Registro Central</p>
      </div>
      <button 
        @click="toggleSearch"
        class="w-12 h-12 flex items-center justify-center rounded-xl transition-all active:scale-95 group"
        :class="isSearchOpen ? 'bg-primary-container text-on-primary' : 'bg-surface-container-high hover:bg-surface-variant text-on-surface-variant'"
      >
        <span class="material-symbols-outlined group-hover:scale-110 transition-transform">{{ isSearchOpen ? 'close' : 'search' }}</span>
      </button>
    </header>

    <!-- Search Bar (Togglable) -->
    <Transition
      enter-active-class="transition duration-300 ease-out"
      enter-from-class="opacity-0 -translate-y-4"
      enter-to-class="opacity-100 translate-y-0"
      leave-active-class="transition duration-200 ease-in"
      leave-from-class="opacity-100 translate-y-0"
      leave-to-class="opacity-0 -translate-y-4"
    >
      <div v-if="isSearchOpen" class="px-1">
        <div class="relative group">
          <span class="material-symbols-outlined absolute left-4 top-1/2 -translate-y-1/2 text-on-surface-variant text-sm group-focus-within:text-[#05E699] transition-colors">search</span>
          <input 
            v-model="searchQuery"
            class="w-full bg-surface-container-lowest border-none rounded-xl py-4 pl-12 pr-4 text-sm font-bold font-headline focus:ring-1 focus:ring-[#05E699]/40 placeholder:text-on-surface-variant/20 shadow-xl" 
            placeholder="Buscar transacciones..." 
            type="text"
            autofocus
          />
        </div>
      </div>
    </Transition>

    <!-- Filters Section -->
    <section class="space-y-6">
      <!-- Month Selection & Summary Info -->
      <div class="flex items-center justify-between px-1">
        <MonthPicker 
          v-model:month="selectedMonth" 
          v-model:year="selectedYear" 
        />
        <div class="text-[10px] font-label font-bold uppercase tracking-widest text-on-surface-variant/40">Resumen Mensual</div>
      </div>

      <!-- Activity Type Tabs (Chips) -->
      <div class="flex gap-2 overflow-x-auto no-scrollbar py-2">
        <button 
          @click="selectType('All')"
          class="px-6 py-2.5 rounded-xl text-xs font-black uppercase tracking-widest transition-all duration-300 whitespace-nowrap"
          :class="selectedType === 'All' ? 'bg-primary-container text-on-primary shadow-lg shadow-primary-container/20' : 'bg-surface-container-high text-on-surface-variant hover:bg-surface-variant'"
        >
          Todo
        </button>
        <button 
          @click="selectType('Income')"
          class="px-6 py-2.5 rounded-xl text-xs font-black uppercase tracking-widest transition-all duration-300 whitespace-nowrap"
          :class="selectedType === 'Income' ? 'bg-primary-container text-on-primary shadow-lg shadow-primary-container/20' : 'bg-surface-container-high text-on-surface-variant hover:bg-surface-variant'"
        >
          Ingresos
        </button>
        <button 
          @click="selectType('Expense')"
          class="px-6 py-2.5 rounded-xl text-xs font-black uppercase tracking-widest transition-all duration-300 whitespace-nowrap"
          :class="selectedType === 'Expense' ? 'bg-primary-container text-on-primary shadow-lg shadow-primary-container/20' : 'bg-surface-container-high text-on-surface-variant hover:bg-surface-variant'"
        >
          Gastos
        </button>
      </div>
    </section>

    <!-- Activity Content -->
    <section v-if="isLoading" class="py-20 flex flex-col items-center justify-center opacity-40">
      <div class="w-12 h-12 rounded-full border-2 border-[#05E699] border-t-transparent animate-spin mb-4"></div>
      <p class="text-[10px] font-black uppercase tracking-widest">Sincronizando Historial...</p>
    </section>

    <section v-else-if="groupedTransactions.length > 0" class="space-y-12">
      <div v-for="group in groupedTransactions" :key="group.date" class="space-y-6">
        <div class="flex items-center gap-4 px-1">
          <h3 class="font-headline text-[10px] font-black uppercase tracking-[0.3em] text-on-surface-variant whitespace-nowrap">{{ group.date }}</h3>
          <div class="h-px w-full bg-white/[0.03]"></div>
        </div>

        <div class="space-y-3">
          <div 
            v-for="tx in group.items" 
            :key="tx.id"
            @click="navigateToEdit(tx)"
            class="group flex items-center justify-between p-5 rounded-xl bg-surface-container-low hover:bg-surface-container transition-all duration-300 hover:-translate-y-1 shadow-sm border border-white/[0.02] cursor-pointer"
          >
            <div class="flex items-center gap-5">
              <div 
                class="w-12 h-12 rounded-xl bg-surface-container flex items-center justify-center group-hover:bg-surface-container-high transition-colors"
                :style="tx.categoryColor ? { color: tx.categoryColor } : { color: '#F3F4F6' }"
              >
                <span class="material-symbols-outlined text-2xl">{{ tx.categoryIcon || (tx.type === 'Income' ? 'payments' : 'receipt_long') }}</span>
              </div>
              <div class="space-y-0.5">
                <p class="text-sm font-bold text-on-surface font-headline leading-tight">{{ tx.description }}</p>
                <p class="text-[10px] text-on-surface-variant/60 font-black uppercase tracking-widest">{{ tx.categoryName }}</p>
              </div>
            </div>
            <div class="text-right space-y-0.5">
              <p 
                class="font-headline font-black text-sm"
                :class="tx.type === 'Income' ? 'text-[#05E699]' : 'text-[#FF4D4D]'"
              >
                {{ tx.type === 'Income' ? '+' : '' }}{{ formatCurrency(tx.amount) }}
              </p>
              <p class="text-[9px] text-on-surface-variant/30 font-bold uppercase">{{ tx.type === 'Income' ? 'Abono Detectado' : 'Débito Procesado' }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Monthly Insight (Visual Break) -->
      <div class="relative overflow-hidden p-8 rounded-2xl bg-gradient-to-br from-surface-container-low to-surface-dim border border-[#05E699]/10 editorial-shadow">
        <div class="relative z-10 space-y-3">
          <div class="flex items-center gap-3">
            <div class="w-8 h-8 rounded-lg bg-[#05E699]/10 flex items-center justify-center text-[#05E699]">
              <span class="material-symbols-outlined text-lg">insights</span>
            </div>
            <p class="text-[10px] font-black text-[#05E699] uppercase tracking-[0.2em]">CEROBASE Insights</p>
          </div>
          <h4 class="font-headline text-lg font-black text-on-surface leading-tight">Has optimizado tus gastos un 12% este mes.</h4>
          <p class="text-xs text-on-surface-variant leading-relaxed max-w-[240px]">Tu estrategia de gasto en categorías esenciales ha mejorado la liquidez de tu portafolio.</p>
        </div>
        <div class="absolute -right-8 -bottom-8 w-40 h-40 bg-[#05E699]/5 rounded-full blur-3xl"></div>
        <span class="material-symbols-outlined absolute right-8 top-1/2 -translate-y-1/2 text-5xl text-[#05E699]/10 animate-pulse">trending_up</span>
      </div>
    </section>

    <!-- Empty State -->
    <section v-else class="py-32 flex flex-col items-center justify-center text-center space-y-6 opacity-40">
      <div class="w-24 h-24 rounded-full bg-surface-container-low flex items-center justify-center">
        <span class="material-symbols-outlined text-6xl">history_toggle_off</span>
      </div>
      <div class="space-y-2">
        <h4 class="text-sm font-bold text-on-surface">Sin Actividad Registrada</h4>
        <p class="text-[10px] font-black uppercase tracking-widest max-w-[200px]">No hay transacciones para el periodo o filtros seleccionados.</p>
      </div>
    </section>
  </div>
</template>

<style scoped>
.no-scrollbar::-webkit-scrollbar {
  display: none;
}
.no-scrollbar {
  -ms-overflow-style: none;
  scrollbar-width: none;
}

.editorial-shadow {
  box-shadow: 0 40px 60px -20px rgba(5, 230, 153, 0.04);
}

.animate-fade-in {
  animation: fadeIn 0.8s ease-out forwards;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}
</style>

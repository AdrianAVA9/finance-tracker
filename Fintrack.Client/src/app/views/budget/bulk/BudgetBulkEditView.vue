<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import budgetService, { type Budget, type BudgetEntryDto } from '@/services/budgetService'

const router = useRouter()
const isLoading = ref(true)
const isSaving = ref(false)
const budgets = ref<Budget[]>([])
const draftBudgets = ref<Record<number, number>>({}) // categoryId -> draftAmount

const currentDate = new Date()
const selectedMonth = ref(currentDate.getMonth() + 1)
const selectedYear = ref(currentDate.getFullYear())
const expectedIncome = ref(0)

const loadBudgets = async () => {
    isLoading.value = true
    try {
        const data = await budgetService.getBudgets(selectedMonth.value, selectedYear.value)
        budgets.value = data.budgets
        expectedIncome.value = data.monthlyIncome
        
        // Initialize draft values
        const drafts: Record<number, number> = {}
        data.budgets.forEach(b => {
            drafts[b.categoryId] = b.limitAmount
        })
        draftBudgets.value = drafts
    } catch (error) {
        console.error('Failed to load budgets', error)
    } finally {
        isLoading.value = false
    }
}

const baselineTotal = computed(() => budgets.value.reduce((sum, b) => sum + b.limitAmount, 0))
const simulatedTotal = computed(() => {
    return Object.values(draftBudgets.value).reduce((sum, amount) => sum + amount, 0)
})
const theGap = computed(() => simulatedTotal.value - baselineTotal.value)

const getDelta = (categoryId: number) => {
    const budget = budgets.value.find(b => b.categoryId === categoryId)
    if (!budget) return 0
    const draftValue = draftBudgets.value[categoryId] ?? budget.limitAmount
    return draftValue - budget.limitAmount
}

const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('es-CR', {
        style: 'currency',
        currency: 'CRC',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
    }).format(amount).replace('CRC', '₡').trim()
}

const discardChanges = () => {
    const drafts: Record<number, number> = {}
    budgets.value.forEach(b => {
        drafts[b.categoryId] = b.limitAmount
    })
    draftBudgets.value = drafts
}

const commitSimulation = async () => {
    isSaving.value = true
    try {
        const budgetEntries: BudgetEntryDto[] = Object.entries(draftBudgets.value).map(([categoryId, amount]) => {
            const id = Number(categoryId);
            const existingBudget = budgets.value.find(b => b.categoryId === id);
            return {
                categoryId: id,
                amount,
                isRecurrent: existingBudget?.isRecurrent || false
            };
        })
        
        await budgetService.upsertBatch({
            month: selectedMonth.value,
            year: selectedYear.value,
            budgets: budgetEntries
        })
        
        router.push({ name: 'BudgetList' })
    } catch (error) {
        console.error('Failed to save simulation', error)
    } finally {
        isSaving.value = false
    }
}

const handleInput = (categoryId: number, event: Event) => {
    const target = event.target as HTMLInputElement
    const value = target.value.replace(/[^0-9]/g, '')
    draftBudgets.value[categoryId] = value ? parseInt(value) : 0
    target.value = draftBudgets.value[categoryId].toLocaleString('es-CR')
}

onMounted(loadBudgets)
</script>

<template>
  <div class="space-y-6 pt-6 px-4">
    <!-- Subtitle / Meta Info -->
    <div v-if="!isLoading" class="px-1">
      <p class="text-[10px] font-bold text-[#849589] uppercase tracking-[0.2em]">
        Edición Múltiple • {{ budgets.length }} categorías
      </p>
    </div>

    <!-- Simulation Table -->
    <div class="bg-[#0c0e12] border border-white/5 rounded-lg overflow-hidden shadow-2xl">
        <!-- Table Header -->
        <div class="flex items-center px-4 py-3 bg-[#1a1c20] text-[10px] font-bold tracking-widest text-[#9CA3AF] uppercase font-mono border-b border-white/5">
            <div class="w-1/2">Categoría & Base</div>
            <div class="w-1/4 text-right">Delta</div>
            <div class="w-1/4 text-right">Nuevo Valor</div>
        </div>

        <!-- List Loader -->
        <div v-if="isLoading" class="p-12 flex flex-col items-center justify-center gap-4 text-center">
            <div class="w-8 h-8 border-2 border-[#05E699] border-t-transparent rounded-full animate-spin"></div>
            <p class="text-xs font-mono text-[#9CA3AF] uppercase tracking-widest">Sincronizando base...</p>
        </div>

        <!-- Simulation List -->
        <div v-else class="divide-y divide-white/5">
            <div v-for="budget in budgets" :key="budget.id" 
                class="flex items-center px-4 py-4 hover:bg-[#1a1c20]/50 transition-colors group">
                <div class="w-1/2 flex items-center gap-3">
                    <span class="material-symbols-outlined text-xl text-[#9CA3AF] opacity-60 shrink-0">
                        {{ budget.categoryIcon || 'category' }}
                    </span>
                    <div>
                        <div class="font-manrope font-semibold text-sm text-[#e2e2e8]">{{ budget.categoryName }}</div>
                        <div class="text-[10px] font-mono text-[#9CA3AF]">{{ formatCurrency(budget.limitAmount) }}</div>
                    </div>
                </div>
                
                <!-- Delta Indicator -->
                <div class="w-1/4 text-right">
                    <span v-if="getDelta(budget.categoryId) === 0" class="text-[11px] font-mono text-[#9CA3AF]">---</span>
                    <span v-else :class="[
                        'text-[11px] font-mono font-bold',
                        getDelta(budget.categoryId) < 0 ? 'text-[#05E699]' : 'text-[#FF4D4D]'
                    ]">
                        {{ getDelta(budget.categoryId) > 0 ? '+' : '' }}{{ formatCurrency(getDelta(budget.categoryId)) }}
                    </span>
                </div>

                <!-- Input Field -->
                <div class="w-1/4 text-right">
                    <div class="flex items-center justify-end">
                        <span class="text-[#9CA3AF]/40 font-mono text-sm mr-0.5">₡</span>
                        <input 
                            class="bg-transparent border-none p-0 text-right font-mono text-lg font-bold text-[#e2e2e8] w-full focus:ring-0 focus:text-[#05E699] transition-colors" 
                            type="text" 
                            :value="draftBudgets[budget.categoryId]?.toLocaleString('es-CR')"
                            @input="handleInput(budget.categoryId, $event)"
                        />
                    </div>
                </div>
            </div>
        </div>
    </div>



    <!-- Spacer for fixed footer -->
    <div class="h-64"></div>

    <!-- Fixed Impact Footer -->
    <footer class="fixed bottom-0 left-0 w-full z-40 bg-[#1E2228] border-t border-white/10 shadow-[0_-8px_40px_-12px_rgba(0,0,0,0.5)]">
        <!-- Totals Row: seamless two-column split -->
        <div class="flex border-b border-white/10">
            <div class="flex-1 px-5 py-4 bg-[#1a1c20]">
                <div class="text-[9px] font-bold tracking-widest text-[#9CA3AF] uppercase mb-1.5 font-mono">Total Base</div>
                <div class="font-mono text-base font-semibold text-[#9CA3AF]">{{ formatCurrency(baselineTotal) }}</div>
            </div>
            <div class="w-px bg-white/10"></div>
            <div class="flex-1 px-5 py-4 bg-[#282a2e] text-right">
                <div class="text-[9px] font-bold tracking-widest text-[#05E699] uppercase mb-1.5 font-mono">Simulado</div>
                <div class="font-mono text-xl font-extrabold text-[#e2e2e8] tracking-tight leading-none">{{ formatCurrency(simulatedTotal) }}</div>
            </div>
        </div>
        <!-- Impact Row: full width -->
        <div :class="[
            'flex items-center justify-between px-5 py-4 transition-all duration-500',
            theGap <= 0 ? 'bg-[#05E699]/10' : 'bg-[#FF4D4D]/10'
        ]">
            <div class="flex flex-col gap-0.5">
                <div :class="[
                    'text-[10px] font-black font-mono tracking-widest uppercase',
                    theGap <= 0 ? 'text-[#05E699]' : 'text-[#FF4D4D]'
                ]">Impacto Neto</div>
                <div class="text-[9px] text-[#9CA3AF] font-mono uppercase tracking-widest">
                    {{ theGap <= 0 ? 'Ahorro proyectado' : 'Aumento de gasto' }}
                </div>
            </div>
            <div :class="[
                'font-mono font-extrabold text-2xl tracking-tight',
                theGap <= 0 ? 'text-[#05E699]' : 'text-[#FF4D4D]'
            ]">
                {{ theGap > 0 ? '+' : '' }}{{ formatCurrency(theGap) }}
            </div>
        </div>

        <!-- Action Buttons inside Footer -->
        <div class="px-5 pb-8 pt-2 grid grid-cols-2 gap-3 bg-[#1E2228]">
            <button 
                @click="discardChanges"
                class="py-4 rounded-lg bg-[#333539] text-[#F3F4F6] font-manrope font-bold text-sm hover:bg-[#2A2F37] transition-all active:scale-[0.98]">
                Descartar
            </button>
            <button 
                @click="commitSimulation"
                :disabled="isSaving"
                class="py-4 rounded-lg bg-[#05E699] text-[#003822] font-manrope font-bold text-sm shadow-lg shadow-[#05E699]/20 hover:opacity-90 transition-all active:scale-[0.98] disabled:opacity-50 disabled:cursor-not-allowed">
                <span v-if="isSaving" class="flex items-center justify-center gap-2 px-4">
                    <div class="w-4 h-4 border-2 border-[#003822] border-t-transparent rounded-full animate-spin"></div>
                </span>
                <span v-else>Aplicar</span>
            </button>
        </div>
    </footer>
  </div>
</template>

<style scoped>
.material-symbols-outlined {
  font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
}

input::-webkit-outer-spin-button,
input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  appearance: none;
  margin: 0;
}

input[type=number] {
  -moz-appearance: textfield;
  appearance: textfield;
}

/* Custom transitions */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>

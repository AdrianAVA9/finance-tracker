<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import api from '@/services/api'

interface ExpenseItem {
  id: string
  categoryId: number | null
  itemAmount: number | null
  description: string
}

// Global/Header Fields
const transactionType = ref<'expense' | 'income'>('expense')
const merchant = ref('')
const totalAmount = ref<number | null>(null)
const date = ref(new Date().toISOString().split('T')[0])
const note = ref('')

// Simple Mode
const singleCategoryId = ref<number | null>(null)

// Recurring Extension
const isRecurring = ref(false)
const recurringFrequency = ref('mensual')
const nextBillingDate = ref('')

// Split Invoice Extension (Itemized Mode)
const isSplitInvoice = ref(false)
const items = ref<ExpenseItem[]>([
  { id: crypto.randomUUID(), categoryId: null, itemAmount: null, description: '' },
  { id: crypto.randomUUID(), categoryId: null, itemAmount: null, description: '' }
])

// Categories Data
const groupedCategories = ref<Record<string, any[]>>({})

const loadCategories = async () => {
  try {
    const { data } = await api.get('/api/v1/expensecategories')
    if (!Array.isArray(data)) {
      console.warn('API returned non-array data:', data)
      return
    }
    
    const grouped = data.reduce((acc: Record<string, any[]>, category: any) => {
      const groupName = category.group?.name || 'Otras Categorías'
      if (!acc[groupName]) acc[groupName] = []
      acc[groupName].push(category)
      return acc
    }, {})
    groupedCategories.value = grouped
  } catch (error) {
    console.error('Failed to load categories', error)
  }
}

onMounted(() => {
  loadCategories()
})

// Computed Logic
const runningSum = computed(() => {
  if (!isSplitInvoice.value) return totalAmount.value || 0
  return items.value.reduce((sum, item) => sum + (Number(item.itemAmount) || 0), 0)
})

const isMathMismatch = computed(() => {
  if (!isSplitInvoice.value) return false
  const total = Number(totalAmount.value) || 0
  return Math.abs(runningSum.value - total) > 0.001
})

const missingAmount = computed(() => {
  const total = Number(totalAmount.value) || 0
  return total - runningSum.value
})

const isSubmitDisabled = computed(() => {
  if (!totalAmount.value || totalAmount.value <= 0) return true
  if (!merchant.value) return true
  if (!date.value) return true
  
  if (isSplitInvoice.value) {
    if (isMathMismatch.value) return true
    if (items.value.some(i => !i.categoryId || !i.itemAmount || i.itemAmount <= 0)) return true
  } else {
    // Requires a category in simple mode
    if (!singleCategoryId.value) return true
  }
  return false
})

// Actions
const toggleSplitMode = () => {
  isSplitInvoice.value = !isSplitInvoice.value
  if (!isSplitInvoice.value) {
    items.value = [] // clear split row data when exiting split mode
  } else {
    // populate initial blank rows
    items.value = [
      { id: crypto.randomUUID(), categoryId: null, itemAmount: null, description: '' },
      { id: crypto.randomUUID(), categoryId: null, itemAmount: null, description: '' }
    ]
  }
}

const addRow = () => {
  items.value.push({ id: crypto.randomUUID(), categoryId: null, itemAmount: null, description: '' })
}

const removeRow = (id: string) => {
  items.value = items.value.filter(i => i.id !== id)
}

interface Props {
  submitting?: boolean
  error?: string
}

const props = defineProps<Props>()

const emit = defineEmits(['submit'])

const saveExpense = () => {
  if (isSubmitDisabled.value) return

  const payload = {
    type: transactionType.value,
    merchant: merchant.value,
    totalAmount: Number(totalAmount.value),
    date: date.value,
    note: note.value,
    isRecurring: isRecurring.value,
    recurringData: isRecurring.value ? {
      frequency: recurringFrequency.value,
      nextDate: nextBillingDate.value
    } : null,
    items: isSplitInvoice.value 
      ? items.value.map(i => ({ categoryId: i.categoryId, itemAmount: Number(i.itemAmount), description: i.description }))
      : [{ categoryId: singleCategoryId.value, itemAmount: Number(totalAmount.value), description: merchant.value }]
  }

  emit('submit', payload)
}
</script>

<template>
  <div class="w-full">
    <!-- MAIN FORM CONTAINER -->
    <form @submit.prevent="saveExpense" class="p-5 space-y-4">

      <!-- Error Message Area -->
      <Transition
        enter-active-class="transform transition duration-300 ease-out"
        enter-from-class="-translate-y-4 opacity-0"
        enter-to-class="translate-y-0 opacity-100"
        leave-active-class="transition duration-200 ease-in"
        leave-from-class="opacity-100"
        leave-to-class="opacity-0"
      >
        <div v-if="error" class="mb-4 p-4 bg-red-500/10 border border-red-500/20 rounded-xl flex items-start gap-3 text-red-500">
          <span class="material-symbols-outlined mt-0.5">error</span>
          <div>
            <h4 class="text-sm font-bold">Error al guardar</h4>
            <p class="text-xs mt-0.5 opacity-80">{{ error }}</p>
          </div>
        </div>
      </Transition>

      <!-- ============================== -->
      <!-- SIMPLE MODE WRAPPER -->
      <!-- ============================== -->
      <div v-if="!isSplitInvoice" class="space-y-6">
          
        <!-- Amount Section -->
        <div class="bg-slate-50 dark:bg-background-dark/50 border border-slate-200 dark:border-slate-800 rounded-xl p-4 text-center">
            <label class="block text-xs font-medium text-slate-500 mb-1">Monto del Ingreso</label> <!-- WAIT, fixing it locally: -->
            <label class="block text-xs font-medium text-slate-500 mb-1">Monto del Gasto</label>
            <div class="flex items-center justify-center gap-2">
                <span class="text-4xl font-bold text-[#FF4D4D]">-</span>
                <span class="text-4xl font-bold text-slate-400 dark:text-slate-500">₡</span>
                <input
                    v-model="totalAmount"
                    type="number"
                    step="0.01"
                    class="bg-transparent border-none text-4xl font-bold text-slate-900 dark:text-white focus:ring-0 w-32 p-0 placeholder-slate-200 dark:placeholder-slate-700"
                    placeholder="0.00"
                    required
                />
            </div>
        </div>

        <div class="grid grid-cols-1 gap-5">
            <!-- Comercio -->
            <div class="space-y-2">
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300">Comercio</label>
                <input
                    v-model="merchant"
                    type="text"
                    class="w-full bg-white dark:bg-background-dark border border-slate-200 dark:border-slate-800 rounded-lg py-3 px-4 focus:ring-2 focus:ring-accent-lime/20 focus:border-accent-lime outline-none transition-all dark:text-white"
                    placeholder="Ej. AutoMercado, Gasolinera..."
                    required
                />
            </div>
            
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <!-- Fecha -->
                <div class="space-y-2">
                    <label class="block text-sm font-medium text-slate-700 dark:text-slate-300">Fecha del gasto</label>
                    <input
                        v-model="date"
                        type="date"
                        class="w-full bg-white dark:bg-background-dark border border-slate-200 dark:border-slate-800 rounded-lg py-3 px-4 focus:ring-2 focus:ring-accent-lime/20 focus:border-accent-lime outline-none transition-all dark:text-white [color-scheme:dark]"
                        required
                    />
                </div>
                <!-- Categoría -->
                <div class="space-y-2">
                    <label class="block text-sm font-medium text-slate-700 dark:text-slate-300">Categoría</label>
                    <div class="relative group h-full">
                        <select v-model="singleCategoryId" class="w-full bg-white dark:bg-background-dark border border-slate-200 dark:border-slate-800 rounded-lg py-3 px-4 text-slate-900 dark:text-white appearance-none focus:ring-2 focus:ring-accent-lime/20 focus:border-accent-lime outline-none transition-all cursor-pointer" required>
                            <option :value="null" disabled>Categoría</option>
                            <optgroup v-for="(cats, groupName) in groupedCategories" :key="groupName" :label="groupName">
                                <option v-for="cat in cats" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
                            </optgroup>
                        </select>
                        <span class="material-symbols-outlined absolute right-3 top-1/2 -translate-y-1/2 text-slate-400 pointer-events-none">expand_more</span>
                    </div>
                </div>
            </div>
            
            <!-- Nota -->
            <div class="space-y-2">
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 font-normal">Notas <span class="text-slate-400">(Opcional)</span></label>
                <textarea
                    v-model="note"
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
                    <span class="text-xs text-slate-500">Convertir en gasto recurrente</span>
                </div>
                <!-- Toggle Switch -->
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
            
            <!-- Recurring Fields -->
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
                            <div class="relative">
                                <select v-model="recurringFrequency" class="w-full bg-white dark:bg-background-dark border border-slate-200 dark:border-slate-800 rounded-lg py-2 px-3 text-sm focus:ring-2 focus:ring-accent-lime/20 focus:border-accent-lime outline-none transition-all dark:text-white appearance-none">
                                    <option value="Monthly">Mensual</option>
                                    <option value="Weekly">Semanal</option>
                                    <option value="Yearly">Anual</option>
                                </select>
                                <span class="material-symbols-outlined absolute right-2 top-1/2 -translate-y-1/2 text-slate-400 pointer-events-none text-lg">expand_more</span>
                            </div>
                        </div>
                        <div class="space-y-2">
                            <label class="block text-[10px] font-bold text-slate-400 uppercase tracking-wider">Próximo cobro</label>
                            <input v-model="nextBillingDate" type="date" class="w-full bg-white dark:bg-background-dark border border-slate-200 dark:border-slate-800 rounded-lg py-2 px-3 text-sm focus:ring-2 focus:ring-accent-lime/20 focus:border-accent-lime outline-none transition-all dark:text-white [color-scheme:dark]" />
                        </div>
                    </div>
                </div>
            </Transition>
        </div>

        <!-- Footer Actions (Simple Mode) -->
        <div class="flex flex-col gap-2 pt-2">
            <button
                type="submit"
                :disabled="isSubmitDisabled || submitting"
                class="w-full bg-primary text-white font-bold py-3.5 rounded-xl hover:bg-primary-hover transition-all shadow-glow active:scale-[0.98] disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
            >
                <span v-if="submitting" class="material-symbols-outlined animate-spin shadow-none">sync</span>
                {{ submitting ? 'Guardando...' : 'Guardar Gasto' }}
            </button>
            <button
                type="button"
                :disabled="submitting"
                class="w-full bg-transparent border border-slate-200 dark:border-slate-800 text-slate-600 dark:text-slate-400 font-medium py-2.5 rounded-xl hover:bg-slate-50 dark:hover:bg-slate-800/50 transition-all"
            >
                Guardar y agregar otro
            </button>
            <button type="button" @click="toggleSplitMode" class="w-fit self-center px-6 py-2 text-slate-400 hover:text-slate-600 dark:hover:text-white transition-colors flex items-center gap-2 text-xs mt-2">
                <span class="material-symbols-outlined text-sm">call_split</span>
                Desglosar en múltiples categorías
            </button>
        </div>
      </div>

      <!-- ============================== -->
      <!-- ITEMIZED MODE WRAPPER -->
      <!-- ============================== -->
      <div v-else class="space-y-6">
        
        <!-- Header Card: Fixed context -->
        <div class="bg-slate-50 dark:bg-background-dark/50 border border-slate-200 dark:border-slate-800 rounded-xl p-5 text-center flex flex-col items-center">
            <span class="text-xs font-semibold text-slate-500 uppercase tracking-wider mb-1">Monto Total a Desglosar</span>
            <div class="flex items-center justify-center gap-2">
                <span class="text-3xl font-bold text-[#FF4D4D]">-</span>
                <span class="text-3xl font-bold text-slate-400">₡</span>
                <span class="text-3xl font-bold text-slate-900 dark:text-white">{{ totalAmount?.toFixed(2) || '0.00' }}</span>
            </div>
            <p class="text-sm font-medium text-slate-700 dark:text-slate-300 mt-2">{{ merchant || 'Comercio no especificado' }} • {{ date }}</p>
        </div>

        <!-- Dynamic List Card -->
        <div class="border border-slate-200 dark:border-slate-800 rounded-xl overflow-hidden">
            <div class="px-5 py-3 border-b border-slate-200 dark:border-slate-800 bg-slate-50 dark:bg-background-dark/30">
                <h3 class="text-sm font-semibold text-slate-700 dark:text-slate-300 flex items-center gap-2">
                <span class="material-symbols-outlined text-sm">list_alt</span>
                Ítems de la factura
                </h3>
            </div>
          
            <div class="p-5 flex flex-col gap-5">
                <!-- Dynamic Rows -->
                <div v-for="(item, index) in items" :key="item.id" class="grid grid-cols-1 gap-4 sm:grid-cols-12 sm:gap-3 items-start group relative pb-4 border-b border-slate-100 dark:border-slate-800/50 last:border-0 last:pb-0">
                    <div class="sm:col-span-4 flex flex-col gap-1.5">
                        <label class="block text-[10px] font-bold text-slate-400 uppercase tracking-wider">Monto</label>
                        <input v-model="item.itemAmount" class="w-full bg-white dark:bg-background-dark border border-slate-200 dark:border-slate-800 text-slate-900 dark:text-white rounded-lg px-3 py-2.5 text-sm outline-none transition-all focus:ring-2 focus:ring-accent-lime/20 focus:border-accent-lime" type="number" step="0.01" required />
                    </div>
                    
                    <div class="sm:col-span-8 flex flex-col gap-1.5">
                        <div class="flex justify-between items-center">
                            <label class="block text-[10px] font-bold text-slate-400 uppercase tracking-wider">Categoría / Detalle</label>
                            <button type="button" @click="removeRow(item.id)" class="text-slate-300 hover:text-red-500 transition-colors bg-transparent border-none">
                                <span class="material-symbols-outlined text-[16px]">close</span>
                            </button>
                        </div>
                        <div class="flex flex-col gap-2">
                            <div class="relative">
                                <select v-model="item.categoryId" class="w-full bg-white dark:bg-background-dark border border-slate-200 dark:border-slate-800 text-slate-900 dark:text-white rounded-lg pl-3 pr-8 py-2.5 text-sm appearance-none outline-none focus:ring-2 focus:ring-accent-lime/20 focus:border-accent-lime" required>
                                    <option :value="null" disabled>Categoría</option>
                                    <optgroup v-for="(cats, groupName) in groupedCategories" :key="groupName" :label="groupName">
                                        <option v-for="cat in cats" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
                                    </optgroup>
                                </select>
                                <span class="material-symbols-outlined absolute right-2 top-2.5 text-slate-400 pointer-events-none text-sm">expand_more</span>
                            </div>
                            <input v-model="item.description" class="w-full bg-white dark:bg-background-dark border border-slate-200 dark:border-slate-800 text-slate-900 dark:text-white rounded-lg px-3 py-2.5 text-sm outline-none focus:ring-2 focus:ring-accent-lime/20 focus:border-accent-lime" type="text" placeholder="Detalle (opcional)..." />
                        </div>
                    </div>
                </div>

                <!-- Add Item Button -->
                <button type="button" @click="addRow" class="w-full py-3 border-2 border-dashed border-slate-200 dark:border-slate-800 rounded-lg text-slate-500 hover:text-primary hover:border-primary hover:bg-primary/5 transition-all flex items-center justify-center gap-2 font-semibold text-sm">
                <span class="material-symbols-outlined text-[18px]">add_circle</span>
                Agregar otro ítem
                </button>
            </div>
          
            <!-- Feedback Math Section -->
            <div v-if="isMathMismatch" class="px-5 py-4 bg-red-50 dark:bg-red-950/20 border-t border-red-100 dark:border-red-900/30 flex items-center justify-between">
                <div class="flex items-center gap-2 text-red-500">
                    <span class="material-symbols-outlined text-sm">error</span>
                    <span class="text-xs font-bold">
                        {{ missingAmount > 0 ? `Faltan ₡${missingAmount.toFixed(2)} por asignar` : `Se excede por ₡${Math.abs(missingAmount).toFixed(2)}` }}
                    </span>
                </div>
                <div class="text-red-400 text-[10px] font-medium hidden sm:block">
                    Suma: ₡{{ runningSum.toFixed(2) }}
                </div>
            </div>
            <div v-else class="px-5 py-4 bg-green-50 dark:bg-green-950/20 border-t border-green-100 dark:border-green-900/30 flex items-center justify-between">
                <div class="flex items-center gap-2 text-green-500">
                    <span class="material-symbols-outlined text-sm">check_circle</span>
                    <span class="text-xs font-bold">Los montos coinciden</span>
                </div>
            </div>
        </div>

        <!-- Actions Footer (Itemized Mode) -->
        <div class="flex flex-col gap-2 pt-2">
            <button
                type="submit"
                :disabled="isSubmitDisabled || submitting"
                class="w-full bg-primary text-white font-bold py-3.5 rounded-xl hover:bg-primary-hover transition-all shadow-glow active:scale-[0.98] disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
            >
                <span v-if="submitting" class="material-symbols-outlined animate-spin shadow-none">sync</span>
                {{ submitting ? 'Guardando...' : 'Guardar Gasto Desglosado' }}
            </button>
            <button
                @click="toggleSplitMode"
                type="button"
                :disabled="submitting"
                class="w-fit self-center px-6 py-2 text-slate-400 hover:text-slate-600 dark:hover:text-white transition-colors flex items-center gap-2 text-xs mt-2"
            >
                <span class="material-symbols-outlined text-sm">arrow_back</span>
                Cancelar y volver a modo simple
            </button>
        </div>
        
      </div>
    </form>
  </div>
</template>

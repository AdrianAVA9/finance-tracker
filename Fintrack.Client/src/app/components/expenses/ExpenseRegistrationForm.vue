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
  <div class="w-full max-w-[640px] flex flex-col gap-6 font-display mx-auto">
    <!-- AI Scan Prompt (Less Prominent Action) -->
    <div v-show="!isSplitInvoice && false" class="group relative p-0.5 rounded-xl bg-gradient-to-r from-primary/40 to-transparent mb-2">
      <button class="w-full flex items-center justify-center gap-2 py-2.5 px-4 bg-primary text-white rounded-lg font-semibold text-sm shadow-md shadow-primary/20 hover:shadow-primary/30 hover:bg-primary/95 hover:scale-[1.01] active:scale-[0.99] transition-all">
        <span class="material-symbols-outlined text-base">auto_awesome</span>
        <span>📸 Escanear Factura con IA</span>
      </button>
    </div>

    <!-- MAIN FORM CONTAINER -->
    <form @submit.prevent="saveExpense" class="flex flex-col gap-6">

      <!-- Error Message Area -->
      <Transition
        enter-active-class="transition duration-300 ease-out"
        enter-from-class="transform -translate-y-4 opacity-0"
        enter-to-class="transform translate-y-0 opacity-100"
        leave-active-class="transition duration-200 ease-in"
        leave-from-class="transform translate-y-0 opacity-100"
        leave-to-class="transform -translate-y-4 opacity-0"
      >
        <div v-if="error" class="bg-red-500/10 border border-red-500/20 rounded-xl p-4 flex items-start gap-3">
          <span class="material-symbols-outlined text-red-500 mt-0.5">error</span>
          <div class="flex-1">
            <h4 class="text-red-500 font-bold text-sm">Error al guardar</h4>
            <p class="text-red-400/80 text-xs mt-0.5">{{ error }}</p>
          </div>
        </div>
      </Transition>

      <!-- ============================== -->
      <!-- SIMPLE MODE WRAPPER -->
      <!-- ============================== -->
      <div v-if="!isSplitInvoice" class="bg-card-dark border border-border-dark rounded-xl shadow-2xl overflow-hidden">
        <div class="p-6 flex flex-col gap-6">
          
          <!-- Expense/Income Toggle -->
          <div v-if="false" class="flex p-1 bg-background-dark rounded-full border border-border-dark">
            <button type="button" @click="transactionType = 'expense'" :class="transactionType === 'expense' ? 'bg-primary text-white shadow-sm' : 'text-text-muted hover:text-text-main'" class="flex-1 py-2 text-sm font-bold rounded-full transition-colors">Gasto</button>
            <button type="button" @click="transactionType = 'income'" :class="transactionType === 'income' ? 'bg-primary text-white shadow-sm' : 'text-text-muted hover:text-text-main'" class="flex-1 py-2 text-sm font-semibold transition-colors">Ingreso</button>
          </div>
          
          <div class="grid grid-cols-1 gap-5">
            <!-- Comercio -->
            <div class="flex flex-col gap-2">
              <label class="text-sm font-semibold text-text-main flex items-center gap-2">
                <span class="material-symbols-outlined text-sm text-primary">store</span> Comercio
              </label>
              <input v-model="merchant" class="w-full bg-background-dark border border-border-dark rounded-lg py-3 px-4 text-text-main focus:ring-1 focus:ring-primary focus:border-primary transition-all outline-none placeholder:text-text-muted/50" placeholder="Ej: AutoMercado" type="text" required />
            </div>
            
            <!-- Monto Total -->
            <div class="flex flex-col gap-2">
              <label class="text-sm font-semibold text-text-main flex items-center gap-2">
                <span class="material-symbols-outlined text-sm text-primary">payments</span> Monto Total
              </label>
              <div class="relative">
                <span class="absolute left-4 top-1/2 -translate-y-1/2 text-primary font-bold">₡</span>
                <input v-model="totalAmount" class="w-full bg-background-dark border border-border-dark rounded-lg py-3 pl-10 pr-4 text-text-main focus:ring-1 focus:ring-primary focus:border-primary transition-all outline-none text-xl font-bold" placeholder="0.00" type="number" step="0.01" required />
              </div>
            </div>
            
            <!-- Row: Fecha & Categoría -->
            <div class="grid grid-cols-2 gap-4">
              <!-- Fecha -->
              <div class="flex flex-col gap-2">
                <label class="text-sm font-semibold text-text-main flex items-center gap-2">
                  <span class="material-symbols-outlined text-sm text-primary">calendar_today</span> Fecha
                </label>
                <input v-model="date" class="w-full bg-background-dark border border-border-dark rounded-lg py-3 px-4 text-text-main focus:ring-1 focus:ring-primary focus:border-primary transition-all outline-none [color-scheme:dark]" type="date" required />
              </div>
              <!-- Categoría -->
              <div class="flex flex-col gap-2">
                <label class="text-sm font-semibold text-text-main flex items-center gap-2">
                  <span class="material-symbols-outlined text-sm text-primary">category</span> Categoría
                </label>
                <div class="relative group h-full">
                  <select v-model="singleCategoryId" class="w-full h-full bg-background-dark border border-border-dark rounded-lg py-3 px-4 text-text-main appearance-none focus:ring-1 focus:ring-primary focus:border-primary outline-none cursor-pointer" required>
                    <option :value="null" disabled>Categoría</option>
                    <optgroup v-for="(cats, groupName) in groupedCategories" :key="groupName" :label="groupName">
                      <option v-for="cat in cats" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
                    </optgroup>
                  </select>
                  <span class="material-symbols-outlined absolute right-3 top-1/2 -translate-y-1/2 text-text-muted pointer-events-none">expand_more</span>
                </div>
              </div>
            </div>
            
            <!-- Nota -->
            <div class="flex flex-col gap-2">
              <label class="text-sm font-semibold text-text-main flex items-center gap-2">
                <span class="material-symbols-outlined text-sm text-text-muted">sticky_note_2</span> Nota <span class="text-[10px] text-text-muted uppercase tracking-widest ml-1">(Opcional)</span>
              </label>
              <textarea v-model="note" class="w-full bg-background-dark border border-border-dark rounded-lg py-3 px-4 text-text-main focus:ring-1 focus:ring-primary focus:border-primary transition-all outline-none resize-none" placeholder="Detalles adicionales..." rows="2"></textarea>
            </div>
          </div>
        </div>

        <!-- Recurring Expense Section -->
        <div class="px-6 py-4 border-t border-border-dark/50 bg-primary/5">
          <div class="flex items-center justify-between mb-4">
            <div class="flex items-center gap-2">
              <span class="material-symbols-outlined text-primary">repeat</span>
              <span class="text-sm font-semibold text-text-main">Convertir en un gasto recurrente</span>
            </div>
            <!-- Toggle Switch -->
            <button type="button" @click="isRecurring = !isRecurring" :class="isRecurring ? 'bg-primary' : 'bg-border-dark'" class="relative inline-flex h-6 w-11 items-center rounded-full transition-colors focus:outline-none focus:ring-2 focus:ring-primary focus:ring-offset-2 focus:ring-offset-background-dark">
              <span :class="isRecurring ? 'translate-x-6' : 'translate-x-1'" class="inline-block h-4 w-4 transform rounded-full bg-white transition-transform"></span>
            </button>
          </div>
          
          <!-- Recurring Fields -->
          <div v-show="isRecurring" class="grid grid-cols-2 gap-4">
            <div class="flex flex-col gap-2">
              <label class="text-[11px] font-bold text-text-muted uppercase tracking-wider">Frecuencia</label>
              <div class="relative">
                <select v-model="recurringFrequency" class="w-full bg-background-dark border border-border-dark rounded-lg py-2 px-3 text-sm text-text-main focus:ring-1 focus:ring-primary focus:border-primary appearance-none outline-none">
                  <option value="Monthly">Mensual</option>
                  <option value="Weekly">Semanal</option>
                  <option value="Yearly">Anual</option>
                </select>
                <span class="material-symbols-outlined absolute right-2 top-1/2 -translate-y-1/2 text-text-muted pointer-events-none text-lg">expand_more</span>
              </div>
            </div>
            <div class="flex flex-col gap-2">
              <label class="text-[11px] font-bold text-text-muted uppercase tracking-wider">Próximo cobro</label>
              <input v-model="nextBillingDate" class="w-full bg-background-dark border border-border-dark rounded-lg py-2 px-3 text-sm text-text-main focus:ring-1 focus:ring-primary focus:border-primary outline-none [color-scheme:dark]" type="date" />
            </div>
          </div>
        </div>

        <!-- Footer Actions (Simple Mode) -->
        <div class="p-6 pt-2 flex flex-col gap-4 border-t border-border-dark bg-background-dark/30">
          <div class="flex flex-col sm:flex-row gap-3">
            <button type="submit" :disabled="isSubmitDisabled || submitting" class="flex-1 py-3.5 bg-primary hover:bg-primary/90 disabled:opacity-50 disabled:cursor-not-allowed text-white rounded-lg font-bold transition-all flex items-center justify-center gap-2 shadow-lg shadow-primary/10">
              <template v-if="submitting">
                <svg class="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                <span>Guardando...</span>
              </template>
              <template v-else>
                <span class="material-symbols-outlined text-xl">save</span> Guardar
              </template>
            </button>
            <button type="button" :disabled="submitting" class="flex-1 py-3.5 bg-card-dark border border-border-dark hover:border-primary/50 text-text-main rounded-lg font-semibold transition-all">
              Guardar y agregar otro
            </button>
          </div>
          <button type="button" @click="toggleSplitMode" class="flex items-center justify-center gap-2 py-2 text-xs font-medium text-text-muted hover:text-primary transition-colors group">
            <span class="material-symbols-outlined text-sm group-hover:scale-110 transition-transform">call_split</span>
            ➗ Desglosar en múltiples categorías
          </button>
        </div>
      </div>

      <!-- ============================== -->
      <!-- ITEMIZED MODE WRAPPER -->
      <!-- ============================== -->
      <div v-else class="flex flex-col gap-6">
        
        <!-- Header Card: Fixed context -->
        <div class="bg-card-dark rounded-lg p-6 shadow-xl border border-border-dark/30">
          <div class="flex flex-col gap-4">
            <div class="flex justify-between items-start">
              <div>
                <span class="text-text-muted text-xs font-semibold uppercase tracking-wider">Comercio</span>
                <p class="text-text-main text-xl font-bold">{{ merchant || 'Sin asignar' }}</p>
              </div>
              <div class="text-right">
                <span class="text-text-muted text-xs font-semibold uppercase tracking-wider">Monto Total</span>
                <p class="text-text-main text-xl font-bold">₡{{ totalAmount?.toFixed(2) || '0.00' }}</p>
              </div>
            </div>
            <div class="flex items-center gap-2 text-text-muted">
              <span class="material-symbols-outlined text-sm">calendar_today</span>
              <span class="text-sm">{{ date }}</span>
              <span class="mx-2 text-border-dark">|</span>
              <span class="material-symbols-outlined text-sm">receipt_long</span>
              <span class="text-sm">Modo Desglosado</span>
            </div>
          </div>
        </div>

        <!-- Dynamic List Card -->
        <div class="bg-card-dark rounded-lg shadow-xl border border-border-dark/30 overflow-hidden">
          <div class="px-6 py-4 border-b border-border-dark/30 bg-primary/5">
            <h3 class="text-text-main text-lg font-semibold flex items-center gap-2">
              <span class="material-symbols-outlined text-primary">list_alt</span>
              Ítems de la factura
            </h3>
          </div>
          
          <div class="p-6 flex flex-col gap-6">
            <!-- Dynamic Rows -->
            <div v-for="(item, index) in items" :key="item.id" class="grid grid-cols-12 gap-3 items-start group">
              <div class="col-span-4 flex flex-col gap-1.5">
                <label class="text-text-muted text-xs font-medium ml-1">Monto (₡)</label>
                <div class="relative rounded-lg group-focus-within:ring-2 group-focus-within:ring-primary/50">
                  <input v-model="item.itemAmount" class="w-full bg-background-dark border border-border-dark text-text-main rounded-lg px-3 py-2.5 text-sm outline-none transition-all" type="number" step="0.01" required />
                </div>
              </div>
              
              <div class="col-span-7 flex flex-col gap-1.5">
                <label class="text-text-muted text-xs font-medium ml-1">Categoría & Descripción</label>
                <div class="flex gap-2">
                  <div class="relative w-1/3">
                    <select v-model="item.categoryId" class="w-full bg-background-dark border border-border-dark text-text-main rounded-lg pl-3 pr-8 py-2.5 text-sm appearance-none outline-none" required>
                      <option :value="null" disabled>Cat</option>
                      <optgroup v-for="(cats, groupName) in groupedCategories" :key="groupName" :label="groupName">
                        <option v-for="cat in cats" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
                      </optgroup>
                    </select>
                    <span class="material-symbols-outlined absolute right-2 top-2.5 text-text-muted pointer-events-none text-sm">expand_more</span>
                  </div>
                  <input v-model="item.description" class="flex-1 bg-background-dark border border-border-dark text-text-main rounded-lg px-3 py-2.5 text-sm outline-none" type="text" placeholder="Detalle..." />
                </div>
              </div>
              
              <div class="col-span-1 pt-8 flex justify-center">
                <button type="button" @click="removeRow(item.id)" class="text-text-muted hover:text-red-400 transition-colors">
                  <span class="material-symbols-outlined">delete</span>
                </button>
              </div>
            </div>

            <!-- Add Item Button -->
            <button type="button" @click="addRow" class="w-full py-3 border-2 border-dashed border-border-dark rounded-lg text-text-muted hover:text-primary hover:border-primary hover:bg-primary/5 transition-all flex items-center justify-center gap-2 font-semibold">
              <span class="material-symbols-outlined">add_circle</span>
              Agregar ítem
            </button>
          </div>
          
          <!-- Feedback Math Section -->
          <div v-if="isMathMismatch" class="px-6 py-4 bg-red-950/20 border-t border-red-900/30 flex items-center justify-between">
            <div class="flex items-center gap-2 text-red-400">
              <span class="material-symbols-outlined text-sm">error</span>
              <span class="text-sm font-medium">
                {{ missingAmount > 0 ? `Faltan ₡${missingAmount.toFixed(2)} por asignar` : `El total se excede por ₡${Math.abs(missingAmount).toFixed(2)}` }}
              </span>
            </div>
            <div class="text-text-muted text-xs">
              Suma actual: ₡{{ runningSum.toFixed(2) }} / ₡{{ totalAmount?.toFixed(2) || '0.00' }}
            </div>
          </div>
          <div v-else class="px-6 py-4 bg-green-950/20 border-t border-green-900/30 flex items-center justify-between">
            <div class="flex items-center gap-2 text-green-400">
              <span class="material-symbols-outlined text-sm">check_circle</span>
              <span class="text-sm font-medium">Los montos coinciden</span>
            </div>
            <div class="text-text-muted text-xs">
              Suma actual: ₡{{ runningSum.toFixed(2) }} / ₡{{ totalAmount?.toFixed(2) || '0.00' }}
            </div>
          </div>
        </div>

        <!-- Actions Footer (Itemized Mode) -->
        <div class="flex flex-col gap-4">
          <div class="grid grid-cols-2 gap-4">
            <button type="submit" :disabled="isSubmitDisabled || submitting" class="w-full bg-primary hover:bg-primary/90 disabled:bg-primary/20 disabled:text-text-muted/50 text-white rounded-lg py-3.5 font-bold transition-all border border-border-dark focus:outline-none flex items-center justify-center gap-2">
              <template v-if="submitting">
                <svg class="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                <span>Guardando...</span>
              </template>
              <span v-else>Guardar</span>
            </button>
            <button type="button" :disabled="submitting" class="w-full bg-border-dark text-text-main hover:bg-border-dark/80 rounded-lg py-3.5 font-bold transition-all border border-border-dark/50 disabled:opacity-50">
              Guardar y agregar otro
            </button>
          </div>
          <button type="button" @click="toggleSplitMode" class="w-fit self-center px-6 py-2 text-text-muted hover:text-text-main transition-colors flex items-center gap-2 text-sm">
            <span class="material-symbols-outlined text-base">arrow_back</span>
            Volver a modo simple
          </button>
        </div>
        
      </div>

    </form>
  </div>
</template>

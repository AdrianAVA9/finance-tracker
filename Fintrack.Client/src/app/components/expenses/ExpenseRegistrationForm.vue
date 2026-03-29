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
  <div class="space-y-10">
    <!-- MAIN FORM CONTAINER -->
    <form @submit.prevent="saveExpense" class="space-y-10">

      <!-- Error Message Area -->
      <Transition
        enter-active-class="transform transition duration-500 ease-out"
        enter-from-class="-translate-y-10 opacity-0"
        enter-to-class="translate-y-0 opacity-100"
        leave-active-class="transition duration-300 ease-in"
        leave-from-class="opacity-100"
        leave-to-class="opacity-0 scale-95"
      >
        <div v-if="error" class="p-5 bg-red-500/10 border border-red-500/20 backdrop-blur-xl rounded-2xl flex items-start gap-4 text-red-500 luminous-shadow-red">
          <span class="material-symbols-outlined mt-1">error</span>
          <div class="space-y-1">
            <h4 class="text-sm font-black uppercase tracking-widest">Error de Registro</h4>
            <p class="text-[11px] font-bold opacity-80 leading-relaxed">{{ error }}</p>
          </div>
        </div>
      </Transition>

      <!-- ============================== -->
      <!-- SIMPLE MODE WRAPPER -->
      <!-- ============================== -->
      <div v-if="!isSplitInvoice" class="space-y-10">
          
        <!-- Hero Amount Section -->
        <section class="relative group">
            <label class="block text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant mb-4 px-1">Monto del Gasto</label>
            <div class="bg-surface-container-low p-10 rounded-2xl border border-white/[0.03] luminous-shadow-sm flex flex-col items-center justify-center transition-all duration-500 focus-within:bg-red-500/[0.02] focus-within:border-red-500/20">
                <div class="flex items-baseline gap-2">
                    <span class="font-headline text-4xl font-black text-[#FF4D4D]">-</span>
                    <span class="font-headline text-4xl font-black text-[#FF4D4D]">₡</span>
                    <input
                        v-model="totalAmount"
                        type="number"
                        step="0.01"
                        class="bg-transparent border-none text-center font-headline text-6xl font-black tracking-tighter focus:ring-0 text-on-surface w-full max-w-[280px] placeholder:text-on-surface-variant/20"
                        placeholder="0"
                        required
                    />
                </div>
            </div>
        </section>

        <div class="grid grid-cols-1 gap-8">
            <!-- Comercio -->
            <section class="space-y-4">
                <label class="block text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant px-1">Comercio</label>
                <input
                    v-model="merchant"
                    type="text"
                    class="w-full bg-surface-container p-5 rounded-xl border border-white/[0.03] focus:border-[#FF4D4D]/30 focus:ring-0 transition-all font-body text-on-surface font-semibold"
                    placeholder="Ej. AutoMercado, Gasolinera..."
                    required
                />
            </section>
            
            <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
                <!-- Fecha -->
                <section class="space-y-4">
                    <label class="block text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant px-1">Fecha del Gasto</label>
                    <input
                        v-model="date"
                        type="date"
                        class="w-full bg-surface-container p-5 rounded-xl border border-white/[0.03] focus:border-[#FF4D4D]/30 focus:ring-0 transition-all font-body text-on-surface font-semibold [color-scheme:dark]"
                        required
                    />
                </section>
                <!-- Categoría -->
                <section class="space-y-4">
                    <label class="block text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant px-1">Categoría</label>
                    <div class="relative group h-full">
                        <select 
                            v-model="singleCategoryId" 
                            class="w-full appearance-none bg-surface-container p-5 rounded-xl border border-white/[0.03] focus:border-[#FF4D4D]/30 focus:ring-0 transition-all font-body text-on-surface font-semibold cursor-pointer" 
                            required
                        >
                            <option :value="null" disabled>Seleccione una categoría</option>
                            <optgroup v-for="(cats, groupName) in groupedCategories" :key="groupName" :label="groupName">
                                <option v-for="cat in cats" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
                            </optgroup>
                        </select>
                        <div class="absolute right-5 top-1/2 -translate-y-1/2 pointer-events-none flex items-center gap-2">
                            <span class="material-symbols-outlined text-on-surface-variant text-xl">unfold_more</span>
                        </div>
                    </div>
                </section>
            </div>
            
            <!-- Nota -->
            <section class="space-y-4">
                <label class="block text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant px-1">Notas <span class="opacity-40">(Opcional)</span></label>
                <textarea
                    v-model="note"
                    rows="2"
                    class="w-full bg-surface-container p-5 rounded-xl border border-white/[0.03] focus:border-[#FF4D4D]/30 focus:ring-0 transition-all font-body text-on-surface font-semibold resize-none"
                    placeholder="Detalles adicionales..."
                ></textarea>
            </section>
        </div>

        <!-- Automation Toggle -->
        <section class="bg-surface-container-low p-6 rounded-2xl border border-white/[0.02]">
            <div class="flex items-center justify-between mb-2">
                <div class="flex gap-4 items-center">
                    <div class="w-12 h-12 rounded-xl bg-[#FF4D4D]/10 flex items-center justify-center text-[#FF4D4D]">
                        <span class="material-symbols-outlined text-2xl">sync</span>
                    </div>
                    <div class="space-y-0.5">
                        <h4 class="text-sm font-bold text-on-surface">Automatización</h4>
                        <p class="text-[10px] font-medium text-on-surface-variant uppercase tracking-wide">Gasto Recurrente</p>
                    </div>
                </div>
                <!-- Toggle Switch -->
                <label class="relative inline-flex items-center cursor-pointer group">
                    <input
                        type="checkbox"
                        v-model="isRecurring"
                        class="sr-only peer"
                    />
                    <div class="w-12 h-6 bg-surface-container-highest rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:start-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-[#FF4D4D] shadow-lg group-active:scale-95"></div>
                </label>
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
                <div v-if="isRecurring" class="mt-6 pt-6 border-t border-white/[0.03] grid grid-cols-1 md:grid-cols-2 gap-6">
                    <div class="space-y-3">
                        <label class="block text-[10px] font-bold text-on-surface-variant uppercase tracking-wider">Frecuencia</label>
                        <div class="relative">
                            <select v-model="recurringFrequency" class="w-full appearance-none bg-surface-container-high p-4 rounded-xl border border-white/[0.03] text-sm font-bold text-on-surface focus:ring-1 focus:ring-red-500/30 outline-none transition-all">
                                <option value="Monthly">Mensual</option>
                                <option value="Weekly">Semanal</option>
                                <option value="Yearly">Anual</option>
                            </select>
                            <span class="material-symbols-outlined absolute right-3 top-1/2 -translate-y-1/2 text-on-surface-variant pointer-events-none text-xl">unfold_more</span>
                        </div>
                    </div>
                    <div class="space-y-3">
                        <label class="block text-[10px] font-bold text-on-surface-variant uppercase tracking-wider">Próximo Cobro</label>
                        <input v-model="nextBillingDate" type="date" class="w-full bg-surface-container-high p-4 rounded-xl border border-white/[0.03] text-sm font-bold text-on-surface focus:ring-1 focus:ring-red-500/30 outline-none transition-all [color-scheme:dark]" />
                    </div>
                </div>
            </Transition>
        </section>

        <!-- Footer Actions (Simple Mode) -->
        <div class="pt-6 space-y-6 flex flex-col items-center">
            <button
                type="submit"
                :disabled="isSubmitDisabled || submitting"
                class="w-full bg-[#FF4D4D] text-white font-headline font-black py-5 rounded-xl hover:brightness-110 active:scale-[0.98] transition-all shadow-xl shadow-red-500/20 disabled:opacity-50 flex items-center justify-center gap-3"
            >
                <span v-if="submitting" class="material-symbols-outlined animate-spin shadow-none">sync</span>
                <span v-else class="material-symbols-outlined">receipt_long</span>
                {{ submitting ? 'Guardando...' : 'Guardar Gasto' }}
            </button>
            <button
                type="button"
                :disabled="submitting"
                class="w-full bg-surface-container-high text-on-surface font-headline font-bold py-5 rounded-xl hover:bg-surface-variant active:scale-[0.98] transition-all"
            >
                Guardar y Agregar Otro
            </button>
            <button 
                type="button" 
                @click="toggleSplitMode" 
                class="inline-flex items-center gap-2 text-[10px] font-black uppercase tracking-[0.2em] text-on-surface-variant hover:text-on-surface transition-colors"
            >
                <span class="material-symbols-outlined text-lg">call_split</span>
                Desglosar en múltiples categorías
            </button>
        </div>
      </div>

      <!-- ============================== -->
      <!-- ITEMIZED MODE WRAPPER -->
      <!-- ============================== -->
      <div v-else class="space-y-10 animate-fade-in-up">
        
        <!-- Header Summary Card -->
        <div class="bg-surface-container-low p-8 rounded-3xl border border-white/[0.03] luminous-shadow-sm flex flex-col items-center text-center">
            <span class="text-[10px] font-black uppercase tracking-[0.2em] text-on-surface-variant mb-3">Total a Desglosar</span>
            <div class="flex items-center justify-center gap-2">
                <span class="font-headline text-2xl font-black text-[#FF4D4D]">-</span>
                <span class="font-headline text-2xl font-black text-[#FF4D4D]">₡</span>
                <span class="font-headline text-5xl font-black text-on-surface tracking-tighter">{{ totalAmount?.toLocaleString() || '0' }}</span>
            </div>
            <div class="mt-6 flex items-center gap-3 px-5 py-2 rounded-full bg-surface-container-high border border-white/[0.03]">
                <span class="text-[10px] font-bold text-on-surface truncate max-w-[120px]">{{ merchant || 'Comercio' }}</span>
                <span class="w-1 h-1 rounded-full bg-on-surface-variant/20"></span>
                <span class="text-[10px] font-bold text-on-surface-variant">{{ date }}</span>
            </div>
        </div>

        <!-- Dynamic List Section -->
        <div class="space-y-6">
            <h3 class="px-1 text-[10px] font-black uppercase tracking-[0.2em] text-on-surface-variant flex items-center gap-2">
                <span class="material-symbols-outlined text-lg">list_alt</span>
                Ítems de la factura
            </h3>
          
            <div class="space-y-4">
                <!-- Dynamic Rows -->
                <div 
                    v-for="(item, index) in items" 
                    :key="item.id" 
                    class="bg-surface-container p-6 rounded-2xl border border-white/[0.03] relative group transition-all duration-300 hover:bg-surface-container-high"
                >
                    <div class="grid grid-cols-1 gap-6">
                        <div class="flex items-start justify-between">
                            <div class="flex-1 space-y-4">
                                <!-- Amount Input -->
                                <div class="space-y-3">
                                    <label class="block text-[9px] font-black text-on-surface-variant uppercase tracking-widest leading-none">Monto</label>
                                    <div class="flex items-center gap-2">
                                        <span class="text-lg font-black text-[#FF4D4D]">₡</span>
                                        <input 
                                            v-model="item.itemAmount" 
                                            class="bg-transparent border-none p-0 text-xl font-black text-on-surface focus:ring-0 placeholder:text-on-surface-variant/20" 
                                            type="number" 
                                            step="0.01" 
                                            placeholder="0.00"
                                            required 
                                        />
                                    </div>
                                </div>
                            </div>
                            <button 
                                type="button" 
                                @click="removeRow(item.id)" 
                                class="p-2 rounded-lg hover:bg-red-500/10 text-on-surface-variant hover:text-red-500 transition-all opacity-0 group-hover:opacity-100"
                            >
                                <span class="material-symbols-outlined text-xl">close</span>
                            </button>
                        </div>
                        
                        <div class="grid grid-cols-1 gap-4 pt-4 border-t border-white/[0.03]">
                            <!-- Category Select -->
                            <div class="relative">
                                <select v-model="item.categoryId" class="w-full appearance-none bg-surface-container-high p-4 rounded-xl border border-white/[0.03] text-sm font-bold text-on-surface focus:ring-1 focus:ring-[#FF4D4D]/20 transition-all cursor-pointer" required>
                                    <option :value="null" disabled>Seleccionar Categoría</option>
                                    <optgroup v-for="(cats, groupName) in groupedCategories" :key="groupName" :label="groupName">
                                        <option v-for="cat in cats" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
                                    </optgroup>
                                </select>
                                <div class="absolute right-4 top-1/2 -translate-y-1/2 pointer-events-none">
                                    <span class="material-symbols-outlined text-on-surface-variant text-lg">unfold_more</span>
                                </div>
                            </div>
                            <!-- Description Input -->
                            <input 
                                v-model="item.description" 
                                class="w-full bg-surface-container-high p-4 rounded-xl border border-white/[0.03] text-sm font-bold text-on-surface focus:ring-1 focus:ring-primary-container/20 transition-all placeholder:text-on-surface-variant/20" 
                                type="text" 
                                placeholder="Detalle (ej. Manzanas, Camiseta...)" 
                            />
                        </div>
                    </div>
                </div>

                <!-- Add Item Button -->
                <button 
                    type="button" 
                    @click="addRow" 
                    class="w-full py-6 border-2 border-dashed border-white/[0.05] rounded-2xl text-on-surface-variant hover:text-on-surface hover:border-white/10 hover:bg-white/[0.02] transition-all flex items-center justify-center gap-3 font-black text-[10px] uppercase tracking-[0.2em]"
                >
                    <span class="material-symbols-outlined text-xl">add_circle</span>
                    Agregar otro ítem
                </button>
            </div>
          
            <!-- Math Feedback Section -->
            <div class="p-6 rounded-2xl transition-all duration-500" :class="isMathMismatch ? 'bg-red-500/5 border border-red-500/10' : 'bg-primary-container/5 border border-primary-container/10'">
                <div class="flex items-center justify-between">
                    <div class="flex items-center gap-3">
                        <div class="w-10 h-10 rounded-xl flex items-center justify-center" :class="isMathMismatch ? 'bg-red-500/10 text-red-500' : 'bg-primary-container/10 text-primary-container'">
                            <span class="material-symbols-outlined text-xl">{{ isMathMismatch ? 'priority_high' : 'check_circle' }}</span>
                        </div>
                        <div class="space-y-0.5">
                            <h4 class="text-[10px] font-black uppercase tracking-widest" :class="isMathMismatch ? 'text-red-500' : 'text-primary-container'">
                                {{ isMathMismatch ? 'Error de Suma' : 'Distribución Correcta' }}
                            </h4>
                            <p class="text-xs font-bold text-on-surface">
                                {{ isMathMismatch 
                                    ? (missingAmount > 0 ? `Faltan ₡${missingAmount.toFixed(2)} por asignar` : `Se excede por ₡${Math.abs(missingAmount).toFixed(2)}`) 
                                    : 'Todos los montos coinciden' 
                                }}
                            </p>
                        </div>
                    </div>
                    <div class="text-right">
                        <p class="text-[9px] font-bold text-on-surface-variant uppercase tracking-widest mb-1">Total Asignado</p>
                        <p class="text-sm font-black text-on-surface">₡{{ runningSum.toFixed(2) }}</p>
                    </div>
                </div>
            </div>
        </div>

        <!-- Actions Footer (Itemized Mode) -->
        <div class="pt-6 space-y-6 flex flex-col items-center">
            <button
                type="submit"
                :disabled="isSubmitDisabled || submitting"
                class="w-full bg-[#FF4D4D] text-white font-headline font-black py-5 rounded-xl hover:brightness-110 active:scale-[0.98] transition-all shadow-xl shadow-red-500/20 disabled:opacity-50 flex items-center justify-center gap-3"
            >
                <span v-if="submitting" class="material-symbols-outlined animate-spin shadow-none">sync</span>
                <span v-else class="material-symbols-outlined">receipt_long</span>
                {{ submitting ? 'Guardando...' : 'Guardar Gasto Desglosado' }}
            </button>
            <button
                @click="toggleSplitMode"
                type="button"
                :disabled="submitting"
                class="inline-flex items-center gap-2 text-[10px] font-black uppercase tracking-[0.2em] text-on-surface-variant hover:text-on-surface transition-colors"
            >
                <span class="material-symbols-outlined text-lg">arrow_back</span>
                Cancelar y volver a modo simple
            </button>
        </div>
        
      </div>
    </form>
  </div>
</template>

<style scoped>
.luminous-shadow-sm {
  box-shadow: 0 20px 40px -20px rgba(5, 230, 153, 0.05);
}

.luminous-shadow-red {
  box-shadow: 0 20px 40px -20px rgba(255, 77, 77, 0.1);
}

.animate-fade-in-up {
  animation: fadeInUp 0.5s ease-out forwards;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Hide spin arrows */
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

select {
  background-image: none !important;
}
</style>

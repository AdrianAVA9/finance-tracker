<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import api from '@/services/api'

interface Category {
  id: number
  name: string
  icon?: string
}

interface Props {
  isOpen: boolean
  month: number
  year: number
  budget?: {
    id: number
    categoryId: number
    limitAmount: number
  }
}

const props = defineProps<Props>()
const emit = defineEmits(['close', 'saved'])

const categories = ref<Category[]>([])
const selectedCategoryId = ref<number | null>(null)
const amount = ref<number | null>(null)
const isSubmitting = ref(false)

const loadCategories = async () => {
  try {
    const { data } = await api.get('/api/v1/expensecategories')
    categories.value = data
  } catch (error) {
    console.error('Failed to load categories', error)
  }
}

watch(() => props.isOpen, (newVal) => {
  if (newVal) {
    if (props.budget) {
      selectedCategoryId.value = props.budget.categoryId
      amount.value = props.budget.limitAmount
    } else {
      selectedCategoryId.value = null
      amount.value = null
    }
  }
})

const handleSave = async () => {
  if (!selectedCategoryId.value || !amount.value) return

  isSubmitting.value = true
  try {
    // We use the batch endpoint but just send one item
    await api.post('/api/v1/budgets/batch', {
      month: props.month,
      year: props.year,
      budgets: [{
        categoryId: selectedCategoryId.value,
        amount: amount.value
      }]
    })
    emit('saved')
    emit('close')
  } catch (error) {
    console.error('Failed to save budget', error)
  } finally {
    isSubmitting.value = false
  }
}

onMounted(loadCategories)
</script>

<template>
  <Transition
    enter-active-class="transition duration-300 ease-out"
    enter-from-class="opacity-0"
    enter-to-class="opacity-100"
    leave-active-class="transition duration-200 ease-in"
    leave-from-class="opacity-100"
    leave-to-class="opacity-0"
  >
    <div v-if="isOpen" class="fixed inset-0 z-[100] flex items-center justify-center p-4 bg-background-dark/80 backdrop-blur-sm">
      <div class="relative w-full max-w-[440px] animate-fade-in-up">
        <!-- Atmospheric background glow -->
        <div class="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[500px] h-[500px] bg-primary/5 blur-[100px] rounded-full pointer-events-none"></div>

        <!-- Card -->
        <div class="bg-white dark:bg-surface-dark rounded-xl shadow-2xl border border-gray-100 dark:border-border-dark overflow-hidden flex flex-col">
          <!-- Header -->
          <div class="flex items-center justify-between px-5 pt-5 pb-1">
            <div>
              <h2 class="text-lg font-bold tracking-tight text-white">{{ budget ? 'Editar Presupuesto' : 'Set Category Budget' }}</h2>
              <p class="text-xs text-text-muted mt-0.5">Define spending limits for your tracking.</p>
            </div>
            <button @click="emit('close')" class="group p-1.5 rounded-lg hover:bg-border-dark transition-colors">
              <span class="material-symbols-outlined text-text-muted group-hover:text-white transition-colors text-[20px]">close</span>
            </button>
          </div>

          <!-- Form Content -->
          <div class="p-5 space-y-5">
            <!-- Category Select -->
            <div class="space-y-1.5">
              <label class="block text-xs font-semibold text-text-muted uppercase tracking-wider">Categoría</label>
              <div class="relative group">
                <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <span class="material-symbols-outlined text-text-muted group-focus-within:text-primary transition-colors text-[18px]">category</span>
                </div>
                <select 
                  v-model="selectedCategoryId"
                  :disabled="!!budget"
                  class="block w-full pl-9 pr-9 py-2.5 bg-background-dark border border-border-dark rounded-lg text-sm text-white focus:ring-1 focus:ring-primary focus:border-primary focus:outline-none appearance-none transition-all cursor-pointer shadow-sm disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  <option :value="null" disabled>Selecciona una categoría...</option>
                  <option v-for="cat in categories" :key="cat.id" :value="cat.id">{{ cat.name }}</option>
                </select>
                <div class="absolute inset-y-0 right-0 pr-3 flex items-center pointer-events-none">
                  <span class="material-symbols-outlined text-text-muted text-[18px]">unfold_more</span>
                </div>
              </div>
            </div>

            <!-- Target Amount -->
            <div class="space-y-1.5">
              <label class="block text-xs font-semibold text-text-muted uppercase tracking-wider">Monto Objetivo</label>
              <div class="relative group">
                <div class="absolute inset-y-0 left-0 pl-3.5 flex items-center pointer-events-none">
                  <span class="text-text-muted font-bold text-xl group-focus-within:text-primary transition-colors">₡</span>
                </div>
                <input 
                  v-model="amount"
                  type="number" 
                  step="0.01"
                  class="block w-full pl-9 pr-14 py-3 bg-background-dark border border-border-dark rounded-lg text-2xl font-bold tracking-tight text-white focus:ring-1 focus:ring-primary focus:border-primary focus:outline-none placeholder-text-muted/20 transition-all shadow-inner" 
                  placeholder="0.00"
                />
                <div class="absolute inset-y-0 right-0 pr-3.5 flex items-center pointer-events-none">
                  <span class="text-[9px] font-bold tracking-wider text-text-muted bg-border-dark px-1.5 py-0.5 rounded uppercase">CRC</span>
                </div>
              </div>
            </div>

            <!-- Frequency Segmented Control -->
            <div class="space-y-1.5">
              <label class="block text-xs font-semibold text-text-muted uppercase tracking-wider">Reinicio</label>
              <div class="grid grid-cols-3 gap-1 p-1 bg-background-dark rounded-lg border border-border-dark">
                <div class="flex items-center justify-center py-1.5 rounded-md text-xs font-semibold text-text-muted cursor-not-allowed opacity-50">Semanal</div>
                <div class="flex items-center justify-center py-1.5 rounded-md text-xs font-semibold bg-primary text-[#121416] shadow-md">Mensual</div>
                <div class="flex items-center justify-center py-1.5 rounded-md text-xs font-semibold text-text-muted cursor-not-allowed opacity-50">Única</div>
              </div>
            </div>

            <!-- Alert Toggle Row -->
            <div class="flex items-center justify-between p-3.5 rounded-xl bg-background-dark border border-border-dark">
              <div class="flex items-center gap-2.5">
                <div class="p-1.5 rounded-full bg-primary/10 text-primary">
                  <span class="material-symbols-outlined text-[18px]">notifications_active</span>
                </div>
                <div class="flex flex-col">
                  <span class="text-xs font-semibold text-white">Alerta al 80% de uso</span>
                  <span class="text-[10px] text-text-muted">Notificación push</span>
                </div>
              </div>
              <label class="relative inline-flex items-center cursor-pointer">
                <input type="checkbox" checked class="sr-only peer">
                <div class="w-9 h-5 bg-[#0f1113] peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full rtl:peer-checked:after:-translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:start-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-4 after:w-4 after:transition-all peer-checked:bg-primary shadow-glow"></div>
              </label>
            </div>
          </div>

          <!-- Action Footer -->
          <div class="px-5 py-4 bg-background-dark/30 border-t border-border-dark flex gap-2.5">
            <button 
              @click="emit('close')"
              class="flex-1 px-4 py-2.5 rounded-lg text-xs font-semibold text-text-muted hover:bg-border-dark transition-colors focus:outline-none"
            >
              Cancelar
            </button>
            <button 
              @click="handleSave"
              :disabled="isSubmitting || !selectedCategoryId || !amount"
              class="flex-[2] px-4 py-2.5 rounded-lg text-xs font-bold bg-primary text-[#121416] hover:brightness-110 active:scale-[0.98] transition-all shadow-glow flex items-center justify-center gap-2 focus:outline-none disabled:opacity-50"
            >
              <span v-if="isSubmitting" class="animate-spin material-symbols-outlined text-[16px]">sync</span>
              <span v-else class="material-symbols-outlined text-[16px]">save</span>
              {{ budget ? 'Actualizar' : 'Guardar Presupuesto' }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </Transition>
</template>

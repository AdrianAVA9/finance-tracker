<script setup lang="ts">
import { computed } from 'vue'
import CategorySelector from '@/app/components/common/CategorySelector.vue'
import SurfaceCard from '@/app/components/common/SurfaceCard.vue'
import AmountInputCard from '@/app/components/common/AmountInputCard.vue'
import AppButton from '@/app/components/common/AppButton.vue'
import {
  useIncomeRegistrationForm,
  INCOME_FREQUENCY_OPTIONS,
  type IncomeDetailsDto,
  type IncomeSubmitPayload
} from '@/app/components/incomes/useIncomeRegistrationForm'

interface Props {
  submitting?: boolean
  error?: string
  initialData?: IncomeDetailsDto | null
}

const props = defineProps<Props>()
const emit = defineEmits<{
  submit: [payload: IncomeSubmitPayload]
  'save-and-another': [payload: IncomeSubmitPayload]
}>()

const {
  amount,
  source,
  date,
  categoryId,
  notes,
  isRecurring,
  frequency,
  nextDate,
  categories,
  selectedCategory,
  isSubmitDisabled,
  buildPayload,
  resetForNewEntry
} = useIncomeRegistrationForm(() => props.initialData ?? null)

const isEditMode = computed(() => !!props.initialData)

function submitForm() {
  if (isSubmitDisabled.value) return
  emit('submit', buildPayload())
}

function emitSaveAndAnother() {
  if (isSubmitDisabled.value) return
  emit('save-and-another', buildPayload())
}

defineExpose({ resetForNewEntry })
</script>

<template>
  <div class="space-y-10">
    <form class="space-y-10" @submit.prevent="submitForm">
      <Transition
        enter-active-class="transform transition duration-500 ease-out"
        enter-from-class="-translate-y-10 opacity-0"
        enter-to-class="translate-y-0 opacity-100"
        leave-active-class="transition duration-300 ease-in"
        leave-from-class="opacity-100"
        leave-to-class="scale-95 opacity-0"
      >
        <SurfaceCard v-if="error" class="!border-red-500/20 !bg-red-500/10">
          <div class="flex items-start gap-4 text-red-500">
            <span class="material-symbols-outlined mt-1">error</span>
            <div class="space-y-1">
              <h4 class="text-sm font-black uppercase tracking-widest">Error</h4>
              <p class="text-[11px] font-bold leading-relaxed opacity-80">{{ error }}</p>
            </div>
          </div>
        </SurfaceCard>
      </Transition>

      <section>
        <label
          class="mb-4 block px-1 text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant"
          for="income-amount-input"
        >
          Monto del Ingreso
        </label>
        <div data-testid="income-amount-card">
          <AmountInputCard
            v-model="amount"
            input-id="income-amount-input"
            currency-symbol="₡"
          />
        </div>
      </section>

      <div class="grid grid-cols-1 gap-6">
        <section class="space-y-4">
          <label
            class="block px-1 text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant"
            for="income-source-input"
          >
            Fuente o Pagador
          </label>
          <input
            id="income-source-input"
            v-model="source"
            data-testid="income-source-input"
            type="text"
            class="w-full rounded-xl border border-white/[0.03] bg-surface-container p-5 font-body font-semibold text-on-surface transition-all focus:border-primary-container/30 focus:ring-0"
            placeholder="Ej. Salario Quincenal, Freelance..."
            required
          />
        </section>

        <div class="grid grid-cols-1 gap-6 md:grid-cols-2">
          <section class="space-y-4">
            <label
              class="block px-1 text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant"
              for="income-date-input"
            >
              Fecha del Depósito
            </label>
            <input
              id="income-date-input"
              v-model="date"
              data-testid="income-date-input"
              type="date"
              class="w-full max-w-full rounded-xl border border-white/[0.03] bg-surface-container p-5 font-body font-semibold text-on-surface transition-all focus:border-primary-container/30 focus:ring-0 [color-scheme:dark]"
              required
            />
          </section>
          <section class="space-y-4">
            <label class="block px-1 text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant">
              Categoría
            </label>
            <CategorySelector
              v-model="categoryId"
              data-testid="income-category-selector"
              :categories="categories"
              placeholder="Selecciona una categoría"
            />
          </section>
        </div>

        <section class="space-y-4">
          <label
            class="block px-1 text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant"
            for="income-notes"
          >
            Notas <span class="opacity-40">(Opcional)</span>
          </label>
          <textarea
            id="income-notes"
            v-model="notes"
            data-testid="income-notes"
            rows="2"
            class="w-full resize-none rounded-xl border border-white/[0.03] bg-surface-container p-5 font-body font-semibold text-on-surface transition-all focus:border-primary-container/30 focus:ring-0"
            placeholder="Detalles adicionales..."
          />
        </section>
      </div>

      <SurfaceCard class="!border-white/[0.02]">
        <div class="mb-2 flex items-center justify-between">
          <div class="flex items-center gap-4">
            <div
              class="flex h-12 w-12 items-center justify-center rounded-xl bg-primary-container/10 text-primary-container"
            >
              <span class="material-symbols-outlined text-2xl">sync</span>
            </div>
            <div class="space-y-0.5">
              <h4 class="text-sm font-bold text-on-surface">Automatización</h4>
              <p class="text-[10px] font-medium uppercase tracking-wide text-on-surface-variant">
                Ingreso Recurrente
              </p>
            </div>
          </div>
          <label class="group relative inline-flex cursor-pointer items-center">
            <input v-model="isRecurring" data-testid="income-recurring-toggle" type="checkbox" class="peer sr-only" />
            <div
              class="peer h-6 w-12 rounded-full bg-surface-container-highest shadow-lg after:absolute after:start-[2px] after:top-[2px] after:h-5 after:w-5 after:rounded-full after:border after:border-gray-300 after:bg-white after:transition-all after:content-[''] peer-checked:bg-primary-container peer-checked:after:translate-x-full peer-checked:after:border-white group-active:scale-95"
            />
          </label>
        </div>

        <Transition
          enter-active-class="transition duration-300 ease-out"
          enter-from-class="-translate-y-2 scale-95 opacity-0"
          enter-to-class="translate-y-0 scale-100 opacity-100"
          leave-active-class="transition duration-200 ease-in"
          leave-from-class="translate-y-0 scale-100 opacity-100"
          leave-to-class="-translate-y-2 scale-95 opacity-0"
        >
          <div
            v-if="isRecurring"
            data-purpose="expanded-fields"
            class="mt-6 grid grid-cols-1 gap-6 border-t border-white/[0.03] pt-6 md:grid-cols-2"
          >
            <div class="space-y-3">
              <label class="block text-[10px] font-bold uppercase tracking-wider text-on-surface-variant">
                Frecuencia
              </label>
              <div class="relative">
                <select
                  v-model="frequency"
                  data-testid="income-frequency-select"
                  class="w-full appearance-none rounded-xl border border-white/[0.03] bg-surface-container-high p-4 text-sm font-bold text-on-surface outline-none transition-all focus:ring-1 focus:ring-primary-container/30"
                >
                  <option v-for="f in INCOME_FREQUENCY_OPTIONS" :key="f.value" :value="f.value">
                    {{ f.label }}
                  </option>
                </select>
                <span
                  class="material-symbols-outlined pointer-events-none absolute right-3 top-1/2 -translate-y-1/2 text-xl text-on-surface-variant"
                >
                  unfold_more
                </span>
              </div>
            </div>
            <div class="space-y-3">
              <label
                class="block text-[10px] font-bold uppercase tracking-wider text-on-surface-variant"
                for="income-next-date"
              >
                Próximo Depósito
              </label>
              <input
                id="income-next-date"
                v-model="nextDate"
                data-testid="income-next-date"
                type="date"
                class="w-full rounded-xl border border-white/[0.03] bg-surface-container-high p-4 text-sm font-bold text-on-surface outline-none transition-all focus:ring-1 focus:ring-primary-container/30 [color-scheme:dark]"
                :required="isRecurring"
              />
            </div>
          </div>
        </Transition>
      </SurfaceCard>

      <div class="flex flex-col items-center space-y-4 pt-6">
        <AppButton
          data-testid="income-form-submit"
          type="submit"
          variant="primary"
          icon="save"
          :loading="submitting"
          :disabled="isSubmitDisabled"
        >
          {{ submitting ? 'Guardando...' : isEditMode ? 'Actualizar Ingreso' : 'Guardar Ingreso' }}
        </AppButton>
        <AppButton
          v-if="!isEditMode"
          data-testid="income-save-and-another"
          type="button"
          variant="secondary"
          :disabled="isSubmitDisabled || submitting"
          @click="emitSaveAndAnother"
        >
          Guardar y Registrar Otro
        </AppButton>
      </div>
    </form>
  </div>
</template>

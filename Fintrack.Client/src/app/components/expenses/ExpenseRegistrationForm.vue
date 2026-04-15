<script setup lang="ts">
import { computed } from 'vue'
import CategorySelector from '@/app/components/common/CategorySelector.vue'
import SurfaceCard from '@/app/components/common/SurfaceCard.vue'
import AmountInputCard from '@/app/components/common/AmountInputCard.vue'
import AppButton from '@/app/components/common/AppButton.vue'
import {
  useExpenseRegistrationForm,
  type ExpenseInitialPayload,
  type ExpenseSubmitPayload
} from '@/app/components/expenses/useExpenseRegistrationForm'

interface Props {
  submitting?: boolean
  error?: string
  initialData?: ExpenseInitialPayload | null
}

const props = defineProps<Props>()
const emit = defineEmits<{
  submit: [payload: ExpenseSubmitPayload]
}>()

const {
  merchant,
  totalAmount,
  date,
  singleCategoryId,
  isSplitInvoice,
  items,
  categories,
  runningSum,
  isMathMismatch,
  missingAmount,
  isSubmitDisabled,
  toggleSplitMode,
  addRow,
  removeRow,
  buildSubmitPayload
} = useExpenseRegistrationForm(() => props.initialData ?? null)

const isEditMode = computed(() => !!props.initialData)

function saveExpense() {
  if (isSubmitDisabled.value) return
  emit('submit', buildSubmitPayload())
}
</script>

<template>
  <div class="space-y-10">
    <form class="space-y-10" @submit.prevent="saveExpense">
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
              <h4 class="text-sm font-black uppercase tracking-widest">Error de Registro</h4>
              <p class="text-[11px] font-bold leading-relaxed opacity-80">{{ error }}</p>
            </div>
          </div>
        </SurfaceCard>
      </Transition>

      <div v-if="!isSplitInvoice" class="space-y-10">
        <section>
          <label
            class="mb-4 block px-1 text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant"
            for="expense-total-amount-input"
          >
            Monto del Gasto
          </label>
          <div data-testid="expense-amount-card">
            <AmountInputCard
              v-model="totalAmount"
              input-id="expense-total-amount-input"
              currency-symbol="₡"
            />
          </div>
        </section>

        <div class="grid grid-cols-1 gap-8">
          <section class="space-y-4">
            <label
              class="block px-1 text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant"
              for="expense-merchant-input"
            >
              Comercio
            </label>
            <input
              id="expense-merchant-input"
              v-model="merchant"
              data-testid="expense-merchant-input"
              type="text"
              class="w-full rounded-xl border border-white/[0.03] bg-surface-container p-5 font-body font-semibold text-on-surface transition-all focus:border-primary-container/30 focus:ring-0"
              placeholder="Ej. AutoMercado, Gasolinera..."
              required
            />
          </section>

          <div class="grid grid-cols-1 gap-8 md:grid-cols-2">
            <section class="space-y-4">
              <label
                class="block px-1 text-[10px] font-bold uppercase tracking-[0.2em] text-on-surface-variant"
                for="expense-simple-date"
              >
                Fecha del Gasto
              </label>
              <input
                id="expense-simple-date"
                v-model="date"
                data-testid="expense-simple-date"
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
                v-model="singleCategoryId"
                data-testid="expense-category-selector"
                :categories="categories"
                placeholder="Seleccione una categoría"
              />
            </section>
          </div>

        </div>

        <div class="flex flex-col items-center space-y-6 pt-6">
          <AppButton
            data-testid="expense-form-submit"
            type="submit"
            variant="primary"
            icon="receipt_long"
            :loading="submitting"
            :disabled="isSubmitDisabled"
          >
            {{ submitting ? 'Guardando...' : isEditMode ? 'Actualizar Gasto' : 'Guardar Gasto' }}
          </AppButton>
          <button
            type="button"
            data-testid="expense-toggle-split"
            class="inline-flex items-center gap-2 text-[10px] font-black uppercase tracking-[0.2em] text-on-surface-variant transition-colors hover:text-on-surface"
            @click="toggleSplitMode"
          >
            <span class="material-symbols-outlined text-lg">call_split</span>
            Desglosar en múltiples categorías
          </button>
        </div>
      </div>

      <div v-else class="space-y-10">
        <SurfaceCard class="flex flex-col items-center text-center">
          <span class="mb-3 text-[10px] font-black uppercase tracking-[0.2em] text-on-surface-variant">
            Total a Desglosar
          </span>
          <div class="flex items-center justify-center gap-2">
            <span class="font-headline text-2xl font-black text-primary-container">₡</span>
            <span class="font-headline text-5xl font-black tracking-tighter text-on-surface">{{
              totalAmount?.toLocaleString() || '0'
            }}</span>
          </div>
          <div
            class="mt-6 flex items-center gap-3 rounded-full border border-white/[0.03] bg-surface-container-high px-5 py-2"
          >
            <span class="max-w-[120px] truncate text-[10px] font-bold text-on-surface">{{
              merchant || 'Comercio'
            }}</span>
            <span class="h-1 w-1 rounded-full bg-on-surface-variant/20" />
            <span class="text-[10px] font-bold text-on-surface-variant">{{ date }}</span>
          </div>
        </SurfaceCard>

        <div class="space-y-6">
          <h3
            class="flex items-center gap-2 px-1 text-[10px] font-black uppercase tracking-[0.2em] text-on-surface-variant"
          >
            <span class="material-symbols-outlined text-lg">list_alt</span>
            Ítems de la factura
          </h3>

          <div class="space-y-4">
            <SurfaceCard
              v-for="item in items"
              :key="item.id"
              class="group relative !p-6 transition-all duration-300 hover:bg-surface-container-high"
            >
              <div class="grid grid-cols-1 gap-6">
                <div class="flex items-start justify-between">
                  <div class="flex-1 space-y-4">
                    <div class="space-y-3">
                      <label
                        class="block text-[9px] font-black uppercase leading-none tracking-widest text-on-surface-variant"
                      >
                        Monto
                      </label>
                      <div class="flex items-center gap-2">
                        <span class="text-lg font-black text-primary-container">₡</span>
                        <input
                          v-model="item.itemAmount"
                          class="border-none bg-transparent p-0 text-xl font-black text-on-surface placeholder:text-on-surface-variant/20 focus:ring-0"
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
                    class="rounded-lg p-2 text-on-surface-variant opacity-0 transition-all hover:bg-red-500/10 hover:text-red-500 group-hover:opacity-100"
                    @click="removeRow(item.id)"
                  >
                    <span class="material-symbols-outlined text-xl">close</span>
                  </button>
                </div>

                <div class="grid grid-cols-1 gap-4 border-t border-white/[0.03] pt-4">
                  <CategorySelector
                    v-model="item.categoryId"
                    :categories="categories"
                    placeholder="Seleccionar Categoría"
                  />
                  <input
                    v-model="item.description"
                    class="w-full rounded-xl border border-white/[0.03] bg-surface-container-high p-4 text-sm font-bold text-on-surface transition-all placeholder:text-on-surface-variant/20 focus:ring-1 focus:ring-primary-container/20"
                    type="text"
                    placeholder="Detalle (ej. Manzanas, Camiseta...)"
                  />
                </div>
              </div>
            </SurfaceCard>

            <button
              type="button"
              class="flex w-full items-center justify-center gap-3 rounded-xl border-2 border-dashed border-white/[0.05] py-6 text-[10px] font-black uppercase tracking-[0.2em] text-on-surface-variant transition-all hover:border-white/10 hover:bg-white/[0.02] hover:text-on-surface"
              @click="addRow"
            >
              <span class="material-symbols-outlined text-xl">add_circle</span>
              Agregar otro ítem
            </button>
          </div>

          <SurfaceCard
            class="transition-all duration-500"
            :class="
              isMathMismatch
                ? '!border-red-500/10 !bg-red-500/5'
                : '!border-primary-container/10 !bg-primary-container/5'
            "
          >
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div
                  class="flex h-10 w-10 items-center justify-center rounded-xl"
                  :class="
                    isMathMismatch ? 'bg-red-500/10 text-red-500' : 'bg-primary-container/10 text-primary-container'
                  "
                >
                  <span class="material-symbols-outlined text-xl">{{
                    isMathMismatch ? 'priority_high' : 'check_circle'
                  }}</span>
                </div>
                <div class="space-y-0.5">
                  <h4
                    class="text-[10px] font-black uppercase tracking-widest"
                    :class="isMathMismatch ? 'text-red-500' : 'text-primary-container'"
                  >
                    {{ isMathMismatch ? 'Error de Suma' : 'Distribución Correcta' }}
                  </h4>
                  <p class="text-xs font-bold text-on-surface">
                    {{
                      isMathMismatch
                        ? missingAmount > 0
                          ? `Faltan ₡${missingAmount.toFixed(2)} por asignar`
                          : `Se excede por ₡${Math.abs(missingAmount).toFixed(2)}`
                        : 'Todos los montos coinciden'
                    }}
                  </p>
                </div>
              </div>
              <div class="text-right">
                <p class="mb-1 text-[9px] font-bold uppercase tracking-widest text-on-surface-variant">
                  Total Asignado
                </p>
                <p class="text-sm font-black text-on-surface">₡{{ runningSum.toFixed(2) }}</p>
              </div>
            </div>
          </SurfaceCard>
        </div>

        <div class="flex flex-col items-center space-y-6 pt-6">
          <AppButton
            data-testid="expense-form-submit"
            type="submit"
            variant="primary"
            icon="receipt_long"
            :loading="submitting"
            :disabled="isSubmitDisabled"
          >
            {{ submitting ? 'Guardando...' : 'Guardar Gasto Desglosado' }}
          </AppButton>
          <button
            type="button"
            data-testid="expense-cancel-split"
            class="inline-flex items-center gap-2 text-[10px] font-black uppercase tracking-[0.2em] text-on-surface-variant transition-colors hover:text-on-surface"
            :disabled="submitting"
            @click="toggleSplitMode"
          >
            <span class="material-symbols-outlined text-lg">arrow_back</span>
            Cancelar y volver a modo simple
          </button>
        </div>
      </div>
    </form>
  </div>
</template>

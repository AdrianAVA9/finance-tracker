<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '@/services/api'
import AppButton from '@/app/components/common/AppButton.vue'
import LoadingIndicator from '@/app/components/common/LoadingIndicator.vue'
import SurfaceCard from '@/app/components/common/SurfaceCard.vue'

const route = useRoute()
const router = useRouter()

const iconChoices = [
  'restaurant',
  'pets',
  'directions_car',
  'shopping_bag',
  'fitness_center',
  'medical_services',
  'school',
  'flight',
  'home',
  'phone',
  'account_balance_wallet',
  'savings',
  'work',
  'more_horiz'
] as const

const colorSwatches = [
  '#05E699',
  '#fb923c',
  '#60a5fa',
  '#c084fc',
  '#f472b6',
  '#facc15',
  '#22d3ee',
  '#77ffbb'
]

interface GroupOption {
  id: string
  name: string
}

const isIncomeRoute = computed(
  () => route.name === 'CategoryIncomeNew' || route.name === 'CategoryIncomeEdit'
)
const isNew = computed(() => route.name === 'CategoryIncomeNew' || route.name === 'CategoryExpenseNew')
const categoryId = computed(
  () => (typeof route.params.categoryId === 'string' ? route.params.categoryId : null) as string | null
)

const isLoading = ref(true)
const isSaving = ref(false)
const loadError = ref<string | null>(null)
const formError = ref<string | null>(null)

const name = ref('')
const description = ref('')
const groupId = ref<string | null>(null)
const selectedIcon = ref<string>(iconChoices[0])
const selectedColor = ref(colorSwatches[0])

const groups = ref<GroupOption[]>([])
const isEditable = ref(true)

const listRouteName = computed(() =>
  isIncomeRoute.value ? 'SettingsIncomeCategories' : 'SettingsExpenseCategories'
)

const canEditForm = computed(() => isNew.value || isEditable.value)
const ctaLabel = computed(() => (isNew.value ? 'Crear categoría' : 'Guardar cambios'))
const bentoTypeLabel = computed(() => (isIncomeRoute.value ? 'Ingreso' : 'Gasto'))

function goToList() {
  void router.push({ name: listRouteName.value })
}

function normalizeId(raw: unknown): string {
  if (typeof raw === 'string') return raw
  if (typeof raw === 'number' || typeof raw === 'bigint') return String(raw)
  return String(raw)
}

async function loadGroups() {
  if (isIncomeRoute.value) {
    return
  }
  try {
    const { data } = await api.get<unknown>('/api/v1/expensecategorygroups')
    if (!Array.isArray(data)) {
      return
    }
    groups.value = data.map((row) => {
      const o = row as Record<string, unknown>
      return {
        id: normalizeId(o['id']),
        name: typeof o['name'] === 'string' ? o['name'] : ''
      }
    })
  } catch {
    loadError.value = 'No se pudieron cargar los grupos. Intenta de nuevo.'
  }
}

function mapLoadedExpense(raw: unknown) {
  const o = raw as Record<string, unknown>
  name.value = typeof o['name'] === 'string' ? o['name'] : ''
  description.value = typeof o['description'] === 'string' ? o['description'] : ''
  const gid = o['groupId'] as string | null | undefined
  groupId.value = gid != null && gid !== '' ? normalizeId(gid) : null
  selectedIcon.value = (typeof o['icon'] === 'string' && o['icon'] ? o['icon'] : iconChoices[0]) as string
  const col = o['color'] as string | null | undefined
  selectedColor.value = typeof col === 'string' && col !== '' ? col : colorSwatches[0]
  isEditable.value = o['isEditable'] !== false
}

function mapLoadedIncome(raw: unknown) {
  const o = raw as Record<string, unknown>
  name.value = typeof o['name'] === 'string' ? o['name'] : ''
  description.value = ''
  groupId.value = null
  selectedIcon.value = (typeof o['icon'] === 'string' && o['icon'] ? o['icon'] : iconChoices[0]) as string
  const col = o['color'] as string | null | undefined
  selectedColor.value = typeof col === 'string' && col !== '' ? col : colorSwatches[0]
  isEditable.value = o['isEditable'] !== false
}

async function loadCategory() {
  if (isNew.value || !categoryId.value) {
    return
  }
  loadError.value = null
  formError.value = null
  try {
    if (isIncomeRoute.value) {
      const { data } = await api.get<unknown>(`/api/v1/incomes/categories/${categoryId.value}`)
      mapLoadedIncome(data)
    } else {
      const { data } = await api.get<unknown>(`/api/v1/expensecategories/${categoryId.value}`)
      mapLoadedExpense(data)
    }
  } catch {
    loadError.value = 'No se pudo cargar la categoría.'
  }
}

function resetDefaultsForNew() {
  name.value = ''
  description.value = ''
  groupId.value = isIncomeRoute.value ? null : (groups.value[0]?.id ?? null)
  selectedIcon.value = iconChoices[0]
  selectedColor.value = colorSwatches[0]
  isEditable.value = true
  formError.value = null
  if (!isIncomeRoute.value) {
    loadError.value = null
  }
}

async function bootstrap() {
  loadError.value = null
  formError.value = null
  isLoading.value = true
  try {
    if (isNew.value) {
      await loadGroups()
      resetDefaultsForNew()
    } else {
      await loadGroups()
      await loadCategory()
    }
  } finally {
    isLoading.value = false
  }
}

watch(
  () => [route.name, categoryId.value] as const,
  () => {
    void bootstrap()
  },
  { immediate: true }
)

function validate(): boolean {
  if (!name.value.trim()) {
    formError.value = 'El nombre de la categoría es obligatorio.'
    return false
  }
  formError.value = null
  return true
}

async function onSubmit() {
  if (!canEditForm.value) return
  if (!validate()) return
  isSaving.value = true
  formError.value = null
  try {
    if (isIncomeRoute.value) {
      if (isNew.value) {
        await api.post('/api/v1/incomes/categories', {
          name: name.value.trim(),
          icon: selectedIcon.value,
          color: selectedColor.value
        })
      } else if (categoryId.value) {
        await api.put(`/api/v1/incomes/categories/${categoryId.value}`, {
          name: name.value.trim(),
          icon: selectedIcon.value,
          color: selectedColor.value
        })
      }
    } else if (isNew.value) {
      await api.post('/api/v1/expensecategories', {
        name: name.value.trim(),
        description: description.value.trim() || null,
        icon: selectedIcon.value,
        color: selectedColor.value,
        groupId: groupId.value
      })
    } else if (categoryId.value) {
      await api.put(`/api/v1/expensecategories/${categoryId.value}`, {
        name: name.value.trim(),
        description: description.value.trim() || null,
        icon: selectedIcon.value,
        color: selectedColor.value,
        groupId: groupId.value
      })
    }
    goToList()
  } catch {
    formError.value = 'No se pudo guardar. Revisá los datos o probá otra vez.'
  } finally {
    isSaving.value = false
  }
}
</script>

<template>
  <form class="space-y-8 pb-32" @submit.prevent="onSubmit">
    <p v-if="isNew" class="text-sm text-on-surface-variant -mt-1">
      Definí nombre, identidad visual y, si aplica, el grupo.
    </p>
    <p v-if="!isNew && !isEditable" class="rounded-xl border border-error/30 bg-error-container/10 px-3 py-2 text-sm text-on-surface">
      Esta categoría es del sistema y no se puede modificar.
    </p>
    <p v-if="loadError" class="rounded-xl border border-error/30 bg-error-container/10 px-3 py-2 text-sm text-on-surface">
      {{ loadError }}
    </p>
    <p v-else-if="formError" class="rounded-xl border border-error/30 bg-error-container/10 px-3 py-2 text-sm text-on-surface">
      {{ formError }}
    </p>

    <LoadingIndicator :is-loading="isLoading" message="Cargando…" />

    <template v-if="!isLoading && !loadError">
      <SurfaceCard>
        <div class="space-y-1">
          <span class="text-[10px] uppercase tracking-wider text-outline">Tipo de rubro</span>
          <p class="font-headline font-bold text-on-surface">{{ bentoTypeLabel }}</p>
        </div>
      </SurfaceCard>

      <section class="space-y-2">
        <label
          for="cat-name"
          class="text-[10px] uppercase tracking-[0.2em] font-bold text-outline"
        >Nombre</label>
        <input
          id="cat-name"
          v-model="name"
          :disabled="!canEditForm"
          type="text"
          class="w-full bg-surface-container-lowest border border-outline-variant/10 rounded-xl py-3.5 px-4 text-on-surface placeholder:text-outline focus:outline-none focus:ring-2 focus:ring-primary/40 transition-all disabled:opacity-60"
          placeholder="p. ej. Cena con clientes"
          autocomplete="off"
        />
      </section>

      <section v-if="!isIncomeRoute" class="space-y-2">
        <label for="cat-desc" class="text-[10px] uppercase tracking-[0.2em] font-bold text-outline">Descripción</label>
        <textarea
          id="cat-desc"
          v-model="description"
          :disabled="!canEditForm"
          rows="2"
          class="w-full bg-surface-container border border-outline-variant/20 rounded-xl py-3 px-4 text-on-surface text-sm placeholder:text-outline focus:ring-2 focus:ring-primary-container/20 disabled:opacity-60"
          placeholder="Opcional"
        />
      </section>

      <section v-if="!isIncomeRoute" class="space-y-4">
        <label
          for="cat-group"
          class="text-[10px] uppercase tracking-[0.2em] font-bold text-outline"
        >Asignar a grupo</label>
        <div class="relative">
          <select
            id="cat-group"
            v-model="groupId"
            :disabled="!canEditForm"
            class="w-full bg-surface-container border-none rounded-xl py-4 px-5 text-on-surface appearance-none focus:ring-2 focus:ring-primary-container/20 font-medium disabled:opacity-60"
          >
            <option v-if="groups.length === 0" :value="null" disabled>— Sin grupos —</option>
            <option
              v-for="g in groups"
              :key="g.id"
              :value="g.id"
            >{{ g.name }}</option>
          </select>
          <div
            class="absolute right-4 top-1/2 -translate-y-1/2 pointer-events-none text-primary-container"
            aria-hidden="true"
          >
            <span class="material-symbols-outlined">expand_more</span>
          </div>
        </div>
      </section>

      <section class="space-y-4">
        <div class="flex justify-between items-end">
          <span class="text-[10px] uppercase tracking-[0.2em] font-bold text-outline">Identidad visual</span>
          <span class="text-[10px] text-primary-container font-bold">Elegir ícono</span>
        </div>
        <div class="grid grid-cols-5 gap-3">
          <button
            v-for="ic in iconChoices"
            :key="ic"
            type="button"
            :disabled="!canEditForm"
            :aria-pressed="selectedIcon === ic"
            class="aspect-square rounded-xl flex items-center justify-center transition-all disabled:opacity-50"
            :class="selectedIcon === ic
              ? 'bg-primary-container text-on-primary-container shadow-[0_0_20px_rgba(5,230,153,0.15)] ring-2 ring-primary-container'
              : 'bg-surface-container text-outline hover:text-on-surface hover:bg-surface-container-high'"
            @click="selectedIcon = ic"
          >
            <span class="material-symbols-outlined" :class="{ 'text-[22px]': selectedIcon === ic }">{{ ic }}</span>
          </button>
        </div>
      </section>

      <section class="space-y-4">
        <span class="text-[10px] uppercase tracking-[0.2em] font-bold text-outline">Color de acento</span>
        <div class="flex items-center gap-3 overflow-x-auto hide-scrollbar pb-1">
          <button
            v-for="(hex, idx) in colorSwatches"
            :key="hex"
            type="button"
            :disabled="!canEditForm"
            :aria-pressed="selectedColor === hex"
            class="w-10 h-10 rounded-full flex-shrink-0 border-2 transition-transform disabled:opacity-50"
            :class="selectedColor === hex
              ? 'ring-2 ring-offset-2 ring-offset-background ring-primary scale-100 border-transparent'
              : 'border-white/5 hover:scale-110 scale-90'"
            :style="{ backgroundColor: hex }"
            :aria-label="`Color ${idx + 1}`"
            @click="selectedColor = hex"
          />
        </div>
      </section>
    </template>

    <div
      class="fixed bottom-0 left-0 w-full p-4 pb-safe bg-gradient-to-t from-background from-80% to-transparent z-20 pointer-events-none"
    >
      <div class="max-w-2xl mx-auto pointer-events-auto">
        <AppButton
          type="submit"
          class="!w-full !py-4 !shadow-[0_8px_30px_rgba(5,230,153,0.2)]"
          variant="primary"
          :loading="isSaving"
          :disabled="!canEditForm"
          :icon="isNew ? 'add' : 'check_circle'"
        >{{ ctaLabel }}</AppButton>
      </div>
    </div>
  </form>
</template>

<style scoped>
.hide-scrollbar::-webkit-scrollbar {
  display: none;
}
.hide-scrollbar {
  -ms-overflow-style: none;
  scrollbar-width: none;
}
</style>

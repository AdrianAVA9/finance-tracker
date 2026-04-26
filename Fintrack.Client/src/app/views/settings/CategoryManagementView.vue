<script setup lang="ts">
import { computed, type Ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import LoadingIndicator from '@/app/components/common/LoadingIndicator.vue'
import SurfaceCard from '@/app/components/common/SurfaceCard.vue'
import { useUserOwnedCategoryList, type UserOwnedCategoryKind } from './composables/useUserOwnedCategoryList'

const route = useRoute()
const router = useRouter()

const kind = computed<UserOwnedCategoryKind>(() =>
  route.name === 'SettingsIncomeCategories' ? 'income' : 'expense'
)

const { isLoading, loadError, search, visibleIncome, expenseSections, load } = useUserOwnedCategoryList(
  kind as Ref<UserOwnedCategoryKind>
)

const newCategoryRoute = computed(() =>
  kind.value === 'income' ? 'CategoryIncomeNew' : 'CategoryExpenseNew'
)

function goToNewCategory() {
  void router.push({ name: newCategoryRoute.value })
}

function goToEdit(id: string) {
  if (kind.value === 'income') {
    void router.push({ name: 'CategoryIncomeEdit', params: { categoryId: id } })
  } else {
    void router.push({ name: 'CategoryExpenseEdit', params: { categoryId: id } })
  }
}
</script>

<template>
  <div class="space-y-6">
    <p v-if="route.meta.subtitle && typeof route.meta.subtitle === 'string'" class="text-sm text-on-surface-variant">
      {{ route.meta.subtitle }}
    </p>

    <div v-if="loadError" class="rounded-xl border border-error/30 bg-error-container/10 px-4 py-3 text-sm text-on-surface">
      {{ loadError }}
      <button type="button" class="ml-2 font-bold text-primary underline" @click="load()">Reintentar</button>
    </div>

    <div class="relative group">
      <div class="absolute inset-y-0 left-3 flex items-center pointer-events-none" aria-hidden="true">
        <span class="material-symbols-outlined text-outline text-lg">search</span>
      </div>
      <input
        v-model="search"
        type="search"
        class="w-full bg-surface-container-lowest border border-outline-variant/10 rounded-xl py-3.5 pl-11 pr-4 text-on-surface placeholder:text-outline focus:outline-none focus:ring-2 focus:ring-primary/40 transition-all"
        placeholder="Buscar categorías..."
        autocomplete="off"
      />
    </div>

    <LoadingIndicator :is-loading="isLoading" message="Cargando categorías" />

    <template v-if="!isLoading && !loadError && kind === 'income'">
      <ul v-if="visibleIncome.length > 0" class="space-y-3" role="list">
        <li v-for="item in visibleIncome" :key="item.id">
          <button
            type="button"
            class="w-full text-left rounded-2xl focus:outline-none focus-visible:ring-2 focus-visible:ring-primary/40"
            @click="goToEdit(item.id)"
          >
            <SurfaceCard>
            <div class="flex items-center gap-4 p-0 -m-1">
              <div
                class="w-12 h-12 rounded-lg flex items-center justify-center text-primary"
                :style="item.color ? { backgroundColor: `${item.color}22` } : undefined"
                :class="!item.color ? 'bg-surface-container-highest' : ''"
              >
                <span v-if="item.icon" class="material-symbols-outlined">{{ item.icon }}</span>
                <span v-else class="material-symbols-outlined">account_balance_wallet</span>
              </div>
              <div class="min-w-0 flex-1">
                <p class="font-body font-semibold text-on-surface truncate">{{ item.name }}</p>
              </div>
            </div>
            </SurfaceCard>
          </button>
        </li>
      </ul>
      <p v-else class="text-center text-sm text-on-surface-variant py-8">No tenés categorías de ingresos propias aún.</p>
    </template>

    <template v-if="!isLoading && !loadError && kind === 'expense'">
      <div v-if="expenseSections.length > 0" class="space-y-10" role="list">
        <section v-for="section in expenseSections" :key="section.groupLabel" class="space-y-3">
          <div class="flex items-center justify-between px-1">
            <h2 class="font-headline font-bold text-lg tracking-tight text-on-surface-variant">
              {{ section.groupLabel }}
            </h2>
            <span class="text-xs font-label uppercase tracking-widest text-outline"
              >{{ section.items.length }} {{ section.items.length === 1 ? 'ítem' : 'ítems' }}</span
            >
          </div>
          <ul class="space-y-3" role="list">
            <li v-for="item in section.items" :key="item.id">
              <button
                type="button"
                class="w-full text-left rounded-2xl focus:outline-none focus-visible:ring-2 focus-visible:ring-primary/40"
                @click="goToEdit(item.id)"
              >
                <SurfaceCard>
                <div class="flex items-center gap-4 p-0 -m-1">
                  <div
                    class="w-12 h-12 rounded-lg flex items-center justify-center bg-surface-container transition-colors group-hover:bg-surface-container-high"
                    :style="item.color ? { color: item.color } : undefined"
                  >
                    <span
                      v-if="item.icon"
                      class="material-symbols-outlined"
                      :style="item.color ? { color: item.color } : undefined"
                      >{{ item.icon }}</span
                    >
                    <span v-else class="material-symbols-outlined text-primary">category</span>
                  </div>
                  <div class="min-w-0 flex-1">
                    <p class="font-body font-semibold text-on-surface truncate">{{ item.name }}</p>
                    <p v-if="item.description" class="text-xs text-outline truncate">{{ item.description }}</p>
                  </div>
                </div>
                </SurfaceCard>
              </button>
            </li>
          </ul>
        </section>
      </div>
      <p v-else class="text-center text-sm text-on-surface-variant py-8">No tenés categorías de gastos propias aún.</p>
    </template>

    <div class="h-20 sm:hidden" aria-hidden="true" />

    <button
      type="button"
      class="fixed bottom-32 right-8 w-14 h-14 flex items-center justify-center rounded-full bg-primary-container text-on-primary-container shadow-2xl shadow-primary-container/40 z-[100] hover:scale-110 active:scale-90 transition-all duration-300"
      aria-label="Registrar categoría"
      @click="goToNewCategory"
    >
      <span class="material-symbols-outlined text-3xl font-black">add</span>
    </button>
  </div>
</template>

<style scoped>
.material-symbols-outlined {
  font-variation-settings: 'FILL' 0, 'wght' 500, 'GRAD' 0, 'opsz' 24;
}
</style>

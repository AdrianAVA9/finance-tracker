<script setup lang="ts">
import { nextTick, ref, watch } from 'vue'

const props = defineProps<{
  isSearchOpen: boolean
  searchQuery: string
}>()

const emit = defineEmits<{
  (event: 'toggle-search'): void
  (event: 'update:search-query', value: string): void
}>()

const searchInput = ref<HTMLInputElement | null>(null)

const updateSearchQuery = (event: Event) => {
  emit('update:search-query', (event.target as HTMLInputElement).value)
}

watch(
  () => props.isSearchOpen,
  async (isOpen) => {
    if (!isOpen) return
    await nextTick()
    searchInput.value?.focus()
  },
)
</script>

<template>
  <div class="space-y-6">
    <header class="flex items-center justify-between px-1">
      <div class="space-y-1">
        <h1 class="font-headline text-3xl font-extrabold tracking-tighter text-on-surface">Actividad</h1>
        <p class="text-[10px] font-bold text-on-surface-variant uppercase tracking-[0.2em]">
          CEROBASE • Registro Central
        </p>
      </div>
      <button
        class="group flex h-12 w-12 items-center justify-center rounded-xl transition-all active:scale-95"
        :class="
          isSearchOpen
            ? 'bg-primary-container text-on-primary'
            : 'bg-surface-container-high text-on-surface-variant hover:bg-surface-variant'
        "
        @click="emit('toggle-search')"
      >
        <span class="material-symbols-outlined transition-transform group-hover:scale-110">
          {{ isSearchOpen ? 'close' : 'search' }}
        </span>
      </button>
    </header>

    <Transition
      enter-active-class="transition duration-300 ease-out"
      enter-from-class="opacity-0 -translate-y-4"
      enter-to-class="opacity-100 translate-y-0"
      leave-active-class="transition duration-200 ease-in"
      leave-from-class="opacity-100 translate-y-0"
      leave-to-class="opacity-0 -translate-y-4"
    >
      <div v-if="isSearchOpen" class="px-1">
        <div class="group relative">
          <span
            class="material-symbols-outlined absolute left-4 top-1/2 -translate-y-1/2 text-sm text-on-surface-variant transition-colors group-focus-within:text-[#05E699]"
          >
            search
          </span>
          <input
            ref="searchInput"
            :value="searchQuery"
            class="w-full rounded-xl border-none bg-surface-container-lowest py-4 pl-12 pr-4 font-headline text-sm font-bold shadow-xl placeholder:text-on-surface-variant/20 focus:ring-1 focus:ring-[#05E699]/40"
            placeholder="Buscar transacciones..."
            type="text"
            @input="updateSearchQuery"
          />
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, Transition } from 'vue'
import type { CSSProperties } from 'vue'

interface Category {
  id: string | number
  name: string
  icon?: string | null
  color?: string | null
  group?: {
    name: string
  }
}

const props = defineProps<{
  modelValue: number | string | null
  categories: Category[]
  label?: string
  placeholder?: string
  disabled?: boolean
}>()

const emit = defineEmits(['update:modelValue'])

const isOpen = ref(false)
const searchQuery = ref('')

const selectedCategory = computed(() => 
  props.categories.find(c => c.id === props.modelValue)
)

const filteredAndGroupedCategories = computed(() => {
  const query = searchQuery.value.toLowerCase().trim()
  
  // First, filter by search query
  const filtered = props.categories.filter(c => 
    c.name.toLowerCase().includes(query) || 
    c.group?.name.toLowerCase().includes(query)
  )

  // Then, group them
  const groups: Record<string, Category[]> = {}
  
  filtered.forEach(cat => {
    const groupName = cat.group?.name || 'Otras Categorías'
    if (!groups[groupName]) groups[groupName] = []
    groups[groupName].push(cat)
  })

  // Sort groups alphabetically (except 'Otras Categorías' usually at the end)
  return Object.keys(groups).sort((a, b) => {
    if (a === 'Otras Categorías') return 1
    if (b === 'Otras Categorías') return -1
    return a.localeCompare(b)
  }).map(name => ({
    name,
    categories: groups[name]
  }))
})

const hasGroups = computed(() => 
  props.categories.some(c => !!c.group?.name)
)

const toColorStyle = (color?: string | null): CSSProperties | undefined =>
  color ? { color } : undefined

const selectCategory = (id: number | string) => {
  emit('update:modelValue', id)
  close()
}

const open = () => {
  if (props.disabled) return
  isOpen.value = true
  searchQuery.value = ''
  // Prevent body scroll
  document.body.style.overflow = 'hidden'
}

const close = () => {
  isOpen.value = false
  // Restore body scroll
  document.body.style.overflow = ''
}
</script>

<template>
  <div class="relative w-full">
    <!-- Trigger UI (Input-like) -->
    <div 
      @click="open"
      class="group w-full bg-surface-container p-5 rounded-xl border border-white/[0.03] transition-all duration-300 cursor-pointer flex items-center justify-between"
      :class="[
        disabled ? 'opacity-50 cursor-not-allowed' : 'hover:bg-surface-container-high active:scale-[0.99]',
        isOpen ? 'ring-2 ring-primary-container/20 border-primary-container/30' : ''
      ]"
    >
      <div class="flex items-center gap-4">
        <div 
          v-if="selectedCategory"
          class="w-10 h-10 rounded-lg bg-primary-container/10 flex items-center justify-center text-primary-container"
          :style="toColorStyle(selectedCategory?.color)"
        >
          <span class="material-symbols-outlined text-2xl">{{ selectedCategory.icon || 'category' }}</span>
        </div>
        <div 
          v-else
          class="w-10 h-10 rounded-lg bg-surface-container-highest flex items-center justify-center text-on-surface-variant/40"
        >
          <span class="material-symbols-outlined text-2xl">category</span>
        </div>
        
        <div class="flex flex-col">
          <span 
            class="text-sm font-bold transition-colors"
            :class="selectedCategory ? 'text-on-surface' : 'text-on-surface-variant/60'"
          >
            {{ selectedCategory ? selectedCategory.name : (placeholder || 'Selecciona Categoría') }}
          </span>
          <span v-if="selectedCategory?.group?.name" class="text-[9px] font-black uppercase tracking-widest text-on-surface-variant/40">
            {{ selectedCategory.group.name }}
          </span>
        </div>
      </div>
      
      <span class="material-symbols-outlined text-on-surface-variant group-hover:text-primary-container transition-colors font-light">
        unfold_more
      </span>
    </div>

    <!-- Selection Overlay (Full Screen) -->
    <Teleport to="body">
      <Transition
        enter-active-class="transition duration-400 ease-out"
        enter-from-class="opacity-0 translate-y-10"
        enter-to-class="opacity-100 translate-y-0"
        leave-active-class="transition duration-300 ease-in"
        leave-from-class="opacity-100 translate-y-0"
        leave-to-class="opacity-0 translate-y-10"
      >
        <div v-if="isOpen" class="fixed inset-0 z-[200] flex flex-col bg-surface-dim/95 backdrop-blur-2xl">
          <!-- Overlay Header -->
          <header class="flex items-center justify-between px-6 pt-16 pb-6">
            <div class="space-y-1">
              <h2 class="font-headline text-2xl font-black tracking-tighter text-on-surface">Seleccionar Categoría</h2>
              <p class="text-[10px] font-bold text-on-surface-variant uppercase tracking-[0.2em]"></p>
            </div>
            <button 
              @click="close"
              class="w-12 h-12 rounded-xl bg-surface-container-high flex items-center justify-center text-on-surface hover:bg-surface-variant transition-colors"
            >
              <span class="material-symbols-outlined">close</span>
            </button>
          </header>

          <!-- Search Section -->
          <div class="px-6 mb-8">
            <div class="relative group">
              <div class="absolute inset-y-0 left-5 flex items-center pointer-events-none">
                <span class="material-symbols-outlined text-primary-container group-focus-within:scale-110 transition-transform">search</span>
              </div>
              <input 
                v-model="searchQuery"
                type="text"
                class="w-full h-16 pl-14 pr-6 bg-surface-container-low border-none rounded-xl focus:ring-2 focus:ring-primary-container/20 text-on-surface font-body font-semibold placeholder:text-on-surface-variant/30 transition-all shadow-xl"
                placeholder="Buscar por nombre o grupo..."
                autofocus
              />
            </div>
          </div>

          <!-- Categories List -->
          <div class="flex-1 overflow-y-auto px-6 pb-20 no-scrollbar">
            <div class="space-y-10">
              <div v-for="group in filteredAndGroupedCategories" :key="group.name" class="space-y-4">
                <!-- Group Header (Only if multiple groups exist or strictly requested) -->
                <div v-if="hasGroups" class="flex items-center gap-4 px-1">
                  <h3 class="font-headline text-[10px] font-black uppercase tracking-[0.3em] text-on-surface-variant whitespace-nowrap">
                    {{ group.name }}
                  </h3>
                  <div class="h-px w-full bg-white/[0.03]"></div>
                </div>

                <div class="bg-surface-container-low rounded-xl overflow-hidden border border-white/[0.02] shadow-sm">
                  <button 
                    v-for="cat in group.categories" 
                    :key="cat.id"
                    @click="selectCategory(cat.id)"
                    class="w-full flex items-center gap-5 p-5 hover:bg-surface-container-high transition-all active:bg-primary-container/5 group/item text-left border-b border-white/[0.02] last:border-0"
                  >
                    <div 
                      class="w-11 h-11 rounded-xl bg-surface-container-high flex items-center justify-center text-primary-container group-hover/item:scale-110 group-hover/item:bg-primary-container/10 transition-all duration-300"
                      :style="toColorStyle(cat.color)"
                    >
                      <span class="material-symbols-outlined text-2xl">{{ cat.icon || 'category' }}</span>
                    </div>
                    <div class="flex flex-col flex-1">
                      <span class="text-sm font-bold text-on-surface group-hover/item:text-primary-container transition-colors">{{ cat.name }}</span>
                    </div>
                    <span 
                      v-if="modelValue === cat.id" 
                      class="material-symbols-outlined text-primary-container animate-in fade-in zoom-in duration-300"
                    >
                      check_circle
                    </span>
                    <span 
                      v-else
                      class="material-symbols-outlined text-on-surface-variant/20 opacity-0 group-hover/item:opacity-100 transition-opacity"
                    >
                      chevron_right
                    </span>
                  </button>
                </div>
              </div>

              <!-- Empty State -->
              <div v-if="filteredAndGroupedCategories.length === 0" class="py-20 flex flex-col items-center justify-center text-center space-y-4">
                <div class="w-20 h-20 rounded-full bg-surface-container-low flex items-center justify-center text-on-surface-variant/20">
                  <span class="material-symbols-outlined text-5xl">search_off</span>
                </div>
                <div class="space-y-1">
                  <h4 class="text-sm font-bold text-on-surface">No se encontraron categorías</h4>
                  <p class="text-[10px] font-black uppercase tracking-widest text-on-surface-variant/40">Intenta con otro término de búsqueda</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </Transition>
    </Teleport>
  </div>
</template>

<style scoped>
.no-scrollbar::-webkit-scrollbar {
  display: none;
}
.no-scrollbar {
  -ms-overflow-style: none;
  scrollbar-width: none;
}

/* Pulse animation for active selection */
@keyframes check-pulse {
  0% { transform: scale(0.8); opacity: 0; }
  50% { transform: scale(1.1); opacity: 1; }
  100% { transform: scale(1); opacity: 1; }
}

.animate-in {
  animation: check-pulse 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}
</style>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'

const props = defineProps<{
  month: number
  year: number
}>()

const emit = defineEmits(['update:month', 'update:year'])

const isOpen = ref(false)
const tempYear = ref(props.year)
const tempMonth = ref(props.month)

// Synchronize with props when opening
watch(isOpen, (val) => {
  if (val) {
    tempYear.value = props.year
    tempMonth.value = props.month
    document.body.style.overflow = 'hidden'
  } else {
    document.body.style.overflow = ''
  }
})

const months = computed(() => {
  const locale = navigator.language || 'es-ES'
  return Array.from({ length: 12 }, (_, i) => {
    const monthId = i + 1
    const name = new Intl.DateTimeFormat(locale, { month: 'long' }).format(new Date(2000, i))
    return { id: monthId, name: name.charAt(0).toUpperCase() + name.slice(1) }
  })
})

const currentSelectionLabel = computed(() => {
  const locale = navigator.language || 'es-ES'
  const monthName = new Intl.DateTimeFormat(locale, { month: 'long' }).format(new Date(tempYear.value, props.month - 1))
  return `${monthName.charAt(0).toUpperCase() + monthName.slice(1)} ${props.year}`
})

const changeYear = (delta: number) => {
  tempYear.value += delta
}

const selectMonth = (mId: number) => {
  tempMonth.value = mId
}

const confirmSelection = () => {
  emit('update:month', tempMonth.value)
  emit('update:year', tempYear.value)
  isOpen.value = false
}

const close = () => {
  isOpen.value = false
}
</script>

<template>
  <div class="relative">
    <!-- Trigger Button -->
    <button 
      @click="isOpen = true"
      class="flex items-center gap-3 bg-surface-container-low px-5 py-3 rounded-xl hover:bg-surface-container active:scale-95 transition-all border border-white/[0.02] shadow-lg group"
    >
      <span class="material-symbols-outlined text-[18px] text-[#05E699] group-hover:scale-110 transition-transform">calendar_today</span>
      <span class="text-sm font-black font-headline text-on-surface tracking-tight">{{ currentSelectionLabel }}</span>
      <span class="material-symbols-outlined text-[18px] text-on-surface-variant/40 group-hover:text-on-surface transition-colors">expand_more</span>
    </button>

    <!-- Overlay Selection (Teleport to Body) -->
    <Teleport to="body">
      <Transition
        enter-active-class="transition duration-300 ease-out"
        enter-from-class="opacity-0"
        enter-to-class="opacity-100"
        leave-active-class="transition duration-200 ease-in"
        leave-from-class="opacity-100"
        leave-to-class="opacity-0"
      >
        <div v-if="isOpen" class="fixed inset-0 z-[110] flex items-end justify-center p-0 sm:p-4">
          <!-- Backdrop -->
          <div class="absolute inset-0 bg-black/70 backdrop-blur-md" @click="close"></div>
          
          <!-- Sheet -->
          <Transition
            enter-active-class="transition duration-500 cubic-bezier(0.175, 0.885, 0.32, 1.275)"
            enter-from-class="translate-y-full"
            enter-to-class="translate-y-0"
            leave-active-class="transition duration-300 ease-in"
            leave-from-class="translate-y-0"
            leave-to-class="translate-y-full"
            appear
          >
            <div 
              class="relative w-full max-w-md bg-surface-container-low rounded-t-2xl sm:rounded-2xl p-8 pointer-events-auto shadow-2xl border border-white/[0.02]"
              @click.stop
            >
              <div class="w-12 h-1.5 bg-on-surface-variant/10 rounded-full mx-auto mb-8"></div>
              
              <!-- Year Selector Header -->
              <div class="flex items-center justify-between mb-8">
                <button 
                  @click="changeYear(-1)"
                  class="w-10 h-10 flex items-center justify-center rounded-lg bg-surface-container hover:bg-surface-variant transition-colors text-on-surface-variant"
                >
                  <span class="material-symbols-outlined">chevron_left</span>
                </button>
                
                <div class="text-center group">
                  <h2 class="text-xs font-black text-[#05E699] uppercase tracking-[0.3em] mb-1">Seleccionar Periodo</h2>
                  <div class="text-3xl font-headline font-black text-on-surface tracking-tighter">{{ tempYear }}</div>
                </div>

                <button 
                  @click="changeYear(1)"
                  class="w-10 h-10 flex items-center justify-center rounded-lg bg-surface-container hover:bg-surface-variant transition-colors text-on-surface-variant"
                >
                  <span class="material-symbols-outlined">chevron_right</span>
                </button>
              </div>

              <!-- Months Grid -->
              <div class="grid grid-cols-3 gap-3 mb-10">
                <button 
                  v-for="m in months" 
                  :key="m.id"
                  @click="selectMonth(m.id)"
                  class="py-3 px-2 rounded-lg text-[11px] font-black uppercase tracking-widest transition-all duration-300"
                  :class="tempMonth === m.id 
                    ? 'bg-[#05E699] text-[#003822] shadow-lg shadow-[#05E699]/20 transform scale-105' 
                    : 'bg-surface-container text-on-surface-variant/60 hover:text-on-surface hover:bg-surface-container-highest'"
                >
                  {{ m.name }}
                </button>
              </div>

              <!-- Action Confirmation -->
              <button 
                @click="confirmSelection"
                class="w-full bg-[#05E699] py-3 rounded-xl text-[#003822] font-headline font-black text-[10px] uppercase tracking-[0.2em] shadow-xl shadow-[#05E699]/10 transition-all active:scale-95 flex items-center justify-center gap-3"
              >
                <span class="material-symbols-outlined text-xl">check_circle</span>
                Confirmar Selección
              </button>
            </div>
          </Transition>
        </div>
      </Transition>
    </Teleport>
  </div>
</template>

<style scoped>
.font-headline {
  font-family: 'Manrope', sans-serif;
}
.font-black {
  font-weight: 800;
}

/* Modal scale animation helper */
.scale-enter-active, .scale-leave-active {
  transition: transform 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275), opacity 0.3s ease;
}
.scale-enter-from, .scale-leave-to {
  transform: scale(0.9) translateY(20px);
  opacity: 0;
}
</style>

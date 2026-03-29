<script setup lang="ts">
import { ref, watch, onMounted, onUnmounted } from 'vue'

interface Props {
  show: boolean
  title: string
  description: string
  confirmText?: string
  cancelText?: string
  isLoading?: boolean
  variant?: 'danger' | 'primary'
}

const props = withDefaults(defineProps<Props>(), {
  confirmText: 'Confirmar',
  cancelText: 'Cancelar',
  isLoading: false,
  variant: 'danger'
})

const emit = defineEmits(['confirm', 'cancel', 'close'])

// Handle ESC key
const handleEsc = (e: KeyboardEvent) => {
  if (e.key === 'Escape' && props.show) {
    emit('cancel')
  }
}

onMounted(() => window.addEventListener('keydown', handleEsc))
onUnmounted(() => window.removeEventListener('keydown', handleEsc))

// Prevent scroll when open
watch(() => props.show, (val) => {
  if (val) {
    document.body.style.overflow = 'hidden'
  } else {
    document.body.style.overflow = ''
  }
})
</script>

<template>
  <Teleport to="body">
    <Transition
      enter-active-class="transition duration-300 ease-out"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition duration-200 ease-in"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div v-if="show" class="fixed inset-0 z-[100] bg-[#111317]/80 backdrop-blur-md flex items-end md:items-center justify-center p-0 md:p-6" @click="emit('cancel')">
        
        <Transition
          enter-active-class="transition duration-500 cubic-bezier(0.32, 0.72, 0, 1)"
          enter-from-class="translate-y-full md:translate-y-10 md:scale-95 md:opacity-0"
          enter-to-class="translate-y-0 md:translate-y-0 md:scale-100 md:opacity-100"
          leave-active-class="transition duration-300 ease-in"
          leave-from-class="translate-y-0 md:translate-y-0 md:scale-100"
          leave-to-class="translate-y-full md:translate-y-10 md:scale-95 md:opacity-0"
          appear
        >
          <div 
            v-if="show"
            class="w-full max-w-lg bg-[#1e2024] rounded-t-3xl md:rounded-2xl p-8 luminous-shadow border-t border-white/[0.05] md:border"
            @click.stop
          >
            <!-- Mobile Handle -->
            <div class="w-12 h-1 bg-white/10 rounded-full mx-auto mb-8 md:hidden"></div>

            <div class="flex flex-col items-center text-center">
              <!-- Icon Header -->
              <div 
                class="mb-6 w-16 h-16 rounded-full flex items-center justify-center"
                :class="variant === 'danger' ? 'bg-red-500/20' : 'bg-[#05E699]/20'"
              >
                <span 
                  class="material-symbols-outlined !text-4xl"
                  :class="variant === 'danger' ? 'text-[#FF4D4D]' : 'text-[#05E699]'"
                  style="font-variation-settings: 'FILL' 1;"
                >
                  {{ variant === 'danger' ? 'warning' : 'check_circle' }}
                </span>
              </div>

              <!-- Content -->
              <h2 class="text-2xl font-black font-headline text-on-surface tracking-tighter mb-3">{{ title }}</h2>
              <p class="text-on-surface-variant font-body text-xs leading-relaxed mb-10 max-w-sm font-semibold opacity-80">
                {{ description }}
              </p>

              <!-- Actions -->
              <div class="w-full space-y-3">
                <button 
                  @click="emit('confirm')"
                  :disabled="isLoading"
                  class="w-full py-5 font-headline font-black text-xs uppercase tracking-[0.2em] rounded-xl active:scale-95 transition-all duration-200 disabled:opacity-50 flex items-center justify-center gap-3"
                  :class="variant === 'danger' ? 'bg-[#FF4D4D] text-white shadow-xl shadow-red-500/20' : 'bg-[#05E699] text-[#003822] shadow-xl shadow-[#05E699]/20'"
                >
                  <span v-if="isLoading" class="material-symbols-outlined animate-spin shadow-none">sync</span>
                  {{ confirmText }}
                </button>
                <button 
                  @click="emit('cancel')"
                  class="w-full py-5 bg-transparent text-on-surface font-headline font-bold text-xs uppercase tracking-[0.2em] border border-white/5 rounded-xl hover:bg-white/[0.03] active:scale-95 transition-all duration-200"
                >
                  {{ cancelText }}
                </button>
              </div>
            </div>

            <!-- Optional Preview Slot -->
            <div v-if="$slots['item-preview']" class="mt-8 pt-8 border-t border-white/[0.03]">
              <slot name="item-preview"></slot>
            </div>
          </div>
        </Transition>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.luminous-shadow {
  box-shadow: 0 -20px 80px -20px rgba(0, 0, 0, 0.6);
}
</style>

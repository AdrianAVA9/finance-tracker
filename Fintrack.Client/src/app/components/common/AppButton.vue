<script setup lang="ts">
import { computed } from 'vue'

const props = withDefaults(
  defineProps<{
    /** Visual style: primary CTA, neutral secondary, or destructive text-style */
    variant?: 'primary' | 'secondary' | 'danger'
    /** Native button type */
    type?: 'button' | 'submit' | 'reset'
    disabled?: boolean
    /** Shows spinner and disables interaction */
    loading?: boolean
    /** Material Symbols icon name (leading); ignored if `#icon` slot is used */
    icon?: string
    /** Full-width row button (typical for mobile forms) */
    fullWidth?: boolean
  }>(),
  {
    variant: 'primary',
    type: 'button',
    fullWidth: true,
  },
)

const variantClass = computed(() => {
  switch (props.variant) {
    case 'secondary':
      return 'bg-surface-container-high text-on-surface font-bold hover:bg-surface-variant'
    case 'danger':
      return 'bg-transparent text-red-500 font-bold hover:bg-red-500/5'
    case 'primary':
    default:
      return 'bg-primary-container text-on-primary-container font-black hover:brightness-110 shadow-xl shadow-primary-container/20'
  }
})

const rootClass = computed(() => [
  'inline-flex items-center justify-center gap-3 rounded-xl py-5 font-headline transition-all active:scale-[0.98] disabled:opacity-50 disabled:pointer-events-none',
  props.fullWidth && 'w-full',
  variantClass.value,
])
</script>

<template>
  <button
    :type="type"
    :disabled="disabled || loading"
    :aria-busy="loading || undefined"
    :class="rootClass"
  >
    <span v-if="loading" class="material-symbols-outlined animate-spin" aria-hidden="true">sync</span>
    <slot v-else-if="$slots.icon" name="icon" />
    <span v-else-if="icon" class="material-symbols-outlined" aria-hidden="true">{{ icon }}</span>
    <slot />
  </button>
</template>

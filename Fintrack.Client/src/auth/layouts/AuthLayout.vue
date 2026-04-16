<script setup lang="ts">
import { RouterView } from 'vue-router'
import SessionBootstrapOverlay from '@/shared/ui/SessionBootstrapOverlay.vue'
import { useSessionBootstrap } from '@/composables/useSessionBootstrap'

const { isInitialized } = useSessionBootstrap()
</script>

<template>
  <SessionBootstrapOverlay v-if="!isInitialized" message="Cargando pagina..." />
  <RouterView v-else />
</template>

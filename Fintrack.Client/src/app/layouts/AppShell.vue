<script setup lang="ts">
import { RouterView } from 'vue-router'
import AppOfflineGate from '@/app/layouts/AppOfflineGate.vue'
import SessionBootstrapOverlay from '@/shared/ui/SessionBootstrapOverlay.vue'
import { useSessionBootstrap } from '@/composables/useSessionBootstrap'

const { isInitialized } = useSessionBootstrap()
</script>

<template>
  <SessionBootstrapOverlay v-if="!isInitialized" />
  <AppOfflineGate>
    <RouterView />
  </AppOfflineGate>
</template>

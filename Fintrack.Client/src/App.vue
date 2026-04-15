<script setup lang="ts">
import AppSessionLoadingOverlay from '@/app/components/common/AppSessionLoadingOverlay.vue'
import { useAppShell } from '@/composables/useAppShell'
import OfflineView from '@/shared/views/OfflineView.vue'
import ReloadPrompt from '@/shared/ui/ReloadPrompt.vue'

const { isInitialized, showOfflinePage } = useAppShell()
</script>

<template>
  <Transition name="fade">
    <AppSessionLoadingOverlay v-if="!isInitialized" />
    <div v-else class="h-full">
      <OfflineView v-if="showOfflinePage" />
      <router-view v-else />
      <ReloadPrompt />
    </div>
  </Transition>
</template>

<style>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.5s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>

<script setup lang="ts">
import OfflineView from '@/app/views/OfflineView.vue'
import { useAppOfflineFullPage } from '@/composables/useAppOfflineFullPage'

const { showOfflinePage } = useAppOfflineFullPage()
</script>

<template>
  <div class="contents">
    <OfflineView v-if="showOfflinePage" />
    <slot v-else />
  </div>
</template>

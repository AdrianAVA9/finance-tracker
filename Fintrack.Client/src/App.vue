<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import { useRouter } from 'vue-router';
import { useAuth } from '@/composables/useAuth';
import { useNetworkStatus } from '@/shared/composables/useNetworkStatus';
import OfflineView from '@/shared/views/OfflineView.vue';
import ReloadPrompt from '@/shared/ui/ReloadPrompt.vue';

const { isInitialized, checkSession } = useAuth();
const { isOnline } = useNetworkStatus();
const router = useRouter();
const showOfflinePage = ref(false);

onMounted(async () => {
    if (!isInitialized.value) {
        await checkSession();
    }
});

// Logic to show OfflineView when navigating while offline
watch(() => router.currentRoute.value.path, () => {
    if (!isOnline.value) {
        showOfflinePage.value = true;
    }
});

// Reset offline view when coming back online
watch(isOnline, (online) => {
    if (online) {
        showOfflinePage.value = false;
    }
});

// Re-evaluate routing once session is established
watch(isInitialized, (initialized) => {
    if (initialized) {
        const { isAuthenticated } = useAuth();
        const currentPath = router.currentRoute.value.path;
        
        const isAuthDomain = currentPath.startsWith('/auth');
        const isAppDomain = currentPath.startsWith('/app');
        const isPublicDomain = !isAuthDomain && !isAppDomain && !currentPath.startsWith('/admin');

        // Force the same logic as router/index.ts since replace(currentPath) might be a no-op
        if ((isPublicDomain || isAuthDomain) && isAuthenticated.value) {
            router.replace('/app');
        } else if (isAppDomain && !isAuthenticated.value) {
            router.replace('/auth/login');
        }
    }
});
</script>

<template>
  <Transition name="fade">
    <div v-if="!isInitialized" 
         class="fixed inset-0 z-[9999] flex flex-col items-center justify-center bg-[#0f172a] text-white">
        <div class="flex flex-col items-center gap-6">
            <div class="w-20 h-20 rounded-3xl bg-primary/10 flex items-center justify-center border border-primary/20 animate-pulse">
                <span class="material-symbols-outlined text-primary text-5xl">wallet</span>
            </div>
            <div class="flex flex-col items-center gap-1">
                <h1 class="text-2xl font-bold tracking-tight">CeroBase</h1>
                <p class="text-slate-400 text-sm animate-pulse">Sincronizando tus finanzas...</p>
            </div>
            <div class="flex gap-1.5 mt-2">
                <div class="w-2 h-2 rounded-full bg-primary animate-bounce [animation-delay:-0.3s]"></div>
                <div class="w-2 h-2 rounded-full bg-primary animate-bounce [animation-delay:-0.15s]"></div>
                <div class="w-2 h-2 rounded-full bg-primary animate-bounce"></div>
            </div>
        </div>
    </div>
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

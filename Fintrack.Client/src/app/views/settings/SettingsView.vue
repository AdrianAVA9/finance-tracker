<script setup lang="ts">
import { computed } from 'vue';
import { useAuth } from '@/composables/useAuth';
import AppButton from '@/app/components/common/AppButton.vue';
import SettingsAccountSection from '@/app/components/settings/SettingsAccountSection.vue';
import SettingsCategoriesSection from '@/app/components/settings/SettingsCategoriesSection.vue';
import SettingsPreferencesSection from '@/app/components/settings/SettingsPreferencesSection.vue';
import clientPackage from '../../../../package.json';

const { user, logout } = useAuth();

const emailLine = computed(() => (typeof user.value === 'string' && user.value ? user.value : '—'));

const displayName = computed(() => {
  const u = user.value;
  if (!u) return 'Usuario';
  if (u.includes('@')) {
    const local = (u.split('@')[0] ?? u).trim();
    if (!local) return u;
    return local
      .split(/[._-]/)
      .map((p) => p.charAt(0).toUpperCase() + p.slice(1).toLowerCase())
      .join(' ');
  }
  return u;
});

const appVersion = clientPackage.version as string;
const appLabel = 'CEROBASE';

async function handleLogout() {
  await logout();
}
</script>

<template>
  <div class="space-y-10 pb-10">
    <SettingsAccountSection
      :display-name="displayName"
      :email="emailLine"
    />
    <SettingsCategoriesSection />
    <SettingsPreferencesSection />
    <section class="pt-2">
      <AppButton
        variant="danger"
        type="button"
        icon="logout"
        class="!bg-surface-container-lowest !border !border-error-container/30 !text-cero-danger hover:!bg-error-container/10"
        @click="handleLogout"
      >
        Cerrar sesión
      </AppButton>
    </section>
    <p
      class="text-center text-[10px] text-on-surface-variant font-bold uppercase tracking-[0.3em] opacity-40"
    >
      {{ appLabel }} v{{ appVersion }}
    </p>
  </div>
</template>

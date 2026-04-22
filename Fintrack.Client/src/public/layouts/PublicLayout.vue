<script setup lang="ts">
import { onMounted } from 'vue';
import { useRouter } from 'vue-router';
import logo from '@/assets/logo.png';
import { useAuth } from '@/composables/useAuth';

const router = useRouter();
const { isInitialized, isAuthenticated, checkSession } = useAuth();

onMounted(async () => {
  if (!isInitialized.value) {
    await checkSession();
  }

  if (isAuthenticated.value) {
    await router.replace('/app');
  }
});
</script>

<template>
  <div class="public-layout">
    <header class="glass" style="display: none;">
      <nav class="container">
        <div class="logo">
          <img :src="logo" alt="CeroBase Logo" class="logo-img" />
          <span>CeroBase</span>
        </div>
        <div class="links">
          <router-link to="/">Home</router-link>
          <router-link to="/pricing">Pricing</router-link>
          <router-link to="/auth/login" class="btn btn-primary">Login</router-link>
        </div>
      </nav>
    </header>

    <main>
      <router-view />
    </main>

    <footer>
      <p>&copy; 2026 CeroBase. All rights reserved.</p>
    </footer>
  </div>
</template>

<style scoped lang="scss">
.public-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

header {
  padding: 1rem 0;
  position: sticky;
  top: 0;
  z-index: 10;

  .container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 0 1rem;
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .logo {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    font-weight: 700;
    font-size: 1.5rem;
    color: hsl(var(--color-primary));

    .logo-img {
      width: 2.5rem;
      height: 2.5rem;
      border-radius: 0.5rem;
      object-fit: cover;
    }
  }

  .links {
    display: flex;
    gap: 1.5rem;
    align-items: center;

    a {
      text-decoration: none;
      color: inherit;
      font-weight: 500;

      &:hover {
        color: hsl(var(--color-primary));
      }

      &.btn-primary {
        background: hsl(var(--color-primary));
        color: white;
        padding: 0.5rem 1rem;
        border-radius: 0.5rem;
        transition: opacity 0.2s;

        &:hover {
          opacity: 0.9;
          color: white;
        }
      }
    }
  }
}

main {
  flex: 1;
}

footer {
  text-align: center;
  padding: 2rem;
  color: hsla(220, 15%, 20%, 0.6);
}
</style>

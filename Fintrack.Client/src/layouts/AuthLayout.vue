<template>
  <div class="auth-layout">
    <div class="auth-card glass">
      <div class="logo">Fintrack</div>
      <router-view />
    </div>
  </div>
</template>

<style scoped lang="scss">
.auth-layout {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: radial-gradient(circle at top right, hsla(var(--color-primary), 0.1), transparent 40%),
              radial-gradient(circle at bottom left, hsla(var(--color-secondary), 0.1), transparent 40%);
}

.auth-card {
  width: 100%;
  max-width: 400px;
  padding: 2rem;
  border-radius: 1rem;
  box-shadow: var(--shadow-lg);

  .logo {
    text-align: center;
    font-weight: 800;
    font-size: 2rem;
    margin-bottom: 2rem;
    background: linear-gradient(135deg, hsl(var(--color-primary)), hsl(var(--color-secondary)));
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
  }
}
</style>

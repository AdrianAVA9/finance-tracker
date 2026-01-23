<template>
  <div class="app-layout">
    <aside class="sidebar glass-dark">
      <div class="logo">Fintrack</div>
      <nav>
        <router-link to="/dashboard">Dashboard</router-link>
        <router-link to="/expenses">Exepenses</router-link>
        <router-link to="/budgets">Budgets</router-link>
      </nav>
    </aside>
    
    <div class="main-content">
      <header class="topbar glass">
        <h2>Dashboard</h2>
        <div class="user-menu">Adrian V.</div>
      </header>
      <main>
        <router-view />
      </main>
    </div>
  </div>
</template>

<style scoped lang="scss">
.app-layout {
  display: flex;
  min-height: 100vh;
}

.sidebar {
  width: 250px;
  display: flex;
  flex-direction: column;
  padding: 1.5rem;
  box-shadow: var(--shadow-lg);
  z-index: 20;

  .logo {
    font-size: 1.5rem;
    font-weight: 700;
    margin-bottom: 3rem;
    color: white;
  }

  nav {
    display: flex;
    flex-direction: column;
    gap: 1rem;

    a {
      color: hsla(220, 20%, 80%, 0.8);
      text-decoration: none;
      padding: 0.75rem 1rem;
      border-radius: 0.5rem;
      transition: all 0.2s;

      &:hover, &.router-link-active {
        color: white;
        background: hsla(var(--color-primary), 0.2);
      }
    }
  }
}

.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.topbar {
  height: 64px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 2rem;
  background: hsla(var(--color-surface), 0.8);
}

main {
  padding: 2rem;
  flex: 1;
  overflow-y: auto;
}
</style>

<template>
  <div class="dashboard-page">
    <h1>Dashboard</h1>
    <div class="stats-grid">
      <div class="stat-card glass">
        <h3>Total Balance</h3>
        <p class="value">$12,450.00</p>
      </div>
      <div class="stat-card glass">
        <h3>Income</h3>
        <p class="value positive">+$4,200.00</p>
      </div>
      <div class="stat-card glass">
        <h3>Expenses</h3>
        <p class="value negative">-$1,850.00</p>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-top: 2rem;
}
.stat-card {
  padding: 1.5rem;
  border-radius: 1rem;
  h3 {
    margin-bottom: 0.5rem;
    font-size: 1rem;
    opacity: 0.7;
  }
  .value {
    font-size: 1.5rem;
    font-weight: bold;
    &.positive { color: hsl(var(--color-success)); }
    &.negative { color: hsl(var(--color-danger)); }
  }
}
</style>

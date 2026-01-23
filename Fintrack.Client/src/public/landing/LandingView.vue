<template>
  <div class="landing-page container">
    <section class="hero">
      <h1 class="text-gradient">Master Your Finance</h1>
      <p>AI-powered expense tracking for the modern era.</p>
      <router-link to="/auth/register" class="cta-btn">Get Started</router-link>
    </section>
  </div>
</template>

<style scoped>
.hero {
  text-align: center;
  padding: 5rem 1rem;
}
.hero h1 {
  font-size: 3rem;
  margin-bottom: 1rem;
}
.cta-btn {
  display: inline-block;
  margin-top: 2rem;
  padding: 1rem 2rem;
  background: hsl(var(--color-primary));
  color: white;
  text-decoration: none;
  border-radius: 2rem;
  font-weight: bold;
}
</style>

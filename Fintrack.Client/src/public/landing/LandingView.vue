<template>
  <div
    class="bg-background-dark text-slate-100 font-display overflow-x-hidden antialiased selection:bg-primary selection:text-white min-h-screen">
    <LandingHeader />
    <main class="relative flex flex-col items-center pt-20">
      <LandingHero />
      <LandingFeatures />
      <LandingHowItWorks />
      <LandingTestimonials />
    </main>
    <LandingFooter />
  </div>
</template>

<script setup lang="ts">
import LandingHeader from './components/LandingHeader.vue';
import LandingHero from './components/LandingHero.vue';
import LandingFeatures from './components/LandingFeatures.vue';
import LandingHowItWorks from './components/LandingHowItWorks.vue';
import LandingTestimonials from './components/LandingTestimonials.vue';
import LandingFooter from './components/LandingFooter.vue';
</script>

<style>
/* Global icon styles for this view */
.material-symbols-outlined {
  font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
}
</style>

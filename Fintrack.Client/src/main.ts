import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'
import './assets/main.css'

// Deferred font loading to unblock LCP
import('@fontsource-variable/material-symbols-outlined/full.css')

const app = createApp(App)

app.use(createPinia())
app.use(router)

app.mount('#app')

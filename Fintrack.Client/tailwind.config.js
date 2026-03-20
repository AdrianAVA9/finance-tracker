/** @type {import('tailwindcss').Config} */
export default {
  darkMode: "class",
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        "primary": "#1f6b7a",
        "primary-hover": "#17525e",
        "accent-lime": "#66CC33",
        "background-light": "#fafaf9",
        "background-dark": "#1c1f22",
        "card-dark": "#2B3036",
        "card-light": "#ffffff",
        "text-secondary-dark": "#9abac1",
        "text-secondary-light": "#64748b",
        "surface-dark": "#1E1E1E",
        "surface-border": "#2A2A2A",
      },
      boxShadow: {
        'soft': '0 4px 20px -2px rgba(0, 0, 0, 0.1)',
        'glow': '0 0 15px rgba(31, 107, 122, 0.3)',
      },
      fontFamily: {
        "display": ["Manrope", "sans-serif"],
        "heading": ["Space Grotesk", "sans-serif"],
      },
      borderRadius: {
        "DEFAULT": "0.5rem",
        "lg": "1rem",
        "xl": "1.5rem",
        "2xl": "2rem",
        "full": "9999px"
      },
      backgroundImage: {
        'gradient-radial': 'radial-gradient(var(--tw-gradient-stops))',
      }
    },
  },
  plugins: [],
}

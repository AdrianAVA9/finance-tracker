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
        "primary": "#0a7d7f",
        "primary-hover": "#086668",
        "accent": "#00C4CC",
        "background-light": "#fafafa",
        "background-dark": "#121212",
        "surface-dark": "#1E1E1E",
        "surface-border": "#2A2A2A",
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

/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{js,ts,jsx,tsx}'],
  theme: {
    extend: {
      colors: {
        ironclad: {
          DEFAULT: '#c0392b',
          dark: '#96281b',
          light: '#e74c3c',
        },
        silent: {
          DEFAULT: '#27ae60',
          dark: '#1e8449',
          light: '#2ecc71',
        },
        defect: {
          DEFAULT: '#2980b9',
          dark: '#1a5276',
          light: '#3498db',
        },
        watcher: {
          DEFAULT: '#8e44ad',
          dark: '#6c3483',
          light: '#9b59b6',
        },
        spire: {
          bg: '#0f0e17',
          surface: '#1a1825',
          card: '#242236',
          border: '#3d3a5c',
          text: '#e8e6f0',
          muted: '#8b8aaa',
          gold: '#f0c040',
          highlight: '#5b5480',
        },
      },
    },
  },
  plugins: [],
}

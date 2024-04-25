/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: 'class',
  content: [
    './src/**/*.{html,ts}',
    './node_modules/preline/preline.js',
  ],
  theme: {    
    extend: {
      colors: {
        dark: { // dark-layers
         l1: '#171717',
         l2: '#2c2c2c',
         l3: '#434343'
        },
        light: { // light-layers
          l1: '#ffffff',
          l2: '#e1e1e1',
          l3: '#c4c4c4'
         }
      },
      borderRadius: {
        'md': '0.24rem'
      }
    },
  },
  plugins: [
    require('preline/plugin'),
    require('@tailwindcss/forms')
  ],
}
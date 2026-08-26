import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
export default defineConfig({ plugins:[vue()], build:{ outDir:'../src/UniversalSimulation.Api/wwwroot', emptyOutDir:true }, server:{ proxy:{ '/api':'http://localhost:5000' } } })

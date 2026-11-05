import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// Фронтенд живёт на своём origin: http://localhost:5173
// Именно этот origin разрешён в appsettings.json -> Cors:AllowedOrigins
export default defineConfig({
  plugins: [react()],
  server: { port: 5173, strictPort: true },
})

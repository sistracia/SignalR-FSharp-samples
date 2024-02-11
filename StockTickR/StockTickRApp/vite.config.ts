import { defineConfig } from "vite";

export default defineConfig({
  build: {
    emptyOutDir: true,
    outDir: "wwwroot",
  },
  server: {
    port: 5000,
    cors: true,
  },
});

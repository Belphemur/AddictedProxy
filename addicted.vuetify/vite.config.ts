// Plugins
import vue from '@vitejs/plugin-vue'
import vuetify, {transformAssetUrls} from 'vite-plugin-vuetify'

// Utilities
import {defineConfig, loadEnv} from "vite";
import {fileURLToPath, URL} from 'node:url'

// https://vitejs.dev/config/


export default defineConfig(({mode}) => {
  const env = loadEnv(mode, "");

  const htmlPlugin = () => {
    return {
      name: "html-transform",
      transformIndexHtml(html: string) {
        return html.replace(/%(.*?)%/g, function (match, p1) {
          return env[p1];
        });
      },
    };
  };

  return {
    build: {
      sourcemap: "inline",
      rollupOptions: {
        output: {
          manualChunks(id) {
            if (id.includes("swagger")) {
              return "swagger";
            } else if (id.includes("sentry")) {
              return "perf";
            } else if (id.includes("chart")) {
              return "chart";
            } else if(id.includes("node_modules")) {
              return "vendor";
            }
          },
        },
      },
    },
    plugins: [
      vue({
        template: {transformAssetUrls},
        script: {
          defineModel: true
        }
      }),
      // https://github.com/vuetifyjs/vuetify-loader/tree/next/packages/vite-plugin
      vuetify({
        autoImport: true,
        styles: {
          configFile: 'src/styles/settings.scss',
        },
      }),
      htmlPlugin(),
    ],
    define: {'process.env': {}},
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url))
      },
      extensions: [
        '.js',
        '.json',
        '.jsx',
        '.mjs',
        '.ts',
        '.tsx',
        '.vue',
      ],
    },
    server: {
      port: 3000,
    },
  }

})

import path from "path";
import { defineConfig, loadEnv } from "vite";
import vue from "@vitejs/plugin-vue";
import AutoImport from "unplugin-auto-import/vite";
import Components from "unplugin-vue-components/vite";
import { ElementPlusResolver } from "unplugin-vue-components/resolvers";
import mkcert from "vite-plugin-mkcert";

import Unocss from "unocss/vite";
import fs from "fs";
import { join } from "path";
import {
  presetAttributify,
  presetIcons,
  presetUno,
  transformerDirectives,
  transformerVariantGroup
} from "unocss";

const pathSrc = path.resolve(__dirname, "src");

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, "");

  const htmlPlugin = () => {
    return {
      name: "html-transform",
      transformIndexHtml(html: string) {
        return html.replace(/%(.*?)%/g, function(match, p1) {
          return env[p1];
        });
      }
    };
  };
  return {
    build: {
      sourcemap: "inline",
      rollupOptions: {
        output: {
          manualChunks(id) {
            if (id.includes("node_modules")) {
              const [, module] = /node_modules\/(@?[a-z0-9-]+?[a-z0-9-]+)/.exec(
                id
              );
              const path = join(
                process.cwd(),
                "node_modules",
                module,
                "package.json"
              );
              if (fs.existsSync(path)) {
                try {
                  const packageJson = require(path);
                  const version = packageJson.version;
                  return `@vendor/${module}_${version}.js`;
                } catch (error) {
                }
              }
            }
          }
        }
      }
    },
    resolve: {
      alias: {
        "~/": `${pathSrc}/`
      }
    },
    css: {
      preprocessorOptions: {
        scss: {
          additionalData: `@use "~/styles/element/index.scss" as *;`
        }
      }
    },
    server: { https: true },
    plugins: [
      mkcert(),
      vue(),
      htmlPlugin(),
      Components({
        // allow auto load markdown components under `./src/components/`
        extensions: ["vue", "md"],
        // allow auto import and register components used in markdown
        include: [/\.vue$/, /\.vue\?vue/, /\.md$/],
        resolvers: [
          ElementPlusResolver({
            importStyle: "sass"
          })
        ],
        dts: "src/components.d.ts"
      }),
      AutoImport({
        resolvers: [ElementPlusResolver()]
      }),

      // https://github.com/antfu/unocss
      // see unocss.config.ts for config
      Unocss({
        presets: [
          presetUno(),
          presetAttributify(),
          presetIcons({
            scale: 1.2,
            warn: true
          })
        ],
        transformers: [transformerDirectives(), transformerVariantGroup()]
      })
    ]
  };
});

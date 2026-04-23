/// <reference types="vitest" />
import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vitest/config';
import plugin from '@vitejs/plugin-vue';
import { VitePWA } from 'vite-plugin-pwa';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import { env } from 'process';
import { cloudflare } from "@cloudflare/vite-plugin";
import prerender from 'vite-plugin-prerender-esm-fix';

const baseFolder =
    env.APPDATA !== undefined && env.APPDATA !== ''
        ? `${env.APPDATA}/ASP.NET/https`
        : `${env.HOME}/.aspnet/https`;

const certificateName = "fintrack.client";
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

/** Cloudflare Pages/Workers and other CI images rarely have a working Puppeteer/Chromium. */
const isCI = !!(
    env.CI ||
    env.CF_PAGES ||
    env.CF_PAGES_BRANCH ||
    env.GITHUB_ACTIONS ||
    env.WORKERS_CI
);
const enablePrerender =
    env.VITE_PRERENDER === 'true' || (!isCI && env.SKIP_PRERENDER !== '1');

if (!isCI && (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath))) {
    if (0 !== child_process.spawnSync('dotnet', [
        'dev-certs',
        'https',
        '--export-path',
        certFilePath,
        '--format',
        'Pem',
        '--no-password',
    ], { stdio: 'inherit', }).status) {
        throw new Error("Could not create certificate.");
    }
}

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
    env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:7185';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [
        plugin(),
        VitePWA({
            registerType: 'prompt',
            injectRegister: 'auto',
            includeAssets: ['favicon.ico', 'splash/*.jpg', 'splash/*.png', 'screenshots/*.png'],
            manifest: {
                name: 'CeroBase',
                short_name: 'CeroBase',
                start_url: '/app',
                scope: '/',
                description: 'Administra tus finanzas con CeroBase. El mejor rastreador de finanzas personales y control de gastos automatizado por IA.',
                theme_color: '#111317',
                background_color: '#111317',
                display: 'standalone',
                lang: 'es',
                categories: ['finance', 'productivity'],
                shortcuts: [
                    {
                        name: 'Registrar Gasto',
                        short_name: 'Gasto',
                        description: 'Registra un nuevo gasto rápidamente',
                        url: '/app/expenses/new',
                        icons: [{ src: 'icons/launchericon-96x96.png', sizes: '96x96' }]
                    },
                    {
                        name: 'Registrar Ingreso',
                        short_name: 'Ingreso',
                        description: 'Registra un nuevo ingreso de dinero',
                        url: '/app/incomes/new',
                        icons: [{ src: 'icons/launchericon-96x96.png', sizes: '96x96' }]
                    },
                    {
                        name: 'Ver Actividad',
                        short_name: 'Actividad',
                        description: 'Revisa tus últimos movimientos',
                        url: '/app/activity',
                        icons: [{ src: 'icons/launchericon-96x96.png', sizes: '96x96' }]
                    }
                ],
                screenshots: [
                    {
                        src: 'screenshots/mobile-dashboard.png',
                        sizes: '539x1600',
                        type: 'image/png',
                        form_factor: 'narrow',
                        label: 'Dashboard móvil de CeroBase'
                    },
                    {
                        src: 'screenshots/desktop-dashboard.png',
                        sizes: '1600x1280',
                        type: 'image/png',
                        form_factor: 'wide',
                        label: 'Dashboard de escritorio de CeroBase'
                    }
                ],
                icons: [
                    {
                        src: 'icons/launchericon-48x48.png',
                        sizes: '48x48',
                        type: 'image/png'
                    },
                    {
                        src: 'icons/launchericon-72x72.png',
                        sizes: '72x72',
                        type: 'image/png'
                    },
                    {
                        src: 'icons/launchericon-96x96.png',
                        sizes: '96x96',
                        type: 'image/png'
                    },
                    {
                        src: 'icons/launchericon-144x144.png',
                        sizes: '144x144',
                        type: 'image/png'
                    },
                    {
                        src: 'icons/launchericon-192x192.png',
                        sizes: '192x192',
                        type: 'image/png'
                    },
                    {
                        src: 'icons/launchericon-512x512.png',
                        sizes: '512x512',
                        type: 'image/png'
                    },
                    {
                        src: 'icons/launchericon-512x512.png',
                        sizes: '512x512',
                        type: 'image/png',
                        purpose: 'maskable'
                    }
                ]
            },
            workbox: {
                globPatterns: ['**/*.{js,css,html,ico,png,svg}'],
                runtimeCaching: [
                    {
                        urlPattern: /^https:\/\/localhost:7185\/api\/.*/i,
                        handler: 'NetworkFirst',
                        options: {
                            cacheName: 'api-cache',
                            expiration: {
                                maxEntries: 100,
                                maxAgeSeconds: 60 * 60 * 24 // 24 hours
                            },
                        }
                    }
                ]
            }
        }),
        cloudflare(),
        ...(enablePrerender
            ? [
                  prerender({
                      staticDir: fileURLToPath(new URL('./dist', import.meta.url)),
                      routes: ['/'],
                      // @ts-ignore - plugin supports these at runtime but types are missing/strict
                      rendererOptions: {
                          renderAfterTime: 10000,
                          inject: {
                              __PRERENDER_INJECTED: true
                          }
                      }
                  })
              ]
            : [])
    ],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    server: {
        proxy: {
            '^/weatherforecast': {
                target,
                secure: false
            },
            '^/register': {
                target,
                secure: false
            },
            '^/login': {
                target,
                secure: false
            },
            '^/refresh': {
                target,
                secure: false
            },
            '^/confirmEmail': {
                target,
                secure: false
            },
            '^/resendConfirmationEmail': {
                target,
                secure: false
            },
            '^/forgotPassword': {
                target,
                secure: false
            },
            '^/resetPassword': {
                target,
                secure: false
            },
            '^/manage/2fa': {
                target,
                secure: false
            },
            '^/manage/info': {
                target,
                secure: false
            },
            '^/api': {
                target,
                secure: false
            }
        },
        port: 5173,
        https: (isCI || !fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) 
            ? undefined 
            : {
                key: fs.readFileSync(keyFilePath),
                cert: fs.readFileSync(certFilePath),
            }
    },
    test: {
        environment: 'jsdom',
        globals: true,
        setupFiles: [],
        root: fileURLToPath(new URL('./', import.meta.url)),
    }
})
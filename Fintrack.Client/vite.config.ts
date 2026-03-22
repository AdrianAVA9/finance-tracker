/// <reference types="vitest" />
import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vitest/config';
import plugin from '@vitejs/plugin-vue';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import { env } from 'process';

import { cloudflare } from "@cloudflare/vite-plugin";

const baseFolder =
    env.APPDATA !== undefined && env.APPDATA !== ''
        ? `${env.APPDATA}/ASP.NET/https`
        : `${env.HOME}/.aspnet/https`;

const certificateName = "fintrack.client";
const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

if (!env.CI && (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath))) {
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
    plugins: [plugin(), cloudflare()],
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
        https: (env.CI || !fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) 
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
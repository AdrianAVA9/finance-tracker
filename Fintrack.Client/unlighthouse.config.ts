import { defineUnlighthouseConfig } from 'unlighthouse/config';

/**
 * Run against a local dev server (HTTPS on port 5173 by default in vite.config):
 *   1. npm run dev
 *   2. npm run unlighthouse
 *
 * CI or HTTP-only dev: UNLIGHTHOUSE_SITE=http://localhost:5173 npm run unlighthouse
 */
export default defineUnlighthouseConfig({
  site: process.env.UNLIGHTHOUSE_SITE ?? 'https://localhost:5173',
  scanner: {
    skipJavascript: false,
  },
  lighthouseOptions: {
    maxWaitForLoad: 45_000,
  },
  puppeteerOptions: {
    args: ['--ignore-certificate-errors', '--allow-insecure-localhost'],
  },
  /** Explicit paths so client-side routes are audited without relying on link crawl alone */
  urls: ['/', '/auth/login', '/auth/register', '/auth/forgot-password'],
});

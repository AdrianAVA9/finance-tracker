import { copyFile, mkdir, readdir, rm, stat } from 'node:fs/promises';
import { join, dirname } from 'node:path';
import { fileURLToPath } from 'node:url';

const root = join(dirname(fileURLToPath(import.meta.url)), '..');
const staging = join(root, '.pwa-icon-staging');
const destDir = join(root, 'public', 'splash');

const sleep = (ms) => new Promise((r) => setTimeout(r, ms));

async function copyWithRetry(src, dest, { attempts = 12, delayMs = 400 } = {}) {
  let lastErr;
  for (let i = 0; i < attempts; i++) {
    try {
      await copyFile(src, dest);
      return;
    } catch (e) {
      lastErr = e;
      if (i < attempts - 1) await sleep(delayMs);
    }
  }
  throw lastErr;
}

try {
  await stat(staging);
} catch {
  console.error('Missing staging folder:', staging);
  process.exit(1);
}

const toCopy = [
  'apple-icon-180.png',
  'manifest-icon-192.maskable.png',
  'manifest-icon-512.maskable.png',
];

await mkdir(destDir, { recursive: true });
let n = 0;
for (const name of toCopy) {
  try {
    await stat(join(staging, name));
  } catch {
    console.warn('Skip missing file:', name);
    continue;
  }
  await copyWithRetry(join(staging, name), join(destDir, name));
  n++;
}
await rm(staging, { recursive: true, force: true });
console.log(`Copied ${n} icon PNG(s) to public/splash/`);

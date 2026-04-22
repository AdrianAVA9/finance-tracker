import { copyFile, mkdir, readdir, rm, stat } from 'node:fs/promises';
import { join, dirname } from 'node:path';
import { fileURLToPath } from 'node:url';

const root = join(dirname(fileURLToPath(import.meta.url)), '..');
const staging = join(root, '.pwa-splash-staging');
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

await mkdir(destDir, { recursive: true });
const files = await readdir(staging);
let n = 0;
for (const name of files) {
  if (!name.startsWith('apple-splash-') || !name.endsWith('.jpg')) continue;
  await copyWithRetry(join(staging, name), join(destDir, name));
  n++;
}
await rm(staging, { recursive: true, force: true });
console.log(`Copied ${n} splash JPG(s) to public/splash/`);

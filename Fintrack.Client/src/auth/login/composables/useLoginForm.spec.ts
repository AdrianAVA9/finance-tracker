import { beforeEach, describe, expect, it, vi } from 'vitest';
import { useLoginForm } from './useLoginForm';

const loginMock = vi.fn();
const pushMock = vi.fn();

vi.mock('@/composables/useAuth', () => ({
  useAuth: () => ({
    login: loginMock,
  }),
}));

vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: pushMock,
  }),
}));

describe('useLoginForm', () => {
  beforeEach(() => {
    loginMock.mockReset();
    pushMock.mockReset();
  });

  it('redirects to /app after successful login', async () => {
    loginMock.mockResolvedValue(true);
    pushMock.mockResolvedValue(undefined);

    const loginForm = useLoginForm();
    loginForm.email.value = 'user@example.com';
    loginForm.password.value = 'pass1234';

    await loginForm.handleLogin();

    expect(loginMock).toHaveBeenCalledWith('user@example.com', 'pass1234');
    expect(pushMock).toHaveBeenCalledWith('/app');
    expect(loginForm.errorMsg.value).toBe('');
    expect(loginForm.loading.value).toBe(false);
  });

  it('sets error message and avoids redirect when login fails', async () => {
    loginMock.mockResolvedValue(false);

    const loginForm = useLoginForm();
    loginForm.email.value = 'user@example.com';
    loginForm.password.value = 'bad-pass';

    await loginForm.handleLogin();

    expect(pushMock).not.toHaveBeenCalled();
    expect(loginForm.errorMsg.value).toBe('Correo o contraseña inválidos.');
    expect(loginForm.loading.value).toBe(false);
  });
});

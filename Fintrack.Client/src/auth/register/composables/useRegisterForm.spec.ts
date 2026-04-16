import { beforeEach, describe, expect, it, vi } from 'vitest';
import { useRegisterForm } from './useRegisterForm';

const registerMock = vi.fn();
const pushMock = vi.fn();

vi.mock('@/composables/useAuth', () => ({
  useAuth: () => ({
    register: registerMock,
  }),
}));

vi.mock('vue-router', () => ({
  useRouter: () => ({
    push: pushMock,
  }),
}));

describe('useRegisterForm', () => {
  beforeEach(() => {
    registerMock.mockReset();
    pushMock.mockReset();
  });

  it('requires terms acceptance before submit', async () => {
    const registerForm = useRegisterForm();
    registerForm.email.value = 'user@example.com';
    registerForm.password.value = 'Password1';
    registerForm.terms.value = false;

    await registerForm.handleRegister();

    expect(registerMock).not.toHaveBeenCalled();
    expect(registerForm.errorMsg.value).toBe('Debes aceptar los Terminos de Servicio para registrarte.');
    expect(registerForm.loading.value).toBe(false);
  });

  it('redirects to /app after successful registration', async () => {
    registerMock.mockResolvedValue({ success: true });
    pushMock.mockResolvedValue(undefined);

    const registerForm = useRegisterForm();
    registerForm.email.value = 'user@example.com';
    registerForm.password.value = 'Password1';
    registerForm.terms.value = true;

    await registerForm.handleRegister();

    expect(registerMock).toHaveBeenCalledWith('user@example.com', 'Password1');
    expect(pushMock).toHaveBeenCalledWith('/app');
    expect(registerForm.errorMsg.value).toBe('');
    expect(registerForm.loading.value).toBe(false);
  });

  it('sets error message and avoids redirect when register fails', async () => {
    registerMock.mockResolvedValue({ success: false });

    const registerForm = useRegisterForm();
    registerForm.email.value = 'user@example.com';
    registerForm.password.value = 'Password1';
    registerForm.terms.value = true;

    await registerForm.handleRegister();

    expect(pushMock).not.toHaveBeenCalled();
    expect(registerForm.errorMsg.value).toBe('Error en el registro. Por favor verifica tus datos.');
    expect(registerForm.loading.value).toBe(false);
  });

  it('shows backend duplicate username message when available', async () => {
    registerMock.mockResolvedValue({
      success: false,
      code: 'DuplicateUserName',
      message: "Username 'adrian.vegaa@outlook.com' is already taken.",
    });

    const registerForm = useRegisterForm();
    registerForm.email.value = 'adrian.vegaa@outlook.com';
    registerForm.password.value = 'Password1';
    registerForm.terms.value = true;

    await registerForm.handleRegister();

    expect(pushMock).not.toHaveBeenCalled();
    expect(registerForm.errorMsg.value).toBe("Username 'adrian.vegaa@outlook.com' is already taken.");
    expect(registerForm.loading.value).toBe(false);
  });
});

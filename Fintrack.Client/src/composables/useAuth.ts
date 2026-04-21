import { ref, computed } from 'vue';
import api from '@/services/api';

type RegisterResult =
  | { success: true }
  | {
      success: false;
      code?: string;
      message?: string;
    };

const user = ref<string | null>(null);
const isLoading = ref(true);
const isInitialized = ref(false);

export function useAuth() {
  const isAuthenticated = computed(() => !!user.value);

  const checkSession = async () => {
    isLoading.value = true;
    try {
      // Use the new AuthController endpoint
      const response = await api.get('/api/v1/auth/session');
      user.value = response.data.user || 'User';
    } catch {
      user.value = null; // 401 Unauthorized means no valid cookie
    } finally {
      isLoading.value = false;
      isInitialized.value = true;
    }
  };

  const login = async (email: string, password: string) => {
    try {
      // Endpoint provided by MapIdentityApi
      // useCookies=true tells the backend to issue a Set-Cookie header instead of returning JSON JWT
      await api.post('/login?useCookies=true', { email, password });
      await checkSession();
      return true;
    } catch (error) {
      console.error('Login failed', error);
      return false;
    }
  };

  const register = async (email: string, password: string): Promise<RegisterResult> => {
    try {
      // Endpoint provided by MapIdentityApi
      await api.post('/register', { email, password });
      // Registration successful, now log them in to get the cookie
      const loginSuccess = await login(email, password);
      return loginSuccess ? { success: true } : { success: false };
    } catch (error) {
      console.error('Registration failed', error);
      const responseData = (error as { response?: { data?: { errors?: Record<string, string[]> } } })?.response?.data;
      const errors = responseData?.errors;

      if (errors && typeof errors === 'object') {
        const [firstCode, firstMessages] = Object.entries(errors)[0] ?? [];
        if (firstCode) {
          return {
            success: false,
            code: firstCode,
            message: Array.isArray(firstMessages) ? firstMessages[0] : undefined,
          };
        }
      }

      return { success: false };
    }
  };

  const logout = async () => {
    try {
      // Call the backend to invalidate the HttpOnly cookie
      await api.post('/api/v1/auth/logout');
      user.value = null;
      // Force a full page reload to clear any cached states
      window.location.href = '/auth/login';
    } catch (error) {
      console.error('Logout failed', error);
      // Fallback: clear local state and redirect anyway
      user.value = null;
      window.location.href = '/auth/login';
    }
  };

  return {
    user,
    isAuthenticated,
    isLoading,
    isInitialized,
    checkSession,
    login,
    register,
    logout,
  };
}

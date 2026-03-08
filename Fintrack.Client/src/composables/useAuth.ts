import { ref, computed } from 'vue';
import api from '@/services/api';

const user = ref<string | null>(null);
const isLoading = ref(true);

export function useAuth() {
  const isAuthenticated = computed(() => !!user.value);

  const checkSession = async () => {
    isLoading.value = true;
    try {
      // The secure endpoint we created earlier allows us to verify if our cookie is valid
      const response = await api.get('/WeatherForecast/secure');
      user.value = response.data.user || 'User';
    } catch (e) {
      user.value = null; // 401 Unauthorized means no valid cookie
    } finally {
      isLoading.value = false;
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

  const register = async (email: string, password: string) => {
    try {
      // Endpoint provided by MapIdentityApi
      await api.post('/register', { email, password });
      // Registration successful, now log them in to get the cookie
      return await login(email, password);
    } catch (error) {
      console.error('Registration failed', error);
      return false;
    }
  };

  const logout = async () => {
    try {
      // Note: MapIdentityApi does not provide a true logout endpoint natively in .NET 8 out of the box in some templates.
      // We will need to define one later manually if it's missing, but for now we clear client state.
      // Often, hitting a custom backend `/logout` endpoint to invalidate the cookie is required.
      user.value = null;
    } catch (error) {
      console.error('Logout failed', error);
    }
  };

  return {
    user,
    isAuthenticated,
    isLoading,
    checkSession,
    login,
    register,
    logout,
  };
}

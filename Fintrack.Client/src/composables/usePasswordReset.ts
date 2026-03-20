import { ref } from 'vue';
import api from '@/services/api';

export function usePasswordReset() {
  const isLoading = ref(false);
  const error = ref('');
  const success = ref(false);

  const sendResetLink = async (email: string): Promise<boolean> => {
    isLoading.value = true;
    error.value = '';
    success.value = false;
    try {
      // ASP.NET Core Identity endpoint from MapIdentityApi
      await api.post('/forgotPassword', { email });
      success.value = true;
      return true;
    } catch (e: any) {
      error.value = e?.response?.data?.detail ?? 'Something went wrong. Please try again.';
      return false;
    } finally {
      isLoading.value = false;
    }
  };

  const resetPassword = async (email: string, resetCode: string, newPassword: string): Promise<boolean> => {
    isLoading.value = true;
    error.value = '';
    success.value = false;
    try {
      // ASP.NET Core Identity endpoint from MapIdentityApi
      await api.post('/resetPassword', { email, resetCode, newPassword });
      success.value = true;
      return true;
    } catch (e: any) {
      const detail = e?.response?.data?.detail ?? '';
      const errors = e?.response?.data?.errors;
      if (errors) {
        const msgs = Object.values(errors).flat() as string[];
        error.value = msgs.join(' ');
      } else {
        error.value = detail || 'Failed to reset password. The link may have expired.';
      }
      return false;
    } finally {
      isLoading.value = false;
    }
  };

  return {
    isLoading,
    error,
    success,
    sendResetLink,
    resetPassword,
  };
}

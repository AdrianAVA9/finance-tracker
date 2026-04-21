import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuth } from '@/composables/useAuth';

export function useLoginForm() {
  const { login } = useAuth();
  const router = useRouter();

  const email = ref('');
  const password = ref('');
  const errorMsg = ref('');
  const loading = ref(false);

  const handleLogin = async () => {
    loading.value = true;
    errorMsg.value = '';

    const success = await login(email.value, password.value);
    loading.value = false;

    if (success) {
      await router.push('/app');
      return;
    }

    errorMsg.value = 'Correo o contraseña inválidos.';
  };

  return {
    email,
    password,
    errorMsg,
    loading,
    handleLogin,
  };
}

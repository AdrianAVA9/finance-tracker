import { computed, ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuth } from '@/composables/useAuth';

type PasswordStrength = {
  label: 'Ninguna' | 'Debil' | 'Media' | 'Buena' | 'Fuerte';
  widthClass: string;
  colorClass: string;
};

export function useRegisterForm() {
  const { register } = useAuth();
  const router = useRouter();

  const fullName = ref('');
  const email = ref('');
  const password = ref('');
  const terms = ref(false);
  const errorMsg = ref('');
  const loading = ref(false);

  const passwordStrength = computed<PasswordStrength>(() => {
    if (password.value.length === 0) return { label: 'Ninguna', widthClass: 'w-0', colorClass: 'bg-white/10' };
    if (password.value.length < 6) return { label: 'Debil', widthClass: 'w-1/4', colorClass: 'bg-red-500' };
    if (password.value.length < 10) return { label: 'Media', widthClass: 'w-2/4', colorClass: 'bg-[#85B2B2]' };
    if (/[A-Z]/.test(password.value) && /[0-9]/.test(password.value)) {
      return { label: 'Fuerte', widthClass: 'w-full', colorClass: 'bg-[#191971]' };
    }

    return { label: 'Buena', widthClass: 'w-3/4', colorClass: 'bg-blue-400' };
  });

  const handleRegister = async () => {
    if (!terms.value) {
      errorMsg.value = 'Debes aceptar los Terminos de Servicio para registrarte.';
      return;
    }

    loading.value = true;
    errorMsg.value = '';

    const success = await register(email.value, password.value);
    loading.value = false;

    if (success) {
      await router.push('/app');
      return;
    }

    errorMsg.value = 'Error en el registro. Por favor verifica tus datos.';
  };

  return {
    fullName,
    email,
    password,
    terms,
    errorMsg,
    loading,
    passwordStrength,
    handleRegister,
  };
}

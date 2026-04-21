import { mount } from '@vue/test-utils';
import { describe, expect, it } from 'vitest';
import LoginFormCard from './LoginFormCard.vue';

const createWrapper = () =>
  mount(LoginFormCard, {
    props: {
      email: 'initial@example.com',
      password: 'secret',
      loading: false,
      errorMsg: '',
    },
    global: {
      stubs: {
        RouterLink: {
          template: '<a><slot /></a>',
        },
      },
    },
  });

describe('LoginFormCard.vue', () => {
  it('emits email and password updates from inputs', async () => {
    const wrapper = createWrapper();
    const inputs = wrapper.findAll('input');

    await inputs[0]?.setValue('new@example.com');
    await inputs[1]?.setValue('new-password');

    expect(wrapper.emitted('update:email')?.[0]).toEqual(['new@example.com']);
    expect(wrapper.emitted('update:password')?.[0]).toEqual(['new-password']);
  });

  it('emits submit when form is submitted', async () => {
    const wrapper = createWrapper();

    await wrapper.find('form').trigger('submit');

    expect(wrapper.emitted('submit')).toBeTruthy();
  });

  it('toggles password visibility from password button', async () => {
    const wrapper = createWrapper();
    const passwordInput = wrapper.find('#password');
    const toggleButton = wrapper.find('button[type="button"]');

    expect(passwordInput.attributes('type')).toBe('password');

    await toggleButton.trigger('click');
    expect(passwordInput.attributes('type')).toBe('text');

    await toggleButton.trigger('click');
    expect(passwordInput.attributes('type')).toBe('password');
  });
});

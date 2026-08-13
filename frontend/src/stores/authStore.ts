import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import apiClient from '@/api/client';
import type { User } from '@/types';

export const useAuthStore = defineStore('auth', () => {
  const savedUser = localStorage.getItem('user_data');
  const user = ref<User | null>(savedUser ? JSON.parse(savedUser) : null);

  const token = ref<string | null>(localStorage.getItem('jwt_token'));
  const isAuthenticated = computed(() => !!token.value);

  async function login(email: string, password: string) {
    const response = await apiClient.post<{ token: string; user: User }>('/auth/login', {
      email,
      password,
    });
    
    token.value = response.data.token;
    user.value = response.data.user;

    localStorage.setItem('jwt_token', response.data.token);
    localStorage.setItem('user_data', JSON.stringify(response.data.user));
    return response.data;
  }

  async function register(name: string, email: string, password: string) {
    const response = await apiClient.post<{ token: string; user: User }>('/auth/register', {
      name,
      email,
      password,
    });

    token.value = response.data.token;
    user.value = response.data.user;

    localStorage.setItem('jwt_token', response.data.token);
    localStorage.setItem('user_data', JSON.stringify(response.data.user));
    return response.data;
  }

  function logout() {
    token.value = null;
    user.value = null;
    localStorage.removeItem('jwt_token');
    localStorage.removeItem('user_data');
  }

  return {
    user,
    token,
    isAuthenticated,
    login,
    register,
    logout,
  };
});

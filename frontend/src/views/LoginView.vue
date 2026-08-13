<template>
  <div class="min-h-screen bg-[#0a0e17] text-slate-100 flex flex-col justify-center items-center p-6 relative overflow-hidden font-sans">
    <!-- Glow Background -->
    <div class="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[500px] h-[500px] bg-[#0f4c81]/25 rounded-full blur-[120px] pointer-events-none"></div>

    <router-link to="/" class="absolute top-8 left-8 flex items-center gap-2 text-sm text-slate-400 hover:text-white transition">
      <ArrowLeft class="w-4 h-4" />
      Back to Home
    </router-link>

    <div class="w-full max-w-md">
      <!-- Logo Header -->
      <div class="text-center mb-8">
        <div class="inline-flex w-12 h-12 rounded-2xl bg-gradient-to-tr from-[#0f4c81] to-[#2563eb] items-center justify-center shadow-lg shadow-blue-900/40 mb-4">
          <TrendingUp class="w-7 h-7 text-white" />
        </div>
        <h2 class="text-2xl font-extrabold text-white tracking-tight">TradingCenter</h2>
        <p class="text-xs text-slate-400 mt-1">Multi-Broker Japanese Blue Dashboard</p>
      </div>

      <!-- Auth Card -->
      <div class="japanese-blue-card p-8 rounded-2xl backdrop-blur-xl">
        <!-- Tabs -->
        <div class="flex border-b border-slate-800 mb-6">
          <button
            @click="activeTab = 'login'"
            :class="[
              'flex-1 py-3 text-sm font-semibold border-b-2 transition-colors',
              activeTab === 'login'
                ? 'border-[#2563eb] text-white'
                : 'border-transparent text-slate-400 hover:text-slate-200'
            ]"
          >
            Sign In
          </button>
          <button
            @click="activeTab = 'register'"
            :class="[
              'flex-1 py-3 text-sm font-semibold border-b-2 transition-colors',
              activeTab === 'register'
                ? 'border-[#2563eb] text-white'
                : 'border-transparent text-slate-400 hover:text-slate-200'
            ]"
          >
            Create Account
          </button>
        </div>

        <!-- Alert Error -->
        <div v-if="errorMessage" class="mb-6 p-4 rounded-xl bg-red-950/60 border border-red-800/80 text-red-300 text-xs flex items-center gap-3">
          <AlertCircle class="w-5 h-5 text-red-400 shrink-0" />
          <span>{{ errorMessage }}</span>
        </div>

        <!-- Login Form -->
        <form v-if="activeTab === 'login'" @submit.prevent="handleLogin" class="space-y-5">
          <div>
            <label class="block text-xs font-semibold text-slate-300 mb-2 uppercase tracking-wider">Email Address</label>
            <div class="relative">
              <Mail class="w-5 h-5 absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-500" />
              <input
                v-model="loginEmail"
                type="email"
                required
                placeholder="name@example.com"
                class="w-full bg-slate-900/90 border border-slate-700/80 focus:border-blue-500 focus:ring-1 focus:ring-blue-500 rounded-xl py-3 pl-11 pr-4 text-sm text-white placeholder-slate-500 outline-none transition"
              />
            </div>
          </div>

          <div>
            <label class="block text-xs font-semibold text-slate-300 mb-2 uppercase tracking-wider">Password</label>
            <div class="relative">
              <Lock class="w-5 h-5 absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-500" />
              <input
                v-model="loginPassword"
                type="password"
                required
                placeholder="••••••••"
                class="w-full bg-slate-900/90 border border-slate-700/80 focus:border-blue-500 focus:ring-1 focus:ring-blue-500 rounded-xl py-3 pl-11 pr-4 text-sm text-white placeholder-slate-500 outline-none transition"
              />
            </div>
          </div>

          <button
            type="submit"
            :disabled="isLoading"
            class="w-full py-3.5 px-4 rounded-xl font-bold bg-[#2563eb] hover:bg-[#1d4ed8] text-white shadow-lg shadow-blue-600/30 transition flex items-center justify-center gap-2 disabled:opacity-50"
          >
            <Loader2 v-if="isLoading" class="w-5 h-5 animate-spin" />
            <span v-else>Sign In</span>
          </button>
        </form>

        <!-- Register Form -->
        <form v-else @submit.prevent="handleRegister" class="space-y-5">
          <div>
            <label class="block text-xs font-semibold text-slate-300 mb-2 uppercase tracking-wider">Full Name</label>
            <div class="relative">
              <User class="w-5 h-5 absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-500" />
              <input
                v-model="regName"
                type="text"
                required
                placeholder="Marco Lima"
                class="w-full bg-slate-900/90 border border-slate-700/80 focus:border-blue-500 focus:ring-1 focus:ring-blue-500 rounded-xl py-3 pl-11 pr-4 text-sm text-white placeholder-slate-500 outline-none transition"
              />
            </div>
          </div>

          <div>
            <label class="block text-xs font-semibold text-slate-300 mb-2 uppercase tracking-wider">Email Address</label>
            <div class="relative">
              <Mail class="w-5 h-5 absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-500" />
              <input
                v-model="regEmail"
                type="email"
                required
                placeholder="name@example.com"
                class="w-full bg-slate-900/90 border border-slate-700/80 focus:border-blue-500 focus:ring-1 focus:ring-blue-500 rounded-xl py-3 pl-11 pr-4 text-sm text-white placeholder-slate-500 outline-none transition"
              />
            </div>
          </div>

          <div>
            <label class="block text-xs font-semibold text-slate-300 mb-2 uppercase tracking-wider">Password</label>
            <div class="relative">
              <Lock class="w-5 h-5 absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-500" />
              <input
                v-model="regPassword"
                type="password"
                required
                placeholder="••••••••"
                class="w-full bg-slate-900/90 border border-slate-700/80 focus:border-blue-500 focus:ring-1 focus:ring-blue-500 rounded-xl py-3 pl-11 pr-4 text-sm text-white placeholder-slate-500 outline-none transition"
              />
            </div>
          </div>

          <button
            type="submit"
            :disabled="isLoading"
            class="w-full py-3.5 px-4 rounded-xl font-bold bg-[#2563eb] hover:bg-[#1d4ed8] text-white shadow-lg shadow-blue-600/30 transition flex items-center justify-center gap-2 disabled:opacity-50"
          >
            <Loader2 v-if="isLoading" class="w-5 h-5 animate-spin" />
            <span v-else>Create Account</span>
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/authStore';
import { TrendingUp, ArrowLeft, Mail, Lock, User, AlertCircle, Loader2 } from '@lucide/vue';

const route = useRoute();
const router = useRouter();
const authStore = useAuthStore();

const activeTab = ref<'login' | 'register'>('login');
const loginEmail = ref('');
const loginPassword = ref('');

const regName = ref('');
const regEmail = ref('');
const regPassword = ref('');

const isLoading = ref(false);
const errorMessage = ref('');

onMounted(() => {
  if (route.query.tab === 'register') {
    activeTab.value = 'register';
  }
});

async function handleLogin() {
  isLoading.value = true;
  errorMessage.value = '';
  try {
    await authStore.login(loginEmail.value, loginPassword.value);
    router.push('/dashboard');
  } catch (err: any) {
    errorMessage.value = err.response?.data?.message || 'Invalid credentials. Please try again.';
  } finally {
    isLoading.value = false;
  }
}

async function handleRegister() {
  isLoading.value = true;
  errorMessage.value = '';
  try {
    await authStore.register(regName.value, regEmail.value, regPassword.value);
    router.push('/dashboard');
  } catch (err: any) {
    if (err.response?.data?.message) {
      errorMessage.value = err.response.data.message;
    } else if (err.response?.data?.errors) {
      const firstError = Object.values(err.response.data.errors)[0] as string[];
      errorMessage.value = firstError ? firstError[0] : 'Validation failed. Please check your inputs.';
    } else {
      errorMessage.value = 'Email already registered. Switch to "Sign In" tab above to log in.';
    }
  } finally {
    isLoading.value = false;
  }
}
</script>

<template>
  <div class="min-h-screen bg-[#09090b] text-zinc-100 flex flex-col justify-center items-center p-6 font-sans">
    <router-link to="/" class="absolute top-8 left-8 flex items-center gap-2 text-xs text-zinc-500 hover:text-zinc-300 font-mono transition">
      <ArrowLeft class="w-3.5 h-3.5" />
      Back
    </router-link>

    <div class="w-full max-w-sm">
      <!-- Header -->
      <div class="mb-6">
        <div class="flex items-center gap-2 mb-2">
          <div class="w-6 h-6 rounded bg-[#1d4ed8] flex items-center justify-center font-bold text-white text-xs">
            TC
          </div>
          <span class="text-sm font-semibold text-white">TradingCenter</span>
        </div>
        <h2 class="text-xl font-semibold text-white tracking-tight">
          {{ activeTab === 'login' ? 'Sign in to your account' : 'Create a new account' }}
        </h2>
      </div>

      <!-- Auth Card -->
      <div class="sober-panel p-6 rounded-lg">
        <!-- Tabs -->
        <div class="grid grid-cols-2 gap-1 bg-[#18181b] p-1 rounded mb-6 text-xs font-medium">
          <button
            @click="switchTab('login')"
            :class="[
              'py-1.5 rounded transition-colors',
              activeTab === 'login' ? 'bg-[#09090b] text-white shadow-sm' : 'text-zinc-400 hover:text-zinc-200'
            ]"
          >
            Sign In
          </button>
          <button
            @click="switchTab('register')"
            :class="[
              'py-1.5 rounded transition-colors',
              activeTab === 'register' ? 'bg-[#09090b] text-white shadow-sm' : 'text-zinc-400 hover:text-zinc-200'
            ]"
          >
            Register
          </button>
        </div>

        <!-- Alert Error -->
        <div v-if="errorMessage" class="mb-5 p-3 rounded bg-red-950/40 border border-red-900/60 text-red-300 text-xs flex items-start justify-between gap-2">
          <div class="flex items-start gap-2">
            <AlertCircle class="w-4 h-4 text-red-400 shrink-0 mt-0.5" />
            <span>{{ errorMessage }}</span>
          </div>
          <button v-if="showSwitchToLoginPrompt" @click="switchTab('login')" class="text-[11px] font-mono underline text-blue-400 hover:text-blue-300 shrink-0">
            Sign In Now
          </button>
        </div>

        <!-- Login Form -->
        <form v-if="activeTab === 'login'" @submit.prevent="handleLogin" class="space-y-4 text-xs">
          <div>
            <label class="block font-medium text-zinc-400 mb-1.5">Email address</label>
            <input
              v-model="loginEmail"
              type="email"
              required
              placeholder="user@example.com"
              class="w-full bg-[#18181b] border border-zinc-800 focus:border-[#1d4ed8] rounded p-2.5 text-zinc-100 placeholder-zinc-600 outline-none"
            />
          </div>

          <div>
            <label class="block font-medium text-zinc-400 mb-1.5">Password</label>
            <input
              v-model="loginPassword"
              type="password"
              required
              placeholder="••••••••"
              class="w-full bg-[#18181b] border border-zinc-800 focus:border-[#1d4ed8] rounded p-2.5 text-zinc-100 placeholder-zinc-600 outline-none"
            />
          </div>

          <button
            type="submit"
            :disabled="isLoading"
            class="w-full py-2.5 rounded font-medium japanese-blue-btn flex items-center justify-center gap-2 disabled:opacity-50 mt-2 text-xs"
          >
            <Loader2 v-if="isLoading" class="w-4 h-4 animate-spin" />
            <span v-else>Sign In</span>
          </button>
        </form>

        <!-- Register Form -->
        <form v-else @submit.prevent="handleRegister" class="space-y-4 text-xs">
          <div>
            <label class="block font-medium text-zinc-400 mb-1.5">Full name</label>
            <input
              v-model="regName"
              type="text"
              required
              placeholder="Marco Lima"
              class="w-full bg-[#18181b] border border-zinc-800 focus:border-[#1d4ed8] rounded p-2.5 text-zinc-100 placeholder-zinc-600 outline-none"
            />
          </div>

          <div>
            <label class="block font-medium text-zinc-400 mb-1.5">Email address</label>
            <input
              v-model="regEmail"
              type="email"
              required
              placeholder="user@example.com"
              class="w-full bg-[#18181b] border border-zinc-800 focus:border-[#1d4ed8] rounded p-2.5 text-zinc-100 placeholder-zinc-600 outline-none"
            />
          </div>

          <div>
            <label class="block font-medium text-zinc-400 mb-1.5">Password</label>
            <input
              v-model="regPassword"
              type="password"
              required
              placeholder="••••••••"
              class="w-full bg-[#18181b] border border-zinc-800 focus:border-[#1d4ed8] rounded p-2.5 text-zinc-100 placeholder-zinc-600 outline-none"
            />
          </div>

          <button
            type="submit"
            :disabled="isLoading"
            class="w-full py-2.5 rounded font-medium japanese-blue-btn flex items-center justify-center gap-2 disabled:opacity-50 mt-2 text-xs"
          >
            <Loader2 v-if="isLoading" class="w-4 h-4 animate-spin" />
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
import { ArrowLeft, AlertCircle, Loader2 } from '@lucide/vue';

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
const showSwitchToLoginPrompt = ref(false);

onMounted(() => {
  if (route.query.tab === 'register') {
    activeTab.value = 'register';
  }
});

function switchTab(tab: 'login' | 'register') {
  activeTab.value = tab;
  errorMessage.value = '';
  showSwitchToLoginPrompt.value = false;
  if (tab === 'login' && regEmail.value) {
    loginEmail.value = regEmail.value;
  }
}

async function handleLogin() {
  isLoading.value = true;
  errorMessage.value = '';
  showSwitchToLoginPrompt.value = false;
  try {
    await authStore.login(loginEmail.value, loginPassword.value);
    router.push('/dashboard');
  } catch (err: any) {
    errorMessage.value = err.response?.data?.message || 'Invalid email or password.';
  } finally {
    isLoading.value = false;
  }
}

async function handleRegister() {
  isLoading.value = true;
  errorMessage.value = '';
  showSwitchToLoginPrompt.value = false;
  try {
    await authStore.register(regName.value, regEmail.value, regPassword.value);
    router.push('/dashboard');
  } catch (err: any) {
    const msg = err.response?.data?.message || '';
    if (msg.includes('already exists') || err.response?.status === 400) {
      errorMessage.value = `An account for ${regEmail.value} already exists in database.`;
      showSwitchToLoginPrompt.value = true;
    } else {
      errorMessage.value = 'Registration failed. Please verify your inputs.';
    }
  } finally {
    isLoading.value = false;
  }
}
</script>

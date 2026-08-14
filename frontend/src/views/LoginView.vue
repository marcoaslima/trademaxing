<template>
  <div class="min-h-screen bg-[#f8fafc] text-slate-900 flex flex-col justify-center items-center p-6 font-sans">
    <router-link to="/" class="absolute top-8 left-8 flex items-center gap-2 text-xs text-slate-500 hover:text-slate-900 font-mono transition">
      <ArrowLeft class="w-3.5 h-3.5" />
      Voltar
    </router-link>

    <div class="w-full max-w-sm">
      <!-- Header -->
      <div class="mb-6 text-center">
        <div class="inline-flex items-center gap-2 mb-3">
          <div class="w-8 h-8 rounded-lg bg-[#059669] flex items-center justify-center font-bold text-white text-xs shadow-xs">
            TC
          </div>
          <span class="text-base font-bold text-slate-900 tracking-tight">TradingCenter</span>
        </div>
        <h2 class="text-xl font-extrabold text-slate-900 tracking-tight">
          {{ activeTab === 'login' ? 'Acessar sua conta' : 'Criar nova conta' }}
        </h2>
      </div>

      <!-- Auth Card -->
      <div class="bg-white border border-slate-200 p-6 rounded-2xl shadow-sm">
        <!-- Tabs -->
        <div class="grid grid-cols-2 gap-1 bg-slate-100 p-1 rounded-xl mb-6 text-xs font-medium">
          <button
            @click="switchTab('login')"
            :class="[
              'py-2 rounded-lg transition-colors font-bold',
              activeTab === 'login' ? 'bg-white text-slate-900 shadow-xs' : 'text-slate-500 hover:text-slate-900'
            ]"
          >
            Entrar
          </button>
          <button
            @click="switchTab('register')"
            :class="[
              'py-2 rounded-lg transition-colors font-bold',
              activeTab === 'register' ? 'bg-white text-slate-900 shadow-xs' : 'text-slate-500 hover:text-slate-900'
            ]"
          >
            Cadastrar
          </button>
        </div>

        <!-- Global Alert Message -->
        <div v-if="errorMessage" class="mb-5 p-3 rounded-xl bg-rose-50 border border-rose-200 text-rose-700 text-xs flex items-start justify-between gap-2">
          <div class="flex items-start gap-2">
            <AlertCircle class="w-4 h-4 text-rose-500 shrink-0 mt-0.5" />
            <span>{{ errorMessage }}</span>
          </div>
          <button v-if="showSwitchToLoginPrompt" @click="switchTab('login')" class="text-[11px] font-mono underline text-[#059669] hover:underline shrink-0 font-bold">
            Entrar Agora
          </button>
        </div>

        <!-- Login Form -->
        <form v-if="activeTab === 'login'" @submit.prevent="handleLogin" class="space-y-4 text-xs">
          <div>
            <label class="block font-semibold text-slate-700 mb-1.5">E-mail</label>
            <input
              v-model="loginEmail"
              type="email"
              required
              placeholder="seu@email.com"
              :class="['w-full bg-slate-50 border rounded-lg p-2.5 text-slate-900 placeholder-slate-400 outline-none transition', fieldErrors.email ? 'border-rose-500' : 'border-slate-200 focus:border-[#059669]']"
            />
            <span v-if="fieldErrors.email" class="text-rose-600 text-[11px] mt-1 block font-mono">{{ fieldErrors.email }}</span>
          </div>

          <div>
            <label class="block font-semibold text-slate-700 mb-1.5">Senha</label>
            <input
              v-model="loginPassword"
              type="password"
              required
              placeholder="••••••••"
              :class="['w-full bg-slate-50 border rounded-lg p-2.5 text-slate-900 placeholder-slate-400 outline-none transition', fieldErrors.password ? 'border-rose-500' : 'border-slate-200 focus:border-[#059669]']"
            />
            <span v-if="fieldErrors.password" class="text-rose-600 text-[11px] mt-1 block font-mono">{{ fieldErrors.password }}</span>
          </div>

          <button
            type="submit"
            :disabled="isLoading"
            class="w-full py-3 rounded-lg font-bold bg-[#059669] hover:bg-[#047857] text-white flex items-center justify-center gap-2 disabled:opacity-50 mt-2 text-xs shadow-xs transition"
          >
            <Loader2 v-if="isLoading" class="w-4 h-4 animate-spin" />
            <span v-else>Entrar na Conta</span>
          </button>
        </form>

        <!-- Register Form -->
        <form v-else @submit.prevent="handleRegister" class="space-y-4 text-xs">
          <div>
            <label class="block font-semibold text-slate-700 mb-1.5">Nome Completo</label>
            <input
              v-model="regName"
              type="text"
              required
              placeholder="Marco Lima"
              :class="['w-full bg-slate-50 border rounded-lg p-2.5 text-slate-900 placeholder-slate-400 outline-none transition', fieldErrors.name ? 'border-rose-500' : 'border-slate-200 focus:border-[#059669]']"
            />
            <span v-if="fieldErrors.name" class="text-rose-600 text-[11px] mt-1 block font-mono">{{ fieldErrors.name }}</span>
          </div>

          <div>
            <label class="block font-semibold text-slate-700 mb-1.5">E-mail</label>
            <input
              v-model="regEmail"
              type="email"
              required
              placeholder="seu@email.com"
              :class="['w-full bg-slate-50 border rounded-lg p-2.5 text-slate-900 placeholder-slate-400 outline-none transition', fieldErrors.email ? 'border-rose-500' : 'border-slate-200 focus:border-[#059669]']"
            />
            <span v-if="fieldErrors.email" class="text-rose-600 text-[11px] mt-1 block font-mono">{{ fieldErrors.email }}</span>
          </div>

          <div>
            <div class="flex justify-between items-center mb-1.5">
              <label class="font-semibold text-slate-700">Senha</label>
              <span class="text-slate-400 text-[10px] font-mono">6 a 12 caracteres</span>
            </div>
            <input
              v-model="regPassword"
              type="password"
              required
              maxlength="12"
              placeholder="••••••••"
              :class="['w-full bg-slate-50 border rounded-lg p-2.5 text-slate-900 placeholder-slate-400 outline-none transition', fieldErrors.password ? 'border-rose-500' : 'border-slate-200 focus:border-[#059669]']"
            />
            <span v-if="fieldErrors.password" class="text-rose-600 text-[11px] mt-1 block font-mono">{{ fieldErrors.password }}</span>
          </div>

          <button
            type="submit"
            :disabled="isLoading"
            class="w-full py-3 rounded-lg font-bold bg-[#059669] hover:bg-[#047857] text-white flex items-center justify-center gap-2 disabled:opacity-50 mt-2 text-xs shadow-xs transition"
          >
            <Loader2 v-if="isLoading" class="w-4 h-4 animate-spin" />
            <span v-else>Criar Minha Conta</span>
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
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

const fieldErrors = reactive<{ name?: string; email?: string; password?: string }>({});

function clearErrors() {
  errorMessage.value = '';
  showSwitchToLoginPrompt.value = false;
  fieldErrors.name = undefined;
  fieldErrors.email = undefined;
  fieldErrors.password = undefined;
}

onMounted(() => {
  if (route.query.tab === 'register') {
    activeTab.value = 'register';
  }
});

function switchTab(tab: 'login' | 'register') {
  activeTab.value = tab;
  clearErrors();
  if (tab === 'login' && regEmail.value) {
    loginEmail.value = regEmail.value;
  }
}

async function handleLogin() {
  clearErrors();
  isLoading.value = true;
  try {
    await authStore.login(loginEmail.value, loginPassword.value);
    router.push('/dashboard');
  } catch (err: any) {
    errorMessage.value = err.response?.data?.message || 'E-mail ou senha inválidos.';
  } finally {
    isLoading.value = false;
  }
}

async function handleRegister() {
  clearErrors();

  if (regName.value.length < 2) {
    fieldErrors.name = 'O nome completo deve ter pelo menos 2 caracteres.';
    errorMessage.value = 'Nome completo muito curto.';
    return;
  }
  if (regPassword.value.length < 6 || regPassword.value.length > 12) {
    fieldErrors.password = 'A senha deve ter entre 6 e 12 caracteres.';
    errorMessage.value = 'Tamanho da senha deve ter entre 6 e 12 caracteres.';
    return;
  }

  isLoading.value = true;
  try {
    await authStore.register(regName.value, regEmail.value, regPassword.value);
    router.push('/dashboard');
  } catch (err: any) {
    console.error('Registration error payload:', err.response?.data);
    const data = err.response?.data;
    
    if (data?.message?.includes('already exists')) {
      errorMessage.value = `O e-mail ${regEmail.value} já possui cadastro.`;
      fieldErrors.email = 'E-mail já cadastrado.';
      showSwitchToLoginPrompt.value = true;
    } else {
      const parsedMsg = parseApiErrors(data);
      if (parsedMsg) {
        errorMessage.value = parsedMsg;
      } else if (err.message) {
        errorMessage.value = `Erro da API: ${err.message}`;
      } else {
        errorMessage.value = 'Falha no cadastro. Verifique a conexão com o servidor.';
      }
    }
  } finally {
    isLoading.value = false;
  }
}

function parseApiErrors(data: any): string {
  if (!data) return '';

  if (data.errors && typeof data.errors === 'object') {
    const list: string[] = [];
    for (const [rawKey, val] of Object.entries(data.errors)) {
      const key = rawKey.replace(/^dto\./i, '');
      const errs = Array.isArray(val) ? val : [val];
      const msg = errs[0];
      if (!msg) continue;

      const lower = key.toLowerCase();
      if (lower.includes('email')) {
        fieldErrors.email = msg;
      } else if (lower.includes('password')) {
        fieldErrors.password = msg;
      } else if (lower.includes('name')) {
        fieldErrors.name = msg;
      }

      list.push(`${key}: ${msg}`);
    }
    if (list.length > 0) {
      return list.join(' | ');
    }
  }

  if (data.message) return data.message;
  if (data.detail) return data.detail;
  if (data.title && data.title !== 'One or more validation errors occurred.') return data.title;
  return '';
}
</script>

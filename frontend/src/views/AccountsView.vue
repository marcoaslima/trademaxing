<template>
  <div class="min-h-screen bg-[#f8fafc] text-slate-900 font-sans selection:bg-[#059669] selection:text-white flex flex-col">
    <!-- Light Header Navbar -->
    <header class="border-b border-slate-200 bg-white sticky top-0 z-40 shadow-xs">
      <div class="max-w-6xl mx-auto px-6 h-16 flex items-center justify-between">
        <div class="flex items-center gap-6">
          <router-link to="/dashboard" class="flex items-center gap-2.5">
            <div class="w-8 h-8 rounded-lg bg-[#059669] flex items-center justify-center font-bold text-white text-xs shadow-xs">
              TC
            </div>
            <span class="text-sm font-bold tracking-tight text-slate-900">TradingCenter</span>
          </router-link>

          <nav class="flex items-center gap-4 text-xs font-medium">
            <router-link to="/dashboard" class="text-slate-500 hover:text-slate-900 transition">Dashboard</router-link>
            <router-link to="/accounts" class="text-[#059669] font-bold border-b-2 border-[#059669] pb-0.5">Contas & Corretoras</router-link>
            <router-link to="/assets" class="text-slate-500 hover:text-slate-900 transition">Catálogo de Ativos</router-link>
          </nav>
        </div>

        <div class="flex items-center gap-4 text-xs">
          <div class="flex items-center gap-3 pl-3 border-l border-slate-200">
            <span class="font-medium text-slate-700 hidden sm:block">{{ authStore.user?.name || 'Investor' }}</span>
            <button
              @click="handleLogout"
              class="p-2 rounded-lg border border-slate-200 bg-slate-100 hover:bg-slate-200 text-slate-500 hover:text-slate-900 transition"
              title="Logout"
            >
              <LogOut class="w-3.5 h-3.5" />
            </button>
          </div>
        </div>
      </div>
    </header>

    <!-- Main Content -->
    <main class="flex-1 max-w-6xl w-full mx-auto px-6 py-8 space-y-6">
      <!-- Title & Action -->
      <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 border-b border-slate-200 pb-4">
        <div>
          <h1 class="text-xl font-bold text-slate-900 tracking-tight">Contas e Corretoras</h1>
          <p class="text-xs text-slate-500 mt-1">Gerencie suas contas de custódia, corretoras brasileiras, internacionais e contas bancárias.</p>
        </div>

        <button
          @click="openAddAccountModal"
          class="px-4 py-2.5 rounded-lg bg-[#059669] hover:bg-[#047857] text-white font-bold text-xs flex items-center gap-2 shadow-xs transition"
        >
          <Plus class="w-4 h-4" />
          <span>+ Nova Conta</span>
        </button>
      </div>

      <!-- Summary Stat Cards -->
      <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
        <div class="bg-white border border-slate-200 p-4 rounded-xl shadow-xs space-y-1">
          <span class="text-slate-400 text-[11px] font-semibold uppercase tracking-wider block">Total de Contas</span>
          <span class="text-2xl font-bold text-slate-900 font-mono">{{ portfolioStore.accounts.length }}</span>
        </div>
        <div class="bg-white border border-slate-200 p-4 rounded-xl shadow-xs space-y-1">
          <span class="text-slate-400 text-[11px] font-semibold uppercase tracking-wider block">Contas BRL</span>
          <span class="text-2xl font-bold text-[#059669] font-mono">{{ portfolioStore.accounts.filter(a => a.baseCurrency === 'BRL').length }}</span>
        </div>
        <div class="bg-white border border-slate-200 p-4 rounded-xl shadow-xs space-y-1">
          <span class="text-slate-400 text-[11px] font-semibold uppercase tracking-wider block">Contas USD</span>
          <span class="text-2xl font-bold text-blue-600 font-mono">{{ portfolioStore.accounts.filter(a => a.baseCurrency === 'USD').length }}</span>
        </div>
      </div>

      <!-- Accounts Table -->
      <div class="bg-white border border-slate-200 rounded-2xl overflow-hidden shadow-xs">
        <table class="w-full text-left text-xs">
          <thead class="bg-slate-50 border-b border-slate-200 text-slate-500 font-semibold text-[11px]">
            <tr>
              <th class="py-3.5 px-4">Nome da Conta</th>
              <th class="py-3.5 px-4">Instituição</th>
              <th class="py-3.5 px-4">Tipo</th>
              <th class="py-3.5 px-4">Moeda Base</th>
              <th class="py-3.5 px-4">Data de Criação</th>
              <th class="py-3.5 px-4 text-right">Ações</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100 font-mono-numbers">
            <tr v-if="portfolioStore.accounts.length === 0">
              <td colspan="6" class="py-12 text-center text-slate-400 font-sans">
                Nenhuma conta cadastrada. Clique em "+ Nova Conta" para adicionar.
              </td>
            </tr>
            <tr v-for="acc in portfolioStore.accounts" :key="acc.id" class="hover:bg-slate-50 transition">
              <td class="py-3.5 px-4 font-bold text-slate-900 font-sans">{{ acc.name }}</td>
              <td class="py-3.5 px-4 text-slate-600 font-sans">{{ acc.institution }}</td>
              <td class="py-3.5 px-4">
                <span class="px-2.5 py-1 rounded-full bg-slate-100 border border-slate-200 text-[10px] font-semibold text-slate-700 font-sans">
                  {{ acc.accountType }}
                </span>
              </td>
              <td class="py-3.5 px-4 font-semibold text-[#059669]">{{ acc.baseCurrency }}</td>
              <td class="py-3.5 px-4 text-slate-400 text-[11px]">{{ formatDateBR(acc.createdAt) }}</td>
              <td class="py-3.5 px-4 text-right">
                <div class="flex items-center justify-end gap-2 font-sans">
                  <button @click="openEditAccountModal(acc)" class="px-2.5 py-1 rounded bg-slate-100 hover:bg-slate-200 text-slate-700 text-[11px] font-semibold transition">
                    Editar
                  </button>
                  <button @click="confirmDeleteAccount(acc)" class="px-2.5 py-1 rounded bg-rose-50 hover:bg-rose-100 text-rose-600 text-[11px] font-semibold transition">
                    Excluir
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </main>

    <!-- Account Modal (Add / Edit) -->
    <div v-if="showAccountModal" class="fixed inset-0 z-50 bg-slate-900/40 backdrop-blur-xs flex items-center justify-center p-4">
      <div class="bg-white border border-slate-200 w-full max-w-md p-6 rounded-2xl shadow-xl space-y-4 text-xs">
        <div class="flex justify-between items-center border-b border-slate-100 pb-3">
          <h3 class="font-bold text-slate-900 text-sm">{{ isEditingAccount ? 'Editar Conta' : 'Nova Conta de Custódia' }}</h3>
          <button @click="showAccountModal = false" class="text-slate-400 hover:text-slate-700">✕</button>
        </div>
        <form @submit.prevent="saveAccount" class="space-y-3">
          <div>
            <label class="block text-slate-600 mb-1 font-medium">Nome Identificador</label>
            <input v-model="accountForm.name" type="text" required placeholder="Ex: XP Investimentos, Aveza Broker" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none focus:border-[#059669]" />
          </div>
          <div>
            <label class="block text-slate-600 mb-1 font-medium">Instituição Financeira</label>
            <input v-model="accountForm.institution" type="text" required placeholder="Ex: XP CCTVM, Avenue Securities" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none focus:border-[#059669]" />
          </div>
          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-slate-600 mb-1 font-medium">Tipo</label>
              <select v-model="accountForm.accountType" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none">
                <option value="Brokerage">Corretora</option>
                <option value="Personal">Pessoal</option>
                <option value="Retirement_FGTS">FGTS</option>
                <option value="Joint">Conjunta</option>
              </select>
            </div>
            <div>
              <label class="block text-slate-600 mb-1 font-medium">Moeda Base</label>
              <select v-model="accountForm.baseCurrency" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none">
                <option value="BRL">BRL (R$)</option>
                <option value="USD">USD ($)</option>
              </select>
            </div>
          </div>
          <div class="flex justify-end gap-2 pt-2">
            <button type="button" @click="showAccountModal = false" class="px-4 py-2 rounded-lg bg-slate-100 text-slate-700 hover:bg-slate-200 font-medium">Cancelar</button>
            <button type="submit" class="px-5 py-2 rounded-lg bg-[#059669] hover:bg-[#047857] text-white font-bold">Salvar Conta</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/authStore';
import { usePortfolioStore } from '@/stores/portfolioStore';
import { formatDateBR } from '@/utils/formatters';
import { Plus, LogOut } from '@lucide/vue';
import type { Account } from '@/types';

const router = useRouter();
const authStore = useAuthStore();
const portfolioStore = usePortfolioStore();

const showAccountModal = ref(false);
const isEditingAccount = ref(false);
const editingAccountId = ref<string | null>(null);

const accountForm = ref<{
  name: string;
  institution: string;
  accountType: 'Personal' | 'Joint' | 'Retirement_FGTS' | 'Brokerage';
  baseCurrency: 'BRL' | 'USD';
}>({
  name: '',
  institution: '',
  accountType: 'Brokerage',
  baseCurrency: 'BRL',
});

onMounted(async () => {
  await portfolioStore.fetchAccounts();
});

function handleLogout() {
  authStore.logout();
  router.push('/login');
}

function openAddAccountModal() {
  isEditingAccount.value = false;
  editingAccountId.value = null;
  accountForm.value = {
    name: '',
    institution: '',
    accountType: 'Brokerage',
    baseCurrency: 'BRL',
  };
  showAccountModal.value = true;
}

function openEditAccountModal(acc: Account) {
  isEditingAccount.value = true;
  editingAccountId.value = acc.id;
  accountForm.value = {
    name: acc.name,
    institution: acc.institution,
    accountType: acc.accountType,
    baseCurrency: acc.baseCurrency,
  };
  showAccountModal.value = true;
}

async function saveAccount() {
  try {
    if (isEditingAccount.value && editingAccountId.value) {
      await portfolioStore.updateAccount(editingAccountId.value, accountForm.value);
    } else {
      await portfolioStore.createAccount(accountForm.value);
    }
    showAccountModal.value = false;
    await portfolioStore.fetchAccounts();
  } catch (err: any) {
    const msg = err.response?.data?.message || err.response?.data?.title || err.message || 'Erro ao salvar conta.';
    alert(`Erro ao salvar conta: ${msg}`);
  }
}

async function confirmDeleteAccount(acc: Account) {
  if (confirm(`Excluir conta "${acc.name}"?`)) {
    try {
      await portfolioStore.deleteAccount(acc.id);
      await portfolioStore.fetchAccounts();
    } catch (err: any) {
      alert(`Erro ao excluir conta: ${err.response?.data?.message || err.message}`);
    }
  }
}
</script>

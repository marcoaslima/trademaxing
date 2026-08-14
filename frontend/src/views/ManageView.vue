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
            <router-link to="/manage" class="text-[#059669] font-bold border-b-2 border-[#059669] pb-0.5">Gerenciar Ativos & Contas</router-link>
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
      <!-- Title & Tab Selection -->
      <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 border-b border-slate-200 pb-4">
        <div>
          <h1 class="text-xl font-bold text-slate-900 tracking-tight">Console de Gerenciamento</h1>
          <p class="text-xs text-slate-500 mt-1">Cadastre, edite ou remova contas de corretora e ativos do catálogo master.</p>
        </div>

        <!-- Tab Switcher -->
        <div class="bg-slate-100 border border-slate-200 p-1 rounded-xl flex items-center gap-1 text-xs font-medium">
          <button
            @click="activeTab = 'accounts'"
            :class="['px-4 py-1.5 rounded-lg transition', activeTab === 'accounts' ? 'bg-white text-slate-900 shadow-xs font-bold' : 'text-slate-500 hover:text-slate-900']"
          >
            Contas / Corretoras ({{ portfolioStore.accounts.length }})
          </button>
          <button
            @click="activeTab = 'assets'"
            :class="['px-4 py-1.5 rounded-lg transition', activeTab === 'assets' ? 'bg-white text-slate-900 shadow-xs font-bold' : 'text-slate-500 hover:text-slate-900']"
          >
            Catálogo de Ativos ({{ portfolioStore.assets.length }})
          </button>
        </div>
      </div>

      <!-- TAB 1: BROKER ACCOUNTS -->
      <div v-if="activeTab === 'accounts'" class="space-y-4">
        <div class="flex justify-between items-center">
          <h2 class="text-sm font-bold text-slate-800">Suas Contas Cadastradas</h2>
          <button
            @click="openAddAccountModal"
            class="px-3.5 py-2 rounded-lg bg-[#059669] hover:bg-[#047857] text-white font-bold text-xs flex items-center gap-1.5 shadow-xs transition"
          >
            <Plus class="w-3.5 h-3.5" />
            <span>+ Nova Conta</span>
          </button>
        </div>

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
                  Nenhuma conta cadastrada. Clique em "+ Nova Conta" acima.
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
                <td class="py-3.5 px-4 text-slate-400 text-[11px]">{{ new Date(acc.createdAt).toLocaleDateString() }}</td>
                <td class="py-3.5 px-4 text-right space-x-2">
                  <button @click="openEditAccountModal(acc)" class="px-2.5 py-1 rounded border border-slate-200 bg-slate-50 hover:bg-slate-100 text-slate-700 font-medium">Editar</button>
                  <button @click="confirmDeleteAccount(acc)" class="px-2.5 py-1 rounded border border-rose-200 bg-rose-50 hover:bg-rose-100 text-rose-700 font-medium">Excluir</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- TAB 2: MASTER ASSETS CATALOG -->
      <div v-if="activeTab === 'assets'" class="space-y-4">
        <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-3">
          <div class="relative w-full max-w-xs">
            <Search class="w-4 h-4 text-slate-400 absolute left-3 top-2.5" />
            <input
              v-model="assetSearch"
              type="text"
              placeholder="Filtrar por nome ou ticker..."
              class="w-full bg-white border border-slate-200 rounded-lg pl-9 pr-3 py-2 text-xs text-slate-900 outline-none focus:border-[#059669]"
            />
          </div>

          <button
            @click="openAddAssetModal"
            class="px-3.5 py-2 rounded-lg bg-[#059669] hover:bg-[#047857] text-white font-bold text-xs flex items-center gap-1.5 shadow-xs transition"
          >
            <Plus class="w-3.5 h-3.5" />
            <span>+ Novo Ativo Master</span>
          </button>
        </div>

        <div class="bg-white border border-slate-200 rounded-2xl overflow-hidden shadow-xs">
          <table class="w-full text-left text-xs">
            <thead class="bg-slate-50 border-b border-slate-200 text-slate-500 font-semibold text-[11px]">
              <tr>
                <th class="py-3.5 px-4">Ativo Master</th>
                <th class="py-3.5 px-4">Ticker</th>
                <th class="py-3.5 px-4">Categoria</th>
                <th class="py-3.5 px-4">Valoração</th>
                <th class="py-3.5 px-4">Moeda</th>
                <th class="py-3.5 px-4">Indexador</th>
                <th class="py-3.5 px-4 text-right">Ações</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-slate-100 font-mono-numbers">
              <tr v-if="filteredAssets.length === 0">
                <td colspan="7" class="py-12 text-center text-slate-400 font-sans">
                  Nenhum ativo master encontrado.
                </td>
              </tr>
              <tr v-for="ast in filteredAssets" :key="ast.id" class="hover:bg-slate-50 transition">
                <td class="py-3.5 px-4">
                  <div class="flex items-center gap-3">
                    <div class="w-7 h-7 rounded-lg bg-slate-100 border border-slate-200 flex items-center justify-center shrink-0">
                      <img v-if="ast.logoUrl" :src="getLogoUrl(ast.logoUrl)" :alt="ast.name" class="w-4 h-4 object-contain" />
                      <span v-else class="text-[10px] font-bold text-slate-500">{{ ast.name.substring(0, 1) }}</span>
                    </div>
                    <span class="font-bold text-slate-900 font-sans">{{ ast.name }}</span>
                  </div>
                </td>
                <td class="py-3.5 px-4 font-mono font-bold text-[#059669]">{{ ast.ticker || '-' }}</td>
                <td class="py-3.5 px-4">
                  <span class="px-2.5 py-1 rounded-full bg-slate-100 border border-slate-200 text-[10px] font-semibold text-slate-700 font-sans">
                    {{ ast.assetCategory }}
                  </span>
                </td>
                <td class="py-3.5 px-4 text-slate-600 font-sans">{{ ast.valuationType }}</td>
                <td class="py-3.5 px-4 font-semibold text-slate-800">{{ ast.currency }}</td>
                <td class="py-3.5 px-4 text-slate-500 font-sans">{{ ast.indexBenchmark || 'Nenhum' }}</td>
                <td class="py-3.5 px-4 text-right space-x-2">
                  <button @click="openEditAssetModal(ast)" class="px-2.5 py-1 rounded border border-slate-200 bg-slate-50 hover:bg-slate-100 text-slate-700 font-medium">Editar</button>
                  <button @click="confirmDeleteAsset(ast)" class="px-2.5 py-1 rounded border border-rose-200 bg-rose-50 hover:bg-rose-100 text-rose-700 font-medium">Excluir</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </main>

    <!-- Account Modal (Add / Edit) -->
    <div v-if="showAccountModal" class="fixed inset-0 z-50 bg-slate-900/40 backdrop-blur-xs flex items-center justify-center p-4">
      <div class="bg-white border border-slate-200 w-full max-w-sm p-6 rounded-2xl shadow-xl space-y-4 text-xs">
        <div class="flex justify-between items-center border-b border-slate-100 pb-3">
          <h3 class="font-bold text-slate-900 text-sm">{{ isEditingAccount ? 'Editar Conta' : 'Nova Conta de Corretora' }}</h3>
          <button @click="showAccountModal = false" class="text-slate-400 hover:text-slate-700">✕</button>
        </div>
        <form @submit.prevent="saveAccount" class="space-y-3">
          <div>
            <label class="block text-slate-600 mb-1 font-medium">Nome da Conta</label>
            <input v-model="accountForm.name" type="text" required class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none focus:border-[#059669]" />
          </div>
          <div>
            <label class="block text-slate-600 mb-1 font-medium">Instituição</label>
            <input v-model="accountForm.institution" type="text" required class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none focus:border-[#059669]" />
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
            <button type="submit" class="px-5 py-2 rounded-lg bg-[#059669] hover:bg-[#047857] text-white font-bold">Salvar</button>
          </div>
        </form>
      </div>
    </div>

    <!-- Asset Modal (Add / Edit) -->
    <div v-if="showAssetModal" class="fixed inset-0 z-50 bg-slate-900/40 backdrop-blur-xs flex items-center justify-center p-4">
      <div class="bg-white border border-slate-200 w-full max-w-md p-6 rounded-2xl shadow-xl space-y-4 text-xs">
        <div class="flex justify-between items-center border-b border-slate-100 pb-3">
          <h3 class="font-bold text-slate-900 text-sm">{{ isEditingAsset ? 'Editar Ativo Master' : 'Novo Ativo Master' }}</h3>
          <button @click="showAssetModal = false" class="text-slate-400 hover:text-slate-700">✕</button>
        </div>
        <form @submit.prevent="saveAsset" class="space-y-3">
          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-slate-600 mb-1 font-medium">Nome do Ativo</label>
              <input v-model="assetForm.name" type="text" required class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none focus:border-[#059669]" />
            </div>
            <div>
              <label class="block text-slate-600 mb-1 font-medium">Ticker (Opcional)</label>
              <input v-model="assetForm.ticker" type="text" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none focus:border-[#059669] uppercase" />
            </div>
          </div>

          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-slate-600 mb-1 font-medium">Categoria</label>
              <select v-model="assetForm.assetCategory" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none">
                <option value="Stock_BR">Ações (BR)</option>
                <option value="Stock_US">Ações (US / Exterior)</option>
                <option value="Etf_BR">ETF (BR)</option>
                <option value="Etf_US">ETF (US / Exterior)</option>
                <option value="FixedIncome_BR">Renda Fixa</option>
                <option value="Crypto">Criptomoedas</option>
                <option value="REIT_BR">FIIs (BR)</option>
                <option value="REIT_US">REITs (US)</option>
                <option value="FGTS">FGTS</option>
                <option value="Cash">Caixa</option>
              </select>
            </div>
            <div>
              <label class="block text-slate-600 mb-1 font-medium">Tipo de Valoração</label>
              <select v-model="assetForm.valuationType" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none">
                <option value="TickerMarket">Cotação de Mercado</option>
                <option value="IndexLinked">Indexado a Índice</option>
                <option value="ManualFixedValue">Valor Fixo Manual</option>
              </select>
            </div>
          </div>

          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-slate-600 mb-1 font-medium">Moeda</label>
              <select v-model="assetForm.currency" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none">
                <option value="BRL">BRL (R$)</option>
                <option value="USD">USD ($)</option>
              </select>
            </div>
            <div>
              <label class="block text-slate-600 mb-1 font-medium">Indexador</label>
              <select v-model="assetForm.indexBenchmark" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none">
                <option value="None">Nenhum</option>
                <option value="CDI">CDI</option>
                <option value="IPCA">IPCA</option>
                <option value="SELIC">SELIC</option>
                <option value="IGPM">IGPM</option>
              </select>
            </div>
          </div>

          <div class="flex justify-end gap-2 pt-2">
            <button type="button" @click="showAssetModal = false" class="px-4 py-2 rounded-lg bg-slate-100 text-slate-700 hover:bg-slate-200 font-medium">Cancelar</button>
            <button type="submit" class="px-5 py-2 rounded-lg bg-[#059669] hover:bg-[#047857] text-white font-bold">Salvar</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/authStore';
import { usePortfolioStore } from '@/stores/portfolioStore';
import { Plus, Search, LogOut } from '@lucide/vue';
import type { Account, Asset } from '@/types';

const router = useRouter();
const authStore = useAuthStore();
const portfolioStore = usePortfolioStore();

const activeTab = ref<'accounts' | 'assets'>('accounts');
const assetSearch = ref('');

const showAccountModal = ref(false);
const isEditingAccount = ref(false);
const editingAccountId = ref<string | null>(null);

const accountForm = ref({
  name: '',
  institution: '',
  accountType: 'Brokerage',
  baseCurrency: 'BRL',
});

const showAssetModal = ref(false);
const isEditingAsset = ref(false);
const editingAssetId = ref<string | null>(null);

const assetForm = ref({
  name: '',
  ticker: '',
  assetCategory: 'Stock_BR',
  valuationType: 'TickerMarket',
  currency: 'BRL',
  indexBenchmark: 'None',
  logoUrl: '',
});

const filteredAssets = computed(() => {
  if (!assetSearch.value.trim()) return portfolioStore.assets;
  const q = assetSearch.value.toLowerCase();
  return portfolioStore.assets.filter(a =>
    a.name.toLowerCase().includes(q) || (a.ticker && a.ticker.toLowerCase().includes(q))
  );
});

onMounted(() => {
  portfolioStore.fetchAccounts();
  portfolioStore.fetchAssets();
});

function handleLogout() {
  authStore.logout();
  router.push('/login');
}

function getLogoUrl(logo: string) {
  if (logo.startsWith('http')) return logo;
  return `http://localhost:8081${logo.startsWith('/') ? '' : '/'}${logo}`;
}

// Account actions
function openAddAccountModal() {
  isEditingAccount.value = false;
  editingAccountId.value = null;
  accountForm.value = { name: '', institution: '', accountType: 'Brokerage', baseCurrency: 'BRL' };
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
  if (isEditingAccount.value && editingAccountId.value) {
    await portfolioStore.updateAccount(editingAccountId.value, accountForm.value);
  } else {
    await portfolioStore.createAccount(accountForm.value);
  }
  showAccountModal.value = false;
  await portfolioStore.fetchAccounts();
}

async function confirmDeleteAccount(acc: Account) {
  if (confirm(`Excluir conta "${acc.name}"?`)) {
    await portfolioStore.deleteAccount(acc.id);
    await portfolioStore.fetchAccounts();
  }
}

// Asset actions
function openAddAssetModal() {
  isEditingAsset.value = false;
  editingAssetId.value = null;
  assetForm.value = {
    name: '',
    ticker: '',
    assetCategory: 'Stock_BR',
    valuationType: 'TickerMarket',
    currency: 'BRL',
    indexBenchmark: 'None',
    logoUrl: '',
  };
  showAssetModal.value = true;
}

function openEditAssetModal(ast: Asset) {
  isEditingAsset.value = true;
  editingAssetId.value = ast.id;
  assetForm.value = {
    name: ast.name,
    ticker: ast.ticker || '',
    assetCategory: ast.assetCategory,
    valuationType: ast.valuationType,
    currency: ast.currency,
    indexBenchmark: ast.indexBenchmark || 'None',
    logoUrl: ast.logoUrl || '',
  };
  showAssetModal.value = true;
}

async function saveAsset() {
  const payload = {
    name: assetForm.value.name,
    ticker: assetForm.value.ticker ? assetForm.value.ticker.toUpperCase() : null,
    assetCategory: assetForm.value.assetCategory,
    valuationType: assetForm.value.valuationType,
    currency: assetForm.value.currency,
    indexBenchmark: assetForm.value.indexBenchmark,
    logoUrl: assetForm.value.logoUrl || null,
  };

  if (isEditingAsset.value && editingAssetId.value) {
    await portfolioStore.updateAsset(editingAssetId.value, payload);
  } else {
    await portfolioStore.createAsset(payload);
  }
  showAssetModal.value = false;
  await portfolioStore.fetchAssets();
}

async function confirmDeleteAsset(ast: Asset) {
  if (confirm(`Excluir ativo master "${ast.name}"?`)) {
    await portfolioStore.deleteAsset(ast.id);
    await portfolioStore.fetchAssets();
  }
}
</script>

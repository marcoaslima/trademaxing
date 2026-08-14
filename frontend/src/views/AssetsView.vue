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
            <router-link to="/accounts" class="text-slate-500 hover:text-slate-900 transition">Contas & Corretoras</router-link>
            <router-link to="/assets" class="text-[#059669] font-bold border-b-2 border-[#059669] pb-0.5">Catálogo de Ativos</router-link>
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
          <h1 class="text-xl font-bold text-slate-900 tracking-tight">Catálogo de Ativos Master</h1>
          <p class="text-xs text-slate-500 mt-1">Cadastre e configure ativos globais (ações, FIIs, Tesouro, criptos, títulos) para uso na sua carteira.</p>
        </div>

        <button
          @click="openAddAssetModal"
          class="px-4 py-2.5 rounded-lg bg-[#059669] hover:bg-[#047857] text-white font-bold text-xs flex items-center gap-2 shadow-xs transition"
        >
          <Plus class="w-4 h-4" />
          <span>+ Novo Ativo Master</span>
        </button>
      </div>

      <!-- Summary Stat Cards -->
      <div class="grid grid-cols-1 sm:grid-cols-4 gap-4">
        <div class="bg-white border border-slate-200 p-4 rounded-xl shadow-xs space-y-1">
          <span class="text-slate-400 text-[11px] font-semibold uppercase tracking-wider block">Total Ativos</span>
          <span class="text-2xl font-bold text-slate-900 font-mono">{{ portfolioStore.assets.length }}</span>
        </div>
        <div class="bg-white border border-slate-200 p-4 rounded-xl shadow-xs space-y-1">
          <span class="text-slate-400 text-[11px] font-semibold uppercase tracking-wider block">Com Ticker (Yahoo)</span>
          <span class="text-2xl font-bold text-[#059669] font-mono">{{ portfolioStore.assets.filter(a => a.valuationType === 'TickerMarket').length }}</span>
        </div>
        <div class="bg-white border border-slate-200 p-4 rounded-xl shadow-xs space-y-1">
          <span class="text-slate-400 text-[11px] font-semibold uppercase tracking-wider block">Indexados (CDI/TR/IPCA)</span>
          <span class="text-2xl font-bold text-purple-600 font-mono">{{ portfolioStore.assets.filter(a => a.valuationType === 'IndexLinked').length }}</span>
        </div>
        <div class="bg-white border border-slate-200 p-4 rounded-xl shadow-xs space-y-1">
          <span class="text-slate-400 text-[11px] font-semibold uppercase tracking-wider block">Ativos BRL / USD</span>
          <span class="text-xl font-bold text-slate-800 font-mono">
            {{ portfolioStore.assets.filter(a => a.currency === 'BRL').length }} <span class="text-xs text-slate-400 font-normal">BRL</span> / {{ portfolioStore.assets.filter(a => a.currency === 'USD').length }} <span class="text-xs text-slate-400 font-normal">USD</span>
          </span>
        </div>
      </div>

      <!-- Assets Table -->
      <div class="bg-white border border-slate-200 rounded-2xl overflow-hidden shadow-xs">
        <table class="w-full text-left text-xs">
          <thead class="bg-slate-50 border-b border-slate-200 text-slate-500 font-semibold text-[11px]">
            <tr>
              <th class="py-3.5 px-4">Nome do Ativo</th>
              <th class="py-3.5 px-4">Ticker</th>
              <th class="py-3.5 px-4">Categoria</th>
              <th class="py-3.5 px-4">Valoração</th>
              <th class="py-3.5 px-4">Moeda</th>
              <th class="py-3.5 px-4">Indexador</th>
              <th class="py-3.5 px-4 text-right">Ações</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100 font-mono-numbers">
            <tr v-if="portfolioStore.assets.length === 0">
              <td colspan="7" class="py-12 text-center text-slate-400 font-sans">
                Nenhum ativo master cadastrado. Clique em "+ Novo Ativo Master" para cadastrar.
              </td>
            </tr>
            <tr v-for="ast in portfolioStore.assets" :key="ast.id" class="hover:bg-slate-50 transition">
              <td class="py-3.5 px-4 font-bold text-slate-900 font-sans">{{ ast.name }}</td>
              <td class="py-3.5 px-4 font-mono font-bold text-[#059669]">
                {{ ast.ticker || '-' }}
              </td>
              <td class="py-3.5 px-4 font-sans">
                <span class="px-2 py-0.5 rounded bg-slate-100 border border-slate-200 text-[11px] font-medium text-slate-700">
                  {{ ast.assetCategory }}
                </span>
              </td>
              <td class="py-3.5 px-4 font-sans text-slate-600">{{ ast.valuationType }}</td>
              <td class="py-3.5 px-4 font-semibold text-slate-800">{{ ast.currency }}</td>
              <td class="py-3.5 px-4 text-slate-500 font-sans">{{ ast.indexBenchmark || 'Nenhum' }}</td>
              <td class="py-3.5 px-4 text-right">
                <div class="flex items-center justify-end gap-2 font-sans">
                  <button @click="openEditAssetModal(ast)" class="px-2.5 py-1 rounded bg-slate-100 hover:bg-slate-200 text-slate-700 text-[11px] font-semibold transition">
                    Editar
                  </button>
                  <button @click="confirmDeleteAsset(ast)" class="px-2.5 py-1 rounded bg-rose-50 hover:bg-rose-100 text-rose-600 text-[11px] font-semibold transition">
                    Excluir
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </main>

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
              <input v-model="assetForm.name" type="text" required placeholder="Ex: Apple Inc, Tesouro Selic 2029" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none focus:border-[#059669]" />
            </div>
            <div>
              <label class="block text-slate-600 mb-1 font-medium">Ticker (Yahoo Finance)</label>
              <input v-model="assetForm.ticker" type="text" placeholder="Ex: AAPL, PETR4.SA" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none focus:border-[#059669] uppercase font-mono" />
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
                <option value="FixedIncome_BR">Renda Fixa (BR)</option>
                <option value="Bond_BR_FixedIncome">Títulos Públicos / CDB</option>
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
                <option value="TickerMarket">Cotação de Mercado (Yahoo)</option>
                <option value="IndexLinked">Indexado a Índice (CDI/TR)</option>
                <option value="FixedRate">Taxa Fixa</option>
                <option value="ManualBalance">Saldo Manual</option>
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
                <option value="TR">TR</option>
                <option value="IGPM">IGPM</option>
              </select>
            </div>
          </div>

          <div class="flex justify-end gap-2 pt-2">
            <button type="button" @click="showAssetModal = false" class="px-4 py-2 rounded-lg bg-slate-100 text-slate-700 hover:bg-slate-200 font-medium">Cancelar</button>
            <button type="submit" class="px-5 py-2 rounded-lg bg-[#059669] hover:bg-[#047857] text-white font-bold">Salvar Ativo</button>
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
import { Plus, LogOut } from '@lucide/vue';
import type { Asset, AssetCategory, ValuationType, IndexBenchmark } from '@/types';

const router = useRouter();
const authStore = useAuthStore();
const portfolioStore = usePortfolioStore();

const showAssetModal = ref(false);
const isEditingAsset = ref(false);
const editingAssetId = ref<string | null>(null);

const assetForm = ref<{
  name: string;
  ticker: string;
  assetCategory: AssetCategory;
  valuationType: ValuationType;
  currency: 'BRL' | 'USD';
  indexBenchmark: IndexBenchmark;
  logoUrl: string;
}>({
  name: '',
  ticker: '',
  assetCategory: 'Stock_BR',
  valuationType: 'TickerMarket',
  currency: 'BRL',
  indexBenchmark: 'None',
  logoUrl: '',
});

onMounted(async () => {
  await portfolioStore.fetchAssets();
});

function handleLogout() {
  authStore.logout();
  router.push('/login');
}

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

  try {
    if (isEditingAsset.value && editingAssetId.value) {
      await portfolioStore.updateAsset(editingAssetId.value, payload);
    } else {
      await portfolioStore.createAsset(payload);
    }
    showAssetModal.value = false;
    await portfolioStore.fetchAssets();
  } catch (err: any) {
    const msg = err.response?.data?.message || err.response?.data?.title || err.message || 'Erro ao salvar ativo.';
    alert(`Erro ao salvar ativo: ${msg}`);
  }
}

async function confirmDeleteAsset(ast: Asset) {
  if (confirm(`Excluir ativo master "${ast.name}"?`)) {
    try {
      await portfolioStore.deleteAsset(ast.id);
      await portfolioStore.fetchAssets();
    } catch (err: any) {
      alert(`Erro ao excluir ativo master: ${err.response?.data?.message || err.message}`);
    }
  }
}
</script>

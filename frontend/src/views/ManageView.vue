<template>
  <div class="min-h-screen bg-[#09090b] text-zinc-100 font-sans selection:bg-[#2563eb] selection:text-white flex flex-col">
    <!-- Navbar -->
    <header class="border-b border-zinc-800 bg-[#09090b] sticky top-0 z-40">
      <div class="max-w-6xl mx-auto px-6 h-16 flex items-center justify-between">
        <div class="flex items-center gap-6">
          <router-link to="/dashboard" class="flex items-center gap-2.5">
            <div class="w-7 h-7 rounded bg-[#1d4ed8] flex items-center justify-center font-bold text-white text-xs">
              TC
            </div>
            <span class="text-sm font-semibold tracking-tight text-white">TradingCenter</span>
          </router-link>

          <nav class="flex items-center gap-4 text-xs font-medium">
            <router-link to="/dashboard" class="text-zinc-400 hover:text-zinc-200 transition">Dashboard</router-link>
            <router-link to="/manage" class="text-white border-b-2 border-blue-600 pb-0.5">Manage Assets & Accounts</router-link>
          </nav>
        </div>

        <div class="flex items-center gap-4 text-xs">
          <div class="flex items-center gap-3 pl-3 border-l border-zinc-800">
            <span class="font-medium text-zinc-200 hidden sm:block">{{ authStore.user?.name || 'Investor' }}</span>
            <button
              @click="handleLogout"
              class="p-2 rounded border border-zinc-800 bg-[#18181b] hover:bg-zinc-800 text-zinc-400 hover:text-white transition"
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
      <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 border-b border-zinc-800 pb-4">
        <div>
          <h1 class="text-xl font-bold text-white tracking-tight">Management Console</h1>
          <p class="text-xs text-zinc-400 mt-1">Add, edit, or delete broker accounts and master asset catalog items.</p>
        </div>

        <!-- Main Tab Switcher -->
        <div class="bg-[#18181b] border border-zinc-800 p-1 rounded-lg flex items-center gap-1 text-xs font-medium">
          <button
            @click="activeTab = 'accounts'"
            :class="['px-4 py-1.5 rounded transition', activeTab === 'accounts' ? 'bg-[#09090b] text-white shadow-sm font-semibold' : 'text-zinc-400 hover:text-zinc-200']"
          >
            Broker Accounts ({{ portfolioStore.accounts.length }})
          </button>
          <button
            @click="activeTab = 'assets'"
            :class="['px-4 py-1.5 rounded transition', activeTab === 'assets' ? 'bg-[#09090b] text-white shadow-sm font-semibold' : 'text-zinc-400 hover:text-zinc-200']"
          >
            Master Asset Catalog ({{ portfolioStore.assets.length }})
          </button>
        </div>
      </div>

      <!-- TAB 1: BROKER ACCOUNTS -->
      <section v-if="activeTab === 'accounts'" class="space-y-4">
        <div class="flex justify-between items-center bg-[#121215] p-3 rounded-lg border border-zinc-800 text-xs">
          <span class="text-zinc-400">Registered Brokerage & Personal Financial Accounts</span>
          <button @click="openAddAccountModal" class="px-4 py-1.5 rounded japanese-blue-btn font-medium">
            + New Broker Account
          </button>
        </div>

        <div class="sober-panel rounded-lg overflow-hidden">
          <table class="w-full text-left text-xs">
            <thead class="bg-[#18181b] text-zinc-400 font-mono border-b border-zinc-800">
              <tr>
                <th class="py-3 px-4 font-normal">Account Name</th>
                <th class="py-3 px-4 font-normal">Institution</th>
                <th class="py-3 px-4 font-normal">Type</th>
                <th class="py-3 px-4 font-normal">Currency</th>
                <th class="py-3 px-4 font-normal text-right">Actions</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-zinc-800/60 font-mono-numbers">
              <tr v-if="portfolioStore.accounts.length === 0">
                <td colspan="5" class="py-12 text-center text-zinc-500 font-sans">
                  No broker accounts found. Click "+ New Broker Account" to create one.
                </td>
              </tr>
              <tr v-for="acc in portfolioStore.accounts" :key="acc.id" class="hover:bg-zinc-900/60 transition">
                <td class="py-3.5 px-4 font-sans font-semibold text-white">
                  {{ acc.name }}
                </td>
                <td class="py-3.5 px-4 font-sans text-zinc-300">
                  {{ acc.institution }}
                </td>
                <td class="py-3.5 px-4 font-sans text-zinc-400">
                  <span class="px-2 py-0.5 rounded bg-zinc-900 border border-zinc-800 text-[11px]">
                    {{ acc.accountType }}
                  </span>
                </td>
                <td class="py-3.5 px-4 font-mono text-zinc-200">
                  {{ acc.baseCurrency }}
                </td>
                <td class="py-3.5 px-4 text-right space-x-2 font-sans">
                  <button @click="openEditAccountModal(acc)" class="px-2.5 py-1 rounded bg-zinc-800 hover:bg-zinc-700 text-zinc-200 transition text-[11px]">
                    Edit
                  </button>
                  <button @click="confirmDeleteAccount(acc)" class="px-2.5 py-1 rounded bg-red-950/60 border border-red-900/60 hover:bg-red-900 text-red-300 transition text-[11px]">
                    Delete
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>

      <!-- TAB 2: MASTER ASSETS -->
      <section v-else class="space-y-4">
        <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-3 bg-[#121215] p-3 rounded-lg border border-zinc-800 text-xs">
          <!-- Search Input -->
          <div class="flex items-center gap-2 w-full sm:w-auto flex-1 max-w-md">
            <input
              v-model="assetSearch"
              type="text"
              placeholder="Search assets by name or ticker..."
              class="w-full bg-[#18181b] border border-zinc-800 focus:border-blue-500 rounded p-2 text-white outline-none"
            />
          </div>

          <button @click="openAddAssetModal" class="w-full sm:w-auto px-4 py-1.5 rounded japanese-blue-btn font-medium">
            + New Master Asset
          </button>
        </div>

        <div class="sober-panel rounded-lg overflow-hidden">
          <table class="w-full text-left text-xs">
            <thead class="bg-[#18181b] text-zinc-400 font-mono border-b border-zinc-800">
              <tr>
                <th class="py-3 px-4 font-normal">Asset</th>
                <th class="py-3 px-3 font-normal">Ticker</th>
                <th class="py-3 px-3 font-normal">Category</th>
                <th class="py-3 px-3 font-normal">Valuation</th>
                <th class="py-3 px-3 font-normal">Currency</th>
                <th class="py-3 px-3 font-normal">Benchmark</th>
                <th class="py-3 px-4 font-normal text-right">Actions</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-zinc-800/60 font-mono-numbers">
              <tr v-if="filteredAssets.length === 0">
                <td colspan="7" class="py-12 text-center text-zinc-500 font-sans">
                  No master assets found matching search criteria.
                </td>
              </tr>
              <tr v-for="ast in filteredAssets" :key="ast.id" class="hover:bg-zinc-900/60 transition">
                <td class="py-3.5 px-4">
                  <div class="flex items-center gap-2.5">
                    <div class="w-6 h-6 rounded bg-zinc-900 border border-zinc-800 flex items-center justify-center shrink-0">
                      <img v-if="ast.logoUrl" :src="getLogoUrl(ast.logoUrl)" :alt="ast.name" class="w-4 h-4 object-contain" />
                      <span v-else class="text-[10px] font-bold text-zinc-500">{{ ast.name.substring(0, 1) }}</span>
                    </div>
                    <span class="font-medium text-white font-sans">{{ ast.name }}</span>
                  </div>
                </td>
                <td class="py-3.5 px-3 font-mono text-blue-400 font-semibold">
                  {{ ast.ticker || '-' }}
                </td>
                <td class="py-3.5 px-3 font-sans text-zinc-300">
                  {{ ast.assetCategory }}
                </td>
                <td class="py-3.5 px-3 font-sans text-zinc-400">
                  {{ ast.valuationType }}
                </td>
                <td class="py-3.5 px-3 font-mono text-zinc-200">
                  {{ ast.currency }}
                </td>
                <td class="py-3.5 px-3 font-sans text-zinc-400">
                  {{ ast.indexBenchmark || 'None' }}
                </td>
                <td class="py-3.5 px-4 text-right space-x-2 font-sans">
                  <button @click="openEditAssetModal(ast)" class="px-2.5 py-1 rounded bg-zinc-800 hover:bg-zinc-700 text-zinc-200 transition text-[11px]">
                    Edit
                  </button>
                  <button @click="confirmDeleteAsset(ast)" class="px-2.5 py-1 rounded bg-red-950/60 border border-red-900/60 hover:bg-red-900 text-red-300 transition text-[11px]">
                    Delete
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>
    </main>

    <!-- Modal: Account (Create / Edit) -->
    <div v-if="showAccountModal" class="fixed inset-0 z-50 bg-black/80 flex items-center justify-center p-4">
      <div class="sober-panel w-full max-w-sm p-5 rounded-lg space-y-4 text-xs">
        <div class="flex justify-between items-center">
          <h3 class="font-semibold text-white">{{ editingAccountId ? 'Edit Broker Account' : 'New Broker Account' }}</h3>
          <button @click="showAccountModal = false" class="text-zinc-500 hover:text-white">✕</button>
        </div>
        <form @submit.prevent="saveAccount" class="space-y-3">
          <div>
            <label class="block text-zinc-400 mb-1">Account Name</label>
            <input v-model="accountForm.name" type="text" required placeholder="XP Brokerage US" class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none focus:border-blue-500" />
          </div>
          <div>
            <label class="block text-zinc-400 mb-1">Institution</label>
            <input v-model="accountForm.institution" type="text" required placeholder="XP / IBKR / Caixa" class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none focus:border-blue-500" />
          </div>
          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-zinc-400 mb-1">Type</label>
              <select v-model="accountForm.accountType" class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none">
                <option value="Brokerage">Brokerage</option>
                <option value="Personal">Personal</option>
                <option value="Retirement_FGTS">Retirement FGTS</option>
                <option value="Joint">Joint</option>
              </select>
            </div>
            <div>
              <label class="block text-zinc-400 mb-1">Currency</label>
              <select v-model="accountForm.baseCurrency" class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none">
                <option value="BRL">BRL</option>
                <option value="USD">USD</option>
              </select>
            </div>
          </div>
          <div class="flex justify-end gap-2 pt-2">
            <button type="button" @click="showAccountModal = false" class="px-3 py-1.5 rounded bg-zinc-800 text-zinc-300">Cancel</button>
            <button type="submit" class="px-4 py-1.5 rounded japanese-blue-btn">Save Account</button>
          </div>
        </form>
      </div>
    </div>

    <!-- Modal: Asset (Create / Edit) -->
    <div v-if="showAssetModal" class="fixed inset-0 z-50 bg-black/80 flex items-center justify-center p-4">
      <div class="sober-panel w-full max-w-md p-5 rounded-lg space-y-4 text-xs">
        <div class="flex justify-between items-center">
          <h3 class="font-semibold text-white">{{ editingAssetId ? 'Edit Master Asset' : 'New Master Asset' }}</h3>
          <button @click="showAssetModal = false" class="text-zinc-500 hover:text-white">✕</button>
        </div>
        <form @submit.prevent="saveAsset" class="space-y-3">
          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-zinc-400 mb-1">Asset Name</label>
              <input v-model="assetForm.name" type="text" required placeholder="Apple Inc" class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none focus:border-blue-500" />
            </div>
            <div>
              <label class="block text-zinc-400 mb-1">Ticker (Optional)</label>
              <input v-model="assetForm.ticker" type="text" placeholder="AAPL / PETR4" class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none focus:border-blue-500 uppercase" />
            </div>
          </div>

          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-zinc-400 mb-1">Category</label>
              <select v-model="assetForm.assetCategory" class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none">
                <option value="Stock_BR">Stock (BR)</option>
                <option value="Stock_US">Stock (US)</option>
                <option value="Etf_BR">ETF (BR)</option>
                <option value="Etf_US">ETF (US)</option>
                <option value="FixedIncome_BR">Fixed Income (BR)</option>
                <option value="Crypto">Crypto</option>
                <option value="REIT_BR">FII (BR)</option>
                <option value="REIT_US">REIT (US)</option>
                <option value="FGTS">FGTS</option>
                <option value="Cash">Cash</option>
              </select>
            </div>
            <div>
              <label class="block text-zinc-400 mb-1">Valuation Type</label>
              <select v-model="assetForm.valuationType" class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none">
                <option value="TickerMarket">Ticker Market</option>
                <option value="IndexLinked">Index Linked</option>
                <option value="ManualFixedValue">Manual Fixed Value</option>
              </select>
            </div>
          </div>

          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-zinc-400 mb-1">Currency</label>
              <select v-model="assetForm.currency" class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none">
                <option value="BRL">BRL</option>
                <option value="USD">USD</option>
              </select>
            </div>
            <div>
              <label class="block text-zinc-400 mb-1">Index Benchmark</label>
              <select v-model="assetForm.indexBenchmark" class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none">
                <option value="None">None</option>
                <option value="CDI">CDI</option>
                <option value="IPCA">IPCA</option>
                <option value="SELIC">SELIC</option>
                <option value="IGPM">IGPM</option>
              </select>
            </div>
          </div>

          <div>
            <label class="block text-zinc-400 mb-1">Logo URL (Optional)</label>
            <input v-model="assetForm.logoUrl" type="text" placeholder="/logos/aapl.png or https://..." class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none" />
          </div>

          <div class="flex justify-end gap-2 pt-2">
            <button type="button" @click="showAssetModal = false" class="px-3 py-1.5 rounded bg-zinc-800 text-zinc-300">Cancel</button>
            <button type="submit" class="px-4 py-1.5 rounded japanese-blue-btn">Save Asset</button>
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
import { LogOut } from '@lucide/vue';
import type { Account, Asset } from '@/types';

const router = useRouter();
const authStore = useAuthStore();
const portfolioStore = usePortfolioStore();

const activeTab = ref<'accounts' | 'assets'>('accounts');
const assetSearch = ref('');

// Account Form & State
const showAccountModal = ref(false);
const editingAccountId = ref<string | null>(null);
const accountForm = ref({
  name: '',
  institution: '',
  accountType: 'Brokerage',
  baseCurrency: 'BRL',
});

// Asset Form & State
const showAssetModal = ref(false);
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

// Account Handlers
function openAddAccountModal() {
  editingAccountId.value = null;
  accountForm.value = { name: '', institution: '', accountType: 'Brokerage', baseCurrency: 'BRL' };
  showAccountModal.value = true;
}

function openEditAccountModal(acc: Account) {
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
  if (editingAccountId.value) {
    await portfolioStore.updateAccount(editingAccountId.value, accountForm.value);
  } else {
    await portfolioStore.createAccount(accountForm.value);
  }
  showAccountModal.value = false;
}

async function confirmDeleteAccount(acc: Account) {
  if (confirm(`Are you sure you want to delete broker account "${acc.name}"?`)) {
    await portfolioStore.deleteAccount(acc.id);
  }
}

// Asset Handlers
function openAddAssetModal() {
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

  if (editingAssetId.value) {
    await portfolioStore.updateAsset(editingAssetId.value, payload);
  } else {
    await portfolioStore.createAsset(payload);
  }
  showAssetModal.value = false;
}

async function confirmDeleteAsset(ast: Asset) {
  if (confirm(`Are you sure you want to delete master asset "${ast.name}"?`)) {
    await portfolioStore.deleteAsset(ast.id);
  }
}
</script>

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
            <router-link to="/dashboard" class="text-white border-b-2 border-blue-600 pb-0.5">Dashboard</router-link>
            <router-link to="/manage" class="text-zinc-400 hover:text-zinc-200 transition">Manage Assets & Accounts</router-link>
          </nav>
        </div>

        <div class="flex items-center gap-4 text-xs">
          <!-- Currency Display Toggle -->
          <div class="bg-[#18181b] border border-zinc-800 p-0.5 rounded flex items-center">
            <button
              @click="displayCurrency = 'BRL'"
              :class="['px-2.5 py-1 rounded transition font-mono', displayCurrency === 'BRL' ? 'bg-[#09090b] text-white font-semibold shadow-sm' : 'text-zinc-400 hover:text-zinc-200']"
            >
              BRL (R$)
            </button>
            <button
              @click="displayCurrency = 'USD'"
              :class="['px-2.5 py-1 rounded transition font-mono', displayCurrency === 'USD' ? 'bg-[#09090b] text-white font-semibold shadow-sm' : 'text-zinc-400 hover:text-zinc-200']"
            >
              USD ($)
            </button>
          </div>

          <!-- Refresh Data -->
          <button
            @click="loadData"
            class="p-2 rounded border border-zinc-800 bg-[#18181b] hover:bg-zinc-800 text-zinc-400 hover:text-white transition"
            title="Refresh Portfolio Data"
          >
            <RefreshCw :class="['w-3.5 h-3.5', portfolioStore.isLoading ? 'animate-spin' : '']" />
          </button>

          <!-- User Info & Logout -->
          <div class="flex items-center gap-3 pl-3 border-l border-zinc-800">
            <div class="text-right hidden sm:block">
              <span class="block font-medium text-zinc-200">{{ authStore.user?.name || 'Investor' }}</span>
            </div>
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
      <!-- Loading State -->
      <div v-if="portfolioStore.isLoading && !summary" class="py-20 text-center text-xs text-zinc-500 font-mono">
        Loading portfolio engine...
      </div>

      <template v-else>
        <!-- Metric Cards Grid -->
        <section class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
          <!-- Total Net Worth -->
          <div class="sober-panel p-5 rounded-lg">
            <span class="text-xs font-mono text-zinc-500 block mb-2">TOTAL NET WORTH</span>
            <div class="text-2xl font-bold text-white font-mono-numbers">
              {{ formatCurrency(displayCurrency === 'BRL' ? (summary?.totalNetWorthBrl || 0) : (summary?.totalNetWorthUsd || 0)) }}
            </div>
            <span class="text-xs font-mono text-zinc-500 mt-2 block">
              Equiv: {{ formatCurrency(displayCurrency === 'BRL' ? (summary?.totalNetWorthUsd || 0) : (summary?.totalNetWorthBrl || 0), displayCurrency === 'BRL' ? 'USD' : 'BRL') }}
            </span>
          </div>

          <!-- Cost Basis -->
          <div class="sober-panel p-5 rounded-lg">
            <span class="text-xs font-mono text-zinc-500 block mb-2">INVESTED CAPITAL</span>
            <div class="text-2xl font-bold text-white font-mono-numbers">
              {{ formatCurrency(displayCurrency === 'BRL' ? (summary?.totalInvestedBrl || 0) : (summary?.totalInvestedUsd || 0)) }}
            </div>
            <span class="text-xs font-mono text-zinc-500 mt-2 block">Total Cost Basis</span>
          </div>

          <!-- Net Return -->
          <div class="sober-panel p-5 rounded-lg">
            <span class="text-xs font-mono text-zinc-500 block mb-2">NET GAIN / LOSS</span>
            <div :class="['text-2xl font-bold font-mono-numbers', (summary?.netGainLossBrl || 0) >= 0 ? 'text-emerald-400' : 'text-rose-400']">
              {{ (summary?.netGainLossBrl || 0) >= 0 ? '+' : '' }}{{ formatCurrency(displayCurrency === 'BRL' ? (summary?.netGainLossBrl || 0) : (summary?.netGainLossUsd || 0)) }}
            </div>
            <span :class="['text-xs font-mono mt-2 block font-semibold', (summary?.netGainLossBrl || 0) >= 0 ? 'text-emerald-400' : 'text-rose-400']">
              Return: {{ calculateOverallReturnPct() }}%
            </span>
          </div>

          <!-- PTAX Rate -->
          <div class="sober-panel p-5 rounded-lg">
            <span class="text-xs font-mono text-zinc-500 block mb-2">OFFICIAL PTAX RATE</span>
            <div class="text-2xl font-bold text-white font-mono-numbers">
              R$ {{ (summary?.usdBrlFxRate || 5.50).toFixed(4) }}
            </div>
            <span class="text-xs font-mono text-zinc-500 mt-2 block">USD/BRL</span>
          </div>
        </section>

        <!-- Actions Toolbar -->
        <section class="flex flex-col sm:flex-row justify-between items-center gap-3 bg-[#121215] p-3 rounded-lg border border-zinc-800 text-xs">
          <div class="flex items-center gap-2 w-full sm:w-auto">
            <button
              @click="showAddAccountModal = true"
              class="px-3 py-1.5 rounded border border-zinc-800 bg-[#18181b] hover:bg-zinc-800 text-zinc-300 transition"
            >
              + Broker Account
            </button>
            <button
              @click="showAddAssetModal = true"
              class="px-3 py-1.5 rounded border border-zinc-800 bg-[#18181b] hover:bg-zinc-800 text-zinc-300 transition"
            >
              + Master Asset
            </button>
          </div>

          <button
            @click="openAddInvestmentModal"
            class="w-full sm:w-auto px-4 py-1.5 rounded japanese-blue-btn font-medium text-xs"
          >
            + Add Holding
          </button>
        </section>

        <!-- Positions Table -->
        <section class="sober-panel rounded-lg overflow-hidden">
          <div class="p-4 border-b border-zinc-800/80 flex items-center justify-between">
            <h2 class="text-xs font-mono text-zinc-400 uppercase tracking-wider">Asset Positions</h2>
            <span class="text-xs font-mono text-zinc-500">{{ positions.length }} item(s)</span>
          </div>

          <div class="overflow-x-auto">
            <table class="w-full text-left text-xs">
              <thead class="bg-[#18181b] text-zinc-400 font-mono border-b border-zinc-800">
                <tr>
                  <th class="py-3 px-4 font-normal">Asset</th>
                  <th class="py-3 px-3 font-normal">Category</th>
                  <th class="py-3 px-3 font-normal text-right">Qty</th>
                  <th class="py-3 px-3 font-normal text-right">Avg Cost</th>
                  <th class="py-3 px-3 font-normal text-right">Current Price</th>
                  <th class="py-3 px-3 font-normal text-right">Total Value</th>
                  <th class="py-3 px-4 font-normal text-right">Return</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-zinc-800/60 font-mono-numbers">
                <tr v-if="positions.length === 0">
                  <td colspan="7" class="py-12 text-center text-zinc-500 font-sans">
                    No active positions found. Click "+ Add Holding" above to add investments.
                  </td>
                </tr>

                <tr v-for="pos in positions" :key="pos.investmentId" class="hover:bg-zinc-900/60 transition">
                  <td class="py-3 px-4">
                    <div class="flex items-center gap-2.5">
                      <div class="w-6 h-6 rounded bg-zinc-900 border border-zinc-800 flex items-center justify-center shrink-0">
                        <img v-if="pos.logoUrl" :src="getLogoUrl(pos.logoUrl)" :alt="pos.name" class="w-4 h-4 object-contain" />
                        <span v-else class="text-[10px] font-bold text-zinc-500">{{ pos.name.substring(0, 1) }}</span>
                      </div>
                      <div>
                        <span class="block font-medium text-white font-sans">{{ pos.name }}</span>
                        <span v-if="pos.ticker" class="text-[11px] text-zinc-500">{{ pos.ticker }}</span>
                      </div>
                    </div>
                  </td>

                  <td class="py-3 px-3 font-sans text-zinc-400">
                    {{ pos.assetCategory }}
                  </td>

                  <td class="py-3 px-3 text-right text-zinc-200">
                    {{ pos.quantity.toLocaleString() }}
                  </td>

                  <td class="py-3 px-3 text-right text-zinc-400">
                    {{ formatCurrency(pos.averagePrice, pos.currency) }}
                  </td>

                  <td class="py-3 px-3 text-right text-blue-400 font-semibold">
                    {{ formatCurrency(pos.currentUnitPrice, pos.currency) }}
                  </td>

                  <td class="py-3 px-3 text-right font-bold text-white">
                    {{ formatCurrency(pos.currentTotalValue, pos.currency) }}
                  </td>

                  <td class="py-3 px-4 text-right">
                    <span :class="[pos.unrealizedGainLoss >= 0 ? 'text-emerald-400' : 'text-rose-400']">
                      {{ pos.unrealizedGainLoss >= 0 ? '+' : '' }}{{ formatCurrency(pos.unrealizedGainLoss, pos.currency) }}
                    </span>
                    <span :class="['block text-[10px]', pos.unrealizedGainLoss >= 0 ? 'text-emerald-500' : 'text-rose-500']">
                      ({{ pos.unrealizedGainLossPercentage >= 0 ? '+' : '' }}{{ pos.unrealizedGainLossPercentage.toFixed(2) }}%)
                    </span>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </section>
      </template>
    </main>

    <!-- Modal 1: Add Broker Account -->
    <div v-if="showAddAccountModal" class="fixed inset-0 z-50 bg-black/80 flex items-center justify-center p-4">
      <div class="sober-panel w-full max-w-sm p-5 rounded-lg space-y-4 text-xs">
        <div class="flex justify-between items-center">
          <h3 class="font-semibold text-white">Add Broker Account</h3>
          <button @click="showAddAccountModal = false" class="text-zinc-500 hover:text-white">✕</button>
        </div>
        <form @submit.prevent="submitAddAccount" class="space-y-3">
          <div>
            <label class="block text-zinc-400 mb-1">Account Name</label>
            <input v-model="newAccount.name" type="text" required placeholder="XP Brokerage US" class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none focus:border-blue-500" />
          </div>
          <div>
            <label class="block text-zinc-400 mb-1">Institution</label>
            <input v-model="newAccount.institution" type="text" required placeholder="XP / IBKR / Caixa" class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none focus:border-blue-500" />
          </div>
          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-zinc-400 mb-1">Type</label>
              <select v-model="newAccount.accountType" class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none">
                <option value="Brokerage">Brokerage</option>
                <option value="Personal">Personal</option>
                <option value="Retirement_FGTS">Retirement FGTS</option>
                <option value="Joint">Joint</option>
              </select>
            </div>
            <div>
              <label class="block text-zinc-400 mb-1">Currency</label>
              <select v-model="newAccount.baseCurrency" class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none">
                <option value="BRL">BRL</option>
                <option value="USD">USD</option>
              </select>
            </div>
          </div>
          <div class="flex justify-end gap-2 pt-2">
            <button type="button" @click="showAddAccountModal = false" class="px-3 py-1.5 rounded bg-zinc-800 text-zinc-300">Cancel</button>
            <button type="submit" class="px-4 py-1.5 rounded japanese-blue-btn">Save Account</button>
          </div>
        </form>
      </div>
    </div>

    <!-- Modal 2: Add Master Asset -->
    <div v-if="showAddAssetModal" class="fixed inset-0 z-50 bg-black/80 flex items-center justify-center p-4">
      <div class="sober-panel w-full max-w-md p-5 rounded-lg space-y-4 text-xs">
        <div class="flex justify-between items-center">
          <h3 class="font-semibold text-white">Add Master Asset Catalog Item</h3>
          <button @click="showAddAssetModal = false" class="text-zinc-500 hover:text-white">✕</button>
        </div>
        <form @submit.prevent="submitAddAsset" class="space-y-3">
          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-zinc-400 mb-1">Asset Name</label>
              <input v-model="newAsset.name" type="text" required placeholder="Apple Inc" class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none focus:border-blue-500" />
            </div>
            <div>
              <label class="block text-zinc-400 mb-1">Ticker (Optional)</label>
              <input v-model="newAsset.ticker" type="text" placeholder="AAPL / PETR4" class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none focus:border-blue-500 uppercase" />
            </div>
          </div>

          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-zinc-400 mb-1">Category</label>
              <select v-model="newAsset.assetCategory" class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none">
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
              <select v-model="newAsset.valuationType" class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none">
                <option value="TickerMarket">Ticker Market</option>
                <option value="IndexLinked">Index Linked</option>
                <option value="ManualFixedValue">Manual Fixed Value</option>
              </select>
            </div>
          </div>

          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-zinc-400 mb-1">Currency</label>
              <select v-model="newAsset.currency" class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none">
                <option value="BRL">BRL</option>
                <option value="USD">USD</option>
              </select>
            </div>
            <div>
              <label class="block text-zinc-400 mb-1">Index Benchmark</label>
              <select v-model="newAsset.indexBenchmark" class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none">
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
            <input v-model="newAsset.logoUrl" type="text" placeholder="/logos/aapl.png or https://..." class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none" />
          </div>

          <div class="flex justify-end gap-2 pt-2">
            <button type="button" @click="showAddAssetModal = false" class="px-3 py-1.5 rounded bg-zinc-800 text-zinc-300">Cancel</button>
            <button type="submit" class="px-4 py-1.5 rounded japanese-blue-btn">Save Asset</button>
          </div>
        </form>
      </div>
    </div>

    <!-- Modal 3: Add Holding / Investment -->
    <div v-if="showAddInvestmentModal" class="fixed inset-0 z-50 bg-black/80 flex items-center justify-center p-4">
      <div class="sober-panel w-full max-w-md p-5 rounded-lg space-y-4 text-xs">
        <div class="flex justify-between items-center">
          <h3 class="font-semibold text-white">Add Holding Position</h3>
          <button @click="showAddInvestmentModal = false" class="text-zinc-500 hover:text-white">✕</button>
        </div>
        <form @submit.prevent="submitAddInvestment" class="space-y-3">
          <div>
            <label class="block text-zinc-400 mb-1">Broker Account</label>
            <select v-model="newHolding.accountId" required class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none">
              <option value="" disabled>-- Select Broker Account --</option>
              <option v-for="acc in portfolioStore.accounts" :key="acc.id" :value="acc.id">
                {{ acc.name }} ({{ acc.institution }} - {{ acc.baseCurrency }})
              </option>
            </select>
          </div>

          <div>
            <label class="block text-zinc-400 mb-1">Master Asset</label>
            <select v-model="newHolding.assetId" required class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none">
              <option value="" disabled>-- Select Master Asset --</option>
              <option v-for="ast in portfolioStore.assets" :key="ast.id" :value="ast.id">
                {{ ast.name }} {{ ast.ticker ? `(${ast.ticker})` : '' }} - {{ ast.assetCategory }} ({{ ast.currency }})
              </option>
            </select>
          </div>

          <div>
            <label class="block text-zinc-400 mb-1">Custom Label / Note (Optional)</label>
            <input v-model="newHolding.customName" type="text" placeholder="e.g. My Long Term Holding" class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none" />
          </div>

          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-zinc-400 mb-1">Quantity</label>
              <input v-model.number="newHolding.quantity" type="number" step="any" min="0.00000001" required placeholder="10" class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none focus:border-blue-500" />
            </div>
            <div>
              <label class="block text-zinc-400 mb-1">Purchase Price per Unit</label>
              <input v-model.number="newHolding.pricePerUnit" type="number" step="any" min="0.01" required placeholder="150.00" class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none focus:border-blue-500" />
            </div>
          </div>

          <div>
            <label class="block text-zinc-400 mb-1">Purchase Date</label>
            <input v-model="newHolding.transactionDate" type="date" required class="w-full bg-[#18181b] border border-zinc-800 rounded p-2 text-white outline-none" />
          </div>

          <div class="flex justify-end gap-2 pt-2">
            <button type="button" @click="showAddInvestmentModal = false" class="px-3 py-1.5 rounded bg-zinc-800 text-zinc-300">Cancel</button>
            <button type="submit" class="px-4 py-1.5 rounded japanese-blue-btn">Save Holding</button>
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
import { RefreshCw, LogOut } from '@lucide/vue';

const router = useRouter();
const authStore = useAuthStore();
const portfolioStore = usePortfolioStore();

const displayCurrency = ref<'BRL' | 'USD'>('BRL');
const showAddAccountModal = ref(false);
const showAddAssetModal = ref(false);
const showAddInvestmentModal = ref(false);

const newAccount = ref({
  name: '',
  institution: '',
  accountType: 'Brokerage',
  baseCurrency: 'BRL',
});

const newAsset = ref({
  name: '',
  ticker: '',
  assetCategory: 'Stock_BR',
  valuationType: 'TickerMarket',
  currency: 'BRL',
  indexBenchmark: 'None',
  logoUrl: '',
});

const newHolding = ref({
  accountId: '',
  assetId: '',
  customName: '',
  quantity: 1,
  pricePerUnit: 100,
  transactionDate: new Date().toISOString().substring(0, 10),
});

const summary = computed(() => portfolioStore.summary);
const positions = computed(() => portfolioStore.summary?.positions || []);

onMounted(() => {
  loadData();
});

async function loadData() {
  await Promise.all([
    portfolioStore.fetchPortfolioSummary(),
    portfolioStore.fetchAccounts(),
    portfolioStore.fetchAssets(),
  ]);
}

function handleLogout() {
  authStore.logout();
  router.push('/login');
}

function calculateOverallReturnPct() {
  if (!summary.value) return '0.00';
  const invested = summary.value.totalInvestedBrl;
  const gain = summary.value.netGainLossBrl;
  if (invested <= 0) return '0.00';
  return ((gain / invested) * 100).toFixed(2);
}

function formatCurrency(val: number, currency?: string) {
  const curr = currency || displayCurrency.value;
  return new Intl.NumberFormat(curr === 'BRL' ? 'pt-BR' : 'en-US', {
    style: 'currency',
    currency: curr,
    minimumFractionDigits: 2,
  }).format(val);
}

function getLogoUrl(logo: string) {
  if (logo.startsWith('http')) return logo;
  return `http://localhost:8081${logo.startsWith('/') ? '' : '/'}${logo}`;
}

async function openAddInvestmentModal() {
  await Promise.all([portfolioStore.fetchAccounts(), portfolioStore.fetchAssets()]);
  if (portfolioStore.accounts.length > 0) {
    newHolding.value.accountId = portfolioStore.accounts[0].id;
  }
  if (portfolioStore.assets.length > 0) {
    newHolding.value.assetId = portfolioStore.assets[0].id;
  }
  showAddInvestmentModal.value = true;
}

async function submitAddAccount() {
  await portfolioStore.createAccount(newAccount.value);
  showAddAccountModal.value = false;
  newAccount.value = { name: '', institution: '', accountType: 'Brokerage', baseCurrency: 'BRL' };
  await portfolioStore.fetchAccounts();
}

async function submitAddAsset() {
  const payload = {
    name: newAsset.value.name,
    ticker: newAsset.value.ticker ? newAsset.value.ticker.toUpperCase() : null,
    assetCategory: newAsset.value.assetCategory,
    valuationType: newAsset.value.valuationType,
    currency: newAsset.value.currency,
    indexBenchmark: newAsset.value.indexBenchmark,
    logoUrl: newAsset.value.logoUrl || null,
  };
  await portfolioStore.createAsset(payload);
  showAddAssetModal.value = false;
  newAsset.value = {
    name: '',
    ticker: '',
    assetCategory: 'Stock_BR',
    valuationType: 'TickerMarket',
    currency: 'BRL',
    indexBenchmark: 'None',
    logoUrl: '',
  };
  await portfolioStore.fetchAssets();
}

async function submitAddInvestment() {
  if (!newHolding.value.accountId || !newHolding.value.assetId) return;

  const inv = await portfolioStore.createInvestment({
    accountId: newHolding.value.accountId,
    assetId: newHolding.value.assetId,
    customName: newHolding.value.customName || undefined,
  });

  const selectedAsset = portfolioStore.assets.find(a => a.id === newHolding.value.assetId);
  const currency = selectedAsset?.currency || 'BRL';

  await portfolioStore.createTransaction({
    investmentId: inv.id,
    accountId: newHolding.value.accountId,
    transactionType: 'Buy',
    transactionDate: new Date(newHolding.value.transactionDate).toISOString(),
    quantity: newHolding.value.quantity,
    pricePerUnit: newHolding.value.pricePerUnit,
    totalAmount: newHolding.value.quantity * newHolding.value.pricePerUnit,
    feeAmount: 0,
    taxAmount: 0,
    currency,
    notes: 'Initial holding purchase',
  });

  showAddInvestmentModal.value = false;
  await portfolioStore.fetchPortfolioSummary();
}
</script>

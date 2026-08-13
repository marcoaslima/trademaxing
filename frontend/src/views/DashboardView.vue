<template>
  <div class="min-h-screen bg-[#0a0e17] text-slate-100 font-sans selection:bg-[#2563eb] selection:text-white flex flex-col">
    <!-- Navbar -->
    <header class="border-b border-slate-800/80 bg-[#0d131f]/90 backdrop-blur-md sticky top-0 z-40">
      <div class="max-w-7xl mx-auto px-6 h-20 flex items-center justify-between">
        <div class="flex items-center gap-3">
          <div class="w-10 h-10 rounded-xl bg-gradient-to-tr from-[#0f4c81] to-[#2563eb] flex items-center justify-center shadow-lg shadow-blue-900/30">
            <TrendingUp class="w-6 h-6 text-white" />
          </div>
          <div>
            <h1 class="text-xl font-bold tracking-tight text-white flex items-center gap-2">
              TRADEMAXING
              <span class="text-[10px] px-2 py-0.5 rounded bg-blue-950 border border-blue-500/40 text-blue-400 font-mono">PRO</span>
            </h1>
            <span class="text-xs text-slate-400 font-mono">Portfolio Dashboard</span>
          </div>
        </div>

        <div class="flex items-center gap-4">
          <!-- Currency Display Toggle -->
          <div class="bg-slate-900 border border-slate-800 p-1 rounded-xl flex items-center">
            <button
              @click="displayCurrency = 'BRL'"
              :class="['px-3 py-1.5 rounded-lg text-xs font-bold transition', displayCurrency === 'BRL' ? 'bg-[#2563eb] text-white shadow' : 'text-slate-400 hover:text-white']"
            >
              R$ BRL
            </button>
            <button
              @click="displayCurrency = 'USD'"
              :class="['px-3 py-1.5 rounded-lg text-xs font-bold transition', displayCurrency === 'USD' ? 'bg-[#2563eb] text-white shadow' : 'text-slate-400 hover:text-white']"
            >
              $ USD
            </button>
          </div>

          <!-- Refresh Data -->
          <button
            @click="loadData"
            class="p-2.5 rounded-xl border border-slate-800 bg-slate-900/80 hover:bg-slate-800 text-slate-300 hover:text-white transition"
            title="Refresh Portfolio Data"
          >
            <RefreshCw :class="['w-4 h-4', portfolioStore.isLoading ? 'animate-spin' : '']" />
          </button>

          <!-- User Info & Logout -->
          <div class="flex items-center gap-3 pl-3 border-l border-slate-800">
            <div class="text-right hidden sm:block">
              <span class="block text-xs font-bold text-white">{{ authStore.user?.name || 'Investor' }}</span>
              <span class="block text-[11px] text-slate-500 font-mono">{{ authStore.user?.email }}</span>
            </div>
            <button
              @click="handleLogout"
              class="p-2.5 rounded-xl border border-red-900/40 bg-red-950/20 hover:bg-red-950/60 text-red-400 transition"
              title="Logout"
            >
              <LogOut class="w-4 h-4" />
            </button>
          </div>
        </div>
      </div>
    </header>

    <!-- Main Body -->
    <main class="flex-1 max-w-7xl w-full mx-auto px-6 py-8 space-y-8">
      <!-- Loading Skeleton -->
      <div v-if="portfolioStore.isLoading && !summary" class="space-y-6">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
          <div v-for="i in 4" :key="i" class="h-32 rounded-2xl bg-slate-900/60 animate-pulse border border-slate-800"></div>
        </div>
      </div>

      <template v-else>
        <!-- KPI Summary Cards -->
        <section class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
          <!-- Card 1: Total Net Worth -->
          <div class="japanese-blue-card p-6 rounded-2xl relative overflow-hidden">
            <div class="flex justify-between items-center mb-3">
              <span class="text-xs font-bold text-slate-400 uppercase tracking-wider">Total Net Worth</span>
              <div class="p-2 rounded-lg bg-blue-500/10 text-blue-400">
                <Wallet class="w-5 h-5" />
              </div>
            </div>
            <div class="text-3xl font-extrabold text-white tracking-tight">
              {{ formatCurrency(displayCurrency === 'BRL' ? (summary?.totalNetWorthBrl || 0) : (summary?.totalNetWorthUsd || 0)) }}
            </div>
            <div class="mt-3 text-xs text-slate-400 flex items-center justify-between">
              <span>Equivalent in {{ displayCurrency === 'BRL' ? 'USD' : 'BRL' }}:</span>
              <span class="font-mono text-slate-200">
                {{ formatCurrency(displayCurrency === 'BRL' ? (summary?.totalNetWorthUsd || 0) : (summary?.totalNetWorthBrl || 0), displayCurrency === 'BRL' ? 'USD' : 'BRL') }}
              </span>
            </div>
          </div>

          <!-- Card 2: Invested Capital -->
          <div class="japanese-blue-card p-6 rounded-2xl">
            <div class="flex justify-between items-center mb-3">
              <span class="text-xs font-bold text-slate-400 uppercase tracking-wider">Invested Capital</span>
              <div class="p-2 rounded-lg bg-blue-500/10 text-blue-400">
                <PiggyBank class="w-5 h-5" />
              </div>
            </div>
            <div class="text-3xl font-extrabold text-white tracking-tight">
              {{ formatCurrency(displayCurrency === 'BRL' ? (summary?.totalInvestedBrl || 0) : (summary?.totalInvestedUsd || 0)) }}
            </div>
            <div class="mt-3 text-xs text-slate-400 flex items-center justify-between">
              <span>Total Cost Basis</span>
              <span class="font-mono text-slate-400 font-semibold">100% Capital</span>
            </div>
          </div>

          <!-- Card 3: Unrealized Gain / Loss -->
          <div class="japanese-blue-card p-6 rounded-2xl">
            <div class="flex justify-between items-center mb-3">
              <span class="text-xs font-bold text-slate-400 uppercase tracking-wider">Net Return / Loss</span>
              <div :class="['p-2 rounded-lg', (summary?.netGainLossBrl || 0) >= 0 ? 'bg-emerald-500/10 text-emerald-400' : 'bg-rose-500/10 text-rose-400']">
                <TrendingUp v-if="(summary?.netGainLossBrl || 0) >= 0" class="w-5 h-5" />
                <TrendingDown v-else class="w-5 h-5" />
              </div>
            </div>
            <div :class="['text-3xl font-extrabold tracking-tight', (summary?.netGainLossBrl || 0) >= 0 ? 'text-emerald-400' : 'text-rose-400']">
              {{ (summary?.netGainLossBrl || 0) >= 0 ? '+' : '' }}{{ formatCurrency(displayCurrency === 'BRL' ? (summary?.netGainLossBrl || 0) : (summary?.netGainLossUsd || 0)) }}
            </div>
            <div class="mt-3 text-xs flex items-center justify-between">
              <span class="text-slate-400">Overall Yield:</span>
              <span :class="['font-mono font-bold px-2 py-0.5 rounded text-[11px]', (summary?.netGainLossBrl || 0) >= 0 ? 'bg-emerald-950 text-emerald-300' : 'bg-rose-950 text-rose-300']">
                {{ calculateOverallReturnPct() }}%
              </span>
            </div>
          </div>

          <!-- Card 4: Official PTAX USD Rate -->
          <div class="japanese-blue-card p-6 rounded-2xl">
            <div class="flex justify-between items-center mb-3">
              <span class="text-xs font-bold text-slate-400 uppercase tracking-wider">Official PTAX FX</span>
              <div class="p-2 rounded-lg bg-blue-500/10 text-blue-400">
                <DollarSign class="w-5 h-5" />
              </div>
            </div>
            <div class="text-3xl font-extrabold text-white tracking-tight font-mono">
              R$ {{ (summary?.usdBrlFxRate || 5.50).toFixed(4) }}
            </div>
            <div class="mt-3 text-xs text-slate-400 flex items-center justify-between">
              <span>BCB Daily PTAX</span>
              <span class="font-mono text-blue-400">USD/BRL</span>
            </div>
          </div>
        </section>

        <!-- Quick Action Bar & Modal Triggers -->
        <section class="flex flex-col sm:flex-row justify-between items-center gap-4 bg-[#0d131f] p-4 rounded-2xl border border-slate-800">
          <div class="flex items-center gap-3 w-full sm:w-auto">
            <button
              @click="showAddAccountModal = true"
              class="flex-1 sm:flex-initial px-4 py-2.5 rounded-xl text-xs font-bold bg-[#162238] hover:bg-[#1e2d4a] border border-blue-500/30 text-blue-300 flex items-center justify-center gap-2 transition"
            >
              <Plus class="w-4 h-4" />
              Add Broker Account
            </button>

            <button
              @click="showAddAssetModal = true"
              class="flex-1 sm:flex-initial px-4 py-2.5 rounded-xl text-xs font-bold bg-[#162238] hover:bg-[#1e2d4a] border border-blue-500/30 text-blue-300 flex items-center justify-center gap-2 transition"
            >
              <Plus class="w-4 h-4" />
              Add Master Asset
            </button>
          </div>

          <button
            @click="showAddInvestmentModal = true"
            class="w-full sm:w-auto px-6 py-2.5 rounded-xl text-xs font-extrabold bg-[#2563eb] hover:bg-[#1d4ed8] text-white shadow-lg shadow-blue-600/30 flex items-center justify-center gap-2 transition"
          >
            <Plus class="w-4 h-4" />
            Add Position / Holding
          </button>
        </section>

        <!-- Holdings Positions Table -->
        <section class="bg-[#0d131f] rounded-2xl border border-slate-800 overflow-hidden shadow-xl">
          <div class="p-6 border-b border-slate-800/80 flex items-center justify-between">
            <div>
              <h2 class="text-lg font-bold text-white">Master Asset Positions</h2>
              <p class="text-xs text-slate-400 mt-0.5">Consolidated portfolio holdings across accounts</p>
            </div>
            <span class="text-xs font-mono px-3 py-1 rounded-full bg-slate-900 border border-slate-800 text-slate-300">
              {{ positions.length }} Position(s)
            </span>
          </div>

          <div class="overflow-x-auto">
            <table class="w-full text-left text-sm text-slate-300">
              <thead class="bg-[#080c14] text-xs font-semibold text-slate-400 uppercase tracking-wider border-b border-slate-800">
                <tr>
                  <th class="py-4 px-6">Asset</th>
                  <th class="py-4 px-4">Category</th>
                  <th class="py-4 px-4 text-right">Quantity</th>
                  <th class="py-4 px-4 text-right">Avg Cost</th>
                  <th class="py-4 px-4 text-right">Current Price</th>
                  <th class="py-4 px-4 text-right">Total Value</th>
                  <th class="py-4 px-6 text-right">Gain / Loss</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-800/60 font-mono text-xs">
                <tr v-if="positions.length === 0">
                  <td colspan="7" class="py-12 text-center text-slate-500 font-sans">
                    No active positions found. Click <strong class="text-blue-400 font-semibold">Add Position</strong> to start tracking!
                  </td>
                </tr>

                <tr v-for="pos in positions" :key="pos.investmentId" class="hover:bg-slate-900/60 transition">
                  <!-- Asset Name & Logo -->
                  <td class="py-4 px-6 font-sans">
                    <div class="flex items-center gap-3">
                      <div class="w-9 h-9 rounded-xl bg-slate-900 border border-slate-700/80 flex items-center justify-center overflow-hidden shrink-0">
                        <img v-if="pos.logoUrl" :src="getLogoUrl(pos.logoUrl)" :alt="pos.name" class="w-6 h-6 object-contain" />
                        <Building2 v-else class="w-5 h-5 text-slate-500" />
                      </div>
                      <div>
                        <span class="block font-bold text-white text-sm">{{ pos.name }}</span>
                        <span v-if="pos.ticker" class="text-xs text-blue-400 font-mono">{{ pos.ticker }}</span>
                      </div>
                    </div>
                  </td>

                  <!-- Category -->
                  <td class="py-4 px-4 font-sans">
                    <span class="px-2.5 py-1 rounded-md text-[10px] font-bold uppercase tracking-wider bg-slate-900 border border-slate-700 text-slate-300">
                      {{ pos.assetCategory }}
                    </span>
                  </td>

                  <!-- Quantity -->
                  <td class="py-4 px-4 text-right font-bold text-white">
                    {{ pos.quantity.toLocaleString() }}
                  </td>

                  <!-- Avg Price -->
                  <td class="py-4 px-4 text-right text-slate-400">
                    {{ formatCurrency(pos.averagePrice, pos.currency) }}
                  </td>

                  <!-- Current Price -->
                  <td class="py-4 px-4 text-right font-bold text-blue-400">
                    {{ formatCurrency(pos.currentUnitPrice, pos.currency) }}
                  </td>

                  <!-- Total Value -->
                  <td class="py-4 px-4 text-right font-extrabold text-white text-sm">
                    {{ formatCurrency(pos.currentTotalValue, pos.currency) }}
                  </td>

                  <!-- Gain / Loss -->
                  <td class="py-4 px-6 text-right">
                    <div :class="['font-bold', pos.unrealizedGainLoss >= 0 ? 'text-emerald-400' : 'text-rose-400']">
                      {{ pos.unrealizedGainLoss >= 0 ? '+' : '' }}{{ formatCurrency(pos.unrealizedGainLoss, pos.currency) }}
                    </div>
                    <span :class="['text-[10px] px-1.5 py-0.5 rounded font-bold', pos.unrealizedGainLoss >= 0 ? 'bg-emerald-950 text-emerald-300' : 'bg-rose-950 text-rose-300']">
                      {{ pos.unrealizedGainLossPercentage >= 0 ? '+' : '' }}{{ pos.unrealizedGainLossPercentage.toFixed(2) }}%
                    </span>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </section>
      </template>
    </main>

    <!-- Modal: Add Broker Account -->
    <div v-if="showAddAccountModal" class="fixed inset-0 z-50 bg-black/80 backdrop-blur-sm flex items-center justify-center p-4">
      <div class="japanese-blue-card w-full max-w-md p-6 rounded-2xl space-y-4">
        <h3 class="text-lg font-bold text-white flex items-center gap-2">
          <Plus class="w-5 h-5 text-blue-400" /> Add Broker Account
        </h3>
        <form @submit.prevent="submitAddAccount" class="space-y-4 text-xs">
          <div>
            <label class="block text-slate-300 font-semibold mb-1">Account Name</label>
            <input v-model="newAccount.name" type="text" required placeholder="Interactive Brokers US" class="w-full bg-slate-900 border border-slate-700 rounded-xl p-3 text-white outline-none focus:border-blue-500" />
          </div>
          <div>
            <label class="block text-slate-300 font-semibold mb-1">Institution</label>
            <input v-model="newAccount.institution" type="text" required placeholder="Caixa / XP / IBKR" class="w-full bg-slate-900 border border-slate-700 rounded-xl p-3 text-white outline-none focus:border-blue-500" />
          </div>
          <div class="grid grid-cols-2 gap-3">
            <div>
              <label class="block text-slate-300 font-semibold mb-1">Account Type</label>
              <select v-model="newAccount.accountType" class="w-full bg-slate-900 border border-slate-700 rounded-xl p-3 text-white outline-none focus:border-blue-500">
                <option value="Brokerage">Brokerage</option>
                <option value="Personal">Personal</option>
                <option value="Retirement_FGTS">Retirement FGTS</option>
                <option value="Joint">Joint</option>
              </select>
            </div>
            <div>
              <label class="block text-slate-300 font-semibold mb-1">Currency</label>
              <select v-model="newAccount.baseCurrency" class="w-full bg-slate-900 border border-slate-700 rounded-xl p-3 text-white outline-none focus:border-blue-500">
                <option value="BRL">BRL (R$)</option>
                <option value="USD">USD ($)</option>
              </select>
            </div>
          </div>
          <div class="flex justify-end gap-3 pt-2">
            <button type="button" @click="showAddAccountModal = false" class="px-4 py-2 rounded-xl bg-slate-800 text-slate-300 hover:text-white">Cancel</button>
            <button type="submit" class="px-5 py-2 rounded-xl bg-blue-600 hover:bg-blue-700 text-white font-bold">Save Account</button>
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
import {
  TrendingUp, TrendingDown, Wallet, PiggyBank, DollarSign,
  Plus, RefreshCw, LogOut, Building2
} from '@lucide/vue';

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

async function submitAddAccount() {
  await portfolioStore.createAccount(newAccount.value);
  showAddAccountModal.value = false;
  newAccount.value = { name: '', institution: '', accountType: 'Brokerage', baseCurrency: 'BRL' };
}
</script>

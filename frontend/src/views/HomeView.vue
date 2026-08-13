<template>
  <div class="min-h-screen bg-[#0a0e17] text-slate-100 flex flex-col font-sans selection:bg-[#2563eb] selection:text-white">
    <!-- Navbar -->
    <nav class="border-b border-slate-800/80 bg-[#0d131f]/80 backdrop-blur-md sticky top-0 z-50">
      <div class="max-w-7xl mx-auto px-6 h-20 flex items-center justify-between">
        <div class="flex items-center gap-3">
          <div class="w-10 h-10 rounded-xl bg-gradient-to-tr from-[#0f4c81] to-[#2563eb] flex items-center justify-center shadow-lg shadow-blue-900/30">
            <TrendingUp class="w-6 h-6 text-white" />
          </div>
          <div>
            <span class="text-xl font-bold tracking-tight bg-clip-text text-transparent bg-gradient-to-r from-white via-slate-200 to-[#3b82f6]">
              TRADEMAXING
            </span>
            <span class="text-xs text-[#60a5fa] block font-mono tracking-widest uppercase">TradingCenter</span>
          </div>
        </div>

        <div class="flex items-center gap-4">
          <router-link
            v-if="!authStore.isAuthenticated"
            to="/login"
            class="px-5 py-2.5 rounded-lg text-sm font-medium text-slate-300 hover:text-white hover:bg-slate-800/60 transition"
          >
            Sign In
          </router-link>
          <router-link
            v-if="!authStore.isAuthenticated"
            to="/login?tab=register"
            class="px-5 py-2.5 rounded-lg text-sm font-semibold bg-[#2563eb] hover:bg-[#1d4ed8] text-white shadow-lg shadow-blue-600/25 transition flex items-center gap-2"
          >
            Get Started
            <ArrowRight class="w-4 h-4" />
          </router-link>
          <router-link
            v-else
            to="/dashboard"
            class="px-5 py-2.5 rounded-lg text-sm font-semibold bg-[#2563eb] hover:bg-[#1d4ed8] text-white shadow-lg shadow-blue-600/25 transition flex items-center gap-2"
          >
            Open Dashboard
            <ArrowRight class="w-4 h-4" />
          </router-link>
        </div>
      </div>
    </nav>

    <!-- Hero Section -->
    <main class="flex-1 max-w-7xl mx-auto px-6 py-20 flex flex-col justify-center items-center text-center relative overflow-hidden">
      <!-- Glow effect -->
      <div class="absolute -top-32 left-1/2 -translate-x-1/2 w-96 h-96 bg-[#0f4c81]/30 rounded-full blur-3xl pointer-events-none"></div>

      <div class="inline-flex items-center gap-2 px-4 py-2 rounded-full border border-blue-500/30 bg-blue-950/40 text-blue-400 text-xs font-mono mb-8 uppercase tracking-wider">
        <Sparkles class="w-4 h-4 text-blue-400" />
        Heterogeneous Multi-Broker Investment Platform
      </div>

      <h1 class="text-5xl sm:text-7xl font-extrabold text-white tracking-tight leading-tight max-w-4xl mb-6">
        Consolidate All Your Investments In
        <span class="bg-clip-text text-transparent bg-gradient-to-r from-[#3b82f6] via-[#60a5fa] to-[#93c5fd]">
          One Japanese Blue Space
        </span>
      </h1>

      <p class="text-slate-400 text-lg sm:text-xl max-w-2xl mb-10 leading-relaxed font-light">
        Seamlessly track your <strong class="text-slate-200">FGTS (TR+3%)</strong>, <strong class="text-slate-200">US Private Bonds</strong>, <strong class="text-slate-200">CDB (CDI)</strong>, and <strong class="text-slate-200">Equities (US/BR Stocks & FIIs)</strong> across multiple brokers in BRL and USD.
      </p>

      <div class="flex flex-col sm:flex-row items-center gap-4 mb-20">
        <router-link
          to="/login"
          class="w-full sm:w-auto px-8 py-4 rounded-xl text-base font-bold bg-[#2563eb] hover:bg-[#1d4ed8] text-white shadow-xl shadow-blue-600/30 transition flex items-center justify-center gap-3"
        >
          Launch TradingCenter
          <ArrowRight class="w-5 h-5" />
        </router-link>
        <a
          href="#features"
          class="w-full sm:w-auto px-8 py-4 rounded-xl text-base font-semibold border border-slate-700 bg-slate-900/50 hover:bg-slate-800 text-slate-300 hover:text-white transition flex items-center justify-center gap-2"
        >
          Explore Features
        </a>
      </div>

      <!-- Feature Grid -->
      <section id="features" class="grid grid-cols-1 md:grid-cols-3 gap-8 text-left w-full pt-12 border-t border-slate-800/80">
        <div class="japanese-blue-card p-8 rounded-2xl">
          <div class="w-12 h-12 rounded-xl bg-blue-600/20 border border-blue-500/30 flex items-center justify-center mb-6 text-blue-400">
            <DollarSign class="w-6 h-6" />
          </div>
          <h3 class="text-xl font-bold text-white mb-3">Official PTAX Tax Sync</h3>
          <p class="text-slate-400 text-sm leading-relaxed">
            Automatic daily sync with Banco Central do Brasil for Buy & Sell PTAX rates, tailored for Receita Federal GCAP & Brazilian Tax reports.
          </p>
        </div>

        <div class="japanese-blue-card p-8 rounded-2xl">
          <div class="w-12 h-12 rounded-xl bg-blue-600/20 border border-blue-500/30 flex items-center justify-center mb-6 text-blue-400">
            <ShieldCheck class="w-6 h-6" />
          </div>
          <h3 class="text-xl font-bold text-white mb-3">Multi-Tenant Isolation</h3>
          <p class="text-slate-400 text-sm leading-relaxed">
            Strict user-level data isolation enforced by EF Core Global Query Filters for bulletproof privacy and security.
          </p>
        </div>

        <div class="japanese-blue-card p-8 rounded-2xl">
          <div class="w-12 h-12 rounded-xl bg-blue-600/20 border border-blue-500/30 flex items-center justify-center mb-6 text-blue-400">
            <Layers class="w-6 h-6" />
          </div>
          <h3 class="text-xl font-bold text-white mb-3">Master Asset Catalog</h3>
          <p class="text-slate-400 text-sm leading-relaxed">
            Zero redundant API calls: AAPL, PETR4, and Tesouro SELIC prices are cached and synced once for all users.
          </p>
        </div>
      </section>
    </main>

    <!-- Footer -->
    <footer class="border-t border-slate-800/80 bg-[#0d131f] py-8 text-center text-xs text-slate-500">
      <div class="max-w-7xl mx-auto px-6 flex flex-col sm:flex-row justify-between items-center gap-4">
        <span>© 2026 TradingCenter. Minimalist Japanese Blue Finance.</span>
        <span class="font-mono text-slate-600">Built with Vue 3 + C# .NET Minimal API</span>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/authStore';
import { TrendingUp, Sparkles, ArrowRight, DollarSign, ShieldCheck, Layers } from '@lucide/vue';

const authStore = useAuthStore();
</script>

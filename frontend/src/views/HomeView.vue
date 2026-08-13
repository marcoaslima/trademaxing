<template>
  <div class="min-h-screen bg-[#09090b] text-zinc-100 flex flex-col font-sans selection:bg-[#2563eb] selection:text-white">
    <!-- Navbar -->
    <header class="border-b border-zinc-800/80 bg-[#09090b]">
      <div class="max-w-6xl mx-auto px-6 h-16 flex items-center justify-between">
        <div class="flex items-center gap-3">
          <div class="w-8 h-8 rounded bg-[#1d4ed8] flex items-center justify-center font-bold text-white text-sm">
            TC
          </div>
          <span class="text-sm font-semibold tracking-tight text-white">TradingCenter</span>
        </div>

        <div class="flex items-center gap-3">
          <router-link
            v-if="!authStore.isAuthenticated"
            to="/login"
            class="px-4 py-1.5 rounded text-xs font-medium text-zinc-400 hover:text-white transition"
          >
            Sign In
          </router-link>
          <router-link
            v-if="!authStore.isAuthenticated"
            to="/login?tab=register"
            class="px-4 py-1.5 rounded text-xs font-medium japanese-blue-btn"
          >
            Get Started
          </router-link>
          <router-link
            v-else
            to="/dashboard"
            class="px-4 py-1.5 rounded text-xs font-medium japanese-blue-btn"
          >
            Open Dashboard
          </router-link>
        </div>
      </div>
    </header>

    <!-- Hero Section -->
    <main class="flex-1 max-w-4xl mx-auto px-6 py-24 flex flex-col justify-center items-start">
      <div class="text-xs font-mono text-zinc-500 uppercase tracking-widest mb-4">
        Portfolio Management Engine
      </div>

      <h1 class="text-4xl sm:text-6xl font-semibold tracking-tight text-white leading-tight mb-6">
        Consolidated tracking for heterogeneous investments.
      </h1>

      <p class="text-zinc-400 text-base sm:text-lg max-w-2xl mb-10 leading-relaxed font-normal">
        Track FGTS, US Private Bonds, BRL Fixed Income, and Global Equities across multiple broker accounts with official BCB PTAX tax calculations.
      </p>

      <div class="flex items-center gap-4 mb-20">
        <router-link
          to="/login"
          class="px-6 py-2.5 rounded text-sm font-medium japanese-blue-btn flex items-center gap-2"
        >
          Access Platform
          <ArrowRight class="w-4 h-4" />
        </router-link>
        <router-link
          to="/login?tab=register"
          class="px-6 py-2.5 rounded text-sm font-medium border border-zinc-800 bg-zinc-900 text-zinc-300 hover:text-white transition"
        >
          Create Account
        </router-link>
      </div>

      <!-- Feature Table -->
      <div class="w-full border-t border-zinc-800/80 pt-12 grid grid-cols-1 md:grid-cols-3 gap-6">
        <div class="sober-panel p-6 rounded-lg">
          <div class="text-xs font-mono text-blue-400 mb-2">01 / TAX & FX</div>
          <h3 class="text-sm font-semibold text-white mb-2">Official PTAX Calculation</h3>
          <p class="text-xs text-zinc-400 leading-relaxed">
            Automated daily BCB SGS integration for acquisition and sale tax conversion under Receita Federal GCAP rules.
          </p>
        </div>

        <div class="sober-panel p-6 rounded-lg">
          <div class="text-xs font-mono text-blue-400 mb-2">02 / ARCHITECTURE</div>
          <h3 class="text-sm font-semibold text-white mb-2">Multi-Tenant Isolation</h3>
          <p class="text-xs text-zinc-400 leading-relaxed">
            Database-level user data segregation enforced by EF Core Global Query Filters.
          </p>
        </div>

        <div class="sober-panel p-6 rounded-lg">
          <div class="text-xs font-mono text-blue-400 mb-2">03 / CATALOG</div>
          <h3 class="text-sm font-semibold text-white mb-2">Master Assets Catalog</h3>
          <p class="text-xs text-zinc-400 leading-relaxed">
            Single-query ticker pricing pipeline eliminating duplicate external API roundtrips.
          </p>
        </div>
      </div>
    </main>

    <!-- Footer -->
    <footer class="border-t border-zinc-800/80 py-6 text-xs text-zinc-600">
      <div class="max-w-6xl mx-auto px-6 flex justify-between items-center font-mono">
        <span>TradingCenter © 2026</span>
        <span>Minimalist Finance Engine</span>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/authStore';
import { ArrowRight } from '@lucide/vue';

const authStore = useAuthStore();
</script>

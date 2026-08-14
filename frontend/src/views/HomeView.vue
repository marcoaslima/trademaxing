<template>
  <div class="min-h-screen bg-[#f8fafc] text-slate-900 flex flex-col font-sans selection:bg-[#059669] selection:text-white">
    <!-- Navbar -->
    <header class="border-b border-slate-200 bg-white sticky top-0 z-40 shadow-xs">
      <div class="max-w-6xl mx-auto px-6 h-16 flex items-center justify-between">
        <div class="flex items-center gap-3">
          <div class="w-8 h-8 rounded-lg bg-[#059669] flex items-center justify-center font-bold text-white text-sm shadow-xs">
            TC
          </div>
          <span class="text-sm font-bold tracking-tight text-slate-900">TradingCenter</span>
        </div>

        <div class="flex items-center gap-3">
          <router-link
            v-if="!authStore.isAuthenticated"
            to="/login"
            class="px-4 py-2 rounded-lg text-xs font-semibold text-slate-600 hover:text-slate-900 transition"
          >
            Entrar
          </router-link>
          <router-link
            v-if="!authStore.isAuthenticated"
            to="/login?tab=register"
            class="px-4 py-2 rounded-lg text-xs font-bold bg-[#059669] hover:bg-[#047857] text-white shadow-xs transition"
          >
            Criar Conta
          </router-link>
          <router-link
            v-else
            to="/dashboard"
            class="px-4 py-2 rounded-lg text-xs font-bold bg-[#059669] hover:bg-[#047857] text-white shadow-xs transition"
          >
            Abrir Carteira
          </router-link>
        </div>
      </div>
    </header>

    <!-- Hero Section -->
    <main class="flex-1 max-w-4xl mx-auto px-6 py-20 flex flex-col justify-center items-start">
      <div class="text-xs font-mono text-[#059669] font-bold uppercase tracking-wider mb-4 bg-emerald-50 border border-emerald-200 px-3 py-1 rounded-full">
        Consolidador Inteligente de Investimentos
      </div>

      <h1 class="text-4xl sm:text-6xl font-extrabold tracking-tight text-slate-900 leading-tight mb-6">
        Gestão minimalista e unificada para seu patrimônio global.
      </h1>

      <p class="text-slate-600 text-base sm:text-lg max-w-2xl mb-10 leading-relaxed font-normal">
        Acompanhe ações do Brasil e Exterior, FGTS, Renda Fixa e Cripto em múltiplas corretoras com conversão PTAX oficial do Banco Central.
      </p>

      <div class="flex items-center gap-4 mb-16">
        <router-link
          to="/login"
          class="px-6 py-3 rounded-xl text-sm font-bold bg-[#059669] hover:bg-[#047857] text-white flex items-center gap-2 shadow-xs transition"
        >
          Acessar Plataforma
          <ArrowRight class="w-4 h-4" />
        </router-link>
        <router-link
          to="/login?tab=register"
          class="px-6 py-3 rounded-xl text-sm font-semibold border border-slate-200 bg-white text-slate-700 hover:bg-slate-50 transition shadow-xs"
        >
          Criar Conta Grátis
        </router-link>
      </div>

      <!-- Feature Table Grid -->
      <div class="w-full border-t border-slate-200 pt-12 grid grid-cols-1 md:grid-cols-3 gap-6">
        <div class="bg-white border border-slate-200 p-6 rounded-2xl shadow-xs">
          <div class="text-xs font-mono font-bold text-[#059669] mb-2">01 / CÂMBIO & IMPOSTOS</div>
          <h3 class="text-sm font-bold text-slate-900 mb-2">Conversão PTAX Oficial</h3>
          <p class="text-xs text-slate-500 leading-relaxed">
            Integração automatizada com API do Banco Central para cotações diárias de aquisição e venda conforme Receita Federal.
          </p>
        </div>

        <div class="bg-white border border-slate-200 p-6 rounded-2xl shadow-xs">
          <div class="text-xs font-mono font-bold text-[#059669] mb-2">02 / SEGURANÇA</div>
          <h3 class="text-sm font-bold text-slate-900 mb-2">Isolamento de Dados</h3>
          <p class="text-xs text-slate-500 leading-relaxed">
            Segregação completa de carteiras e transações por usuário com autenticação JWT e criptografia.
          </p>
        </div>

        <div class="bg-white border border-slate-200 p-6 rounded-2xl shadow-xs">
          <div class="text-xs font-mono font-bold text-[#059669] mb-2">03 / PERFORMANCE</div>
          <h3 class="text-sm font-bold text-slate-900 mb-2">Catálogo Master de Ativos</h3>
          <p class="text-xs text-slate-500 leading-relaxed">
            Mapeamento unificado de tickers e cotações sem duplicar requisições em APIs externas.
          </p>
        </div>
      </div>
    </main>

    <!-- Footer -->
    <footer class="border-t border-slate-200 bg-white py-6 text-xs text-slate-500">
      <div class="max-w-6xl mx-auto px-6 flex justify-between items-center font-mono">
        <span>TradingCenter © 2026</span>
        <span>Plataforma Minimalista de Investimentos</span>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/authStore';
import { ArrowRight } from '@lucide/vue';

const authStore = useAuthStore();
</script>

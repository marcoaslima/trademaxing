<template>
  <div class="min-h-screen bg-[#f8fafc] text-slate-900 font-sans selection:bg-[#059669] selection:text-white flex flex-col">
    <!-- Top Light Navigation Header -->
    <header class="border-b border-slate-200 bg-white sticky top-0 z-40 shadow-xs">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 h-16 flex items-center justify-between gap-4">
        <!-- Search Bar & Brand -->
        <div class="flex items-center gap-6 flex-1 max-w-xl">
          <router-link to="/dashboard" class="flex items-center gap-2.5 shrink-0">
            <div class="w-8 h-8 rounded-lg bg-[#059669] flex items-center justify-center font-bold text-white text-xs shadow-xs">
              TC
            </div>
            <span class="text-sm font-bold text-slate-900 tracking-tight hidden md:inline">TradingCenter</span>
          </router-link>

          <!-- Search Input -->
          <div class="relative w-full">
            <Search class="w-4 h-4 text-slate-400 absolute left-3 top-2.5" />
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Pesquise por ativos, tickers e notícias"
              class="w-full bg-slate-50 border border-slate-200 focus:border-[#059669] rounded-lg pl-9 pr-3 py-1.5 text-xs text-slate-800 placeholder-slate-400 outline-none transition"
            />
          </div>
          <!-- Navigation Links -->
          <nav class="hidden md:flex items-center gap-4 text-xs font-medium border-l border-slate-200 pl-4">
            <router-link to="/dashboard" class="text-[#059669] font-bold border-b-2 border-[#059669] pb-0.5">Dashboard</router-link>
            <router-link to="/accounts" class="text-slate-500 hover:text-slate-900 transition">Contas & Corretoras</router-link>
            <router-link to="/assets" class="text-slate-500 hover:text-slate-900 transition">Catálogo de Ativos</router-link>
          </nav>
        </div>

        <!-- Toolbar & User Actions -->
        <div class="flex items-center gap-2 shrink-0 text-xs">
          <button
            @click="handleSyncMarketData"
            :disabled="portfolioStore.isSyncingPrices"
            class="flex items-center gap-1.5 px-3 py-1.5 rounded-lg border border-slate-200 text-slate-700 bg-white hover:bg-slate-50 disabled:opacity-50 transition font-medium text-xs shadow-xs"
            title="Sincronizar cotações do Yahoo Finance e taxas do Banco Central"
          >
            <RefreshCw :class="['w-3.5 h-3.5 text-[#059669]', portfolioStore.isSyncingPrices ? 'animate-spin' : '']" />
            <span>{{ portfolioStore.isSyncingPrices ? 'Sincronizando...' : 'Sincronizar Preços' }}</span>
          </button>

          <button
            @click="openAddInvestmentModal"
            class="px-3.5 py-1.5 rounded-lg bg-[#059669] hover:bg-[#047857] text-white font-medium flex items-center gap-1.5 shadow-xs transition"
          >
            <Plus class="w-3.5 h-3.5" />
            <span>+ Negociar</span>
          </button>

          <!-- Currency Selector Toggle -->
          <div class="bg-slate-100 border border-slate-200 p-0.5 rounded-lg flex items-center">
            <button
              @click="displayCurrency = 'BRL'"
              :class="['px-2.5 py-1 rounded-md text-xs font-mono transition', displayCurrency === 'BRL' ? 'bg-white text-slate-900 font-bold shadow-xs' : 'text-slate-500 hover:text-slate-800']"
            >
              🇧🇷 BRL
            </button>
            <button
              @click="displayCurrency = 'USD'"
              :class="['px-2.5 py-1 rounded-md text-xs font-mono transition', displayCurrency === 'USD' ? 'bg-white text-slate-900 font-bold shadow-xs' : 'text-slate-500 hover:text-slate-800']"
            >
              🇺🇸 USD
            </button>
          </div>

          <!-- User Menu & Logout -->
          <div class="flex items-center gap-3 pl-3 border-l border-slate-200">
            <span class="font-medium text-slate-700 text-xs hidden sm:block">{{ authStore.user?.name || 'Investor' }}</span>
            <button
              @click="handleLogout"
              class="p-1.5 rounded-lg bg-slate-100 border border-slate-200 hover:bg-slate-200 text-slate-500 hover:text-slate-900 transition"
              title="Sair"
            >
              <LogOut class="w-3.5 h-3.5" />
            </button>
          </div>
        </div>
      </div>
    </header>

    <!-- Main Content Layout -->
    <main class="flex-1 max-w-7xl w-full mx-auto px-4 sm:px-6 py-8 space-y-8">
      <!-- Loading State -->
      <div v-if="portfolioStore.isLoading && !summary" class="py-24 text-center font-mono text-xs text-slate-400">
        Carregando informações da carteira...
      </div>

      <template v-else>
        <!-- TOP SECTION (Inspired by Image 1: Net Worth Header & Quick Action Cards) -->
        <section class="space-y-6">
          <div class="grid grid-cols-1 lg:grid-cols-12 gap-6 items-start">
            <!-- Left: Net Worth Big Numbers -->
            <div class="lg:col-span-7 space-y-2">
              <span class="text-xs font-semibold text-slate-500 uppercase tracking-wider block">Patrimônio Consolidado</span>
              <div class="text-4xl font-extrabold text-slate-900 tracking-tight font-mono-numbers">
                {{ formatCurrency(displayCurrency === 'BRL' ? (summary?.totalNetWorthBrl || 0) : (summary?.totalNetWorthUsd || 0)) }}
              </div>
              <div class="flex items-center gap-3 text-xs text-slate-500">
                <span class="underline cursor-pointer hover:text-[#059669]">Entenda seu Patrimônio</span>
                <span>•</span>
                <span>Custo Basis: <strong class="text-slate-800 font-mono">{{ formatCurrency(displayCurrency === 'BRL' ? (summary?.totalInvestedBrl || 0) : (summary?.totalInvestedUsd || 0)) }}</strong></span>
                <span>•</span>
                <span>Variação Total: <strong :class="[(summary?.netGainLossBrl || 0) >= 0 ? 'text-[#059669]' : 'text-rose-600']" class="font-mono">{{ (summary?.netGainLossBrl || 0) >= 0 ? '+' : '' }}{{ formatCurrency(displayCurrency === 'BRL' ? (summary?.netGainLossBrl || 0) : (summary?.netGainLossUsd || 0)) }} ({{ calculateOverallReturnPct() }}%)</strong></span>
              </div>
            </div>

            <!-- Right: Available Cash Card -->
            <div class="lg:col-span-5 bg-slate-100 border border-slate-200 rounded-2xl p-5 shadow-xs flex justify-between items-center">
              <div>
                <span class="text-xs font-semibold text-slate-600 block mb-1">Disponível em Caixa</span>
                <div class="text-2xl font-bold text-slate-900 font-mono-numbers">
                  {{ formatCurrency(availableCash) }}
                </div>
              </div>
              <button @click="showAddAccountModal = true" class="px-3.5 py-1.5 rounded-full border border-slate-300 bg-white hover:bg-slate-50 text-xs font-semibold text-slate-700 shadow-xs transition">
                Gerenciar Contas
              </button>
            </div>
          </div>

          <!-- Quick Action Buttons Row -->
          <div class="grid grid-cols-2 sm:grid-cols-4 gap-3">
            <button @click="openAddInvestmentModal" class="bg-white border border-slate-200 hover:border-[#059669] rounded-xl p-3 flex items-center gap-3 transition shadow-xs group">
              <div class="w-8 h-8 rounded-lg bg-emerald-50 text-[#059669] flex items-center justify-center shrink-0 group-hover:bg-[#059669] group-hover:text-white transition">
                <Plus class="w-4 h-4" />
              </div>
              <div class="text-left">
                <span class="block text-xs font-bold text-slate-800">Nova Posição</span>
                <span class="text-[11px] text-slate-400">Adicionar ativo</span>
              </div>
            </button>

            <button @click="showAddAccountModal = true" class="bg-white border border-slate-200 hover:border-[#059669] rounded-xl p-3 flex items-center gap-3 transition shadow-xs group">
              <div class="w-8 h-8 rounded-lg bg-emerald-50 text-[#059669] flex items-center justify-center shrink-0 group-hover:bg-[#059669] group-hover:text-white transition">
                <Wallet class="w-4 h-4" />
              </div>
              <div class="text-left">
                <span class="block text-xs font-bold text-slate-800">Nova Conta</span>
                <span class="text-[11px] text-slate-400">Corretora ou Banco</span>
              </div>
            </button>

            <button @click="showAddAssetModal = true" class="bg-white border border-slate-200 hover:border-[#059669] rounded-xl p-3 flex items-center gap-3 transition shadow-xs group">
              <div class="w-8 h-8 rounded-lg bg-emerald-50 text-[#059669] flex items-center justify-center shrink-0 group-hover:bg-[#059669] group-hover:text-white transition">
                <Layers class="w-4 h-4" />
              </div>
              <div class="text-left">
                <span class="block text-xs font-bold text-slate-800">Ativo Master</span>
                <span class="text-[11px] text-slate-400">Cadastrar no Catálogo</span>
              </div>
            </button>

            <div class="bg-white border border-slate-200 rounded-xl p-3 flex items-center gap-3 shadow-xs">
              <div class="w-8 h-8 rounded-lg bg-blue-50 text-blue-600 flex items-center justify-center shrink-0">
                <TrendingUp class="w-4 h-4" />
              </div>
              <div class="text-left">
                <span class="block text-xs font-bold text-slate-800">Dólar PTAX</span>
                <span class="text-[11px] font-mono text-slate-500">R$ {{ (summary?.usdBrlFxRate || 5.50).toFixed(4) }}</span>
              </div>
            </div>
          </div>

          <!-- Portfólio Summary Banner -->
          <div class="bg-slate-100/90 border border-slate-200 rounded-2xl p-5 flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-full bg-[#059669] text-white flex items-center justify-center shadow-xs shrink-0">
                <PieChart class="w-5 h-5" />
              </div>
              <div>
                <span class="text-xs font-bold text-slate-700 block">Resumo do Portfólio</span>
                <div class="text-xl font-bold text-slate-900 font-mono-numbers">
                  {{ formatCurrency(displayCurrency === 'BRL' ? (summary?.totalNetWorthBrl || 0) : (summary?.totalNetWorthUsd || 0)) }}
                </div>
              </div>
            </div>

            <div class="text-right font-mono">
              <span :class="[(summary?.netGainLossBrl || 0) >= 0 ? 'text-[#059669]' : 'text-rose-600']" class="text-sm font-bold block">
                {{ (summary?.netGainLossBrl || 0) >= 0 ? '+' : '' }}{{ formatCurrency(displayCurrency === 'BRL' ? (summary?.netGainLossBrl || 0) : (summary?.totalNetWorthUsd || 0)) }} ({{ calculateOverallReturnPct() }}%)
              </span>
              <span class="text-[11px] text-slate-500">Lucro / Prejuízo não realizado</span>
            </div>
          </div>
        </section>

        <!-- MAIN TABLE SECTION (Inspired by Image 2: Portfólio de Investimentos Clean Holdings Table) -->
        <section class="space-y-4">
          <div class="flex justify-between items-center">
            <h2 class="text-xl font-bold text-slate-900 tracking-tight">Portfólio de Investimentos</h2>
            <button @click="expandAll = !expandAll" class="text-xs font-semibold text-[#059669] hover:underline flex items-center gap-1">
              {{ expandAll ? 'Recolher todos -' : 'Expandir todos +' }}
            </button>
          </div>

          <!-- Accordion Category Cards Container -->
          <div class="space-y-4">
            <div v-if="categoryGroups.length === 0" class="bg-white border border-slate-200 rounded-2xl p-12 text-center text-xs text-slate-500 font-sans shadow-xs">
              Nenhum ativo registrado na sua carteira. Clique em "+ Negociar" para começar a investir.
            </div>

            <div
              v-for="cat in categoryGroups"
              :key="cat.category"
              class="bg-white border border-slate-200/90 rounded-2xl overflow-hidden shadow-xs transition hover:border-slate-300"
            >
              <!-- Category Header Banner -->
              <div
                @click="toggleCategory(cat.category)"
                class="px-6 py-4 flex items-center justify-between cursor-pointer hover:bg-slate-50/80 transition text-xs select-none"
              >
                <div class="flex items-center gap-3">
                  <div class="w-8 h-8 rounded-lg bg-slate-100 border border-slate-200 flex items-center justify-center text-[#059669] font-bold text-xs shrink-0">
                    <Briefcase class="w-4 h-4 text-[#059669]" />
                  </div>
                  <div>
                    <span class="block font-bold text-slate-900 text-sm tracking-tight">{{ formatCategoryName(cat.category) }}</span>
                    <span class="text-[11px] text-slate-500 font-mono">{{ cat.items.length }} ativo(s) • {{ cat.percentage.toFixed(1) }}% da carteira</span>
                  </div>
                </div>

                <div class="flex items-center gap-4">
                  <div class="text-right font-mono">
                    <span class="font-bold text-slate-900 text-sm block">
                      {{ formatCurrency(cat.totalValue) }}
                    </span>
                    <span :class="[cat.totalGainLoss >= 0 ? 'text-[#059669]' : 'text-rose-600']" class="text-[11px] font-semibold block">
                      {{ cat.totalGainLoss >= 0 ? '+' : '' }}{{ formatCurrency(cat.totalGainLoss) }} ({{ cat.returnPct.toFixed(2) }}%)
                    </span>
                  </div>
                  <ChevronDown :class="['w-4 h-4 text-slate-400 transition-transform duration-200', isCategoryExpanded(cat.category) ? 'rotate-180 text-[#059669]' : '']" />
                </div>
              </div>

              <!-- Holdings Table -->
              <div v-if="isCategoryExpanded(cat.category)" class="border-t border-slate-100 bg-white p-4 overflow-x-auto">
                <table class="w-full text-left text-xs">
                  <thead class="text-slate-500 font-semibold border-b border-slate-100 text-[11px]">
                    <tr>
                      <th class="py-3 px-3">Ativo</th>
                      <th class="py-3 px-2 text-right">Cotação</th>
                      <th class="py-3 px-2 text-right">Quantidade</th>
                      <th class="py-3 px-2 text-right">Preço médio</th>
                      <th class="py-3 px-2 text-right">Valor atual</th>
                      <th class="py-3 px-3 text-right">Lucro / Prejuízo</th>
                      <th class="py-3 px-2"></th>
                    </tr>
                  </thead>
                  <tbody class="divide-y divide-slate-100 font-mono-numbers">
                    <tr v-for="pos in cat.items" :key="pos.investmentId" @click="openPositionModal(pos)" class="hover:bg-slate-50 cursor-pointer transition group">
                      <!-- Ativo -->
                      <td class="py-3.5 px-3">
                        <div class="flex items-center gap-3">
                          <div class="w-7 h-7 rounded-lg bg-slate-100 border border-slate-200 flex items-center justify-center shrink-0">
                            <img v-if="pos.logoUrl" :src="getLogoUrl(pos.logoUrl)" :alt="pos.name" class="w-4 h-4 object-contain" />
                            <span v-else class="text-[10px] font-bold text-slate-600">{{ pos.name.substring(0, 1) }}</span>
                          </div>
                          <div>
                            <span class="block font-bold text-slate-900 font-sans text-xs group-hover:text-[#059669] transition">{{ pos.ticker || pos.name }}</span>
                            <span class="text-[11px] text-slate-500 font-sans block truncate max-w-[150px]">{{ pos.name }}</span>
                          </div>
                        </div>
                      </td>

                      <!-- Cotação -->
                      <td class="py-3.5 px-2 text-right">
                        <span class="font-semibold text-slate-800 block">{{ formatCurrency(pos.currentUnitPrice, pos.currency) }}</span>
                        <span class="text-[10px] text-emerald-600 block">Cotação Atual</span>
                      </td>

                      <!-- Quantidade -->
                      <td class="py-3.5 px-2 text-right text-slate-700">
                        {{ pos.quantity.toLocaleString() }}
                      </td>

                      <!-- Preço Médio -->
                      <td class="py-3.5 px-2 text-right text-slate-600">
                        {{ formatCurrency(pos.averagePrice, pos.currency) }}
                      </td>

                      <!-- Valor Atual -->
                      <td class="py-3.5 px-2 text-right font-bold text-slate-900">
                        {{ formatCurrency(pos.currentTotalValue, pos.currency) }}
                      </td>

                      <!-- Lucro / Prejuízo ($ e %) -->
                      <td class="py-3.5 px-3 text-right" :class="[pos.unrealizedGainLoss >= 0 ? 'text-[#059669]' : 'text-rose-600']">
                        <span class="block font-bold">{{ pos.unrealizedGainLoss >= 0 ? '+ ' : '' }}{{ formatCurrency(pos.unrealizedGainLoss, pos.currency) }}</span>
                        <span class="text-[10px] block font-semibold opacity-90">{{ pos.unrealizedGainLossPercentage >= 0 ? '+ ' : '' }}{{ pos.unrealizedGainLossPercentage.toFixed(2) }}%</span>
                      </td>

                      <!-- Action Button -->
                      <td class="py-3.5 px-2 text-right text-slate-400 group-hover:text-[#059669]">
                        <button class="p-1 rounded bg-slate-100 hover:bg-[#059669] hover:text-white transition" title="Ver Histórico & Editar">
                          <History class="w-3.5 h-3.5" />
                        </button>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </section>
      </template>
    </main>

    <!-- Modal 1: Add Broker Account -->
    <div v-if="showAddAccountModal" class="fixed inset-0 z-50 bg-slate-900/40 backdrop-blur-xs flex items-center justify-center p-4">
      <div class="bg-white border border-slate-200 w-full max-w-sm p-6 rounded-2xl shadow-xl space-y-4 text-xs">
        <div class="flex justify-between items-center border-b border-slate-100 pb-3">
          <h3 class="font-bold text-slate-900 text-sm">Nova Conta de Corretora</h3>
          <button @click="showAddAccountModal = false" class="text-slate-400 hover:text-slate-700">✕</button>
        </div>
        <form @submit.prevent="submitAddAccount" class="space-y-3">
          <div>
            <label class="block text-slate-600 mb-1 font-medium">Nome da Conta</label>
            <input v-model="newAccount.name" type="text" required placeholder="Avenue US / XP Investimentos" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none focus:border-[#059669]" />
          </div>
          <div>
            <label class="block text-slate-600 mb-1 font-medium">Instituição</label>
            <input v-model="newAccount.institution" type="text" required placeholder="Avenue / XP / IBKR / Caixa" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none focus:border-[#059669]" />
          </div>
          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-slate-600 mb-1 font-medium">Tipo</label>
              <select v-model="newAccount.accountType" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none">
                <option value="Brokerage">Corretora</option>
                <option value="Personal">Pessoal</option>
                <option value="Retirement_FGTS">FGTS</option>
                <option value="Joint">Conjunta</option>
              </select>
            </div>
            <div>
              <label class="block text-slate-600 mb-1 font-medium">Moeda Base</label>
              <select v-model="newAccount.baseCurrency" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none">
                <option value="BRL">BRL (R$)</option>
                <option value="USD">USD ($)</option>
              </select>
            </div>
          </div>
          <div class="flex justify-end gap-2 pt-2">
            <button type="button" @click="showAddAccountModal = false" class="px-4 py-2 rounded-lg bg-slate-100 text-slate-700 hover:bg-slate-200 font-medium">Cancelar</button>
            <button type="submit" class="px-5 py-2 rounded-lg bg-[#059669] hover:bg-[#047857] text-white font-bold">Salvar Conta</button>
          </div>
        </form>
      </div>
    </div>

    <!-- Modal 2: Add Master Asset -->
    <div v-if="showAddAssetModal" class="fixed inset-0 z-50 bg-slate-900/40 backdrop-blur-xs flex items-center justify-center p-4">
      <div class="bg-white border border-slate-200 w-full max-w-md p-6 rounded-2xl shadow-xl space-y-4 text-xs">
        <div class="flex justify-between items-center border-b border-slate-100 pb-3">
          <h3 class="font-bold text-slate-900 text-sm">Cadastrar Ativo Master</h3>
          <button @click="showAddAssetModal = false" class="text-slate-400 hover:text-slate-700">✕</button>
        </div>
        <form @submit.prevent="submitAddAsset" class="space-y-3">
          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-slate-600 mb-1 font-medium">Nome do Ativo</label>
              <input v-model="newAsset.name" type="text" required placeholder="Apple Inc" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none focus:border-[#059669]" />
            </div>
            <div>
              <label class="block text-slate-600 mb-1 font-medium">Ticker (Opcional)</label>
              <input v-model="newAsset.ticker" type="text" placeholder="AAPL / AMD" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none focus:border-[#059669] uppercase" />
            </div>
          </div>

          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-slate-600 mb-1 font-medium">Categoria</label>
              <select v-model="newAsset.assetCategory" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none">
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
              <select v-model="newAsset.valuationType" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none">
                <option value="TickerMarket">Cotação de Mercado</option>
                <option value="IndexLinked">Indexado a Índice</option>
                <option value="FixedRate">Taxa Fixa</option>
                <option value="ManualBalance">Saldo Manual</option>
                <option value="ManualFixedValue">Valor Fixo Manual</option>
              </select>
            </div>
          </div>

          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-slate-600 mb-1 font-medium">Moeda</label>
              <select v-model="newAsset.currency" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none">
                <option value="BRL">BRL (R$)</option>
                <option value="USD">USD ($)</option>
              </select>
            </div>
            <div>
              <label class="block text-slate-600 mb-1 font-medium">Indexador</label>
              <select v-model="newAsset.indexBenchmark" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none">
                <option value="None">Nenhum</option>
                <option value="CDI">CDI</option>
                <option value="IPCA">IPCA</option>
                <option value="SELIC">SELIC</option>
                <option value="IGPM">IGPM</option>
              </select>
            </div>
          </div>

          <div class="flex justify-end gap-2 pt-2">
            <button type="button" @click="showAddAssetModal = false" class="px-4 py-2 rounded-lg bg-slate-100 text-slate-700 hover:bg-slate-200 font-medium">Cancelar</button>
            <button type="submit" class="px-5 py-2 rounded-lg bg-[#059669] hover:bg-[#047857] text-white font-bold">Salvar Ativo</button>
          </div>
        </form>
      </div>
    </div>

    <!-- Modal 3: Add Holding / Investment -->
    <div v-if="showAddInvestmentModal" class="fixed inset-0 z-50 bg-slate-900/40 backdrop-blur-xs flex items-center justify-center p-4">
      <div class="bg-white border border-slate-200 w-full max-w-md p-6 rounded-2xl shadow-xl space-y-4 text-xs">
        <div class="flex justify-between items-center border-b border-slate-100 pb-3">
          <h3 class="font-bold text-slate-900 text-sm">+ Nova Posição de Investimento</h3>
          <button @click="showAddInvestmentModal = false" class="text-slate-400 hover:text-slate-700">✕</button>
        </div>
        <form @submit.prevent="submitAddInvestment" class="space-y-3">
          <div>
            <label class="block text-slate-600 mb-1 font-medium">Conta de Corretora</label>
            <select v-model="newHolding.accountId" required class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none">
              <option value="" disabled>-- Selecione a Conta --</option>
              <option v-for="acc in portfolioStore.accounts" :key="acc.id" :value="acc.id">
                {{ acc.name }} ({{ acc.institution }} - {{ acc.baseCurrency }})
              </option>
            </select>
          </div>

          <div>
            <label class="block text-slate-600 mb-1 font-medium">Ativo Master</label>
            <select v-model="newHolding.assetId" required class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none">
              <option value="" disabled>-- Selecione o Ativo --</option>
              <option v-for="ast in portfolioStore.assets" :key="ast.id" :value="ast.id">
                {{ ast.name }} {{ ast.ticker ? `(${ast.ticker})` : '' }} - {{ ast.assetCategory }} ({{ ast.currency }})
              </option>
            </select>
          </div>

          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-slate-600 mb-1 font-medium">Quantidade</label>
              <input v-model.number="newHolding.quantity" type="number" step="any" min="0.00000001" required placeholder="10" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none focus:border-[#059669]" />
            </div>
            <div>
              <label class="block text-slate-600 mb-1 font-medium">Preço de Compra</label>
              <input v-model.number="newHolding.pricePerUnit" type="number" step="any" min="0.01" required placeholder="150.00" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none focus:border-[#059669]" />
            </div>
          </div>

          <div>
            <label class="block text-slate-600 mb-1 font-medium">Data da Operação</label>
            <input v-model="newHolding.transactionDate" type="date" required class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none" />
          </div>

          <div class="flex justify-end gap-2 pt-2">
            <button type="button" @click="showAddInvestmentModal = false" class="px-4 py-2 rounded-lg bg-slate-100 text-slate-700 hover:bg-slate-200 font-medium">Cancelar</button>
            <button type="submit" class="px-5 py-2 rounded-lg bg-[#059669] hover:bg-[#047857] text-white font-bold">Salvar Operação</button>
          </div>
        </form>
      </div>
    </div>

    <!-- Modal 4: Position Details & Transaction History -->
    <div v-if="showPositionModal" class="fixed inset-0 z-50 bg-slate-900/50 backdrop-blur-xs flex items-center justify-center p-4">
      <div class="bg-white border border-slate-200 w-full max-w-2xl p-6 rounded-2xl shadow-2xl space-y-5 text-xs max-h-[90vh] flex flex-col">
        <!-- Header -->
        <div class="flex justify-between items-start border-b border-slate-100 pb-4 shrink-0">
          <div>
            <div class="flex items-center gap-2">
              <h3 class="font-bold text-slate-900 text-base">{{ selectedPosition?.name }}</h3>
              <span class="px-2 py-0.5 rounded bg-emerald-50 border border-emerald-200 text-[#059669] font-mono font-bold text-[11px]">
                {{ selectedPosition?.ticker || selectedPosition?.assetCategory }}
              </span>
            </div>
            <p class="text-slate-500 text-xs mt-0.5">
              Histórico de operações e configurações da posição
            </p>
          </div>
          <button @click="showPositionModal = false" class="text-slate-400 hover:text-slate-700 p-1">✕</button>
        </div>

        <!-- Position Overview Summary Row -->
        <div class="grid grid-cols-4 gap-3 bg-slate-50 p-3 rounded-xl border border-slate-100 text-center font-mono-numbers shrink-0">
          <div>
            <span class="text-[10px] text-slate-400 uppercase tracking-wider block font-sans">Quantidade</span>
            <span class="font-bold text-slate-900 text-sm">{{ selectedPosition?.quantity.toLocaleString() }}</span>
          </div>
          <div>
            <span class="text-[10px] text-slate-400 uppercase tracking-wider block font-sans">Preço Médio</span>
            <span class="font-bold text-slate-800 text-sm">{{ formatCurrency(selectedPosition?.averagePrice || 0, selectedPosition?.currency) }}</span>
          </div>
          <div>
            <span class="text-[10px] text-slate-400 uppercase tracking-wider block font-sans">Valor Atual</span>
            <span class="font-bold text-slate-900 text-sm">{{ formatCurrency(selectedPosition?.currentTotalValue || 0, selectedPosition?.currency) }}</span>
          </div>
          <div>
            <span class="text-[10px] text-slate-400 uppercase tracking-wider block font-sans">Lucro / Prejuízo</span>
            <span :class="[(selectedPosition?.unrealizedGainLoss || 0) >= 0 ? 'text-[#059669]' : 'text-rose-600']" class="font-bold text-sm">
              {{ (selectedPosition?.unrealizedGainLoss || 0) >= 0 ? '+' : '' }}{{ formatCurrency(selectedPosition?.unrealizedGainLoss || 0, selectedPosition?.currency) }}
            </span>
          </div>
        </div>

        <!-- Modal Tabs -->
        <div class="flex items-center gap-2 border-b border-slate-200 shrink-0">
          <button
            @click="positionModalTab = 'transactions'"
            :class="['pb-2 px-3 font-bold transition text-xs flex items-center gap-1.5', positionModalTab === 'transactions' ? 'text-[#059669] border-b-2 border-[#059669]' : 'text-slate-500 hover:text-slate-800']"
          >
            <History class="w-3.5 h-3.5" />
            <span>Histórico de Operações ({{ positionTransactions.length }})</span>
          </button>
          <button
            @click="positionModalTab = 'editPosition'"
            :class="['pb-2 px-3 font-bold transition text-xs flex items-center gap-1.5', positionModalTab === 'editPosition' ? 'text-[#059669] border-b-2 border-[#059669]' : 'text-slate-500 hover:text-slate-800']"
          >
            <Edit3 class="w-3.5 h-3.5" />
            <span>Editar Posição / Corrigir Ativo</span>
          </button>
        </div>

        <!-- TAB 1: Transactions History -->
        <div v-if="positionModalTab === 'transactions'" class="flex-1 overflow-y-auto space-y-4 pr-1">
          <div class="flex justify-between items-center">
            <span class="text-slate-600 font-semibold text-xs">Registro de compras, vendas e rendimentos</span>
            <button
              @click="toggleAddTxForm"
              class="px-3 py-1.5 rounded-lg bg-[#059669] hover:bg-[#047857] text-white font-bold text-xs flex items-center gap-1 shadow-xs transition"
            >
              <Plus class="w-3.5 h-3.5" />
              <span>+ Adicionar Transação</span>
            </button>
          </div>

          <!-- Inline New/Edit Transaction Form -->
          <div v-if="showTxForm" class="bg-slate-50 border border-slate-200 p-4 rounded-xl space-y-3">
            <h4 class="font-bold text-slate-800 text-xs">{{ editingTxId ? 'Editar Transação' : 'Nova Transação' }}</h4>
            <div class="grid grid-cols-3 gap-2">
              <div>
                <label class="block text-slate-600 mb-1 font-medium">Tipo</label>
                <select v-model="txForm.transactionType" class="w-full bg-white border border-slate-200 rounded-lg p-2 text-slate-900 outline-none">
                  <option value="Buy">Compra</option>
                  <option value="Sell">Venda</option>
                  <option value="Deposit">Aporte / Depósito</option>
                  <option value="Withdrawal">Saque / Resgate</option>
                  <option value="YieldAccrual">Rendimento</option>
                  <option value="Dividend">Provento / Dividendo</option>
                </select>
              </div>
              <div>
                <label class="block text-slate-600 mb-1 font-medium">Quantidade</label>
                <input v-model.number="txForm.quantity" type="number" step="any" min="0.00000001" required class="w-full bg-white border border-slate-200 rounded-lg p-2 text-slate-900 outline-none" />
              </div>
              <div>
                <label class="block text-slate-600 mb-1 font-medium">Preço por Unidade</label>
                <input v-model.number="txForm.pricePerUnit" type="number" step="any" min="0" required class="w-full bg-white border border-slate-200 rounded-lg p-2 text-slate-900 outline-none" />
              </div>
            </div>
            <div class="grid grid-cols-2 gap-2">
              <div>
                <label class="block text-slate-600 mb-1 font-medium">Data da Operação</label>
                <input v-model="txForm.transactionDate" type="date" required class="w-full bg-white border border-slate-200 rounded-lg p-2 text-slate-900 outline-none" />
              </div>
              <div>
                <label class="block text-slate-600 mb-1 font-medium">Notas / Observações</label>
                <input v-model="txForm.notes" type="text" placeholder="Ex: Aporte mensal" class="w-full bg-white border border-slate-200 rounded-lg p-2 text-slate-900 outline-none" />
              </div>
            </div>
            <div class="flex justify-end gap-2 pt-1">
              <button type="button" @click="showTxForm = false" class="px-3 py-1.5 rounded-lg bg-slate-200 text-slate-700 hover:bg-slate-300 font-medium">Cancelar</button>
              <button type="button" @click="handleSaveTransaction" class="px-4 py-1.5 rounded-lg bg-[#059669] hover:bg-[#047857] text-white font-bold">Salvar Transação</button>
            </div>
          </div>

          <!-- Loading state -->
          <div v-if="isLoadingTransactions" class="py-8 text-center text-slate-400 font-mono text-xs">
            Carregando transações...
          </div>

          <!-- Transactions List Table -->
          <div v-else class="bg-white border border-slate-200 rounded-xl overflow-hidden shadow-xs">
            <table class="w-full text-left text-xs">
              <thead class="bg-slate-50 border-b border-slate-200 text-slate-500 font-semibold text-[11px]">
                <tr>
                  <th class="py-2.5 px-3">Data</th>
                  <th class="py-2.5 px-3">Tipo</th>
                  <th class="py-2.5 px-3 text-right">Qtd</th>
                  <th class="py-2.5 px-3 text-right">Preço Unit.</th>
                  <th class="py-2.5 px-3 text-right">Total</th>
                  <th class="py-2.5 px-3 text-right">Ações</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-100 font-mono-numbers">
                <tr v-if="positionTransactions.length === 0">
                  <td colspan="6" class="py-8 text-center text-slate-400 font-sans">
                    Nenhuma operação registrada para este investimento.
                  </td>
                </tr>
                <tr v-for="tx in positionTransactions" :key="tx.id" class="hover:bg-slate-50">
                  <td class="py-2.5 px-3 font-sans text-slate-700">{{ new Date(tx.transactionDate).toLocaleDateString() }}</td>
                  <td class="py-2.5 px-3 font-sans">
                    <span :class="[tx.transactionType === 'Buy' || tx.transactionType === 'Deposit' ? 'bg-emerald-50 text-[#059669] border-emerald-200' : 'bg-rose-50 text-rose-600 border-rose-200']" class="px-2 py-0.5 rounded border text-[10px] font-bold">
                      {{ tx.transactionType === 'Buy' ? 'Compra' : tx.transactionType === 'Sell' ? 'Venda' : tx.transactionType }}
                    </span>
                  </td>
                  <td class="py-2.5 px-3 text-right font-semibold text-slate-900">{{ tx.quantity.toLocaleString() }}</td>
                  <td class="py-2.5 px-3 text-right text-slate-600">{{ formatCurrency(tx.pricePerUnit, tx.currency) }}</td>
                  <td class="py-2.5 px-3 text-right font-bold text-slate-900">{{ formatCurrency(tx.totalAmount, tx.currency) }}</td>
                  <td class="py-2.5 px-3 text-right font-sans">
                    <div class="flex items-center justify-end gap-1.5">
                      <button @click="editTx(tx)" class="p-1 rounded text-slate-400 hover:text-slate-900 hover:bg-slate-100 transition" title="Editar Operação">
                        <Edit3 class="w-3.5 h-3.5" />
                      </button>
                      <button @click="deleteTx(tx.id)" class="p-1 rounded text-slate-400 hover:text-rose-600 hover:bg-rose-50 transition" title="Excluir Operação">
                        <Trash2 class="w-3.5 h-3.5" />
                      </button>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- TAB 2: Edit Position / Change Asset -->
        <div v-else-if="positionModalTab === 'editPosition'" class="flex-1 overflow-y-auto space-y-4 pr-1">
          <div class="p-3 bg-amber-50 border border-amber-200 rounded-xl text-amber-800 text-xs">
            <strong class="font-bold">Errou ao selecionar o ativo?</strong> Você pode trocar o ativo master desta posição (ex: mudando de AAPL para outro ativo) sem precisar deletar suas operações!
          </div>

          <form @submit.prevent="handleSavePositionEdit" class="space-y-3">
            <div>
              <label class="block text-slate-600 mb-1 font-medium">Ativo Master Vinculado</label>
              <select v-model="positionEditForm.assetId" required class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none focus:border-[#059669]">
                <option v-for="ast in portfolioStore.assets" :key="ast.id" :value="ast.id">
                  {{ ast.name }} {{ ast.ticker ? `(${ast.ticker})` : '' }} - {{ ast.assetCategory }} ({{ ast.currency }})
                </option>
              </select>
            </div>

            <div>
              <label class="block text-slate-600 mb-1 font-medium">Conta / Corretora de Custódia</label>
              <select v-model="positionEditForm.accountId" required class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none focus:border-[#059669]">
                <option v-for="acc in portfolioStore.accounts" :key="acc.id" :value="acc.id">
                  {{ acc.name }} ({{ acc.institution }} - {{ acc.baseCurrency }})
                </option>
              </select>
            </div>

            <div>
              <label class="block text-slate-600 mb-1 font-medium">Nome Personalizado / Apelido (Opcional)</label>
              <input v-model="positionEditForm.customName" type="text" placeholder="Ex: Minha posição de longo prazo" class="w-full bg-slate-50 border border-slate-200 rounded-lg p-2.5 text-slate-900 outline-none focus:border-[#059669]" />
            </div>

            <div class="pt-3 border-t border-slate-100 flex justify-between items-center">
              <button type="button" @click="handleDeletePosition" class="px-4 py-2 rounded-lg bg-rose-50 border border-rose-200 hover:bg-rose-100 text-rose-600 font-bold transition flex items-center gap-1.5">
                <Trash2 class="w-3.5 h-3.5" />
                <span>Excluir Posição Inteira</span>
              </button>

              <button type="submit" class="px-5 py-2 rounded-lg bg-[#059669] hover:bg-[#047857] text-white font-bold transition">
                Salvar Alterações
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/authStore';
import { usePortfolioStore } from '@/stores/portfolioStore';
import { Search, Plus, Wallet, Layers, LogOut, TrendingUp, PieChart, ChevronDown, Briefcase, RefreshCw, History, Edit3, Trash2 } from '@lucide/vue';
import type { PositionSummary, Transaction } from '@/types';

const router = useRouter();
const authStore = useAuthStore();
const portfolioStore = usePortfolioStore();

const displayCurrency = ref<'BRL' | 'USD'>('BRL');
const searchQuery = ref('');
const expandedCategories = ref<string[]>([]);
const expandAll = ref(true);

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

const filteredPositions = computed(() => {
  if (!searchQuery.value.trim()) return positions.value;
  const q = searchQuery.value.toLowerCase();
  return positions.value.filter(p =>
    p.name.toLowerCase().includes(q) || (p.ticker && p.ticker.toLowerCase().includes(q))
  );
});

const availableCash = computed(() => {
  const cashPositions = positions.value.filter(p => p.assetCategory === 'Cash');
  if (cashPositions.length === 0) return 0;
  return cashPositions.reduce((sum, p) => {
    if (displayCurrency.value === 'BRL') {
      return sum + (p.currency === 'BRL' ? p.currentTotalValue : p.currentTotalValue * (summary.value?.usdBrlFxRate || 5.5));
    } else {
      return sum + (p.currency === 'USD' ? p.currentTotalValue : p.currentTotalValue / (summary.value?.usdBrlFxRate || 5.5));
    }
  }, 0);
});

// Group Positions by Category
const categoryGroups = computed(() => {
  const map = new Map<string, PositionSummary[]>();
  const netWorth = summary.value?.totalNetWorthBrl || 1;
  const fxRate = summary.value?.usdBrlFxRate || 5.5;

  for (const pos of filteredPositions.value) {
    const cat = pos.assetCategory || 'Outros';
    if (!map.has(cat)) map.set(cat, []);
    map.get(cat)!.push(pos);
  }

  const result: Array<{
    category: string;
    items: PositionSummary[];
    totalValue: number;
    totalGainLoss: number;
    returnPct: number;
    percentage: number;
  }> = [];

  for (const [category, items] of map.entries()) {
    let totalValBrl = 0;
    let totalCostBrl = 0;

    for (const item of items) {
      const itemValBrl = item.currency === 'USD' ? item.currentTotalValue * fxRate : item.currentTotalValue;
      const itemCostBrl = item.currency === 'USD' ? item.totalCost * fxRate : item.totalCost;
      totalValBrl += itemValBrl;
      totalCostBrl += itemCostBrl;
    }

    const totalValue = displayCurrency.value === 'BRL' ? totalValBrl : totalValBrl / fxRate;
    const totalCost = displayCurrency.value === 'BRL' ? totalCostBrl : totalCostBrl / fxRate;
    const totalGainLoss = totalValue - totalCost;
    const returnPct = totalCost > 0 ? (totalGainLoss / totalCost) * 100 : 0;
    const percentage = netWorth > 0 ? (totalValBrl / netWorth) * 100 : 0;

    result.push({
      category,
      items,
      totalValue: isNaN(totalValue) ? 0 : totalValue,
      totalGainLoss: isNaN(totalGainLoss) ? 0 : totalGainLoss,
      returnPct: isNaN(returnPct) ? 0 : returnPct,
      percentage: isNaN(percentage) ? 0 : percentage,
    });
  }

  if (expandAll.value && expandedCategories.value.length === 0 && result.length > 0) {
    expandedCategories.value = result.map(r => r.category);
  }

  return result.sort((a, b) => b.totalValue - a.totalValue);
});

function isCategoryExpanded(cat: string) {
  return expandAll.value || expandedCategories.value.includes(cat);
}

function toggleCategory(cat: string) {
  const idx = expandedCategories.value.indexOf(cat);
  if (idx >= 0) {
    expandedCategories.value.splice(idx, 1);
  } else {
    expandedCategories.value.push(cat);
  }
}

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

function formatCategoryName(cat: string) {
  const names: Record<string, string> = {
    Stock_BR: 'Ações (BR)',
    Stock_US: 'Ações (US / Exterior)',
    Etf_BR: 'ETFs (BR)',
    Etf_US: 'ETFs (US)',
    FixedIncome_BR: 'Renda Fixa',
    Crypto: 'Criptomoedas',
    REIT_BR: 'FIIs (BR)',
    REIT_US: 'REITs (US)',
    FGTS: 'Tesouro Direto & FGTS',
    Cash: 'Caixa',
  };
  return names[cat] || cat;
}

function calculateOverallReturnPct() {
  if (!summary.value) return '0.00';
  const invested = summary.value.totalInvestedBrl;
  const gain = summary.value.netGainLossBrl;
  if (!invested || invested <= 0 || isNaN(invested) || isNaN(gain)) return '0.00';
  const pct = (gain / invested) * 100;
  return isNaN(pct) ? '0.00' : pct.toFixed(2);
}

function formatCurrency(val: number, currency?: string) {
  if (val === undefined || val === null || isNaN(val)) return 'R$ 0,00';
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

  if (portfolioStore.accounts.length === 0) {
    alert('Nenhuma conta cadastrada. Por favor, crie uma conta de corretora primeiro!');
    showAddAccountModal.value = true;
    return;
  }

  if (portfolioStore.assets.length === 0) {
    alert('Nenhum ativo master cadastrado no catálogo. Por favor, cadastre um ativo master primeiro!');
    showAddAssetModal.value = true;
    return;
  }

  newHolding.value.accountId = portfolioStore.accounts[0].id;
  newHolding.value.assetId = portfolioStore.assets[0].id;
  showAddInvestmentModal.value = true;
}

async function submitAddAccount() {
  try {
    await portfolioStore.createAccount(newAccount.value);
    showAddAccountModal.value = false;
    newAccount.value = { name: '', institution: '', accountType: 'Brokerage', baseCurrency: 'BRL' };
    await portfolioStore.fetchAccounts();
  } catch (err: any) {
    const msg = err.response?.data?.message || err.response?.data?.title || err.message || 'Erro ao criar conta.';
    alert(`Erro ao adicionar conta: ${msg}`);
  }
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
  try {
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
  } catch (err: any) {
    const msg = err.response?.data?.message || err.response?.data?.title || err.message || 'Erro ao criar ativo.';
    alert(`Erro ao cadastrar ativo: ${msg}`);
  }
}

async function submitAddInvestment() {
  if (!newHolding.value.accountId || !newHolding.value.assetId) {
    alert('Por favor, selecione a conta e o ativo master.');
    return;
  }

  try {
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
      notes: 'Operação TradingCenter',
    });

    showAddInvestmentModal.value = false;
    await portfolioStore.fetchPortfolioSummary();
  } catch (err: any) {
    const msg = err.response?.data?.message || err.message || 'Erro ao registrar operação.';
    alert(`Erro ao adicionar operação: ${msg}`);
  }
}

// Market Price Sync
async function handleSyncMarketData() {
  try {
    await portfolioStore.syncMarketData();
    alert('Cotações e taxas sincronizadas com sucesso! As posições foram recalculadas.');
  } catch (err: any) {
    alert(`Erro ao sincronizar preços: ${err.response?.data?.message || err.message || 'Falha na comunicação com o servidor'}`);
  }
}

// Position Modal State & Logic
const showPositionModal = ref(false);
const selectedPosition = ref<PositionSummary | null>(null);
const positionTransactions = ref<Transaction[]>([]);
const isLoadingTransactions = ref(false);
const positionModalTab = ref<'transactions' | 'editPosition'>('transactions');

const positionEditForm = ref({
  assetId: '',
  accountId: '',
  customName: '',
});

const showTxForm = ref(false);
const editingTxId = ref<string | null>(null);
const txForm = ref({
  transactionType: 'Buy',
  transactionDate: new Date().toISOString().substring(0, 10),
  quantity: 1,
  pricePerUnit: 0,
  notes: '',
});

async function openPositionModal(pos: PositionSummary) {
  selectedPosition.value = pos;
  positionModalTab.value = 'transactions';
  showTxForm.value = false;
  editingTxId.value = null;

  const assetMatch = portfolioStore.assets.find(a => a.name === pos.name || a.ticker === pos.ticker);
  const accountMatch = portfolioStore.accounts[0];

  positionEditForm.value = {
    assetId: assetMatch?.id || portfolioStore.assets[0]?.id || '',
    accountId: accountMatch?.id || portfolioStore.accounts[0]?.id || '',
    customName: pos.name,
  };

  showPositionModal.value = true;
  await fetchPositionTransactions();
}

async function fetchPositionTransactions() {
  if (!selectedPosition.value) return;
  isLoadingTransactions.value = true;
  try {
    positionTransactions.value = await portfolioStore.fetchTransactions(selectedPosition.value.investmentId);
  } catch (err: any) {
    console.error('Failed to load transactions', err);
  } finally {
    isLoadingTransactions.value = false;
  }
}

function toggleAddTxForm() {
  editingTxId.value = null;
  txForm.value = {
    transactionType: 'Buy',
    transactionDate: new Date().toISOString().substring(0, 10),
    quantity: 1,
    pricePerUnit: selectedPosition.value?.averagePrice || 0,
    notes: '',
  };
  showTxForm.value = true;
}

function editTx(tx: Transaction) {
  editingTxId.value = tx.id;
  txForm.value = {
    transactionType: tx.transactionType as any,
    transactionDate: new Date(tx.transactionDate).toISOString().substring(0, 10),
    quantity: tx.quantity,
    pricePerUnit: tx.pricePerUnit,
    notes: tx.notes || '',
  };
  showTxForm.value = true;
}

async function handleSaveTransaction() {
  if (!selectedPosition.value) return;
  try {
    const payload = {
      investmentId: selectedPosition.value.investmentId,
      accountId: positionEditForm.value.accountId || portfolioStore.accounts[0]?.id,
      transactionType: txForm.value.transactionType,
      transactionDate: new Date(txForm.value.transactionDate).toISOString(),
      quantity: txForm.value.quantity,
      pricePerUnit: txForm.value.pricePerUnit,
      totalAmount: txForm.value.quantity * txForm.value.pricePerUnit,
      feeAmount: 0,
      taxAmount: 0,
      currency: selectedPosition.value.currency,
      notes: txForm.value.notes,
    };

    if (editingTxId.value) {
      await portfolioStore.updateTransaction(editingTxId.value, payload);
    } else {
      await portfolioStore.createTransaction(payload);
    }

    showTxForm.value = false;
    await fetchPositionTransactions();
    await portfolioStore.fetchPortfolioSummary();
    
    const updatedPos = portfolioStore.summary?.positions.find(p => p.investmentId === selectedPosition.value?.investmentId);
    if (updatedPos) selectedPosition.value = updatedPos;
  } catch (err: any) {
    alert(`Erro ao salvar transação: ${err.message}`);
  }
}

async function deleteTx(txId: string) {
  if (!confirm('Excluir esta transação?')) return;
  try {
    await portfolioStore.deleteTransaction(txId);
    await fetchPositionTransactions();
    await portfolioStore.fetchPortfolioSummary();
    const updatedPos = portfolioStore.summary?.positions.find(p => p.investmentId === selectedPosition.value?.investmentId);
    if (updatedPos) selectedPosition.value = updatedPos;
  } catch (err: any) {
    alert(`Erro ao excluir transação: ${err.message}`);
  }
}

async function handleSavePositionEdit() {
  if (!selectedPosition.value) return;
  try {
    await portfolioStore.updateInvestment(selectedPosition.value.investmentId, {
      accountId: positionEditForm.value.accountId,
      assetId: positionEditForm.value.assetId,
      customName: positionEditForm.value.customName || undefined,
    });

    showPositionModal.value = false;
    await portfolioStore.fetchPortfolioSummary();
    alert('Posição atualizada com sucesso!');
  } catch (err: any) {
    alert(`Erro ao atualizar posição: ${err.message}`);
  }
}

async function handleDeletePosition() {
  if (!selectedPosition.value) return;
  if (!confirm(`Tem certeza que deseja excluir a posição "${selectedPosition.value.name}" e todas as suas operações?`)) return;

  try {
    await portfolioStore.deleteInvestment(selectedPosition.value.investmentId);
    showPositionModal.value = false;
    await portfolioStore.fetchPortfolioSummary();
  } catch (err: any) {
    alert(`Erro ao excluir posição: ${err.message}`);
  }
}
</script>

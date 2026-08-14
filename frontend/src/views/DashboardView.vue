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
        </div>

        <!-- Toolbar & User Actions -->
        <div class="flex items-center gap-2 shrink-0 text-xs">
          <button
            @click="openAddInvestmentModal"
            class="px-3.5 py-1.5 rounded-lg bg-[#059669] hover:bg-[#047857] text-white font-medium flex items-center gap-1.5 shadow-xs transition"
          >
            <Plus class="w-3.5 h-3.5" />
            <span>+ Negociar</span>
          </button>

          <router-link
            to="/manage"
            class="hidden lg:flex items-center gap-1 px-3 py-1.5 rounded-lg border border-slate-200 text-slate-700 bg-white hover:bg-slate-50 transition font-medium text-xs"
          >
            <Layers class="w-3.5 h-3.5 text-slate-500" />
            <span>Gerenciar Ativos</span>
          </router-link>

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
                  {{ formatCurrency(displayCurrency === 'BRL' ? 1250 : 227.27) }}
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
                      <th class="py-3 px-2 text-right">Lucro ou prejuízo ($)</th>
                      <th class="py-3 px-3 text-right">Lucro ou prejuízo (%)</th>
                      <th class="py-3 px-2"></th>
                    </tr>
                  </thead>
                  <tbody class="divide-y divide-slate-100 font-mono-numbers">
                    <tr v-for="pos in cat.items" :key="pos.investmentId" class="hover:bg-slate-50/80 transition group">
                      <!-- Ativo -->
                      <td class="py-3.5 px-3">
                        <div class="flex items-center gap-3">
                          <div class="w-7 h-7 rounded-lg bg-slate-100 border border-slate-200 flex items-center justify-center shrink-0">
                            <img v-if="pos.logoUrl" :src="getLogoUrl(pos.logoUrl)" :alt="pos.name" class="w-4 h-4 object-contain" />
                            <span v-else class="text-[10px] font-bold text-slate-600">{{ pos.name.substring(0, 1) }}</span>
                          </div>
                          <div>
                            <span class="block font-bold text-slate-900 font-sans text-xs">{{ pos.ticker || pos.name }}</span>
                            <span class="text-[11px] text-slate-500 font-sans block truncate max-w-[150px]">{{ pos.name }}</span>
                          </div>
                        </div>
                      </td>

                      <!-- Cotação -->
                      <td class="py-3.5 px-2 text-right">
                        <span class="font-semibold text-slate-800 block">{{ formatCurrency(pos.currentUnitPrice, pos.currency) }}</span>
                        <span class="text-[10px] text-emerald-600 block">+ 0,50%</span>
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

                      <!-- Lucro/Prejuízo ($) -->
                      <td class="py-3.5 px-2 text-right font-semibold" :class="[pos.unrealizedGainLoss >= 0 ? 'text-[#059669]' : 'text-rose-600']">
                        {{ pos.unrealizedGainLoss >= 0 ? '+ ' : '' }}{{ formatCurrency(pos.unrealizedGainLoss, pos.currency) }}
                      </td>

                      <!-- Lucro/Prejuízo (%) -->
                      <td class="py-3.5 px-3 text-right font-bold" :class="[pos.unrealizedGainLossPercentage >= 0 ? 'text-[#059669]' : 'text-rose-600']">
                        {{ pos.unrealizedGainLossPercentage >= 0 ? '+ ' : '' }}{{ pos.unrealizedGainLossPercentage.toFixed(2) }}%
                      </td>

                      <!-- Arrow -->
                      <td class="py-3.5 px-2 text-right text-slate-400 group-hover:text-slate-800">
                        <ChevronRight class="w-4 h-4 ml-auto" />
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
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/authStore';
import { usePortfolioStore } from '@/stores/portfolioStore';
import { Search, Plus, Wallet, Layers, LogOut, TrendingUp, PieChart, ChevronDown, ChevronRight, Briefcase } from '@lucide/vue';
import type { PositionSummary } from '@/types';

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

// Group Positions by Category
const categoryGroups = computed(() => {
  const map = new Map<string, PositionSummary[]>();
  const netWorth = summary.value?.totalNetWorthBrl || 1;

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
    const totalValue = items.reduce((sum, i) => sum + (displayCurrency.value === 'BRL' ? i.currentTotalValue : i.currentTotalValue / (summary.value?.usdBrlFxRate || 5.5)), 0);
    const totalCost = items.reduce((sum, i) => sum + (displayCurrency.value === 'BRL' ? i.totalCost : i.totalCost / (summary.value?.usdBrlFxRate || 5.5)), 0);
    const totalGainLoss = totalValue - totalCost;
    const returnPct = totalCost > 0 ? (totalGainLoss / totalCost) * 100 : 0;
    
    const totalBrl = items.reduce((sum, i) => sum + i.currentTotalValue, 0);
    const percentage = netWorth > 0 ? (totalBrl / netWorth) * 100 : 0;

    result.push({ category, items, totalValue, totalGainLoss, returnPct, percentage });
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
</script>

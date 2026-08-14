<template>
  <div class="min-h-screen bg-[#12141d] text-zinc-100 font-sans selection:bg-[#00e676] selection:text-black flex flex-col">
    <!-- Top TradeMap Navigation Header -->
    <header class="border-b border-zinc-800/80 bg-[#161922] sticky top-0 z-40">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 h-16 flex items-center justify-between gap-4">
        <!-- Search Bar & Brand -->
        <div class="flex items-center gap-4 flex-1 max-w-xl">
          <router-link to="/dashboard" class="flex items-center gap-2.5 shrink-0">
            <div class="w-8 h-8 rounded bg-[#00e676] flex items-center justify-center font-black text-black text-xs shadow-sm">
              TM
            </div>
            <span class="text-sm font-bold text-white tracking-tight hidden md:inline">TradeMap</span>
          </router-link>

          <!-- Search Input -->
          <div class="relative w-full">
            <Search class="w-4 h-4 text-zinc-500 absolute left-3 top-2.5" />
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Pesquise por ativos, notícias e muito mais"
              class="w-full bg-[#1c1f2b] border border-zinc-800 focus:border-[#00e676] rounded-md pl-9 pr-3 py-1.5 text-xs text-white placeholder-zinc-500 outline-none transition"
            />
          </div>
        </div>

        <!-- Action Toolbar & Buttons -->
        <div class="flex items-center gap-2 shrink-0 text-xs">
          <button
            @click="openAddInvestmentModal"
            class="px-3.5 py-1.5 rounded-md bg-[#00e676] hover:bg-[#00c853] text-black font-semibold flex items-center gap-1.5 shadow-sm transition"
          >
            <Zap class="w-3.5 h-3.5 fill-black" />
            <span>+ Negociar</span>
          </button>

          <button
            @click="showAddAccountModal = true"
            class="hidden lg:flex items-center gap-1 px-3 py-1.5 rounded-md border border-[#00e676]/60 text-[#00e676] bg-[#161922] hover:bg-[#00e676]/10 transition font-medium text-[11px]"
          >
            <Wallet class="w-3.5 h-3.5" />
            <span>Boleta Manual</span>
          </button>

          <router-link
            to="/manage"
            class="hidden lg:flex items-center gap-1 px-3 py-1.5 rounded-md border border-[#00e676]/60 text-[#00e676] bg-[#161922] hover:bg-[#00e676]/10 transition font-medium text-[11px]"
          >
            <Layers class="w-3.5 h-3.5" />
            <span>Gerenciar Ativos</span>
          </router-link>

          <!-- Currency Selector Toggle -->
          <div class="bg-[#1c1f2b] border border-zinc-800 p-0.5 rounded-md flex items-center">
            <button
              @click="displayCurrency = 'BRL'"
              :class="['px-2 py-0.5 rounded text-[11px] font-mono transition', displayCurrency === 'BRL' ? 'bg-[#00e676] text-black font-bold' : 'text-zinc-400 hover:text-white']"
            >
              🇧🇷 BRL
            </button>
            <button
              @click="displayCurrency = 'USD'"
              :class="['px-2 py-0.5 rounded text-[11px] font-mono transition', displayCurrency === 'USD' ? 'bg-[#00e676] text-black font-bold' : 'text-zinc-400 hover:text-white']"
            >
              🇺🇸 USD
            </button>
          </div>

          <!-- User Menu & Logout -->
          <div class="flex items-center gap-2 pl-2 border-l border-zinc-800">
            <button
              @click="handleLogout"
              class="p-1.5 rounded-md bg-[#1c1f2b] border border-zinc-800 hover:bg-zinc-800 text-zinc-400 hover:text-white transition"
              title="Sair"
            >
              <LogOut class="w-3.5 h-3.5" />
            </button>
          </div>
        </div>
      </div>
    </header>

    <!-- Main TradeMap Dashboard Content Grid -->
    <main class="flex-1 max-w-7xl w-full mx-auto px-4 sm:px-6 py-6 grid grid-cols-1 lg:grid-cols-12 gap-6">
      <!-- Loading State -->
      <div v-if="portfolioStore.isLoading && !summary" class="lg:col-span-12 py-24 text-center font-mono text-xs text-zinc-500">
        Carregando patrimônio do TradeMap...
      </div>

      <template v-else>
        <!-- LEFT PANEL (Patrimônio, Donut Allocation & Performance Stats) -->
        <aside class="lg:col-span-5 space-y-6">
          <!-- Patrimônio Card -->
          <div class="bg-[#161922] border border-zinc-800/90 rounded-xl p-5 shadow-lg space-y-5">
            <!-- Header -->
            <div class="flex items-center justify-between border-b border-zinc-800/80 pb-3">
              <div class="flex items-center gap-2">
                <Wallet class="w-4 h-4 text-[#00e676]" />
                <span class="text-sm font-semibold text-[#00e676]">Patrimônio</span>
              </div>
              <span class="text-[11px] font-mono text-zinc-400 bg-[#1c1f2b] px-2 py-0.5 rounded">
                PTAX: R$ {{ (summary?.usdBrlFxRate || 5.50).toFixed(4) }}
              </span>
            </div>

            <!-- Big Net Worth Value -->
            <div>
              <div class="text-3xl font-extrabold text-[#00e676] tracking-tight font-mono-numbers">
                {{ formatCurrency(displayCurrency === 'BRL' ? (summary?.totalNetWorthBrl || 0) : (summary?.totalNetWorthUsd || 0)) }}
              </div>
              <div class="text-xs text-zinc-400 font-mono mt-1 space-x-3">
                <span>Custo: <strong class="text-zinc-200">{{ formatCurrency(displayCurrency === 'BRL' ? (summary?.totalInvestedBrl || 0) : (summary?.totalInvestedUsd || 0)) }}</strong></span>
                <span>Variação: <strong :class="[(summary?.netGainLossBrl || 0) >= 0 ? 'text-emerald-400' : 'text-rose-400']">{{ (summary?.netGainLossBrl || 0) >= 0 ? '+' : '' }}{{ formatCurrency(displayCurrency === 'BRL' ? (summary?.netGainLossBrl || 0) : (summary?.netGainLossUsd || 0)) }} ({{ calculateOverallReturnPct() }}%)</strong></span>
              </div>
            </div>

            <!-- Donut Chart Allocation -->
            <div class="relative flex items-center justify-center py-2 min-h-[200px]">
              <Doughnut v-if="chartData.labels.length > 0" :data="chartData" :options="chartOptions" class="max-h-[220px]" />
              <div v-else class="text-xs text-zinc-500 font-mono text-center">
                Sem posições ativas para gráfico
              </div>
            </div>
          </div>

          <!-- Meu Desempenho Card (12 Meses Benchmark Comparison) -->
          <div class="bg-[#161922] border border-zinc-800/90 rounded-xl p-5 shadow-lg space-y-4">
            <div class="flex items-center justify-between border-b border-zinc-800/80 pb-3">
              <span class="text-xs font-bold text-[#00e676] flex items-center gap-1.5">
                <TrendingUp class="w-4 h-4 text-[#00e676]" />
                Meu Desempenho - 12 Meses
              </span>
              <span class="text-[10px] text-zinc-500 font-mono">Consolidado</span>
            </div>

            <div class="text-xs text-zinc-300 font-mono space-y-1">
              <span>Retorno Carteira: <strong class="text-[#00e676] font-bold">{{ calculateOverallReturnPct() }}%</strong></span>
            </div>

            <!-- Horizontal Comparison Bars -->
            <div class="space-y-2 text-xs font-mono">
              <!-- Carteira Bar -->
              <div>
                <div class="flex justify-between text-[11px] mb-1">
                  <span class="text-zinc-400">Carteira</span>
                  <span class="text-[#00e676] font-bold">{{ calculateOverallReturnPct() }}%</span>
                </div>
                <div class="w-full bg-[#1c1f2b] h-2.5 rounded-full overflow-hidden">
                  <div class="bg-[#00e676] h-full rounded-full transition-all duration-500" :style="{ width: Math.min(Math.max(parseFloat(calculateOverallReturnPct()) * 3, 5), 100) + '%' }"></div>
                </div>
              </div>

              <!-- IBOV Bar -->
              <div>
                <div class="flex justify-between text-[11px] mb-1">
                  <span class="text-zinc-400">Ibov</span>
                  <span class="text-blue-400 font-bold">21,16%</span>
                </div>
                <div class="w-full bg-[#1c1f2b] h-2.5 rounded-full overflow-hidden">
                  <div class="bg-blue-500 h-full rounded-full" style="width: 65%"></div>
                </div>
              </div>

              <!-- CDI Bar -->
              <div>
                <div class="flex justify-between text-[11px] mb-1">
                  <span class="text-zinc-400">CDI</span>
                  <span class="text-indigo-400 font-bold">14,74%</span>
                </div>
                <div class="w-full bg-[#1c1f2b] h-2.5 rounded-full overflow-hidden">
                  <div class="bg-indigo-500 h-full rounded-full" style="width: 48%"></div>
                </div>
              </div>

              <!-- Dólar Bar -->
              <div>
                <div class="flex justify-between text-[11px] mb-1">
                  <span class="text-zinc-400">Dólar</span>
                  <span class="text-rose-400 font-bold">-4,06%</span>
                </div>
                <div class="w-full bg-[#1c1f2b] h-2.5 rounded-full overflow-hidden">
                  <div class="bg-rose-500 h-full rounded-full" style="width: 15%"></div>
                </div>
              </div>
            </div>

            <!-- Financial Metrics Badges -->
            <div class="grid grid-cols-3 gap-2 pt-2 text-center text-[11px] font-mono border-t border-zinc-800/80">
              <div class="bg-[#1c1f2b] p-2 rounded border border-zinc-800">
                <span class="block text-zinc-500 text-[10px]">Sharpe</span>
                <span class="font-bold text-white">0,20</span>
              </div>
              <div class="bg-[#1c1f2b] p-2 rounded border border-zinc-800">
                <span class="block text-zinc-500 text-[10px]">Volatilidade</span>
                <span class="font-bold text-white">11,01%</span>
              </div>
              <div class="bg-[#1c1f2b] p-2 rounded border border-zinc-800">
                <span class="block text-zinc-500 text-[10px]">Beta (IBOV)</span>
                <span class="font-bold text-white">0,43</span>
              </div>
            </div>

            <!-- Circular Stats Badges -->
            <div class="grid grid-cols-3 gap-2 pt-1 text-center font-mono">
              <div class="p-2 rounded-lg border border-[#00e676]/40 bg-[#00e676]/5 text-[10px]">
                <span class="block font-bold text-[#00e676]">6 de 13</span>
                <span class="text-zinc-400 text-[9px]">meses > CDI</span>
              </div>
              <div class="p-2 rounded-lg border border-[#00e676]/40 bg-[#00e676]/5 text-[10px]">
                <span class="block font-bold text-[#00e676]">9 de 13</span>
                <span class="text-zinc-400 text-[9px]">meses positivo</span>
              </div>
              <div class="p-2 rounded-lg border border-[#00e676]/40 bg-[#00e676]/5 text-[10px]">
                <span class="block font-bold text-[#00e676]">5 de 13</span>
                <span class="text-zinc-400 text-[9px]">meses > IBOV</span>
              </div>
            </div>
          </div>
        </aside>

        <!-- RIGHT PANEL (Meus Ativos Accordion Categories) -->
        <section class="lg:col-span-7 space-y-4">
          <div class="bg-[#161922] border border-zinc-800/90 rounded-xl p-5 shadow-lg space-y-4">
            <!-- Header -->
            <div class="flex items-center justify-between border-b border-zinc-800/80 pb-3">
              <div class="flex items-center gap-2">
                <PieChart class="w-4 h-4 text-[#00e676]" />
                <h2 class="text-sm font-bold text-[#00e676]">Meus Ativos</h2>
              </div>
              <span class="text-xs text-zinc-400 font-mono">{{ filteredPositions.length }} ativo(s)</span>
            </div>

            <!-- Category Accordions -->
            <div class="space-y-3">
              <div v-if="categoryGroups.length === 0" class="py-16 text-center text-xs text-zinc-500 font-sans">
                Nenhum ativo encontrado na carteira. Clique em "+ Negociar" para adicionar.
              </div>

              <div
                v-for="cat in categoryGroups"
                :key="cat.category"
                class="bg-[#1c1f2b] border border-zinc-800/80 rounded-lg overflow-hidden transition"
              >
                <!-- Category Accordion Header -->
                <button
                  @click="toggleCategory(cat.category)"
                  class="w-full px-4 py-3.5 flex items-center justify-between hover:bg-zinc-800/40 transition text-xs text-left"
                >
                  <div class="flex items-center gap-3">
                    <!-- Circle Percentage Badge -->
                    <div class="w-10 h-10 rounded-full border border-[#00e676]/50 bg-[#00e676]/10 flex items-center justify-center text-[10px] font-bold text-[#00e676] font-mono shrink-0">
                      {{ cat.percentage.toFixed(1) }}%
                    </div>
                    <div>
                      <span class="block font-bold text-white text-sm">{{ formatCategoryName(cat.category) }}</span>
                      <span class="text-[11px] text-zinc-400 font-mono">{{ cat.items.length }} ativo(s)</span>
                    </div>
                  </div>

                  <div class="flex items-center gap-3">
                    <span class="font-bold text-white font-mono text-sm">
                      {{ formatCurrency(cat.totalValue) }}
                    </span>
                    <ChevronDown :class="['w-4 h-4 text-zinc-400 transition-transform duration-200', expandedCategories.includes(cat.category) ? 'rotate-180 text-[#00e676]' : '']" />
                  </div>
                </button>

                <!-- Expanded Asset Items Table -->
                <div v-if="expandedCategories.includes(cat.category)" class="border-t border-zinc-800/60 bg-[#161922] p-3 overflow-x-auto">
                  <table class="w-full text-left text-xs">
                    <thead class="text-zinc-500 font-mono border-b border-zinc-800 text-[11px]">
                      <tr>
                        <th class="py-2 px-3">Ativo</th>
                        <th class="py-2 px-2 text-right">Qtd</th>
                        <th class="py-2 px-2 text-right">Custo Médio</th>
                        <th class="py-2 px-2 text-right">Preço Atual</th>
                        <th class="py-2 px-2 text-right">Valor Total</th>
                        <th class="py-2 px-3 text-right">Lucro/Prejuízo</th>
                      </tr>
                    </thead>
                    <tbody class="divide-y divide-zinc-800/40 font-mono-numbers">
                      <tr v-for="pos in cat.items" :key="pos.investmentId" class="hover:bg-zinc-900/60 transition">
                        <td class="py-2.5 px-3">
                          <div class="flex items-center gap-2">
                            <div class="w-5 h-5 rounded bg-zinc-900 border border-zinc-800 flex items-center justify-center shrink-0">
                              <img v-if="pos.logoUrl" :src="getLogoUrl(pos.logoUrl)" :alt="pos.name" class="w-3.5 h-3.5 object-contain" />
                              <span v-else class="text-[9px] font-bold text-zinc-500">{{ pos.name.substring(0, 1) }}</span>
                            </div>
                            <div>
                              <span class="block font-medium text-white font-sans text-xs">{{ pos.name }}</span>
                              <span v-if="pos.ticker" class="text-[10px] text-zinc-500 font-mono">{{ pos.ticker }}</span>
                            </div>
                          </div>
                        </td>

                        <td class="py-2.5 px-2 text-right text-zinc-200">
                          {{ pos.quantity.toLocaleString() }}
                        </td>

                        <td class="py-2.5 px-2 text-right text-zinc-400">
                          {{ formatCurrency(pos.averagePrice, pos.currency) }}
                        </td>

                        <td class="py-2.5 px-2 text-right text-[#00e676] font-semibold">
                          {{ formatCurrency(pos.currentUnitPrice, pos.currency) }}
                        </td>

                        <td class="py-2.5 px-2 text-right font-bold text-white">
                          {{ formatCurrency(pos.currentTotalValue, pos.currency) }}
                        </td>

                        <td class="py-2.5 px-3 text-right">
                          <span :class="[pos.unrealizedGainLoss >= 0 ? 'text-emerald-400' : 'text-rose-400']">
                            {{ pos.unrealizedGainLoss >= 0 ? '+' : '' }}{{ formatCurrency(pos.unrealizedGainLoss, pos.currency) }}
                          </span>
                          <span :class="['block text-[9px]', pos.unrealizedGainLoss >= 0 ? 'text-emerald-500' : 'text-rose-500']">
                            ({{ pos.unrealizedGainLossPercentage >= 0 ? '+' : '' }}{{ pos.unrealizedGainLossPercentage.toFixed(2) }}%)
                          </span>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          </div>
        </section>
      </template>
    </main>

    <!-- Modal 1: Add Broker Account -->
    <div v-if="showAddAccountModal" class="fixed inset-0 z-50 bg-black/80 flex items-center justify-center p-4">
      <div class="bg-[#161922] border border-zinc-800 w-full max-w-sm p-5 rounded-xl space-y-4 text-xs">
        <div class="flex justify-between items-center border-b border-zinc-800 pb-2">
          <h3 class="font-bold text-white">Nova Conta / Corretora</h3>
          <button @click="showAddAccountModal = false" class="text-zinc-500 hover:text-white">✕</button>
        </div>
        <form @submit.prevent="submitAddAccount" class="space-y-3">
          <div>
            <label class="block text-zinc-400 mb-1">Nome da Conta</label>
            <input v-model="newAccount.name" type="text" required placeholder="XP Investimentos" class="w-full bg-[#1c1f2b] border border-zinc-800 rounded p-2 text-white outline-none focus:border-[#00e676]" />
          </div>
          <div>
            <label class="block text-zinc-400 mb-1">Instituição</label>
            <input v-model="newAccount.institution" type="text" required placeholder="XP / IBKR / Caixa" class="w-full bg-[#1c1f2b] border border-zinc-800 rounded p-2 text-white outline-none focus:border-[#00e676]" />
          </div>
          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-zinc-400 mb-1">Tipo</label>
              <select v-model="newAccount.accountType" class="w-full bg-[#1c1f2b] border border-zinc-800 rounded p-2 text-white outline-none">
                <option value="Brokerage">Corretora</option>
                <option value="Personal">Pessoal</option>
                <option value="Retirement_FGTS">FGTS</option>
                <option value="Joint">Conjunta</option>
              </select>
            </div>
            <div>
              <label class="block text-zinc-400 mb-1">Moeda Base</label>
              <select v-model="newAccount.baseCurrency" class="w-full bg-[#1c1f2b] border border-zinc-800 rounded p-2 text-white outline-none">
                <option value="BRL">BRL (R$)</option>
                <option value="USD">USD ($)</option>
              </select>
            </div>
          </div>
          <div class="flex justify-end gap-2 pt-2">
            <button type="button" @click="showAddAccountModal = false" class="px-3 py-1.5 rounded bg-zinc-800 text-zinc-300">Cancelar</button>
            <button type="submit" class="px-4 py-1.5 rounded bg-[#00e676] text-black font-semibold">Salvar Conta</button>
          </div>
        </form>
      </div>
    </div>

    <!-- Modal 2: Add Master Asset -->
    <div v-if="showAddAssetModal" class="fixed inset-0 z-50 bg-black/80 flex items-center justify-center p-4">
      <div class="bg-[#161922] border border-zinc-800 w-full max-w-md p-5 rounded-xl space-y-4 text-xs">
        <div class="flex justify-between items-center border-b border-zinc-800 pb-2">
          <h3 class="font-bold text-white">Cadastrar Novo Ativo Master</h3>
          <button @click="showAddAssetModal = false" class="text-zinc-500 hover:text-white">✕</button>
        </div>
        <form @submit.prevent="submitAddAsset" class="space-y-3">
          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-zinc-400 mb-1">Nome do Ativo</label>
              <input v-model="newAsset.name" type="text" required placeholder="Apple Inc" class="w-full bg-[#1c1f2b] border border-zinc-800 rounded p-2 text-white outline-none focus:border-[#00e676]" />
            </div>
            <div>
              <label class="block text-zinc-400 mb-1">Ticker (Opcional)</label>
              <input v-model="newAsset.ticker" type="text" placeholder="AAPL / PETR4" class="w-full bg-[#1c1f2b] border border-zinc-800 rounded p-2 text-white outline-none focus:border-[#00e676] uppercase" />
            </div>
          </div>

          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-zinc-400 mb-1">Categoria</label>
              <select v-model="newAsset.assetCategory" class="w-full bg-[#1c1f2b] border border-zinc-800 rounded p-2 text-white outline-none">
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
              <label class="block text-zinc-400 mb-1">Tipo de Valoração</label>
              <select v-model="newAsset.valuationType" class="w-full bg-[#1c1f2b] border border-zinc-800 rounded p-2 text-white outline-none">
                <option value="TickerMarket">Cotação de Mercado</option>
                <option value="IndexLinked">Indexado a Índice</option>
                <option value="ManualFixedValue">Valor Fixo Manual</option>
              </select>
            </div>
          </div>

          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-zinc-400 mb-1">Moeda</label>
              <select v-model="newAsset.currency" class="w-full bg-[#1c1f2b] border border-zinc-800 rounded p-2 text-white outline-none">
                <option value="BRL">BRL (R$)</option>
                <option value="USD">USD ($)</option>
              </select>
            </div>
            <div>
              <label class="block text-zinc-400 mb-1">Indexador</label>
              <select v-model="newAsset.indexBenchmark" class="w-full bg-[#1c1f2b] border border-zinc-800 rounded p-2 text-white outline-none">
                <option value="None">Nenhum</option>
                <option value="CDI">CDI</option>
                <option value="IPCA">IPCA</option>
                <option value="SELIC">SELIC</option>
                <option value="IGPM">IGPM</option>
              </select>
            </div>
          </div>

          <div class="flex justify-end gap-2 pt-2">
            <button type="button" @click="showAddAssetModal = false" class="px-3 py-1.5 rounded bg-zinc-800 text-zinc-300">Cancelar</button>
            <button type="submit" class="px-4 py-1.5 rounded bg-[#00e676] text-black font-semibold">Salvar Ativo</button>
          </div>
        </form>
      </div>
    </div>

    <!-- Modal 3: Add Holding / Investment -->
    <div v-if="showAddInvestmentModal" class="fixed inset-0 z-50 bg-black/80 flex items-center justify-center p-4">
      <div class="bg-[#161922] border border-zinc-800 w-full max-w-md p-5 rounded-xl space-y-4 text-xs">
        <div class="flex justify-between items-center border-b border-zinc-800 pb-2">
          <h3 class="font-bold text-white">+ Negociar / Adicionar Posição</h3>
          <button @click="showAddInvestmentModal = false" class="text-zinc-500 hover:text-white">✕</button>
        </div>
        <form @submit.prevent="submitAddInvestment" class="space-y-3">
          <div>
            <label class="block text-zinc-400 mb-1">Conta / Corretora</label>
            <select v-model="newHolding.accountId" required class="w-full bg-[#1c1f2b] border border-zinc-800 rounded p-2 text-white outline-none">
              <option value="" disabled>-- Selecione a Conta --</option>
              <option v-for="acc in portfolioStore.accounts" :key="acc.id" :value="acc.id">
                {{ acc.name }} ({{ acc.institution }} - {{ acc.baseCurrency }})
              </option>
            </select>
          </div>

          <div>
            <label class="block text-zinc-400 mb-1">Ativo Master</label>
            <select v-model="newHolding.assetId" required class="w-full bg-[#1c1f2b] border border-zinc-800 rounded p-2 text-white outline-none">
              <option value="" disabled>-- Selecione o Ativo Master --</option>
              <option v-for="ast in portfolioStore.assets" :key="ast.id" :value="ast.id">
                {{ ast.name }} {{ ast.ticker ? `(${ast.ticker})` : '' }} - {{ ast.assetCategory }} ({{ ast.currency }})
              </option>
            </select>
          </div>

          <div class="grid grid-cols-2 gap-2">
            <div>
              <label class="block text-zinc-400 mb-1">Quantidade</label>
              <input v-model.number="newHolding.quantity" type="number" step="any" min="0.00000001" required placeholder="10" class="w-full bg-[#1c1f2b] border border-zinc-800 rounded p-2 text-white outline-none focus:border-[#00e676]" />
            </div>
            <div>
              <label class="block text-zinc-400 mb-1">Preço de Compra</label>
              <input v-model.number="newHolding.pricePerUnit" type="number" step="any" min="0.01" required placeholder="150.00" class="w-full bg-[#1c1f2b] border border-zinc-800 rounded p-2 text-white outline-none focus:border-[#00e676]" />
            </div>
          </div>

          <div>
            <label class="block text-zinc-400 mb-1">Data da Operação</label>
            <input v-model="newHolding.transactionDate" type="date" required class="w-full bg-[#1c1f2b] border border-zinc-800 rounded p-2 text-white outline-none" />
          </div>

          <div class="flex justify-end gap-2 pt-2">
            <button type="button" @click="showAddInvestmentModal = false" class="px-3 py-1.5 rounded bg-zinc-800 text-zinc-300">Cancelar</button>
            <button type="submit" class="px-4 py-1.5 rounded bg-[#00e676] text-black font-semibold">Salvar Operação</button>
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
import { Search, Zap, Wallet, Layers, LogOut, TrendingUp, PieChart, ChevronDown } from '@lucide/vue';
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from 'chart.js';
import { Doughnut } from 'vue-chartjs';
import type { PositionSummary } from '@/types';

ChartJS.register(ArcElement, Tooltip, Legend);

const router = useRouter();
const authStore = useAuthStore();
const portfolioStore = usePortfolioStore();

const displayCurrency = ref<'BRL' | 'USD'>('BRL');
const searchQuery = ref('');
const expandedCategories = ref<string[]>([]);

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

// Group Positions by TradeMap Asset Category
const categoryGroups = computed(() => {
  const map = new Map<string, PositionSummary[]>();
  const netWorth = summary.value?.totalNetWorthBrl || 1;

  for (const pos of filteredPositions.value) {
    const cat = pos.assetCategory || 'Outros';
    if (!map.has(cat)) map.set(cat, []);
    map.get(cat)!.push(pos);
  }

  const result: Array<{ category: string; items: PositionSummary[]; totalValue: number; percentage: number }> = [];

  for (const [category, items] of map.entries()) {
    const totalValue = items.reduce((sum, i) => sum + (displayCurrency.value === 'BRL' ? i.currentTotalValue : i.currentTotalValue / (summary.value?.usdBrlFxRate || 5.5)), 0);
    const totalBrl = items.reduce((sum, i) => sum + i.currentTotalValue, 0);
    const percentage = netWorth > 0 ? (totalBrl / netWorth) * 100 : 0;
    result.push({ category, items, totalValue, percentage });
  }

  // Automatically expand all categories by default if not set
  if (expandedCategories.value.length === 0 && result.length > 0) {
    expandedCategories.value = result.map(r => r.category);
  }

  return result.sort((a, b) => b.totalValue - a.totalValue);
});

// Chart.js Donut Data
const chartData = computed(() => {
  const labels = categoryGroups.value.map(c => formatCategoryName(c.category));
  const data = categoryGroups.value.map(c => c.totalValue);
  const colors = [
    '#00e676', '#3b82f6', '#8b5cf6', '#ec4899', '#f59e0b',
    '#10b981', '#06b6d4', '#6366f1', '#a855f7', '#f43f5e'
  ];

  return {
    labels,
    datasets: [
      {
        backgroundColor: colors.slice(0, labels.length),
        borderWidth: 0,
        data,
      },
    ],
  };
});

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'bottom' as const,
      labels: {
        color: '#9ca3af',
        font: { size: 10 },
        boxWidth: 10,
      },
    },
  },
  cutout: '70%',
};

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

function toggleCategory(cat: string) {
  const idx = expandedCategories.value.indexOf(cat);
  if (idx >= 0) {
    expandedCategories.value.splice(idx, 1);
  } else {
    expandedCategories.value.push(cat);
  }
}

function formatCategoryName(cat: string) {
  const names: Record<string, string> = {
    Stock_BR: 'Ações (BR)',
    Stock_US: 'Exterior (US)',
    Etf_BR: 'ETFs (BR)',
    Etf_US: 'ETFs (US)',
    FixedIncome_BR: 'Renda Fixa',
    Crypto: 'Criptomoedas',
    REIT_BR: 'FIIs (BR)',
    REIT_US: 'REITs (US)',
    FGTS: 'Tesouro Direto & FGTS',
    Cash: 'Caixa & Provisões',
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
    notes: 'Operação TradeMap',
  });

  showAddInvestmentModal.value = false;
  await portfolioStore.fetchPortfolioSummary();
}
</script>

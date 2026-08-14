import { defineStore } from 'pinia';
import { ref } from 'vue';
import apiClient from '@/api/client';
import type { PortfolioSummary, PortfolioSnapshot, Account, Asset, Investment, Transaction } from '@/types';

export const usePortfolioStore = defineStore('portfolio', () => {
  const summary = ref<PortfolioSummary | null>(null);
  const snapshots = ref<PortfolioSnapshot[]>([]);
  const accounts = ref<Account[]>([]);
  const assets = ref<Asset[]>([]);
  const investments = ref<Investment[]>([]);
  const isSyncingPrices = ref(false);
  const isLoading = ref(false);
  const error = ref<string | null>(null);

  async function fetchPortfolioSummary() {
    isLoading.value = true;
    error.value = null;
    try {
      const response = await apiClient.get<PortfolioSummary>('/portfolio/summary');
      summary.value = response.data;
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Failed to load portfolio summary.';
    } finally {
      isLoading.value = false;
    }
  }

  async function fetchPortfolioHistory() {
    try {
      const response = await apiClient.get<PortfolioSnapshot[]>('/portfolio/history');
      snapshots.value = response.data;
    } catch (err: any) {
      console.error('Failed to fetch portfolio history', err);
    }
  }

  async function fetchAccounts() {
    try {
      const response = await apiClient.get<Account[]>('/accounts');
      accounts.value = response.data;
    } catch (err: any) {
      console.error('Failed to fetch accounts', err);
    }
  }

  async function createAccount(data: { name: string; institution: string; accountType: string; baseCurrency: string }) {
    const response = await apiClient.post<Account>('/accounts', data);
    accounts.value.push(response.data);
    return response.data;
  }

  async function fetchAssets() {
    try {
      const response = await apiClient.get<Asset[]>('/assets');
      assets.value = response.data;
    } catch (err: any) {
      console.error('Failed to fetch assets', err);
    }
  }

  async function createAsset(data: any) {
    const response = await apiClient.post<Asset>('/assets', data);
    assets.value.push(response.data);
    return response.data;
  }

  async function createInvestment(data: { accountId: string; assetId: string; customName?: string; interestRate?: number; maturityDate?: string }) {
    const response = await apiClient.post<Investment>('/investments', data);
    await fetchPortfolioSummary();
    return response.data;
  }

  async function updateInvestment(id: string, data: { accountId: string; assetId: string; customName?: string; interestRate?: number; maturityDate?: string }) {
    const response = await apiClient.put<Investment>(`/investments/${id}`, data);
    await fetchPortfolioSummary();
    return response.data;
  }

  async function deleteInvestment(id: string) {
    await apiClient.delete(`/investments/${id}`);
    await fetchPortfolioSummary();
  }

  async function fetchTransactions(investmentId?: string): Promise<Transaction[]> {
    const params = investmentId ? { investmentId } : {};
    const response = await apiClient.get<Transaction[]>('/transactions', { params });
    return response.data;
  }

  async function createTransaction(data: any) {
    const response = await apiClient.post('/transactions', data);
    await fetchPortfolioSummary();
    return response.data;
  }

  async function updateTransaction(id: string, data: any) {
    const response = await apiClient.put(`/transactions/${id}`, data);
    await fetchPortfolioSummary();
    return response.data;
  }

  async function deleteTransaction(id: string) {
    await apiClient.delete(`/transactions/${id}`);
    await fetchPortfolioSummary();
  }

  async function updateAccount(id: string, data: any) {
    const response = await apiClient.put<Account>(`/accounts/${id}`, data);
    await fetchAccounts();
    return response.data;
  }

  async function deleteAccount(id: string) {
    await apiClient.delete(`/accounts/${id}`);
    await fetchAccounts();
  }

  async function updateAsset(id: string, data: any) {
    const response = await apiClient.put<Asset>(`/assets/${id}`, data);
    await fetchAssets();
    return response.data;
  }

  async function deleteAsset(id: string) {
    await apiClient.delete(`/assets/${id}`);
    await fetchAssets();
  }

  async function syncMarketData() {
    isSyncingPrices.value = true;
    try {
      await apiClient.post('/market-data/sync');
      await fetchPortfolioSummary();
    } finally {
      isSyncingPrices.value = false;
    }
  }

  return {
    summary,
    snapshots,
    accounts,
    assets,
    investments,
    isLoading,
    isSyncingPrices,
    error,
    fetchPortfolioSummary,
    fetchPortfolioHistory,
    fetchAccounts,
    createAccount,
    updateAccount,
    deleteAccount,
    fetchAssets,
    createAsset,
    updateAsset,
    deleteAsset,
    createInvestment,
    updateInvestment,
    deleteInvestment,
    fetchTransactions,
    createTransaction,
    updateTransaction,
    deleteTransaction,
    syncMarketData,
  };
});

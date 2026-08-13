export interface User {
  id: string;
  email: string;
  name: string;
  baseCurrency: string;
  token?: string;
}

export interface Account {
  id: string;
  name: string;
  institution: string;
  accountType: 'Personal' | 'Joint' | 'Retirement_FGTS' | 'Brokerage';
  baseCurrency: 'BRL' | 'USD';
  createdAt: string;
}

export interface Asset {
  id: string;
  name: string;
  ticker?: string;
  assetCategory: string;
  valuationType: string;
  currency: 'BRL' | 'USD';
  indexBenchmark?: string;
  logoUrl?: string;
}

export interface Investment {
  id: string;
  accountId: string;
  assetId: string;
  name: string;
  ticker?: string;
  customName?: string;
  assetCategory: string;
  valuationType: string;
  currency: 'BRL' | 'USD';
  indexBenchmark?: string;
  interestRate?: number;
  maturityDate?: string;
  logoUrl?: string;
  createdAt: string;
}

export interface PositionSummary {
  investmentId: string;
  name: string;
  ticker?: string;
  assetCategory: string;
  valuationType: string;
  quantity: number;
  averagePrice: number;
  totalCost: number;
  currentUnitPrice: number;
  currentTotalValue: number;
  unrealizedGainLoss: number;
  unrealizedGainLossPercentage: number;
  currency: string;
  logoUrl?: string;
}

export interface PortfolioSummary {
  totalNetWorthBrl: number;
  totalNetWorthUsd: number;
  totalInvestedBrl: number;
  totalInvestedUsd: number;
  netGainLossBrl: number;
  netGainLossUsd: number;
  usdBrlFxRate: number;
  positions: PositionSummary[];
}

export interface PortfolioSnapshot {
  date: string;
  totalValueBrl: number;
  totalValueUsd: number;
  totalInvestedBrl: number;
  totalInvestedUsd: number;
  netGainLossBrl: number;
  netGainLossUsd: number;
}

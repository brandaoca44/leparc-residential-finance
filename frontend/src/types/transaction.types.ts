/**
 * Valores aceitos e retornados pelo backend para o tipo
 * de uma transação financeira.
 */
export const TransactionType = {
  Expense: "Expense",
  Income: "Income",
} as const;

export type TransactionType =
  (typeof TransactionType)[keyof typeof TransactionType];

/**
 * Representa uma transação financeira retornada pela API.
 */
export interface Transaction {
  id: number;
  description: string;
  amount: number;
  type: TransactionType;
  personId: number;
  personName: string;
}

/**
 * Dados necessários para cadastrar uma transação.
 */
export interface CreateTransactionRequest {
  description: string;
  amount: number;
  type: TransactionType;
  personId: number;
}
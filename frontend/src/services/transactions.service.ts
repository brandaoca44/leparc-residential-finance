import { api } from "../api/axios";

import type {
  CreateTransactionRequest,
  Transaction,
} from "../types/transaction.types";

/**
 * Centraliza as operações HTTP relacionadas às transações.
 */
export const transactionService = {
  /**
   * Obtém todas as transações cadastradas.
   */
  async getAll(): Promise<Transaction[]> {
    const response = await api.get<Transaction[]>("/transactions");

    return response.data;
  },

  /**
   * Cadastra uma nova transação.
   */
  async create(
    request: CreateTransactionRequest,
  ): Promise<Transaction> {
    const response = await api.post<Transaction>(
      "/transactions",
      request,
    );

    return response.data;
  },
};
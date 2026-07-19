import { api } from "../api/axios";
import type { FinancialSummary } from "../types/report.types";

const REPORT_SUMMARY_ENDPOINT = "/reports/summary";

/**
 * Centraliza as operações relacionadas aos relatórios financeiros.
 */
export const reportService = {
  /**
   * Obtém o resumo financeiro geral do sistema.
   */
  async getSummary(): Promise<FinancialSummary> {
    const response = await api.get<FinancialSummary>(
      REPORT_SUMMARY_ENDPOINT,
    );

    return response.data;
  },
};
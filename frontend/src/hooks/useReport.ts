import { useQuery } from "@tanstack/react-query";

import { reportService } from "../services/report.service";

const REPORT_SUMMARY_QUERY_KEY = ["reports", "summary"] as const;

/**
 * Consulta os indicadores financeiros consolidados do sistema.
 */
export function useReport() {
  return useQuery({
    queryKey: REPORT_SUMMARY_QUERY_KEY,
    queryFn: reportService.getSummary,
  });
}
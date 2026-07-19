import { useQuery } from "@tanstack/react-query";

import { QUERY_KEYS } from "../constants/queryKeys";
import { transactionService } from "../services/transactions.service";

/**
 * Consulta todas as transações cadastradas.
 */
export function useTransactions() {
    return useQuery({
        queryKey: QUERY_KEYS.transactions,
        queryFn: transactionService.getAll
    });
}
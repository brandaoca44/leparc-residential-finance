import { useMutation, useQueryClient } from "@tanstack/react-query";

import { QUERY_KEYS } from "../constants/queryKeys";
import { transactionService } from "../services/transactions.service";

/**
 * Responsável pelo cadastro de transações financeiras.
 */
export function useCreateTransaction() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: transactionService.create,

        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.transactions
            });
        }
    });
}
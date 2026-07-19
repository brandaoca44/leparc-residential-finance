import { useMutation, useQueryClient } from "@tanstack/react-query";

import { QUERY_KEYS } from "../constants/queryKeys";
import { peopleService } from "../services/people.service";

/**
 * Responsável pelo cadastro de novas pessoas.
 */
export function useCreatePerson() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: peopleService.create,

        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.people
            });
        }
    });
}
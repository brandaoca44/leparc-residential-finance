import { useMutation, useQueryClient } from "@tanstack/react-query";

import { QUERY_KEYS } from "../constants/queryKeys";
import { peopleService } from "../services/people.service";

/**
 * Remove uma pessoa cadastrada.
 */
export function useDeletePerson() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: peopleService.remove,

    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: QUERY_KEYS.people,
        }),
        queryClient.invalidateQueries({
          queryKey: QUERY_KEYS.transactions,
        }),
      ]);
    },
  });
}
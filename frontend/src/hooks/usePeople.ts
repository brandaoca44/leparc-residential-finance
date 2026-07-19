import { useQuery } from "@tanstack/react-query";

import { QUERY_KEYS } from "../constants/queryKeys";
import { peopleService } from "../services/people.service";

/**
 * Consulta todas as pessoas cadastradas.
 */
export function usePeople() {
    return useQuery({
        queryKey: QUERY_KEYS.people,
        queryFn: peopleService.getAll
    });
}
import { QueryClient } from "@tanstack/react-query";

/**
 * Instância global do TanStack Query.
 *
 * Responsável por controlar cache, invalidação de consultas
 * e sincronização automática dos dados consumidos pela aplicação.
 */
export const queryClient = new QueryClient({
    defaultOptions: {
        queries: {
            retry: 1,
            refetchOnWindowFocus: false,
            staleTime: 1000 * 60 * 5
        },
        mutations: {
            retry: 0
        }
    }
});
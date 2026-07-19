/**
 * Centraliza as chaves utilizadas pelo TanStack Query.
 *
 * Evita duplicação de strings e reduz erros de digitação
 * durante invalidações e consultas.
 */
export const QUERY_KEYS = {
    people: ["people"],
    transactions: ["transactions"]
} as const;
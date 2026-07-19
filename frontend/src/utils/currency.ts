/**
 * Formata valores monetários utilizando o padrão brasileiro.
 *
 * @param value Valor a ser formatado.
 * @returns Valor formatado em Real (R$).
 */
export function formatCurrency(value: number): string {
    return new Intl.NumberFormat("pt-BR", {
        style: "currency",
        currency: "BRL"
    }).format(value);
}
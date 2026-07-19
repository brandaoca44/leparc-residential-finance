import type { ApiError } from "../types/api.types";

/**
 * Verifica se o valor recebido segue o contrato de erro da aplicação.
 */
export function isApiError(error: unknown): error is ApiError {
  return (
    typeof error === "object" &&
    error !== null &&
    "message" in error &&
    typeof error.message === "string"
  );
}

/**
 * Obtém uma mensagem segura para apresentação ao usuário.
 */
export function getErrorMessage(error: unknown): string {
  if (isApiError(error)) {
    return error.message;
  }

  if (error instanceof Error) {
    return error.message;
  }

  return "Ocorreu um erro inesperado.";
}

/**
 * Retorna os erros de validação enviados pelo backend.
 */
export function getValidationErrors(
  error: unknown,
): Record<string, string[]> {
  if (!isApiError(error) || !error.errors) {
    return {};
  }

  return error.errors;
}
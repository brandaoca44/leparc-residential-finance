/**
 * Representa o formato padrão de erro utilizado pela aplicação.
 */
export interface ApiError {
  message: string;
  status?: number;
  errors?: Record<string, string[]>;
}

/**
 * Estrutura compatível com respostas de validação do ASP.NET Core.
 */
export interface ProblemDetailsResponse {
  title?: string;
  detail?: string;
  status?: number;
  errors?: Record<string, string[]>;
}
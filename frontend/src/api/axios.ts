import axios, {
  AxiosError,
  type InternalAxiosRequestConfig,
} from "axios";

import type {
  ApiError,
  ProblemDetailsResponse,
} from "../types/api.types";

const apiUrl = import.meta.env.VITE_API_URL;

if (!apiUrl) {
  throw new Error(
    "A variável de ambiente VITE_API_URL não foi configurada.",
  );
}

/**
 * Cliente HTTP central da aplicação.
 */
export const api = axios.create({
  baseURL: apiUrl,
  timeout: 10000,
  headers: {
    Accept: "application/json",
    "Content-Type": "application/json",
  },
});

/**
 * Mantém somente configurações seguras antes do envio da requisição.
 */
api.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    config.headers.Accept = "application/json";

    return config;
  },
);

/**
 * Converte respostas de erro da API em um contrato previsível.
 */
api.interceptors.response.use(
  (response) => response,
  (error: AxiosError<ProblemDetailsResponse>) => {
    const responseData = error.response?.data;

    const apiError: ApiError = {
      message:
        responseData?.detail ??
        responseData?.title ??
        getFallbackMessage(error),
      status: error.response?.status,
      errors: responseData?.errors,
    };

    return Promise.reject(apiError);
  },
);

function getFallbackMessage(
  error: AxiosError<ProblemDetailsResponse>,
): string {
  if (error.code === "ECONNABORTED") {
    return "O servidor demorou mais que o esperado para responder.";
  }

  if (!error.response) {
    return "Não foi possível estabelecer comunicação com o servidor.";
  }

  if (error.response.status >= 500) {
    return "O servidor encontrou um erro inesperado.";
  }

  return "Não foi possível concluir a operação.";
}
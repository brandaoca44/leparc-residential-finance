/**
 * Representa uma pessoa cadastrada no sistema.
 */
export interface Person {
  id: number;
  name: string;
  age: number;
}

/**
 * Dados necessários para cadastrar uma pessoa.
 */
export interface CreatePersonRequest {
  name: string;
  age: number;
}
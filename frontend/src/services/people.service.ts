import { api } from "../api/axios";

import type {
  CreatePersonRequest,
  Person,
} from "../types/people.types";

/**
 * Centraliza as operações HTTP relacionadas às pessoas.
 */
export const peopleService = {
  /**
   * Obtém todas as pessoas cadastradas.
   */
  async getAll(): Promise<Person[]> {
    const response = await api.get<Person[]>("/people");

    return response.data;
  },

  /**
   * Cadastra uma nova pessoa.
   */
  async create(request: CreatePersonRequest): Promise<Person> {
    const response = await api.post<Person>("/people", request);

    return response.data;
  },

  /**
   * Remove uma pessoa pelo identificador.
   */
  async remove(id: number): Promise<void> {
    await api.delete(`/people/${id}`);
  },
};
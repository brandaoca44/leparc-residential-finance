import {
  type ChangeEvent,
  type FormEvent,
  useState,
} from "react";
import { useNavigate } from "react-router-dom";
import toast from "react-hot-toast";

import { useCreatePerson } from "../../hooks/useCreatePerson";
import type { CreatePersonRequest } from "../../types/people.types";
import { getErrorMessage } from "../../utils/error";

import styles from "./PersonCreatePage.module.css";

interface PersonFormState {
  name: string;
  age: string;
}

interface PersonFormErrors {
  name?: string;
  age?: string;
}

const INITIAL_FORM_STATE: PersonFormState = {
  name: "",
  age: "",
};

/**
 * Renderiza e controla o formulário de cadastro de pessoas.
 */
export function PersonCreatePage() {
  const navigate = useNavigate();
  const createPerson = useCreatePerson();

  const [form, setForm] =
    useState<PersonFormState>(INITIAL_FORM_STATE);

  const [errors, setErrors] =
    useState<PersonFormErrors>({});

  function handleNameChange(
    event: ChangeEvent<HTMLInputElement>,
  ) {
    setForm((currentForm) => ({
      ...currentForm,
      name: event.target.value,
    }));

    if (errors.name) {
      setErrors((currentErrors) => ({
        ...currentErrors,
        name: undefined,
      }));
    }
  }

  function handleAgeChange(
    event: ChangeEvent<HTMLInputElement>,
  ) {
    const value = event.target.value;

    /*
     * Aceita somente dígitos no estado do formulário.
     * A validação definitiva também é realizada antes do envio.
     */
    if (value !== "" && !/^\d+$/.test(value)) {
      return;
    }

    setForm((currentForm) => ({
      ...currentForm,
      age: value,
    }));

    if (errors.age) {
      setErrors((currentErrors) => ({
        ...currentErrors,
        age: undefined,
      }));
    }
  }

  function validateForm(): PersonFormErrors {
    const validationErrors: PersonFormErrors = {};
    const normalizedName = form.name.trim();
    const parsedAge = Number(form.age);

    if (!normalizedName) {
      validationErrors.name = "Informe o nome da pessoa.";
    }

    if (!form.age) {
      validationErrors.age = "Informe a idade.";
    } else if (
      !Number.isInteger(parsedAge) ||
      parsedAge < 0
    ) {
      validationErrors.age =
        "A idade deve ser um número inteiro não negativo.";
    }

    return validationErrors;
  }

  async function handleSubmit(
    event: FormEvent<HTMLFormElement>,
  ) {
    event.preventDefault();

    if (createPerson.isPending) {
      return;
    }

    const validationErrors = validateForm();

    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    const request: CreatePersonRequest = {
      name: form.name.trim(),
      age: Number(form.age),
    };

    try {
      await createPerson.mutateAsync(request);

      toast.success("Pessoa cadastrada com sucesso.");
      navigate("/people");
    } catch (error: unknown) {
      toast.error(getErrorMessage(error));
    }
  }

  function handleCancel() {
    navigate("/people");
  }

  return (
    <main className={styles.page}>
      <header className={styles.header}>
        <div>
          <span className={styles.eyebrow}>
            Gerenciamento de pessoas
          </span>

          <h1 className={styles.title}>Nova pessoa</h1>

          <p className={styles.description}>
            Cadastre uma pessoa para vinculá-la às
            transações residenciais.
          </p>
        </div>
      </header>

      <section
        className={styles.card}
        aria-labelledby="person-form-title"
      >
        <div className={styles.cardHeader}>
          <h2
            id="person-form-title"
            className={styles.cardTitle}
          >
            Dados pessoais
          </h2>

          <p className={styles.cardDescription}>
            Os campos marcados com asterisco são
            obrigatórios.
          </p>
        </div>

        <form
          className={styles.form}
          onSubmit={handleSubmit}
          noValidate
        >
          <div className={styles.field}>
            <label
              className={styles.label}
              htmlFor="person-name"
            >
              Nome
              <span
                className={styles.required}
                aria-hidden="true"
              >
                *
              </span>
            </label>

            <input
              id="person-name"
              name="name"
              type="text"
              value={form.name}
              onChange={handleNameChange}
              placeholder="Digite o nome completo"
              autoComplete="name"
              disabled={createPerson.isPending}
              aria-invalid={Boolean(errors.name)}
              aria-describedby={
                errors.name
                  ? "person-name-error"
                  : undefined
              }
              className={`${styles.input} ${
                errors.name ? styles.invalid : ""
              }`}
            />

            {errors.name && (
              <span
                id="person-name-error"
                className={styles.error}
                role="alert"
              >
                {errors.name}
              </span>
            )}
          </div>

          <div className={styles.field}>
            <label
              className={styles.label}
              htmlFor="person-age"
            >
              Idade
              <span
                className={styles.required}
                aria-hidden="true"
              >
                *
              </span>
            </label>

            <input
              id="person-age"
              name="age"
              type="text"
              inputMode="numeric"
              pattern="[0-9]*"
              value={form.age}
              onChange={handleAgeChange}
              placeholder="Digite a idade"
              autoComplete="off"
              disabled={createPerson.isPending}
              aria-invalid={Boolean(errors.age)}
              aria-describedby={
                errors.age
                  ? "person-age-error"
                  : "person-age-helper"
              }
              className={`${styles.input} ${
                errors.age ? styles.invalid : ""
              }`}
            />

            {errors.age ? (
              <span
                id="person-age-error"
                className={styles.error}
                role="alert"
              >
                {errors.age}
              </span>
            ) : (
              <span
                id="person-age-helper"
                className={styles.helper}
              >
                Pessoas menores de 18 anos poderão ter
                apenas despesas cadastradas.
              </span>
            )}
          </div>

          <div className={styles.actions}>
            <button
              type="button"
              className={styles.secondaryButton}
              onClick={handleCancel}
              disabled={createPerson.isPending}
            >
              Cancelar
            </button>

            <button
              type="submit"
              className={styles.primaryButton}
              disabled={createPerson.isPending}
              aria-busy={createPerson.isPending}
            >
              {createPerson.isPending
                ? "Salvando..."
                : "Cadastrar pessoa"}
            </button>
          </div>
        </form>
      </section>
    </main>
  );
}
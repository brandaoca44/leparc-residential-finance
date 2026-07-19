import {
  type ChangeEvent,
  type FormEvent,
  useMemo,
  useState,
} from "react";
import { useNavigate } from "react-router-dom";
import toast from "react-hot-toast";

import { useCreateTransaction } from "../../hooks/useCreateTransaction";
import { usePeople } from "../../hooks/usePeople";
import {
  TransactionType,
  type CreateTransactionRequest,
  type TransactionType as TransactionTypeValue,
} from "../../types/transaction.types";
import { getErrorMessage } from "../../utils/error";

import styles from "./TransactionCreatePage.module.css";

interface TransactionFormState {
  description: string;
  amountDigits: string;
  type: TransactionTypeValue | "";
  personId: string;
}

interface TransactionFormErrors {
  description?: string;
  amount?: string;
  type?: string;
  personId?: string;
}

const INITIAL_FORM_STATE: TransactionFormState = {
  description: "",
  amountDigits: "",
  type: "",
  personId: "",
};

/**
 * Renderiza e controla o cadastro de transações financeiras.
 */
export function TransactionCreatePage() {
  const navigate = useNavigate();
  const peopleQuery = usePeople();
  const createTransaction = useCreateTransaction();

  const [form, setForm] =
    useState<TransactionFormState>(INITIAL_FORM_STATE);

  const [errors, setErrors] =
    useState<TransactionFormErrors>({});

  const orderedPeople = useMemo(() => {
    return [...(peopleQuery.data ?? [])].sort((first, second) =>
      first.name.localeCompare(second.name, "pt-BR"),
    );
  }, [peopleQuery.data]);

  const selectedPerson = useMemo(() => {
    const personId = Number(form.personId);

    if (!Number.isInteger(personId)) {
      return undefined;
    }

    return orderedPeople.find((person) => person.id === personId);
  }, [form.personId, orderedPeople]);

  const isMinor = selectedPerson
    ? selectedPerson.age < 18
    : false;

  const formattedAmount = formatAmountInput(form.amountDigits);

  function handleDescriptionChange(
    event: ChangeEvent<HTMLInputElement>,
  ) {
    setForm((currentForm) => ({
      ...currentForm,
      description: event.target.value,
    }));

    clearFieldError("description");
  }

  function handleAmountChange(
    event: ChangeEvent<HTMLInputElement>,
  ) {
    const digits = event.target.value.replace(/\D/g, "");

    setForm((currentForm) => ({
      ...currentForm,
      amountDigits: digits,
    }));

    clearFieldError("amount");
  }

  function handlePersonChange(
    event: ChangeEvent<HTMLSelectElement>,
  ) {
    const personId = event.target.value;
    const person = orderedPeople.find(
      (item) => item.id === Number(personId),
    );

    setForm((currentForm) => ({
      ...currentForm,
      personId,
      /*
       * Ao selecionar um menor de idade, a transação é
       * automaticamente ajustada para despesa.
       */
      type:
        person?.age !== undefined && person.age < 18
          ? TransactionType.Expense
          : currentForm.type,
    }));

    setErrors((currentErrors) => ({
      ...currentErrors,
      personId: undefined,
      type: undefined,
    }));
  }

    function handleTypeChange(
        event: ChangeEvent<HTMLInputElement>,
    ) {
        const value = event.target.value;

        if (!isTransactionType(value)) {
            return;
        }

        if (isMinor && value === TransactionType.Income) {
            return;
        }

        setForm((currentForm) => ({
            ...currentForm,
            type: value,
        }));

        clearFieldError("type");
    }

  function clearFieldError(
    field: keyof TransactionFormErrors,
  ) {
    setErrors((currentErrors) => {
      if (!currentErrors[field]) {
        return currentErrors;
      }

      return {
        ...currentErrors,
        [field]: undefined,
      };
    });
  }

  function validateForm(): TransactionFormErrors {
    const validationErrors: TransactionFormErrors = {};
    const description = form.description.trim();
    const amount = parseAmount(form.amountDigits);
    const personId = Number(form.personId);

    if (!description) {
      validationErrors.description =
        "Informe a descrição da transação.";
    }

    if (!form.amountDigits || amount <= 0) {
      validationErrors.amount =
        "Informe um valor maior que zero.";
    }

    if (!Number.isFinite(amount)) {
      validationErrors.amount =
        "Informe um valor monetário válido.";
    }

    if (!Number.isInteger(personId)) {
      validationErrors.personId = "Selecione uma pessoa.";
    } else if (
      !orderedPeople.some((person) => person.id === personId)
    ) {
      validationErrors.personId =
        "A pessoa selecionada não existe no cadastro.";
    }

    if (form.type === "") {
      validationErrors.type =
        "Selecione o tipo da transação.";
    }

    /*
     * A validação é repetida antes do envio para que a regra
     * não dependa somente do campo visual desabilitado.
     */
    if (
      selectedPerson &&
      selectedPerson.age < 18 &&
      form.type === TransactionType.Income
    ) {
      validationErrors.type =
        "Pessoas menores de 18 anos podem ter apenas despesas.";
    }

    return validationErrors;
  }

  async function handleSubmit(
    event: FormEvent<HTMLFormElement>,
  ) {
    event.preventDefault();

    if (createTransaction.isPending) {
      return;
    }

    const validationErrors = validateForm();

    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    if (
      form.type === "" ||
      !selectedPerson
    ) {
      return;
    }

    const request: CreateTransactionRequest = {
      description: form.description.trim(),
      amount: parseAmount(form.amountDigits),
      type: form.type,
      personId: selectedPerson.id,
    };

    try {
      await createTransaction.mutateAsync(request);

      toast.success("Transação cadastrada com sucesso.");
      navigate("/transactions");
    } catch (error: unknown) {
      toast.error(getErrorMessage(error));
    }
  }

  return (
    <main className={styles.page}>
      <header className={styles.pageHeader}>
        <span className={styles.eyebrow}>
          Controle financeiro
        </span>

        <h1 className={styles.title}>Nova transação</h1>

        <p className={styles.description}>
          Registre uma receita ou despesa vinculada a uma
          pessoa cadastrada.
        </p>
      </header>

      <section
        className={styles.card}
        aria-labelledby="transaction-form-title"
      >
        <div className={styles.cardHeader}>
          <h2
            id="transaction-form-title"
            className={styles.cardTitle}
          >
            Dados da transação
          </h2>

          <p className={styles.cardDescription}>
            Todos os campos são obrigatórios.
          </p>
        </div>

        {peopleQuery.isLoading ? (
          <div
            className={styles.feedback}
            aria-live="polite"
            aria-busy="true"
          >
            Carregando pessoas cadastradas...
          </div>
        ) : peopleQuery.isError ? (
          <div className={styles.feedback} role="alert">
            <strong>
              Não foi possível carregar as pessoas.
            </strong>

            <span>
              {getErrorMessage(peopleQuery.error)}
            </span>

            <button
              type="button"
              className={styles.retryButton}
              onClick={() => peopleQuery.refetch()}
              disabled={peopleQuery.isFetching}
            >
              {peopleQuery.isFetching
                ? "Tentando novamente..."
                : "Tentar novamente"}
            </button>
          </div>
        ) : orderedPeople.length === 0 ? (
          <div className={styles.feedback}>
            <strong>
              Nenhuma pessoa cadastrada.
            </strong>

            <span>
              Cadastre uma pessoa antes de registrar uma
              transação.
            </span>

            <button
              type="button"
              className={styles.primaryButton}
              onClick={() => navigate("/people/new")}
            >
              Cadastrar pessoa
            </button>
          </div>
        ) : (
          <form
            className={styles.form}
            onSubmit={handleSubmit}
            noValidate
          >
            <div className={styles.field}>
              <label
                className={styles.label}
                htmlFor="transaction-person"
              >
                Pessoa
                <RequiredMark />
              </label>

              <select
                id="transaction-person"
                name="personId"
                value={form.personId}
                onChange={handlePersonChange}
                disabled={createTransaction.isPending}
                className={`${styles.control} ${
                  errors.personId ? styles.invalid : ""
                }`}
                aria-invalid={Boolean(errors.personId)}
                aria-describedby={
                  errors.personId
                    ? "transaction-person-error"
                    : undefined
                }
              >
                <option value="">
                  Selecione uma pessoa
                </option>

                {orderedPeople.map((person) => (
                  <option
                    key={person.id}
                    value={person.id}
                  >
                    {person.name} — {person.age}{" "}
                    {person.age === 1 ? "ano" : "anos"}
                  </option>
                ))}
              </select>

              {errors.personId && (
                <FieldError
                  id="transaction-person-error"
                  message={errors.personId}
                />
              )}
            </div>

            <div className={styles.field}>
              <label
                className={styles.label}
                htmlFor="transaction-description"
              >
                Descrição
                <RequiredMark />
              </label>

              <input
                id="transaction-description"
                name="description"
                type="text"
                value={form.description}
                onChange={handleDescriptionChange}
                placeholder="Ex.: Conta de energia"
                disabled={createTransaction.isPending}
                className={`${styles.control} ${
                  errors.description ? styles.invalid : ""
                }`}
                aria-invalid={Boolean(errors.description)}
                aria-describedby={
                  errors.description
                    ? "transaction-description-error"
                    : undefined
                }
              />

              {errors.description && (
                <FieldError
                  id="transaction-description-error"
                  message={errors.description}
                />
              )}
            </div>

            <div className={styles.field}>
              <label
                className={styles.label}
                htmlFor="transaction-amount"
              >
                Valor
                <RequiredMark />
              </label>

              <div className={styles.amountWrapper}>
                <span
                  className={styles.currencyPrefix}
                  aria-hidden="true"
                >
                  R$
                </span>

                <input
                  id="transaction-amount"
                  name="amount"
                  type="text"
                  inputMode="numeric"
                  value={formattedAmount}
                  onChange={handleAmountChange}
                  placeholder="0,00"
                  disabled={createTransaction.isPending}
                  className={`${styles.amountInput} ${
                    errors.amount ? styles.invalid : ""
                  }`}
                  aria-invalid={Boolean(errors.amount)}
                  aria-describedby={
                    errors.amount
                      ? "transaction-amount-error"
                      : "transaction-amount-helper"
                  }
                />
              </div>

              {errors.amount ? (
                <FieldError
                  id="transaction-amount-error"
                  message={errors.amount}
                />
              ) : (
                <span
                  id="transaction-amount-helper"
                  className={styles.helper}
                >
                  O valor deve ser maior que zero.
                </span>
              )}
            </div>

            <fieldset className={styles.typeFieldset}>
              <legend className={styles.label}>
                Tipo
                <RequiredMark />
              </legend>

              <div className={styles.typeOptions}>
                <label
                  className={`${styles.typeOption} ${
                    form.type === TransactionType.Expense
                      ? styles.typeOptionSelected
                      : ""
                  }`}
                >
                  <input
                    type="radio"
                    name="type"
                    value={TransactionType.Expense}
                    checked={
                      form.type === TransactionType.Expense
                    }
                    onChange={handleTypeChange}
                    disabled={createTransaction.isPending}
                  />

                  <span>
                    <strong>Despesa</strong>
                    <small>Valor de saída</small>
                  </span>
                </label>

                <label
                  className={`${styles.typeOption} ${
                    form.type === TransactionType.Income
                      ? styles.typeOptionSelected
                      : ""
                  } ${
                    isMinor
                      ? styles.typeOptionDisabled
                      : ""
                  }`}
                >
                  <input
                    type="radio"
                    name="type"
                    value={TransactionType.Income}
                    checked={
                      form.type === TransactionType.Income
                    }
                    onChange={handleTypeChange}
                    disabled={
                      createTransaction.isPending || isMinor
                    }
                  />

                  <span>
                    <strong>Receita</strong>
                    <small>Valor de entrada</small>
                  </span>
                </label>
              </div>

              {isMinor && (
                <p
                  className={styles.businessRule}
                  role="status"
                >
                  {selectedPerson?.name} é menor de idade.
                  Apenas despesas podem ser cadastradas.
                </p>
              )}

              {errors.type && (
                <FieldError
                  id="transaction-type-error"
                  message={errors.type}
                />
              )}
            </fieldset>

            <div className={styles.actions}>
              <button
                type="button"
                className={styles.secondaryButton}
                onClick={() => navigate("/transactions")}
                disabled={createTransaction.isPending}
              >
                Cancelar
              </button>

              <button
                type="submit"
                className={styles.primaryButton}
                disabled={createTransaction.isPending}
                aria-busy={createTransaction.isPending}
              >
                {createTransaction.isPending
                  ? "Salvando..."
                  : "Cadastrar transação"}
              </button>
            </div>
          </form>
        )}
      </section>
    </main>
  );
}

function RequiredMark() {
  return (
    <span className={styles.required} aria-hidden="true">
      *
    </span>
  );
}

interface FieldErrorProps {
  id: string;
  message: string;
}

function FieldError({
  id,
  message,
}: FieldErrorProps) {
  return (
    <span
      id={id}
      className={styles.error}
      role="alert"
    >
      {message}
    </span>
  );
}

function parseAmount(digits: string): number {
  if (!digits) {
    return 0;
  }

  return Number(digits) / 100;
}

function formatAmountInput(digits: string): string {
  if (!digits) {
    return "";
  }

  const amount = parseAmount(digits);

  return new Intl.NumberFormat("pt-BR", {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(amount);
}

function isTransactionType(
  value: string,
): value is TransactionTypeValue {
  return (
    value === TransactionType.Expense ||
    value === TransactionType.Income
  );
}
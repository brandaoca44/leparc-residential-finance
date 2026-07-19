import { useMemo } from "react";
import { Link } from "react-router-dom";

import { useTransactions } from "../../hooks/useTransactions";
import {
  TransactionType,
  type Transaction,
} from "../../types/transaction.types";
import { formatCurrency } from "../../utils/currency";
import { getErrorMessage } from "../../utils/error";

import styles from "./TransactionsPage.module.css";

/**
 * Renderiza a listagem das transações cadastradas.
 */
export function TransactionsPage() {
  const transactionsQuery = useTransactions();

  const orderedTransactions = useMemo(() => {
    return [...(transactionsQuery.data ?? [])].sort(
      (firstTransaction, secondTransaction) =>
        secondTransaction.id - firstTransaction.id,
    );
  }, [transactionsQuery.data]);

  if (transactionsQuery.isLoading) {
    return (
      <main className={styles.page}>
        <PageHeader />

        <section
          className={styles.grid}
          aria-label="Carregando transações"
          aria-busy="true"
        >
          {Array.from({ length: 4 }, (_, index) => (
            <TransactionSkeleton key={index} />
          ))}
        </section>
      </main>
    );
  }

  if (transactionsQuery.isError) {
    return (
      <main className={styles.page}>
        <PageHeader />

        <section className={styles.feedbackCard} role="alert">
          <h2 className={styles.feedbackTitle}>
            Não foi possível carregar as transações
          </h2>

          <p className={styles.feedbackText}>
            {getErrorMessage(transactionsQuery.error)}
          </p>

          <button
            type="button"
            className={styles.retryButton}
            onClick={() => transactionsQuery.refetch()}
            disabled={transactionsQuery.isFetching}
          >
            {transactionsQuery.isFetching
              ? "Tentando novamente..."
              : "Tentar novamente"}
          </button>
        </section>
      </main>
    );
  }

  return (
    <main className={styles.page}>
      <PageHeader />

      {orderedTransactions.length === 0 ? (
        <EmptyState />
      ) : (
        <section
          className={styles.grid}
          aria-label="Transações cadastradas"
        >
          {orderedTransactions.map((transaction) => (
            <TransactionCard
              key={transaction.id}
              transaction={transaction}
            />
          ))}
        </section>
      )}
    </main>
  );
}

function PageHeader() {
  return (
    <header className={styles.pageHeader}>
      <div className={styles.headerContent}>
        <span className={styles.eyebrow}>
          Controle financeiro
        </span>

        <h1 className={styles.title}>Transações</h1>

        <p className={styles.description}>
          Consulte todas as receitas e despesas registradas
          para as pessoas da residência.
        </p>
      </div>

      <Link
        to="/transactions/new"
        className={styles.createButton}
      >
        Nova transação
      </Link>
    </header>
  );
}

interface TransactionCardProps {
  transaction: Transaction;
}

function TransactionCard({
  transaction,
}: TransactionCardProps) {
  const isIncome =
    transaction.type === TransactionType.Income;

  const typeLabel = isIncome ? "Receita" : "Despesa";

  return (
    <article className={styles.card}>
      <div className={styles.cardHeader}>
        <div className={styles.transactionIdentity}>
          <span
            className={`${styles.typeIndicator} ${
              isIncome
                ? styles.incomeIndicator
                : styles.expenseIndicator
            }`}
            aria-hidden="true"
          >
            {isIncome ? "+" : "−"}
          </span>

          <div className={styles.transactionInfo}>
            <h2 className={styles.transactionDescription}>
              {transaction.description}
            </h2>

            <span className={styles.transactionId}>
              Transação #{transaction.id}
            </span>
          </div>
        </div>

        <span
          className={`${styles.typeBadge} ${
            isIncome
              ? styles.incomeBadge
              : styles.expenseBadge
          }`}
        >
          {typeLabel}
        </span>
      </div>

      <div className={styles.cardBody}>
        <div className={styles.personSection}>
          <span className={styles.personAvatar} aria-hidden="true">
            {getInitials(transaction.personName)}
          </span>

          <div className={styles.personContent}>
            <span className={styles.detailLabel}>
              Pessoa responsável
            </span>

            <strong className={styles.personName}>
              {transaction.personName}
            </strong>
          </div>
        </div>

        <div className={styles.details}>
          <div className={styles.detailItem}>
            <span className={styles.detailLabel}>
              Identificador da pessoa
            </span>

            <strong className={styles.detailValue}>
              #{transaction.personId}
            </strong>
          </div>

          <div className={styles.detailItem}>
            <span className={styles.detailLabel}>
              Tipo da movimentação
            </span>

            <strong className={styles.detailValue}>
              {typeLabel}
            </strong>
          </div>
        </div>
      </div>

      <footer className={styles.cardFooter}>
        <span className={styles.amountLabel}>
          Valor da transação
        </span>

        <strong
          className={`${styles.amount} ${
            isIncome
              ? styles.incomeAmount
              : styles.expenseAmount
          }`}
        >
          {isIncome ? "+" : "−"}{" "}
          {formatCurrency(transaction.amount)}
        </strong>
      </footer>
    </article>
  );
}

function EmptyState() {
  return (
    <section className={styles.emptyState}>
      <div className={styles.emptyIcon} aria-hidden="true">
        R$
      </div>

      <h2 className={styles.feedbackTitle}>
        Nenhuma transação cadastrada
      </h2>

      <p className={styles.feedbackText}>
        Registre uma receita ou despesa para começar a
        acompanhar as movimentações da residência.
      </p>

      <Link
        to="/transactions/new"
        className={styles.emptyAction}
      >
        Cadastrar primeira transação
      </Link>
    </section>
  );
}

function TransactionSkeleton() {
  return (
    <div
      className={`${styles.card} ${styles.skeletonCard}`}
      aria-hidden="true"
    >
      <div className={styles.skeletonHeader}>
        <div className={styles.skeletonCircle} />

        <div className={styles.skeletonHeaderContent}>
          <div className={styles.skeletonTitle} />
          <div className={styles.skeletonLineShort} />
        </div>
      </div>

      <div className={styles.skeletonBody}>
        <div className={styles.skeletonLine} />
        <div className={styles.skeletonLine} />
      </div>

      <div className={styles.skeletonFooter}>
        <div className={styles.skeletonAmount} />
      </div>
    </div>
  );
}

function getInitials(name: string): string {
  const normalizedParts = name
    .trim()
    .split(/\s+/)
    .filter(Boolean);

  if (normalizedParts.length === 0) {
    return "--";
  }

  const firstInitial = normalizedParts[0][0];

  const lastInitial =
    normalizedParts.length > 1
      ? normalizedParts[normalizedParts.length - 1][0]
      : "";

  return `${firstInitial}${lastInitial}`.toUpperCase();
}
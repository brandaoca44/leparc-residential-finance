import { Link } from "react-router-dom";

import { useReport } from "../../hooks/useReport";
import { formatCurrency } from "../../utils/currency";

import styles from "./DashboardPage.module.css";

interface SummaryCardProps {
  title: string;
  value: string;
  description: string;
  variant: "people" | "transactions" | "income" | "expense";
  linkTo?: string;
}

interface BalanceCardProps {
  balance: number;
}

/**
 * Renderiza um indicador resumido do Dashboard.
 */
function SummaryCard({
  title,
  value,
  description,
  variant,
  linkTo,
}: SummaryCardProps) {
  const content = (
    <>
      <div className={styles.cardHeader}>
        <span
          className={`${styles.cardIndicator} ${styles[variant]}`}
          aria-hidden="true"
        />

        <span className={styles.cardTitle}>{title}</span>
      </div>

      <strong className={styles.cardValue}>{value}</strong>

      <span className={styles.cardDescription}>{description}</span>
    </>
  );

  if (linkTo) {
    return (
      <Link
        className={`${styles.summaryCard} ${styles.summaryCardLink}`}
        to={linkTo}
        aria-label={`Acessar ${title.toLowerCase()}`}
      >
        {content}
      </Link>
    );
  }

  return <article className={styles.summaryCard}>{content}</article>;
}

/**
 * Renderiza o saldo financeiro geral.
 */
function BalanceCard({ balance }: BalanceCardProps) {
  const isPositive = balance >= 0;

  const balanceClassName = isPositive
    ? styles.positiveBalance
    : styles.negativeBalance;

  return (
    <article className={styles.balanceCard}>
      <div className={styles.balanceContent}>
        <span className={styles.balanceLabel}>Saldo atual</span>

        <strong className={`${styles.balanceValue} ${balanceClassName}`}>
          {formatCurrency(balance)}
        </strong>

        <p className={styles.balanceDescription}>
          Resultado consolidado entre todas as receitas e despesas
          registradas.
        </p>
      </div>

      <div
        className={`${styles.balanceStatus} ${
          isPositive
            ? styles.positiveStatus
            : styles.negativeStatus
        }`}
      >
        {isPositive ? "Saldo positivo" : "Saldo negativo"}
      </div>
    </article>
  );
}

/**
 * Renderiza o estado de carregamento do Dashboard.
 */
function DashboardSkeleton() {
  return (
    <div
      className={styles.skeletonContainer}
      aria-label="Carregando indicadores financeiros"
      aria-busy="true"
    >
      <div className={styles.skeletonBalance} />

      <div className={styles.skeletonGrid}>
        {Array.from({ length: 4 }, (_, index) => (
          <div className={styles.skeletonCard} key={index}>
            <div className={styles.skeletonTitle} />
            <div className={styles.skeletonValue} />
            <div className={styles.skeletonDescription} />
          </div>
        ))}
      </div>
    </div>
  );
}

/**
 * Renderiza a página principal de indicadores financeiros.
 */
export function DashboardPage() {
  const {
    data: summary,
    isLoading,
    isError,
    refetch,
    isFetching,
  } = useReport();

  if (isLoading) {
    return (
      <main className={styles.page}>
        <header className={styles.pageHeader}>
          <div>
            <span className={styles.eyebrow}>
              Visão geral
            </span>

            <h1 className={styles.pageTitle}>
              Dashboard financeiro
            </h1>

            <p className={styles.pageDescription}>
              Acompanhe os principais indicadores financeiros do sistema.
            </p>
          </div>
        </header>

        <DashboardSkeleton />
      </main>
    );
  }

  if (isError || !summary) {
    return (
      <main className={styles.page}>
        <section className={styles.errorState}>
          <div
            className={styles.errorIcon}
            aria-hidden="true"
          >
            !
          </div>

          <h1 className={styles.errorTitle}>
            Não foi possível carregar o Dashboard
          </h1>

          <p className={styles.errorDescription}>
            Ocorreu um erro ao consultar os indicadores financeiros.
            Verifique a conexão com o servidor e tente novamente.
          </p>

          <button
            className={styles.retryButton}
            type="button"
            onClick={() => void refetch()}
            disabled={isFetching}
          >
            {isFetching ? "Tentando novamente..." : "Tentar novamente"}
          </button>
        </section>
      </main>
    );
  }

  return (
    <main className={styles.page}>
      <header className={styles.pageHeader}>
        <div>
          <span className={styles.eyebrow}>Visão geral</span>

          <h1 className={styles.pageTitle}>
            Dashboard financeiro
          </h1>

          <p className={styles.pageDescription}>
            Acompanhe os principais indicadores financeiros do sistema.
          </p>
        </div>

        <Link
          className={styles.primaryAction}
          to="/transactions/new"
        >
          Nova transação
        </Link>
      </header>

      <BalanceCard balance={summary.balance} />

      <section
        className={styles.summarySection}
        aria-labelledby="summary-title"
      >
        <div className={styles.sectionHeader}>
          <div>
            <h2
              className={styles.sectionTitle}
              id="summary-title"
            >
              Resumo financeiro
            </h2>

            <p className={styles.sectionDescription}>
              Indicadores consolidados com base nos dados cadastrados.
            </p>
          </div>
        </div>

        <div className={styles.summaryGrid}>
          <SummaryCard
            title="Pessoas"
            value={String(summary.totalPeople)}
            description={
              summary.totalPeople === 1
                ? "Pessoa cadastrada"
                : "Pessoas cadastradas"
            }
            variant="people"
            linkTo="/people"
          />

          <SummaryCard
            title="Transações"
            value={String(summary.totalTransactions)}
            description={
              summary.totalTransactions === 1
                ? "Transação registrada"
                : "Transações registradas"
            }
            variant="transactions"
            linkTo="/transactions"
          />

          <SummaryCard
            title="Receitas"
            value={formatCurrency(summary.totalIncome)}
            description="Total de entradas financeiras"
            variant="income"
          />

          <SummaryCard
            title="Despesas"
            value={formatCurrency(summary.totalExpense)}
            description="Total de saídas financeiras"
            variant="expense"
          />
        </div>
      </section>
    </main>
  );
}
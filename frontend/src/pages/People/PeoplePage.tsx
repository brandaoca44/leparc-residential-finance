import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import toast from "react-hot-toast";

import { useDeletePerson } from "../../hooks/useDeletePerson";
import { usePeople } from "../../hooks/usePeople";
import type { Person } from "../../types/people.types";
import { getErrorMessage } from "../../utils/error";

import styles from "./PeoplePage.module.css";

/**
 * Exibe as pessoas cadastradas e permite sua exclusão.
 */
export function PeoplePage() {
  const peopleQuery = usePeople();
  const deletePerson = useDeletePerson();

  const [deletingPersonId, setDeletingPersonId] =
    useState<number | null>(null);

  const orderedPeople = useMemo(() => {
    return [...(peopleQuery.data ?? [])].sort(
      (firstPerson, secondPerson) =>
        firstPerson.id - secondPerson.id,
    );
  }, [peopleQuery.data]);

  async function handleDelete(person: Person) {
    const confirmed = window.confirm(
      `Deseja excluir ${person.name}?\n\nAs transações vinculadas a essa pessoa também serão excluídas.`,
    );

    if (!confirmed || deletePerson.isPending) {
      return;
    }

    setDeletingPersonId(person.id);

    try {
      await deletePerson.mutateAsync(person.id);

      toast.success("Pessoa excluída com sucesso.");
    } catch (error: unknown) {
      toast.error(getErrorMessage(error));
    } finally {
      setDeletingPersonId(null);
    }
  }

  if (peopleQuery.isLoading) {
    return (
      <main className={styles.page}>
        <PageHeader />

        <section
          className={styles.grid}
          aria-label="Carregando pessoas"
          aria-busy="true"
        >
          {Array.from({ length: 3 }, (_, index) => (
            <div
              key={index}
              className={`${styles.card} ${styles.skeletonCard}`}
            >
              <div className={styles.skeletonTitle} />
              <div className={styles.skeletonLine} />
              <div className={styles.skeletonLineShort} />
              <div className={styles.skeletonButton} />
            </div>
          ))}
        </section>
      </main>
    );
  }

  if (peopleQuery.isError) {
    return (
      <main className={styles.page}>
        <PageHeader />

        <section className={styles.feedbackCard} role="alert">
          <h2 className={styles.feedbackTitle}>
            Não foi possível carregar as pessoas
          </h2>

          <p className={styles.feedbackText}>
            {getErrorMessage(peopleQuery.error)}
          </p>

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
        </section>
      </main>
    );
  }

  return (
    <main className={styles.page}>
      <PageHeader />

      {orderedPeople.length === 0 ? (
        <section className={styles.emptyState}>
          <div className={styles.emptyIcon} aria-hidden="true">
            PF
          </div>

          <h2 className={styles.emptyTitle}>
            Nenhuma pessoa cadastrada
          </h2>

          <p className={styles.emptyText}>
            Cadastre uma pessoa para começar a registrar
            transações residenciais.
          </p>

          <Link
            to="/people/new"
            className={styles.emptyAction}
          >
            Cadastrar primeira pessoa
          </Link>
        </section>
      ) : (
        <section
          className={styles.grid}
          aria-label="Pessoas cadastradas"
        >
          {orderedPeople.map((person) => {
            const isDeleting =
              deletePerson.isPending &&
              deletingPersonId === person.id;

            return (
              <article
                key={person.id}
                className={styles.card}
              >
                <div className={styles.cardHeader}>
                  <div
                    className={styles.avatar}
                    aria-hidden="true"
                  >
                    {getInitials(person.name)}
                  </div>

                  <div className={styles.personInfo}>
                    <h2 className={styles.personName}>
                      {person.name}
                    </h2>

                    <span className={styles.personId}>
                      Identificador #{person.id}
                    </span>
                  </div>
                </div>

                <div className={styles.details}>
                  <div className={styles.detailItem}>
                    <span className={styles.detailLabel}>
                      Idade
                    </span>

                    <strong className={styles.detailValue}>
                      {person.age}{" "}
                      {person.age === 1 ? "ano" : "anos"}
                    </strong>
                  </div>

                  <div className={styles.detailItem}>
                    <span className={styles.detailLabel}>
                      Transações permitidas
                    </span>

                    <strong className={styles.detailValue}>
                      {person.age < 18
                        ? "Somente despesas"
                        : "Receitas e despesas"}
                    </strong>
                  </div>
                </div>

                <div className={styles.cardFooter}>
                  <button
                    type="button"
                    className={styles.deleteButton}
                    onClick={() => handleDelete(person)}
                    disabled={deletePerson.isPending}
                    aria-label={`Excluir ${person.name}`}
                    aria-busy={isDeleting}
                  >
                    {isDeleting
                      ? "Excluindo..."
                      : "Excluir pessoa"}
                  </button>
                </div>
              </article>
            );
          })}
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
          Gerenciamento
        </span>

        <h1 className={styles.title}>Pessoas</h1>

        <p className={styles.description}>
          Consulte e gerencie as pessoas vinculadas às
          transações residenciais.
        </p>
      </div>

      <Link to="/people/new" className={styles.createButton}>
        Nova pessoa
      </Link>
    </header>
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
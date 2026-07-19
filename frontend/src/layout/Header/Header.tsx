import { Bell, Menu } from "lucide-react";

import styles from "./Header.module.css";

interface HeaderProps {
  pageTitle: string;
  onMenuOpen: () => void;
}

/**
 * Renderiza o cabeçalho superior da aplicação.
 *
 * O componente apresenta o título correspondente à rota atual e disponibiliza
 * o controle de abertura do menu lateral em dispositivos menores.
 */
export function Header({ pageTitle, onMenuOpen }: HeaderProps) {
  return (
    <header className={styles.header}>
      <div className={styles.leftContent}>
        <button
          type="button"
          className={styles.menuButton}
          onClick={onMenuOpen}
          aria-label="Abrir menu lateral"
          aria-controls="application-sidebar"
        >
          <Menu size={22} aria-hidden="true" />
        </button>

        <div>
          <span className={styles.contextLabel}>Visão geral</span>
          <h1 className={styles.title}>{pageTitle}</h1>
        </div>
      </div>

      <div className={styles.actions}>
        <button
          type="button"
          className={styles.notificationButton}
          aria-label="Notificações"
          title="Notificações"
        >
          <Bell size={20} strokeWidth={1.8} aria-hidden="true" />
        </button>

        <div className={styles.divider} aria-hidden="true" />

        <div className={styles.profile}>
          <div className={styles.avatar} aria-hidden="true">
            LP
          </div>

          <div className={styles.profileDetails}>
            <strong className={styles.profileName}>Administrador</strong>
            <span className={styles.profileRole}>Gestão residencial</span>
          </div>
        </div>
      </div>
    </header>
  );
}
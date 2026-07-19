import { Landmark, X } from "lucide-react";

import { Navigation } from "../Navigation/Navigation";
import styles from "./Sidebar.module.css";

interface SidebarProps {
  isOpen: boolean;
  onClose: () => void;
}

/**
 * Renderiza o menu lateral da aplicação.
 *
 * Em dispositivos móveis, o componente pode ser aberto ou fechado pelo layout.
 * Em telas maiores, permanece visível como parte fixa da estrutura principal.
 */
export function Sidebar({ isOpen, onClose }: SidebarProps) {
  const sidebarClasses = `${styles.sidebar} ${
    isOpen ? styles.sidebarOpen : ""
  }`;

  return (
    <aside
      id="application-sidebar"
      className={sidebarClasses}
      aria-label="Menu lateral"
    >
      <div className={styles.header}>
        <div className={styles.brand}>
          <div className={styles.brandIcon} aria-hidden="true">
            <Landmark size={24} strokeWidth={1.8} />
          </div>

          <div>
            <strong className={styles.brandName}>LeParc</strong>
            <span className={styles.brandDescription}>
              Residential Finance
            </span>
          </div>
        </div>

        <button
          type="button"
          className={styles.closeButton}
          onClick={onClose}
          aria-label="Fechar menu lateral"
        >
          <X size={22} aria-hidden="true" />
        </button>
      </div>

      <div className={styles.navigationArea}>
        <span className={styles.sectionLabel}>Menu principal</span>

        <Navigation onNavigate={onClose} />
      </div>

      <div className={styles.footer}>
        <div className={styles.footerContent}>
          <span className={styles.statusIndicator} aria-hidden="true" />

          <div>
            <strong className={styles.statusTitle}>Sistema operacional</strong>
            <span className={styles.statusDescription}>
              Serviços disponíveis
            </span>
          </div>
        </div>
      </div>
    </aside>
  );
}
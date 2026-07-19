import { useEffect, useMemo, useState } from "react";
import { Outlet, useLocation } from "react-router-dom";

import { navigationItems } from "../../routes/navigation";
import { Header } from "../Header/Header";
import { Sidebar } from "../Sidebar/Sidebar";
import styles from "./AppLayout.module.css";

/**
 * Organiza a estrutura visual compartilhada entre as páginas da aplicação.
 *
 * O componente controla a navegação lateral em dispositivos móveis, resolve
 * o título da página atual e disponibiliza a área em que as rotas filhas
 * são renderizadas.
 */
export function AppLayout() {
  const [isSidebarOpen, setIsSidebarOpen] = useState(false);
  const location = useLocation();

  const pageTitle = useMemo(() => {
    const currentNavigationItem = navigationItems.find(
      (item) => item.path === location.pathname,
    );

    return currentNavigationItem?.label ?? "Página não encontrada";
  }, [location.pathname]);

  /**
   * Fecha o menu móvel sempre que a navegação altera a rota atual.
   */
  useEffect(() => {
    setIsSidebarOpen(false);
  }, [location.pathname]);

  function handleSidebarOpen(): void {
    setIsSidebarOpen(true);
  }

  function handleSidebarClose(): void {
    setIsSidebarOpen(false);
  }

  return (
    <div className={styles.layout}>
      <Sidebar isOpen={isSidebarOpen} onClose={handleSidebarClose} />

      {isSidebarOpen && (
        <button
          type="button"
          className={styles.overlay}
          onClick={handleSidebarClose}
          aria-label="Fechar menu lateral"
        />
      )}

      <div className={styles.contentArea}>
        <Header pageTitle={pageTitle} onMenuOpen={handleSidebarOpen} />

        <main className={styles.main}>
          <div className={`app-container ${styles.mainContent}`}>
            <Outlet />
          </div>
        </main>
      </div>
    </div>
  );
}
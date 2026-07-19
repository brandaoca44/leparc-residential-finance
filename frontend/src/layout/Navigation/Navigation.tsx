import { NavLink } from "react-router-dom";

import { navigationItems } from "../../routes/navigation";
import styles from "./Navigation.module.css";

interface NavigationProps {
  onNavigate?: () => void;
}

/**
 * Renderiza os links principais da aplicação.
 *
 * O componente utiliza a configuração centralizada de navegação e identifica
 * automaticamente a rota ativa por meio do React Router.
 */
export function Navigation({ onNavigate }: NavigationProps) {
  return (
    <nav className={styles.navigation} aria-label="Navegação principal">
      <ul className={styles.list}>
        {navigationItems.map(({ icon: Icon, label, path }) => (
          <li key={path} className={styles.item}>
            <NavLink
              to={path}
              onClick={onNavigate}
              className={({ isActive }) =>
                `${styles.link} ${isActive ? styles.active : ""}`
              }
            >
              <Icon
                className={styles.icon}
                size={20}
                strokeWidth={1.8}
                aria-hidden="true"
              />

              <span>{label}</span>
            </NavLink>
          </li>
        ))}
      </ul>
    </nav>
  );
}
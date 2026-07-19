import type { LucideIcon } from "lucide-react";
import {
  ArrowLeftRight,
  CirclePlus,
  LayoutDashboard,
  UserPlus,
  Users,
} from "lucide-react";

/**
 * Representa uma opção disponível no menu principal da aplicação.
 */
export interface NavigationItem {
  label: string;
  path: string;
  icon: LucideIcon;
}

/**
 * Centraliza as rotas exibidas na navegação principal.
 *
 * A configuração compartilhada evita duplicação de títulos, caminhos
 * e ícones entre os componentes responsáveis pelo layout.
 */
export const navigationItems: NavigationItem[] = [
  {
    label: "Dashboard",
    path: "/dashboard",
    icon: LayoutDashboard,
  },
  {
    label: "Pessoas",
    path: "/people",
    icon: Users,
  },
  {
    label: "Nova pessoa",
    path: "/people/new",
    icon: UserPlus,
  },
  {
    label: "Transações",
    path: "/transactions",
    icon: ArrowLeftRight,
  },
  {
    label: "Nova transação",
    path: "/transactions/new",
    icon: CirclePlus,
  },
];
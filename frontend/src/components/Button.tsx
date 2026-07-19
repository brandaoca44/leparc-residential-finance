import type {
  ButtonHTMLAttributes,
  ReactNode,
} from "react";

import styles from "./Button.module.css";

type ButtonVariant = "primary" | "secondary" | "danger";

interface ButtonProps
  extends ButtonHTMLAttributes<HTMLButtonElement> {
  children: ReactNode;
  variant?: ButtonVariant;
  fullWidth?: boolean;
  isLoading?: boolean;
}

/**
 * Botão reutilizável com suporte a variações visuais e carregamento.
 */
export function Button({
  children,
  variant = "primary",
  fullWidth = false,
  isLoading = false,
  disabled,
  className = "",
  type = "button",
  ...buttonProps
}: ButtonProps) {
  const buttonClassName = [
    styles.button,
    styles[variant],
    fullWidth ? styles.fullWidth : "",
    className,
  ]
    .filter(Boolean)
    .join(" ");

  return (
    <button
      {...buttonProps}
      type={type}
      className={buttonClassName}
      disabled={disabled || isLoading}
      aria-busy={isLoading}
    >
      {isLoading ? "Processando..." : children}
    </button>
  );
}
import {
  forwardRef,
  type InputHTMLAttributes,
} from "react";

import styles from "./Input.module.css";

interface InputProps
  extends InputHTMLAttributes<HTMLInputElement> {
  label: string;
  error?: string;
  helperText?: string;
}

/**
 * Campo de entrada com rótulo, mensagem auxiliar e validação acessível.
 */
export const Input = forwardRef<HTMLInputElement, InputProps>(
  function Input(
    {
      id,
      label,
      error,
      helperText,
      className = "",
      required,
      ...inputProps
    },
    ref,
  ) {
    const inputId = id ?? inputProps.name;
    const errorId = error ? `${inputId}-error` : undefined;
    const helperId =
      helperText && !error ? `${inputId}-helper` : undefined;

    return (
      <div className={styles.field}>
        <label className={styles.label} htmlFor={inputId}>
          {label}

          {required && (
            <span className={styles.required} aria-hidden="true">
              *
            </span>
          )}
        </label>

        <input
          {...inputProps}
          ref={ref}
          id={inputId}
          required={required}
          className={[
            styles.input,
            error ? styles.invalid : "",
            className,
          ]
            .filter(Boolean)
            .join(" ")}
          aria-invalid={Boolean(error)}
          aria-describedby={errorId ?? helperId}
        />

        {error ? (
          <span
            id={errorId}
            className={styles.error}
            role="alert"
          >
            {error}
          </span>
        ) : (
          helperText && (
            <span id={helperId} className={styles.helper}>
              {helperText}
            </span>
          )
        )}
      </div>
    );
  },
);
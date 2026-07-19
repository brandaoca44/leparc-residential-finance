import { BrowserRouter } from "react-router-dom";
import { Toaster } from "react-hot-toast";

import { AppRoutes } from "./routes/AppRoutes";

/**
 * Inicializa os provedores globais e o sistema de rotas da aplicação.
 */
function App() {
  return (
    <BrowserRouter>
      <Toaster
        position="top-right"
        reverseOrder={false}
        toastOptions={{
          duration: 4000,
          style: {
            border: "1px solid var(--color-border)",
            borderRadius: "var(--border-radius-md)",
            background: "var(--color-surface)",
            color: "var(--color-text-primary)",
            boxShadow: "var(--shadow-md)",
          },
        }}
      />

      <AppRoutes />
    </BrowserRouter>
  );
}

export default App;
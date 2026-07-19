import { Navigate, Route, Routes } from "react-router-dom";

import { AppLayout } from "../layout/AppLayout/AppLayout";

import { DashboardPage } from "../pages/Dashboard/DashboardPage";
import { NotFoundPage } from "../pages/NotFound/NotFoundPage";
import { PeoplePage } from "../pages/People/PeoplePage";
import { PersonCreatePage } from "../pages/PersonCreate/PersonCreatePage";
import { TransactionCreatePage } from "../pages/TransactionCreate/TransactionCreatePage";
import { TransactionsPage } from "../pages/Transactions/TransactionsPage";

/**
 * Define as rotas disponíveis na aplicação.
 */
export function AppRoutes() {
  return (
    <Routes>
      <Route element={<AppLayout />}>
        <Route index element={<Navigate to="/dashboard" replace />} />

        <Route path="/dashboard" element={<DashboardPage />} />

        <Route path="/people" element={<PeoplePage />} />

        <Route path="/people/new" element={<PersonCreatePage />} />

        <Route path="/transactions" element={<TransactionsPage />} />

        <Route
          path="/transactions/new"
          element={<TransactionCreatePage />}
        />
      </Route>

      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
}
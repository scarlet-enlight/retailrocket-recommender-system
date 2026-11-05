import { Routes, Route } from 'react-router-dom'
import Layout from './components/Layout'
import MainPage from './pages/MainPage'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import TermsOfServicePage from './pages/TermsOfServicePage'
import ForgottenPasswordPage from './pages/ForgottenPasswordPage'

function App() {
  return (
      <Routes>
          <Route element={<Layout />}>
              <Route path="/" element={<MainPage />} />
              <Route path="/login" element={<LoginPage />} />
              <Route path="/register" element={<RegisterPage />} />
              <Route path="/tos" element={<TermsOfServicePage />} />
              <Route path="/forgot-password" element={<ForgottenPasswordPage />} />
          </Route>
      </Routes>
  )
}

export default App

import { Routes, Route } from 'react-router-dom'
import Layout from './components/Layout'
import MainPage from './pages/MainPage'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import TermsOfServicePage from './pages/TermsOfServicePage'
import ForgottenPasswordPage from './pages/ForgottenPasswordPage'
import AboutUsPage from "./pages/AboutUsPage.tsx";
import PasswordResetPage from "./pages/PasswordResetPage.tsx";
import ContactPage from "./pages/ContactPage.tsx";
import CookiesPage from "./pages/CookiesPage.tsx";


function App() {
  return (
      <Routes>
          <Route element={<Layout />}>
              <Route path="/" element={<MainPage />} />
              <Route path="/login" element={<LoginPage />} />
              <Route path="/register" element={<RegisterPage />} />
              <Route path="/tos" element={<TermsOfServicePage />} />
              <Route path="/contact" element={<ContactPage />}/>
              <Route path="/about-us" element={<AboutUsPage />}/>
              <Route path="/cookies" element={<CookiesPage />}/>
              <Route path="/forgot-password" element={<ForgottenPasswordPage />} />
              <Route path="/reset-password" element={<PasswordResetPage />}/>
          </Route>
      </Routes>
  )
}

export default App

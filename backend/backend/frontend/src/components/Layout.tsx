import { Outlet, Link, useNavigate } from "react-router-dom"
import { useAuth } from "../auth/AuthContext"

export default function Layout() {
    const { user, loading, logout } = useAuth();
    const navigate = useNavigate();

    const handleLogout = async () => {
        await logout();
        navigate("/");
    };

    return (
        <>
            <header className="site-header border-bottom">
                <div className="container d-flex align-items-center justify-content-between py-3">
                    <Link to="/" className="brand">RetailRocket</Link>
                    <nav className="d-flex align-items-center gap-3">
                        {loading ? null : user ? (
                            <>
                                <span className="text-secondary">Welcome, {user.username}.</span>
                                <button type="button" className="btn btn-outline-dark btn-sm" onClick={handleLogout}>Logout</button>
                            </>
                        ) : (
                            <>
                                <Link to="/login">Log In</Link>
                                <Link to="/register" className="btn btn-primary btn-sm">Sign Up</Link>
                            </>
                        )}
                    </nav>
                </div>
            </header>
            <main className="container py-4"><Outlet /></main>
            <footer className="site-footer border-top py-3 mt-5">
                <div className="container small text-secondary d-flex justify-content-center align-items-center">
                    RetailRocket © 2026 - All rights reserved
                </div>
            </footer>
        </>
    )
}
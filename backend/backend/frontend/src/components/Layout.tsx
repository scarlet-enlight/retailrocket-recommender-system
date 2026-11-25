import { Outlet, Link, useNavigate } from "react-router-dom"
import { useAuth } from "../auth/AuthContext"

export default function Layout() {
    const { user, loading, logout } = useAuth();
    const navigate = useNavigate();

    const searchProducts = async () => {

    }

    const handleLogout = async () => {
        await logout();
        navigate("/");
    };

    return (
        <>
            <div className="overflow-x-hidden">
                <header className="site-header border-bottom">
                    <div className="container d-flex align-items-center justify-content-between py-3">
                        <Link to="/" className="brand">RetailRocket</Link>
                        <form onSubmit={searchProducts} noValidate>
                            <div className="d-inline-flex align-items-center">
                                <label htmlFor="button"  className="form-label"/>
                                <input type="text" className="form-control form-control-md" placeholder="Search Products" />
                                <button type="submit" className="btn btn-primary">
                                    <img src="/icons/icons8-magnifying-glass-50.png" alt="Magnifying loop" width="25px"/>
                                </button>
                            </div>
                        </form>
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
                <main className="container py-4"><Outlet />

                </main>
                <footer className="row d-flex site-footer border-top py-3 mt-5 d-grid gap-5">
                    <div className="col-sm small text-secondary d-flex align-items-center justify-content-center">
                        RetailRocket © 2026 - All rights reserved
                    </div>
                    <div className="col-sm small text-secondary d-flex align-items-center justify-content-center gap-3">
                        <Link className="text-body link-underline-light d-flex" to="/tos">Terms of Service</Link>
                        <Link className="text-body link-underline-light d-flex" to="/cookies">Cookies</Link>
                        <Link className="text-body link-underline-light d-flex" to="/contact">Contact</Link>
                        <Link className="text-body link-underline-light d-flex" to="/about-us">About Us</Link>
                    </div>
                </footer>
            </div>
        </>
    )
}
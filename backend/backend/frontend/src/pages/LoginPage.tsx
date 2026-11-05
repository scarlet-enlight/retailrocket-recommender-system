import React, { useState } from "react";
import {Link, useLocation, useNavigate} from "react-router-dom";
import { getErrorMessage } from "../api/getErrorMessage.ts";
import { useAuth } from "../auth/AuthContext.tsx";

const LoginPage = () => {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [rememberMe, setRememberMe] = useState(false);
    const [showOtherMethods, setShowOtherMethods] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    const { login } = useAuth();
    const navigate = useNavigate();
    // two last variables tells the last visited page
    const location = useLocation();
    const from = (location.state as { from?: string} | null)?.from ?? "/";

    const handleLogin = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);

        if (!email.trim() || !password) {
            setError("All fields are required.");
            return;
        }

        try{
            setLoading(true);
            await login(email.trim(), password, rememberMe);
            navigate(from, { replace: true});
        }
        catch(err){
            setError(getErrorMessage(err));
            return;
        }
        finally{
            setLoading(false);
        }
    }

    return (
        <div className="auth-card">
            <h1 className="h3 mb-4">Log In</h1>
            <form onSubmit={handleLogin} noValidate>
                <div className="mb-3">
                    <label htmlFor="email" className="form-label">Email:</label>
                    <input type="email" id="email" className="form-control" name="email"
                           placeholder="Your email address" value={email} autoComplete="email" required
                           onChange={(e) => setEmail(e.target.value)}/>
                </div>
                <div className="mb-3">
                    <label htmlFor="password" className="form-label">Password:</label>
                    <input type="password" id="password" className="form-control" name="password"
                           placeholder="Your password" value={password} autoComplete="password" required
                           onChange={(e) => setPassword(e.target.value)}/>
                </div>
                <div className="form-check mb-3">
                    <input type="checkbox" id="rememberMe" className="form-check-input" name="checkbox" checked={rememberMe}
                           onChange={(e) => setRememberMe(e.target.checked)}/>
                    <label htmlFor="remember-me" className="form-check-label">Remember me</label>
                </div>

                {error && <div className="alert alert-danger py-2" role="alert">{error}</div>}

                <div className="d-grid gap-2">
                    <button type="submit" className="btn btn-primary" disabled={loading}>
                        {loading ? "Logging in..." : "Log In"}
                    </button>

                    <button type="button" className="btn btn-link btn-sm" onClick={() => setShowOtherMethods((v) => !v)}>
                        { showOtherMethods ? "Hide other methods" : "Show different methods of logging in" }
                    </button>

                    {showOtherMethods && (
                        <div>
                            {/* TODO: WebAuthn and Google flows signing up must be added */}
                            <button type="button" className="btn btn-outline-dark" disabled>
                                {loading ? "Logging in..." : "Log In with Security Key"}
                            </button>
                            <button type="button" className="btn btn-outline-dark" disabled>
                                {loading ? "Logging in..." : "Log In with Google Account"}
                            </button>
                        </div>
                    )}
                </div>
            </form>
            <p className="small mt-4 mb-1"><Link to="/forgot-password">Forgot password</Link></p>
            <p className="small mb-0">Don't have an account? <Link to="/register">Sign up now!</Link></p>
        </div>
    );
};

export default LoginPage;
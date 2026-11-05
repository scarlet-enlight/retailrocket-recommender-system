import React, { useState } from 'react'
import { Link, useNavigate } from "react-router-dom";
import api from "../api/axiosConfig.ts";
import { getErrorMessage } from "../api/getErrorMessage.ts"

const RegisterPage: React.FC = () => {
    const [username, setUsername] = useState<string>("");
    const [email, setEmail] = useState<string>("");
    const [password, setPassword] = useState<string>("");
    const [repeatPassword, setRepeatPassword] = useState<string>("");
    const [acceptedTos, setAcceptedTos] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    const navigate = useNavigate();

    const handleRegister = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);

        if (!username.trim() || !email.trim() || !password || !repeatPassword) {
            setError("All fields are required.");
            return;
        }
        if (password !== repeatPassword) {
            setError("Password must match.");
            return;
        }
        if (!acceptedTos){
            setError("You must accept the Terms of Service.");
            return;
        }

        try{
            setLoading(true);
            await api.post('/user', { username: username.trim(), email: email.trim(), password });
            navigate("/login");
        }
        catch (err) {
            setError(getErrorMessage(err));
        }
        finally{
            setLoading(false);
        }
    }

    return (
        <div className="register-card">
            <h1 className="h3 mb-4">Sign Up</h1>
            <form onSubmit={handleRegister} noValidate>
                <div className="mb-3">
                    <label htmlFor="username" className="form-label">Username: </label>
                    <input type="text" id="username" className="form-control" name="username"
                           placeholder="Your new username" value={username} autoComplete="username" required
                           onChange={(e) => setUsername(e.target.value)} />
                </div>
                <div className="mb-3">
                    <label htmlFor="email" className="form-label">Email: </label>
                    <input type="email" id="email" className="form-control" name="email"
                           placeholder="Your email address" value={email} autoComplete="email" required
                           onChange={(e) => setEmail(e.target.value)} />
                </div>
                <div className="mb-3">
                    <label htmlFor="password" className="form-label">Password: </label>
                    <input type="password" id="password" className="form-control" name="password"
                           placeholder="Your new password" value={password} autoComplete="password" required
                           onChange={(e) => setPassword(e.target.value)}/>
                </div>
                <div className="mb-3">
                    <label htmlFor="repeatPassword" className="form-label">Repeat password: </label>
                    <input type="password" id="repeatPassword" className="form-control" name="repeatPassword"
                           placeholder="Repeat new password" value={repeatPassword} autoComplete="repeatPassword" required
                           onChange={(e) => setRepeatPassword(e.target.value)}/>
                </div>
                <div className="form-check mb-3">
                    <input type="checkbox" id="allAgreements" className="form-check-input" value="allAgreements"></input>
                    <label htmlFor="all-agreements" className="form-check-label">Check all agreements</label>
                </div>
                <hr/>
                <div className="form-check mb-3">
                    <input type="checkbox" id="tosAgreement" className="form-check-input" value="tosAgreement"
                           onChange={(e) => setAcceptedTos(e.target.checked)}></input>
                    <label htmlFor="tos-agreement" className="form-check-label">
                        <span className="text-danger">*</span> I agree that I've read and acknowledged policy of <Link to="/tos">RetailRocket's Terms of Service</Link>
                    </label>
                </div>
                <div className="form-check mb-3">
                    <input type="checkbox" id="otherAgreement" className="form-check-input" value="otherAgreement"></input>
                    <label htmlFor="other-agreements" className="form-check-label">Newsletter and other agreements here and below</label>
                </div>
                <div className="form-check mb-3">
                    <span className="text-danger">*</span><span className="text-secondary"> Required agreements</span>
                </div>

                {error && <div className="alert alert-danger py-2" role="alert">{error}</div>}

                <div className="d-grid gap-2">
                    <button type="submit" className="btn btn-primary" disabled={loading}>
                        {loading ? "Creating account..." : "Create Account"}
                    </button>
                </div>
            </form>
        </div>
    )
}

export default RegisterPage;
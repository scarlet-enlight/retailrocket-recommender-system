import React, { useState } from 'react'
import { Link, useNavigate } from "react-router-dom";
import { checkPasswordLength } from "../scripts/passwordValidator.ts";
import api from "../api/axiosConfig.ts";
import { getErrorMessage } from "../api/getErrorMessage.ts"
import PasswordInput from "../components/PasswordInput.tsx";

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
        if (!checkPasswordLength(password) || !checkPasswordLength(repeatPassword)) {
            setError("Passwords' length must be at least 16 characters. Meet only this criteria, seriously.");
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
            <form onSubmit={handleRegister}>
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
                    <PasswordInput id={"password"}
                                   label={"Password: "}
                                   value={password}
                                   onChange={setPassword}
                                   autoComplete={"new-password"}
                                   placeholder={"Your new password"}/>
                    <div className="form-control mt-4 mb-4 text-bg-light">
                        Passwords can be fun and creative without being stingy with requirements. We care about your safety and privacy
                            so we just ask you a few things:
                            <ul>
                                <li>Password must have at least 16 characters long.</li>
                                <li>Insert whatever characters and symbols you like - spaces, non-latin, own alphabet, etc.</li>
                                <li>Make it creative but not easy to guess.</li>
                            </ul>
                    </div>
                </div>
                <div className="mb-3">
                    <PasswordInput id={"repeatPassword"}
                                   label={"Repeat Password: "}
                                   value={repeatPassword}
                                   onChange={setRepeatPassword}
                                   autoComplete={"repeat-password"}
                                   placeholder={"Repeat new password"}/>
                </div>
                <div className="form-check mb-3">
                    <input type="checkbox" id="allAgreements" className="form-check-input" value="allAgreements"></input>
                    <label htmlFor="allAgreements" className="form-check-label">Check all agreements</label>
                </div>
                <hr/>
                <div className="form-check mb-3">
                    <input type="checkbox" id="tosAgreement" className="form-check-input" value="tosAgreement"
                           onChange={(e) => setAcceptedTos(e.target.checked)}></input>
                    <label htmlFor="tosAgreement" className="form-check-label">
                        <span className="text-danger">*</span> I agree that I've read and acknowledged policy of <Link to="/tos">RetailRocket's Terms of Service</Link>
                    </label>
                </div>
                <div className="form-check mb-3">
                    <input type="checkbox" id="otherAgreements" className="form-check-input" value="otherAgreements"></input>
                    <label htmlFor="otherAgreements" className="form-check-label">Newsletter and other agreements here and below</label>
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
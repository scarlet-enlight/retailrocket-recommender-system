import { useState } from "react";

interface Props {
    id: string;
    label: string;
    value: string;
    onChange: (value: string) => void;
    autoComplete: "new-password" | "current-password" | "repeat-password";
    placeholder: string;
}

export default function PasswordInput({ id, label, value, onChange, autoComplete, placeholder } : Props) {
    const [visible, setVisible] = useState(false);

    return (
        <>
            <label htmlFor={id} className="form-label">{label}</label>
            <div className="d-flex">
                <input type={visible ? "text" : "password"} id={id} className="form-control" name="password"
                       placeholder={placeholder} value={value} autoComplete={autoComplete} required
                       onChange={(e) => onChange(e.target.value)}/>
                <button type="button" className="btn btn-primary"
                        onClick={() => setVisible((v) => !v)}
                        aria-label={visible ? "Hide Password" : "Show Password"} aria-pressed={visible}>
                    <img src="/icons/icons8-eye-24.png" alt="Eye"/>
                </button>
            </div>
        </>
    );
}
import { createContext,  useCallback, useContext, useEffect, useState } from "react"
import api from "../api/axiosConfig.ts";

export interface User {
    userId: string;
    username: string;
}

interface AuthContextValue {
    user: User | null,
    loading: boolean,
    login: (email: string, password: string, rememberMe: boolean) => Promise<void>;
    logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
    const [user, setUser] = useState<User | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const controller = new AbortController();
        api.get<User>("/user/me", { signal: controller.signal })
            .then((res) => setUser(res.data))
            .catch(() => setUser(null))
            .finally(() => {
                if (!controller.signal.aborted) setLoading(false);
            });
        return () => controller.abort();
    }, []);

    const login = useCallback(async (email: string, password: string, rememberMe: boolean) => {
        const res = await api.post<User>("/user/login", { email, password, rememberMe });
        setUser({ userId: res.data.userId, username: res.data.username });
    }, []);

    const logout = useCallback(async () => {
        try{
            await api.post("/user/logout");
        }
        finally{
            setUser(null);
        }
    }, []);

    return (
        <AuthContext.Provider value={{ user, loading, login, logout }}>
            {children}
        </AuthContext.Provider>
    )
}

export function useAuth() {
    const context = useContext(AuthContext);
    if (!context) throw new Error("AuthProvider returns undefined");
    return context;
}
import axios from "axios";

export function getErrorMessage(err: unknown): string {
    if (axios.isAxiosError(err)) {
        if (err.response?.data?.message) return err.response.data.message;
        if (!err.response) return "Cannot reach the server.";
    }
    return "Something went wrong. Please try again.";
}
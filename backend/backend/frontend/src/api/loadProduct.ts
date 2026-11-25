import api from "../api/axiosConfig.ts";
import type {Product} from "../interfaces/product.ts";

export async function loadProduct(id: string, signal?: AbortSignal): Promise<Product>{
    const res = await api.get<Product>(`/product/${id}`, { signal });
    return res.data;
}
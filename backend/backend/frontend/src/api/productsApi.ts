import api from "../api/axiosConfig.ts"
import type { Product } from "../interfaces/product.ts"

function shuffleProducts<T>(array: T[]): T[] {
    let currentIndex = array.length, randomIndex;
    while (currentIndex != 0) {
        randomIndex = Math.floor(Math.random() * currentIndex);
        currentIndex--;

        [array[currentIndex], array[randomIndex]] = [array[randomIndex], array[currentIndex]];
    }
    return array;
}

export async function getRandomProducts(): Promise<Product[]> {
    const res = await api.get<Product[]>("/product");
    res.data = shuffleProducts(res.data);
    return res.data;
}
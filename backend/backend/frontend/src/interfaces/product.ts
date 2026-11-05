export interface Category {
    parentCategory: Category;
    name: string;
}

export interface Product {
    productId: string;
    name: string;
    price: number;
    category: Category;
    isAvailable: boolean;
    imageUrl?: string;
}
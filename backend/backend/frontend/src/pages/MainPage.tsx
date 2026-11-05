import { useEffect, useState} from "react";
import axios from "axios";
import { getRandomProducts } from "../api/productsApi.ts";
import { getErrorMessage } from "../api/getErrorMessage.ts";
import ProductCard from "../components/ProductCard.tsx";
import type {Product} from "../interfaces/product.ts"

const MainPage = () => {
    const [products, setProducts] = useState<Product[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        setLoading(true);
        setError(null);

        getRandomProducts()
            .then(setProducts)
            .catch((error) => {
                if (axios.isCancel(error)) return;
                setError(getErrorMessage(error));
            })
            .finally(() => {
                setLoading(false);
            });
    }, []);

    return (
        <div>
            <div className="d-flex align-items-baseline justify-content-between mb-4">
                <h1 className="h2 mb-0">Today's picks</h1>
            </div>

            {loading && <div className="text-secondary">Loading products...</div>}
            {error && <div className="alert alert-danger" role="alert">{error}</div>}
            {!loading && !error && products.length === 0 && <p className="text-secondary">No products to show.</p>}
            <section className="row row-cols-2 row-cols-md-3 row-cols-lg-4 g-3">
                {products.map((p) => (
                    <div className="col" key={p.productId}>
                        <ProductCard product={p} />
                    </div>
                ))}
            </section>
        </div>
    )
}

export default MainPage;
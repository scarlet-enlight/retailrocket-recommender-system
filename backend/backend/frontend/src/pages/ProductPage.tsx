import { useEffect, useState } from "react";
import { Link, useParams} from "react-router-dom";
import type { Product } from "../interfaces/product.ts";
import { loadProduct } from "../api/loadProduct.ts";
import { getErrorMessage } from "../api/getErrorMessage.ts";
import axios from "axios";

const DEFAULT_IMAGE = "/images/default-image.jpeg";

const ProductPage = () => {
    const [product, setProduct] = useState<Product>();
    const [src, setSrc] = useState(product?.imageUrl || DEFAULT_IMAGE);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const {guid} = useParams();

    useEffect(() => {
        if (!guid) return;

        const controller = new AbortController();
        setLoading(true);
        setError(null);

        loadProduct(guid, controller.signal)
            .then(setProduct)
            .catch((err) => {
                if (axios.isCancel(err)) return;
                setError(getErrorMessage(err))
            })
            .finally(() => {
                if (!controller.signal.aborted) setLoading(false)
            });

        return () => controller.abort();
    }, [guid]);

    return(
        <div>
            <Link className="mb-3 d-sm-inline-block" to="/">Return to products</Link>
            {loading && <div className="text-secondary">Loading product...</div>}
            {error && <div className="alert alert-danger" role="alert">{error}</div>}
            {!loading && !error && !product && (
                <p className="text-secondary">Product is not available.</p>
            )}

            {product && (
                <article className="row g-4 g-lg-5">
                    <div className="col-md-6">
                        <div className="product-photo ratio ratio-4x3 border">
                            <img
                                src={src}
                                alt={product.name}
                                onError={() => { if (src !== DEFAULT_IMAGE) setSrc(DEFAULT_IMAGE); }}
                            />
                        </div>
                    </div>
                    <div className="col-md-6">
                        {product.category?.name && (
                            <p className="text-secondary small text-uppercase mb-1">{product.category.name}</p>
                        )}
                        <h1 className="h1">{product.name}</h1>
                        <p>{product.price.toFixed(2)} PLN</p>

                        <p className={product.isAvailable ? "text-success" : "text-danger"}>
                            {product.isAvailable ? `In Stock - ${product.stock} pcs` : "Currently unavailable"}
                        </p>
                        <button type="button" className="btn btn-primary" disabled={!product.isAvailable}>
                            Add to cart
                        </button>
                    </div>
                </article>
            )}
        </div>
    )
}

export default ProductPage;
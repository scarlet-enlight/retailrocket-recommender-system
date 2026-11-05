import {Link} from "react-router-dom";
import type { Product } from "../interfaces/product.ts";
import {useState} from "react";

const DEFAULT_IMAGE = "/images/default-image.jpeg";

export default function ProductCard({ product }: {product: Product }) {
    const [src, setSrc] = useState(product.imageUrl || DEFAULT_IMAGE);

    return (
        <article className="product-card position-relative d-flex flex-column">
            <div className="ration ratio-16x9 border-bottom">
                <img
                    src={src}
                    alt={product.name}
                    loading="lazy"
                    width="300"
                    height="200"
                    onError={() => { if (src !== DEFAULT_IMAGE) setSrc(DEFAULT_IMAGE); }}
                />
                <div className="p-3 d-flex flex-column flex-grow-1">
                    <h3 className="h6 product-title">
                        <Link to={`/product/${product.productId}`} className="stretched-link">
                            {product.name}
                        </Link>
                    </h3>
                    <p className="price mt-auto mb-0">{product.price.toFixed(2)} PLN</p>
                </div>
            </div>
        </article>
    )
}
import { IProduct } from "@/types/Product";
import axios from "axios";

/**
 * Product Service.
 * Provides UI Business logic associated with products.
 */
export class ProductService {
  API_URL = process.env.VUE_APP_API_URL;

  async archive(productId: number) {
    const result = await axios.patch(`${this.API_URL}/product/${productId}`);
    return result.data;
  }

  async save(newProduct: IProduct) {
    const result = await axios.post(`${this.API_URL}/product/`, newProduct);
    return result.data;
  }
}

import { nanoid } from 'nanoid';

export type CartType = {
  id: string;
  cartItems: CartItem[];
  deliveryMethodId?: number;
  clientSecret?: string;
  paymentIntentId?: string;
};

export type CartItem = {
  productId: number;
  productName: string;
  price: number;
  qty: number;
  pictureUrl: string;
  productBrand: string;
  productType: string;
};

export class Cart implements CartType {
  id = nanoid();
  cartItems: CartItem[] = [];
  deliveryMethodId?: number;
  clientSecret?: string;
  paymentIntentId?: string;
}

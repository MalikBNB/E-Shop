import { nanoid } from 'nanoid';

export type CartType = {
  id: string;
  cartItems: CartItem[];
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
}

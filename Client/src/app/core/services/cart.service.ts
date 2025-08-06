import { computed, inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Cart, CartItem } from '../../shared/models/cart';
import { Product } from '../../shared/models/product';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  baseUrl = environment.baseUrl;
  private http = inject(HttpClient);
  cart = signal<Cart | null>(null);
  itemCount = computed(() => {
    return this.cart()?.cartItems.reduce((sum, item) => sum + item.qty, 0);
  });
  totals = computed(() => {
    const cart = this.cart();
    if (!cart) return null;

    const subtotal = cart.cartItems.reduce(
      (sum, item) => sum + item.price * item.qty,
      0
    );
    const shipping = 0;
    const discount = 0;
    return {
      subtotal,
      shipping,
      discount,
      total: subtotal + shipping - discount,
    };
  });

  getCart(id: string) {
    return this.http.get<Cart>(`${this.baseUrl}carts?id=${id}`).pipe(
      map((cart) => {
        this.cart.set(cart);
        return cart;
      })
    );
  }

  SetCart(cart: Cart) {
    return this.http.post<Cart>(`${this.baseUrl}carts`, cart).subscribe({
      next: (cart) => this.cart.set(cart),
    });
  }

  addItemToCart(item: CartItem | Product, qty = 1) {
    const cart = this.cart() ?? this.createCart();
    if (this.isProduct(item)) {
      item = this.mapProductToCartItem(item);
    }

    cart.cartItems = this.addOrUpdateItem(cart.cartItems, item, qty);
    this.SetCart(cart);
  }

  removeItemFromCart(productId: number, qty = 1) {
    const cart = this.cart();
    if (!cart) return;

    const index = cart.cartItems.findIndex((c) => c.productId === productId);
    if (index === -1) return;

    if (cart.cartItems[index].qty > qty) {
      cart.cartItems[index].qty -= qty;
    } else {
      cart.cartItems.splice(index, 1);
    }

    if (cart.cartItems.length === 0) {
      this.deleteCart();
    } else {
      this.SetCart(cart);
    }
  }

  deleteCart() {
    return this.http
      .delete(`${this.baseUrl}carts?id=${this.cart()?.id}`)
      .subscribe({
        next: () => {
          localStorage.removeItem('cart_id');
          this.cart.set(null);
        },
      });
  }

  private addOrUpdateItem(
    cartItems: CartItem[],
    item: CartItem,
    qty: number
  ): CartItem[] {
    const index = cartItems.findIndex((x) => x.productId === item.productId);
    if (index === -1) {
      item.qty = qty;
      cartItems.push(item);
    } else {
      cartItems[index].qty += qty;
    }

    return cartItems;
  }

  private isProduct(item: CartItem | Product): item is Product {
    return (item as Product).id !== undefined;
  }

  private createCart(): Cart {
    const cart = new Cart();
    localStorage.setItem('cart_id', cart.id);
    return cart;
  }

  private mapProductToCartItem(item: Product): CartItem {
    return {
      productId: item.id,
      productName: item.name,
      price: item.price,
      pictureUrl: item.pictureUrl,
      qty: 0,
      productBrand: item.productBrand,
      productType: item.productType,
    };
  }
}

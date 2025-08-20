import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Pagination } from '../../shared/models/pagination';
import { Product } from '../../shared/models/product';
import { Brand } from '../../shared/models/brand';
import { Type } from '../../shared/models/type';
import { ShopParams } from '../../shared/models/shopParams';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ShopService {
  baseUrl = environment.baseUrl;
  private http = inject(HttpClient);
  types: Type[] = [];
  brands: Brand[] = [];

  getProducts(shopParams: ShopParams) {
    let params = new HttpParams();

    if (shopParams.brandIds.length > 0) {
      params = params.append('brandId', shopParams.brandIds.join(','));
    }

    if (shopParams.typeIds.length > 0) {
      params = params.append('typeId', shopParams.typeIds.join(','));
    }

    if (shopParams.sort) params = params.append('sort', shopParams.sort);

    if (shopParams.search) params = params.append('search', shopParams.search);

    params = params.append('pageSize', shopParams.pageSize);
    params = params.append('pageIndex', shopParams.pageNumber);

    return this.http.get<Pagination<Product>>(`${this.baseUrl}products`, {
      params,
    });
  }

  getProduct(id: number) {
    return this.http.get<Product>(`${this.baseUrl}products/${id}`);
  }

  getTypes() {
    if (this.types.length > 0) return;
    return this.http.get<Type[]>(`${this.baseUrl}products/types`).subscribe({
      next: (response) => (this.types = response),
      error: (error) => console.log(error),
    });
  }

  getBrands() {
    if (this.brands.length > 0) return;
    return this.http.get<Brand[]>(`${this.baseUrl}products/brands`).subscribe({
      next: (response) => (this.brands = response),
      error: (error) => console.log(error),
    });
  }
}

import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { DeliveryMetod } from '../../shared/models/deliveryMethod';
import { map, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CheckoutService {
  baseUrl = environment.baseUrl;
  private http = inject(HttpClient);
  deliveryMethods: DeliveryMetod[] = [];

  getDeliveryMethod() {
    if(this.deliveryMethods.length > 0) return of(this.deliveryMethods);
    return this.http.get<DeliveryMetod[]>(`${this.baseUrl}payments/delivery-methods`).pipe(
      map(methods => {
        this.deliveryMethods = methods.sort((a, b) => b.price - a.price);
        return methods;
      })
    )
  }
}

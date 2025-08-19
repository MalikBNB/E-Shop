import { Component, inject, OnInit } from '@angular/core';
import { OrderService } from '../../../core/services/order.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from '../../../core/services/account.service';
import { Order } from '../../../shared/models/order';
import { CommonModule } from '@angular/common';
import { AddressPipe } from '../../../shared/pipes/address-pipe';
import { MatCard } from '@angular/material/card';
import { MatButton } from '@angular/material/button';
import { CardPipe } from '../../../shared/pipes/card-pipe';

@Component({
  selector: 'app-order-detailed',
  imports: [CommonModule, AddressPipe, CardPipe, MatCard, MatButton],
  templateUrl: './order-detailed.component.html',
  styleUrl: './order-detailed.component.scss',
})
export class OrderDetailedComponent implements OnInit {
  private orderService = inject(OrderService);
  private activatedRoute = inject(ActivatedRoute);
  private accountService = inject(AccountService);
  // private adminService = inject(AdminService);
  private router = inject(Router);
  order?: Order;
  // buttonText = this.accountService.isAdmin() ? 'Return to admin' : 'Return to orders'
  buttonText = 'Return to orders';

  ngOnInit(): void {
    this.loadOrder();
  }

  onReturnClick() {
    // this.accountService.isAdmin()
    //   ? this.router.navigateByUrl('/admin')
    //   : this.router.navigateByUrl('/orders')

    this.router.navigateByUrl('/orders');
  }

  loadOrder() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (!id) return;

    // const loadOrderData = this.accountService.isAdmin()
    //   ? this.adminService.getOrder(+id)
    //   : this.orderService.getOrderDetailed(+id);

    const loadOrderData = this.orderService.getOrderDetailed(+id);

    loadOrderData.subscribe({
      next: (order) => {
        this.order = order;
        console.log(order);
      },
    });
  }
}

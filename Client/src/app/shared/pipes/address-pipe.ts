import { Pipe, PipeTransform } from '@angular/core';
import { ConfirmationToken } from '@stripe/stripe-js';

@Pipe({
  name: 'address',
})
export class AddressPipe implements PipeTransform {
  transform(value?: ConfirmationToken['shipping'], ...args: unknown[]): unknown {
    if (value?.address && value.name) {
      const { line1, line2, city, country, postal_code } = value.address;
      return `${value.name}, ${line1}${
        line2 ? ', ' + line2 : ''
      }, ${city}, ${postal_code}, ${country}`;
    }

    return 'Unknown address';
  }
}

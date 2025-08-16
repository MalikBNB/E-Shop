export type User = {
  firstName: string;
  lastName: string;
  email: string;
  address: Address;
}

export type Address = {
  line1: string;
  line2: string;
  street: string;
  state: string;
  zipCode: string;
  country: string;
}
